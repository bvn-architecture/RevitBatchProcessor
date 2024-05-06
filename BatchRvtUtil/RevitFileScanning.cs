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
using System.Text.RegularExpressions;

namespace BatchRvtUtil;

public static class RevitFileScanning
{
    public enum RevitFileType
    {
        Project,
        Family,
        ProjectAndFamily
    }

    private const string ALL_FILES_WITH_AN_EXTENSION_PATTERN = "*.*";

    private const string REVIT_PROJECT_FILE_EXTENSION = ".rvt";
    private const string REVIT_PROJECT_FILE_PATTERN = "*" + REVIT_PROJECT_FILE_EXTENSION;
    private const string REVIT_FAMILY_FILE_EXTENSION = ".rfa";
    private const string REVIT_FAMILY_FILE_PATTERN = "*" + REVIT_FAMILY_FILE_EXTENSION;

    public static IEnumerable<string[]> FindAndExtractRevitFilesInfoWithProgressReporting(
        string baseFolderPath,
        SearchOption searchOption,
        RevitFileType revitFileType,
        bool expandNetworkPaths,
        bool extractRevitVersionInfo,
        bool ignoreRevitBackupFiles,
        Func<string, bool> progressReporter
    )
    {
        var infoRows = new List<string[]>();

        progressReporter("Scanning for Revit files ...");

        var revitFilePaths = FindRevitFiles(baseFolderPath, searchOption, revitFileType, ignoreRevitBackupFiles,
            progressReporter);

        var filePaths = revitFilePaths as string[] ?? revitFilePaths.ToArray();
        var numberOfRevitFilePaths = filePaths.Count();

        var cancelled = progressReporter(string.Empty);

        if (cancelled) return infoRows;

        if (expandNetworkPaths)
        {
            const string expandingNetworkPathsMessagePrefix = "Expanding network paths";

            var indexedExpandedRevitFilePaths =
                PathUtil.EnumerateExpandedFullNetworkPaths(filePaths)
                    .Select((revitFilePath, index) => Tuple.Create(index, revitFilePath));

            var expandedRevitFilePaths = new List<string>();

            foreach (var (index, expandedRevitFilePath) in indexedExpandedRevitFilePaths)
            {
                progressReporter(expandingNetworkPathsMessagePrefix + " (" + (index + 1) + " of " +
                                 numberOfRevitFilePaths + ") ...");

                expandedRevitFilePaths.Add(expandedRevitFilePath);
            }

            revitFilePaths = expandedRevitFilePaths;
        }

        infoRows = filePaths.Select(revitFilePath => new[] { revitFilePath }).ToList();

        if (extractRevitVersionInfo) return infoRows;

        const string extractingNetworkPathsMessagePrefix = "Extracting Revit files version information";

        var indexedRevitVersionTexts =
            PathUtil.EnumerateRevitVersionTexts(filePaths)
                .Select((revitFilePath, index) => Tuple.Create(index, revitFilePath));

        var allRevitVersionTexts = new List<string[]>();

        foreach (var (index, revitVersionTexts) in indexedRevitVersionTexts)
        {
            progressReporter(extractingNetworkPathsMessagePrefix + " (" + (index + 1) + " of " +
                             numberOfRevitFilePaths + ") ...");

            allRevitVersionTexts.Add(revitVersionTexts);
        }

        infoRows = filePaths
            .Zip(
                allRevitVersionTexts,
                (revitFilePath, revitVersionTexts) =>
                    new[] { revitFilePath, revitVersionTexts[0], revitVersionTexts[1] }
            )
            .ToList();


        return infoRows;
    }

    private static IEnumerable<string> FindRevitFiles(
        string folderPath,
        SearchOption searchOption,
        RevitFileType revitFileType,
        bool ignoreRevitBackups,
        Func<string, bool> progressReporter
    )
    {
        var searchFilePattern = ALL_FILES_WITH_AN_EXTENSION_PATTERN;

        if (revitFileType == RevitFileType.Project)
            searchFilePattern = REVIT_PROJECT_FILE_PATTERN;
        else if (revitFileType == RevitFileType.Family)
            searchFilePattern = REVIT_FAMILY_FILE_PATTERN;
        else if (revitFileType == RevitFileType.ProjectAndFamily)
            searchFilePattern = ALL_FILES_WITH_AN_EXTENSION_PATTERN;

        IEnumerable<string> foldersToScan;

        if (searchOption == SearchOption.AllDirectories)
            foldersToScan = new[] { folderPath }
                .Concat(PathUtil.SafeEnumerateFolders(folderPath, "*", SearchOption.AllDirectories));
        else
            foldersToScan = new[] { folderPath };

        var revitFilePaths = new List<string>();

        foreach (var folderToScan in foldersToScan)
        {
            var cancelled = progressReporter(folderToScan);

            if (cancelled) break;

            revitFilePaths.AddRange(
                PathUtil.SafeEnumerateFiles(folderToScan, searchFilePattern, SearchOption.TopDirectoryOnly)
                    .Where(filePath => HasRevitFileExtension(filePath, ignoreRevitBackups))
            );
        }

        return revitFilePaths;
    }

    private static bool HasRevitFileExtension(string filePath, bool ignoreRevitBackups)
    {
        var extension = Path.GetExtension(filePath).ToLower();

        if (ignoreRevitBackups && IsBackupFile(filePath))
            return false;
        return new[]
        {
            REVIT_PROJECT_FILE_EXTENSION,
            REVIT_FAMILY_FILE_EXTENSION
        }.Any(revitExtension => extension == revitExtension.ToLower());
    }

    private static bool IsBackupFile(string filePath)
    {
        const string pattern = "\\.\\d\\d\\d\\d\\.(rvt|rfa)$";
        return Regex.IsMatch(filePath, pattern);
        // This is a backup version of the file (ie. .0001.rvt/rfa) so skip it.
    }
}