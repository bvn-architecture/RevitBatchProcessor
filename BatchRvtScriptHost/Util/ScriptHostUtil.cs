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
using System.Collections.Specialized;
using System.IO;

namespace BatchRvt.ScriptHost;

public static class ScriptHostUtil
{
    // NOTE: must be the same as BATCH_RVT_ERROR_WINDOW_TITLE defined in script_host_error.py.
    public const string BATCH_RVT_ERROR_WINDOW_TITLE = "BatchRvt Script Error";

    private const string BatchScriptHostFilename = "revit_script_host.py";
    private const string BATCHRVT_SCRIPTS_FOLDER_PATH__ENVIRONMENT_VARIABLE_NAME = "BATCHRVT__SCRIPTS_FOLDER_PATH";

    public static void ExecuteBatchScriptHost(
        string pluginFolderPath,
        object uiApplicationObject
    )
    {
        var environmentVariables = GetEnvironmentVariables();

        if (environmentVariables == null) return;
        var batchRvtScriptsFolderPath = GetBatchRvtScriptsFolderPath(environmentVariables);

        if (batchRvtScriptsFolderPath == null) return;
        var engine = ScriptUtil.CreatePythonEngine();

        ScriptUtil.AddBuiltinVariables(
            engine,
            new Dictionary<string, object>
            {
                { "__revit__", uiApplicationObject },
            });

        var mainModuleScope = ScriptUtil.CreateMainModule(engine);

        var pluginFullFolderPath = Path.GetFullPath(pluginFolderPath);

        var scriptHostFilePath = Path.Combine(batchRvtScriptsFolderPath, BatchScriptHostFilename);
        var batchRvtFolderPath = GetBatchRvtFolderPath(environmentVariables);

        ScriptUtil.AddSearchPaths(engine, new[]
        {
            batchRvtScriptsFolderPath,
            pluginFullFolderPath,
            batchRvtFolderPath
        });

        ScriptUtil.AddPythonStandardLibrary(mainModuleScope);

        var scriptSource = ScriptUtil.CreateScriptSourceFromFile(engine, scriptHostFilePath);

        scriptSource.Execute(mainModuleScope);
    }

    private static string GetParentFolder(string folderPath)
    {
        return Directory.GetParent(folderPath)?.FullName;
    }

    private static string RemoveTrailingDirectorySeparators(string folderPath)
    {
        return folderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private static string GetBatchRvtFolderPath(StringDictionary environmentVariables)
    {
        var batchRvtScriptsFolderPath = GetBatchRvtScriptsFolderPath(environmentVariables);

        return batchRvtScriptsFolderPath != null
            ? GetParentFolder(RemoveTrailingDirectorySeparators(batchRvtScriptsFolderPath))
            : null;
    }

    private static string GetBatchRvtScriptsFolderPath(StringDictionary environmentVariables)
    {
        return GetEnvironmentVariable(
            environmentVariables,
            BATCHRVT_SCRIPTS_FOLDER_PATH__ENVIRONMENT_VARIABLE_NAME
        );
    }

    private static string GetEnvironmentVariable(StringDictionary environmentVariables, string variableName)
    {
        return environmentVariables[variableName];
    }

    private static StringDictionary GetEnvironmentVariables()
    {
        try
        {
            var environmentVariables = new StringDictionary();
            var ev = Environment.GetEnvironmentVariables();
            foreach (string key in ev.Keys)
            {
                environmentVariables[key] = ev[key].ToString();
            }

            return environmentVariables;
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }
}