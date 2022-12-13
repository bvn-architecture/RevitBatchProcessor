//
// Revit Batch Processor
//
// Copyright (c) 2020  Daniel Rumery, BVN
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace BatchRvtUtil;

public static class RevitVersion
{
    public enum SupportedRevitVersion
    {
        Revit2015 = 0,
        Revit2016 = 1,
        Revit2017 = 2,
        Revit2018 = 3,
        Revit2019 = 4,
        Revit2020 = 5,
        Revit2021 = 6,
        Revit2022 = 7,
        Revit2023 = 8
    }

    private const string REVIT_EXECUTABLE_FILE_NAME = "Revit.exe";

    private static string GetVersionNumber(SupportedRevitVersion supportedRevitVersion)
    {
        var versionName = Enum.GetName(typeof(SupportedRevitVersion), supportedRevitVersion);
        return versionName?.Remove(0, 5);
    }

    private static string GetAddinPath(SupportedRevitVersion supportedRevitVersion)
    {
        return $".\\Autodesk\\Revit\\Addins\\{GetVersionNumber(supportedRevitVersion)}";
    }

    internal static string GetAddinName(SupportedRevitVersion supportedRevitVersion)
    {
        return $"BatchRvtAddin{GetVersionNumber(supportedRevitVersion)}.addin";
    }

    private static Dictionary<SupportedRevitVersion, string> REVIT_EXECUTABLE_FOLDER_PATHS()
    {
        return (from versionName in Enum.GetNames(typeof(SupportedRevitVersion))
            select (SupportedRevitVersion)Enum.Parse(typeof(SupportedRevitVersion), versionName)
            into enumOfVersion
            let installLocation = GetRevitInstallPath(enumOfVersion)
            where installLocation != null
            select enumOfVersion).ToDictionary(enumOfVersion => enumOfVersion, GetRevitInstallPath);
    }

    private static string GetRevitInstallPath(SupportedRevitVersion supportedRevitVersion)
    {
        var appPath = $@"SOFTWARE\Autodesk\Revit\{GetVersionNumber(supportedRevitVersion)}";
        if (appPath == null) throw new ArgumentNullException(nameof(appPath));
        using var sk = Registry.LocalMachine.OpenSubKey(appPath);
        if (sk is null) return null;

        string revitSubkey = null;
        foreach (var revitKey in sk.GetSubKeyNames())
        {
            if (!revitKey.Contains("REVIT-")) continue;

            revitSubkey = revitKey;
        }

        if (revitSubkey == null) return null;

        using var rk = sk.OpenSubKey(revitSubkey);
        var installLocation = rk?.GetValue("InstallationLocation");
        return installLocation?.ToString();
    }

    private static string GetRevitExecutableFolderPath(SupportedRevitVersion revitVersion)
    {
        if (GetRevitInstallPath(revitVersion) == null) return null;
        return File.Exists(Path.Combine(GetRevitInstallPath(revitVersion) ?? string.Empty,
            REVIT_EXECUTABLE_FILE_NAME))
            ? GetRevitInstallPath(revitVersion)
            : null;
    }

    public static IEnumerable<SupportedRevitVersion> GetInstalledRevitVersions()
    {
        return REVIT_EXECUTABLE_FOLDER_PATHS().Keys
            .Where(IsRevitVersionInstalled)
            .Where(BatchRvt.IsBatchRvtAddinInstalled)
            .ToList();
    }

    private static string GetRevitExecutableFilePath(SupportedRevitVersion revitVersion)
    {
        var folderPath = GetRevitExecutableFolderPath(revitVersion);

        return folderPath != null ? Path.Combine(folderPath, REVIT_EXECUTABLE_FILE_NAME) : null;
    }

    private static bool IsRevitVersionInstalled(SupportedRevitVersion revitVersion)
    {
        return File.Exists(GetRevitExecutableFilePath(revitVersion));
    }

    public static SupportedRevitVersion GetMinimumInstalledRevitVersion()
    {
        return GetInstalledRevitVersions().OrderBy(supportedRevitVersion => supportedRevitVersion).FirstOrDefault();
    }


    public static string GetRevitVersionText(SupportedRevitVersion supportedRevitVersion)
    {
        return GetVersionNumber(supportedRevitVersion) ?? "UNSUPPORTED";
    }

    public static bool IsSupportedRevitVersionNumber(string revitVersionNumber)
    {
        return Enum.TryParse($"Revit{revitVersionNumber}", out SupportedRevitVersion _);
    }

    public static SupportedRevitVersion GetSupportedRevitVersion(string revitVersionNumber)
    {
        Enum.TryParse($"Revit{revitVersionNumber}", out SupportedRevitVersion version);
        return version;
    }

    public static string GetRevitAddinsFolderPath(SupportedRevitVersion revitVersion,
        Environment.SpecialFolder specialFolder)
    {
        return Path.Combine(Environment.GetFolderPath(specialFolder), GetAddinPath(revitVersion));
    }
}