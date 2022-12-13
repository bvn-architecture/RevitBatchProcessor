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

using System.Collections.Generic;
using System.Linq;
using BatchRvt.ScriptHost.Util;
using IronPython.Modules;
using MSScripting = Microsoft.Scripting;
using ScriptingHosting = Microsoft.Scripting.Hosting;
using IronPythonHosting = IronPython.Hosting;

namespace BatchRvt.ScriptHost;

public static class ScriptUtil
{
    private const string PYTHON_LIB_ZIP_NAME = "python_27_lib.zip";

    public static void AddPythonStandardLibrary(ScriptingHosting.ScriptScope scope)
    {
        var thisAssembly = typeof(ScriptUtil).Assembly;
        var pythonLibResourceName = thisAssembly.GetManifestResourceNames()
            .Single(name => name.ToLowerInvariant().EndsWith(PYTHON_LIB_ZIP_NAME.ToLowerInvariant()));
        var importer = new ResourceMetaPathImporter(thisAssembly, pythonLibResourceName);
        dynamic sysModule = IronPythonHosting.Python.GetSysModule(scope.Engine);
        sysModule.meta_path.append(importer);
    }

    private static void AddVariables(ScriptingHosting.ScriptScope scope,
        IEnumerable<KeyValuePair<string, object>> variables)
    {
        foreach (var kv in variables) scope.SetVariable(kv.Key, kv.Value);
    }

    public static void AddBuiltinVariables(ScriptingHosting.ScriptEngine engine,
        IEnumerable<KeyValuePair<string, object>> variables)
    {
        AddVariables(IronPythonHosting.Python.GetBuiltinModule(engine), variables);
    }

    public static void AddSearchPaths(ScriptingHosting.ScriptEngine engine,
        IEnumerable<string> additionalSearchPaths)
    {
        var searchPaths = engine.GetSearchPaths();

        foreach (var path in additionalSearchPaths) searchPaths.Add(path);

        engine.SetSearchPaths(searchPaths);
    }

    public static ScriptingHosting.ScriptEngine CreatePythonEngine()
    {
        var engineOptions = new Dictionary<string, object>
        {
            { "FullFrames", true }
            /*{ "Debug", true },*/
        };

        var engine = IronPythonHosting.Python.CreateEngine(engineOptions);

        return engine;
    }

    public static ScriptingHosting.ScriptScope CreateMainModule(ScriptingHosting.ScriptEngine engine)
    {
        var mainModuleScope = IronPythonHosting.Python.CreateModule(engine, "__main__");

        return mainModuleScope;
    }

    public static ScriptingHosting.ScriptSource CreateScriptSourceFromFile(
        ScriptingHosting.ScriptEngine engine, string sourceFilePath
    )
    {
        var sourceText = TextFileUtil.ReadAllText(sourceFilePath);

        var scriptSource =
            engine.CreateScriptSourceFromString(sourceText, sourceFilePath, MSScripting.SourceCodeKind.Statements);

        return scriptSource;
    }
}