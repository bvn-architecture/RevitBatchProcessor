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
using BatchRvt.ScriptHost;
using ScriptingHosting = Microsoft.Scripting.Hosting;
using IronPythonHosting = IronPython.Hosting;

namespace BatchRvtUtil;

public static class PathUtil
{
    private static ScriptingHosting.ScriptEngine _engine;
    private static ScriptingHosting.ScriptScope _pathUtilModuleScope;
    private static ScriptingHosting.ScriptScope _revitFileListModuleScope;
    private static ScriptingHosting.ScriptScope _revitFileVersionModuleScope;
    private static dynamic _pythonFunctionExpandedFullNetworkPath;
    private static dynamic _pythonFunctionRevitFileInfo;
    private static dynamic _pythonFunctionGetRevitVersionNumberTextFromRevitVersionText;

    public static bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    public static bool DirectoryExists(string folderPath)
    {
        return Directory.Exists(folderPath);
    }

    public static string GetFullPath(string path)
    {
        return Path.GetFullPath(path);
    }

    public static string GetLocalAppDataFolderPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }

    public static string GetExistingFileDirectoryPath(string existingFilePath)
    {
        string initialDirectory = null;

        if (File.Exists(existingFilePath)) initialDirectory = Path.GetDirectoryName(existingFilePath);

        return initialDirectory;
    }

    public static bool HasExtension(string filePath, string extension)
    {
        return Path.GetExtension(filePath).ToLower() == extension.ToLower();
    }

    private static T IgnoringPathExceptions<T>(Func<T> func)
    {
        var result = default(T);

        try
        {
            result = func();
        }
        catch (UnauthorizedAccessException e)
        {
            // Do nothing.
        }
        catch (PathTooLongException e)
        {
            // Do nothing.
        }
        catch (IOException e)
        {
            // Do nothing.
        }

        return result;
    }

    public static IEnumerable<string> SafeEnumerateFiles(string root, string pattern, SearchOption searchOption)
    {
        // NOTE: DirectoryInfo can throw path-related exceptions, hence the use of IgnoringPathExceptions here.
        return IgnoringPathExceptions(
            () => { return SafeEnumerateFiles(new DirectoryInfo(root), pattern, searchOption); }
        ) ?? Enumerable.Empty<string>();
    }

    // See https://stackoverflow.com/questions/13130052/directoryinfo-enumeratefiles-causes-unauthorizedaccessexception-and-other
    private static IEnumerable<string> SafeEnumerateFiles(DirectoryInfo root, string pattern,
        SearchOption searchOption)
    {
        if (root is not { Exists: true }) yield break;
        var topLevelFilePaths = IgnoringPathExceptions(
            () =>
            {
                return root
                    .EnumerateFiles(pattern, SearchOption.TopDirectoryOnly)
                    .Select(fileInfo => fileInfo.FullName)
                    .ToList(); // Ensures these are fully enumerate here so any exceptions are caught.
            }
        );

        foreach (var filePath in topLevelFilePaths ?? Enumerable.Empty<string>()) yield return filePath;

        if (searchOption != SearchOption.AllDirectories) yield break;
        {
            var subFolderFilePaths = IgnoringPathExceptions(
                () =>
                {
                    var topLevelFolderPaths = root
                        .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                        .ToList(); // Ensures these are fully enumerate here so any exceptions are caught.

                    return topLevelFolderPaths.SelectMany(dir =>
                        SafeEnumerateFiles(dir, pattern, searchOption));
                }
            );

            foreach (var filePath in subFolderFilePaths ?? Enumerable.Empty<string>()) yield return filePath;
        }
    }

    public static IEnumerable<string> SafeEnumerateFolders(string root, string pattern, SearchOption searchOption)
    {
        // NOTE: DirectoryInfo can throw path-related exceptions, hence the use of IgnoringPathExceptions here.
        return IgnoringPathExceptions(
            () => SafeEnumerateFolders(new DirectoryInfo(root), pattern, searchOption)) ?? Enumerable.Empty<string>();
    }

    // See https://stackoverflow.com/questions/13130052/directoryinfo-enumeratefiles-causes-unauthorizedaccessexception-and-other
    private static IEnumerable<string> SafeEnumerateFolders(DirectoryInfo root, string pattern,
        SearchOption searchOption)
    {
        if (root is not { Exists: true }) yield break;
        var topLevelFolderPaths = IgnoringPathExceptions(
            () =>
            {
                return root
                    .EnumerateDirectories(pattern, SearchOption.TopDirectoryOnly)
                    .Select(folderInfo => folderInfo.FullName)
                    .ToList(); // Ensures these are fully enumerate here so any exceptions are caught.
            }
        );

        foreach (var folderPath in topLevelFolderPaths ?? Enumerable.Empty<string>()) yield return folderPath;

        if (searchOption != SearchOption.AllDirectories) yield break;
        {
            var subFolderFolderPaths = IgnoringPathExceptions(
                () =>
                {
                    var subFolderPaths = root
                        .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                        .ToList(); // Ensures these are fully enumerate here so any exceptions are caught.

                    return subFolderPaths.SelectMany(dir => SafeEnumerateFolders(dir, pattern, searchOption));
                }
            );

            foreach (var folderPath in subFolderFolderPaths ?? Enumerable.Empty<string>())
                yield return folderPath;
        }
    }

    private static void InitPythonFunctions()
    {
        var needToAddSearchPath = _engine == null;

        _engine = _engine ?? ScriptUtil.CreatePythonEngine();

        if (needToAddSearchPath)
        {
            var scriptsFolderPath = BatchRvt.GetBatchRvtScriptsFolderPath();

            ScriptUtil.AddSearchPaths(_engine, new[] { scriptsFolderPath });
        }

        _pathUtilModuleScope = _pathUtilModuleScope ?? IronPythonHosting.Python.ImportModule(_engine, "path_util");
        _pythonFunctionExpandedFullNetworkPath = _pythonFunctionExpandedFullNetworkPath ??
                                                  _pathUtilModuleScope.GetVariable("ExpandedFullNetworkPath");

        _revitFileListModuleScope = _revitFileListModuleScope ??
                                   IronPythonHosting.Python.ImportModule(_engine, "revit_file_list");
        _pythonFunctionRevitFileInfo =
            _pythonFunctionRevitFileInfo ?? _revitFileListModuleScope.GetVariable("RevitFileInfo");

        _revitFileVersionModuleScope = _revitFileVersionModuleScope ??
                                      IronPythonHosting.Python.ImportModule(_engine, "revit_file_version");
        _pythonFunctionGetRevitVersionNumberTextFromRevitVersionText =
            _pythonFunctionGetRevitVersionNumberTextFromRevitVersionText ??
            _revitFileVersionModuleScope.GetVariable("GetRevitVersionNumberTextFromRevitVersionText");
    }

    private static string ExpandedFullNetworkPath(string fullPath)
    {
        return _pythonFunctionExpandedFullNetworkPath(fullPath) as string ?? string.Empty;
    }

    public static IEnumerable<string> EnumerateExpandedFullNetworkPaths(IEnumerable<string> fullPaths)
    {
        InitPythonFunctions();

        return fullPaths.Select(ExpandedFullNetworkPath);
    }

    private static string[] GetRevitVersionTexts(string fullPath)
    {
        var revitVersionText = _pythonFunctionRevitFileInfo(fullPath).TryGetRevitVersionText() as string ??
                               string.Empty;
        var revitVersionNumberText =
            _pythonFunctionGetRevitVersionNumberTextFromRevitVersionText(revitVersionText) as string ??
            string.Empty;

        return new[] { revitVersionNumberText, revitVersionText };
    }

    public static IEnumerable<string[]> EnumerateRevitVersionTexts(IEnumerable<string> fullPaths)
    {
        InitPythonFunctions();

        return fullPaths.Select(GetRevitVersionTexts);
    }
}