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
        Revit2023 = 8,
        Revit2024 = 9,
        Revit2025 = 10,
    }

    private const string REVIT_EXECUTABLE_FILE_NAME = "Revit.exe";

    private static readonly Dictionary<SupportedRevitVersion, string> REVIT_ADDINS_RELATIVE_PATHS =
        new Dictionary<SupportedRevitVersion, string>()
        {
            { SupportedRevitVersion.Revit2015, @".\Autodesk\Revit\Addins\2015" },
            { SupportedRevitVersion.Revit2016, @".\Autodesk\Revit\Addins\2016" },
            { SupportedRevitVersion.Revit2017, @".\Autodesk\Revit\Addins\2017" },
            { SupportedRevitVersion.Revit2018, @".\Autodesk\Revit\Addins\2018" },
            { SupportedRevitVersion.Revit2019, @".\Autodesk\Revit\Addins\2019" },
            { SupportedRevitVersion.Revit2020, @".\Autodesk\Revit\Addins\2020" },
            { SupportedRevitVersion.Revit2021, @".\Autodesk\Revit\Addins\2021" },
            { SupportedRevitVersion.Revit2022, @".\Autodesk\Revit\Addins\2022" },
            { SupportedRevitVersion.Revit2023, @".\Autodesk\Revit\Addins\2023" },
            { SupportedRevitVersion.Revit2024, @".\Autodesk\Revit\Addins\2024" },
            { SupportedRevitVersion.Revit2025, @".\Autodesk\Revit\Addins\2025" }
        };

    private static readonly Dictionary<SupportedRevitVersion, string> SUPPORTED_REVIT_VERSION_NUMBERS =
        new Dictionary<SupportedRevitVersion, string>()
        {
            { SupportedRevitVersion.Revit2015, "2015" },
            { SupportedRevitVersion.Revit2016, "2016" },
            { SupportedRevitVersion.Revit2017, "2017" },
            { SupportedRevitVersion.Revit2018, "2018" },
            { SupportedRevitVersion.Revit2019, "2019" },
            { SupportedRevitVersion.Revit2020, "2020" },
            { SupportedRevitVersion.Revit2021, "2021" },
            { SupportedRevitVersion.Revit2022, "2022" },
            { SupportedRevitVersion.Revit2023, "2023" },
            { SupportedRevitVersion.Revit2024, "2024" },
            { SupportedRevitVersion.Revit2025, "2025" }
        };

    private static Dictionary<SupportedRevitVersion, string> REVIT_EXECUTABLE_FOLDER_PATHS()
    {
        var revitInstallPaths = new Dictionary<SupportedRevitVersion, string>();
        foreach (var versionName in Enum.GetNames(typeof(SupportedRevitVersion)))
        {
            SupportedRevitVersion enumOfVersion = (SupportedRevitVersion)Enum.Parse(typeof(SupportedRevitVersion), versionName);

            var installLocation = GetRevitInstallPath(enumOfVersion);
            if (installLocation == null)
            {
                continue;
            }
            revitInstallPaths.Add(enumOfVersion, GetRevitInstallPath(enumOfVersion));
        }

        return revitInstallPaths;
    }


    public static string GetVersionNumber(SupportedRevitVersion supportedRevitVersion)
    {
        var versionName = Enum.GetName(typeof(SupportedRevitVersion), supportedRevitVersion);
        return versionName?.Remove(0, 5);
    }

    public static string GetAddinPath(SupportedRevitVersion supportedRevitVersion)
    {
        return $".\\Autodesk\\Revit\\Addins\\{GetVersionNumber(supportedRevitVersion)}";
    }

    public static string GetAddinName(SupportedRevitVersion supportedRevitVersion)
    {
        return $"BatchRvtAddin{GetVersionNumber(supportedRevitVersion)}.addin";
    }


    private static readonly Dictionary<SupportedRevitVersion, string> REVIT_LOCAL_FOLDER_PATHS =
        new Dictionary<SupportedRevitVersion, string>()
        {
            { SupportedRevitVersion.Revit2015, @"C:\REVIT_LOCAL2015" },
            { SupportedRevitVersion.Revit2016, @"C:\REVIT_LOCAL2016" },
            { SupportedRevitVersion.Revit2017, @"C:\REVIT_LOCAL2017" },
            { SupportedRevitVersion.Revit2018, @"C:\REVIT_LOCAL2018" },
            { SupportedRevitVersion.Revit2019, @"C:\REVIT_LOCAL2019" },
            { SupportedRevitVersion.Revit2020, @"C:\REVIT_LOCAL2020" },
            { SupportedRevitVersion.Revit2021, @"C:\REVIT_LOCAL2021" },
            { SupportedRevitVersion.Revit2022, @"C:\REVIT_LOCAL2022" },
            { SupportedRevitVersion.Revit2023, @"C:\REVIT_LOCAL2023" },
            { SupportedRevitVersion.Revit2024, @"C:\REVIT_LOCAL2024" },
            { SupportedRevitVersion.Revit2025, @"C:\REVIT_LOCAL2025" },
        };


    public static string GetRevitInstallPath(SupportedRevitVersion supportedRevitVersion)
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

    public static string GetRevitExecutableFolderPath(SupportedRevitVersion revitVersion)
    {
        if (GetRevitInstallPath(revitVersion) == null) return null;
        return File.Exists(Path.Combine(GetRevitInstallPath(revitVersion) ?? string.Empty,
            REVIT_EXECUTABLE_FILE_NAME))
            ? GetRevitInstallPath(revitVersion)
            : null;
    }

    public static string GetRevitLocalFolderPath(SupportedRevitVersion revitVersion)
    {
        return REVIT_LOCAL_FOLDER_PATHS.ContainsKey(revitVersion) ?
            REVIT_LOCAL_FOLDER_PATHS[revitVersion] : null;
    }

    public static string GetRevitLocalFilePath(SupportedRevitVersion revitVersion, string centralFilePath)
    {
        string localFilePath = null;

        var localFolderPath = GetRevitLocalFolderPath(revitVersion);

        if (localFolderPath != null)
        {
            var localFileName = Path.GetFileNameWithoutExtension(centralFilePath) + "_" + Environment.UserName + Path.GetExtension(centralFilePath);

            localFilePath = Path.Combine(localFolderPath, localFileName);
        }

        return localFilePath;
    }

    public static IEnumerable<SupportedRevitVersion> GetInstalledRevitVersions()
    {
        return REVIT_EXECUTABLE_FOLDER_PATHS().Keys
            .Where(IsRevitVersionInstalled)
            .Where(BatchRvt.IsBatchRvtAddinInstalled)
            .ToList();
    }

    public static string GetRevitExecutableFilePath(SupportedRevitVersion revitVersion)
    {
        var folderPath = GetRevitExecutableFolderPath(revitVersion);

        return folderPath != null ? Path.Combine(folderPath, REVIT_EXECUTABLE_FILE_NAME) : null;
    }

    public static bool IsRevitVersionInstalled(SupportedRevitVersion revitVersion)
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