//
// Revit Batch Processor
//
// Copyright (c) 2017  Daniel Rumery, BVN
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
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;

namespace BatchRvtUtil
{
    public static class ScriptDataUtil
    {
        private const string SCRIPT_DATA_FILENAME_PREFIX = "Session.ScriptData.";
        private const string SCRIPT_DATA_FILE_EXTENSION = ".json";

        public class ScriptData : IPersistent
        {
            private readonly PersistentSettings persistentSettings;

            public readonly StringSetting SessionId = new StringSetting("sessionId");
            public readonly StringSetting RevitFilePath = new StringSetting("revitFilePath");
            public readonly BooleanSetting EnableDataExport = new BooleanSetting("enableDataExport");
            public readonly StringSetting SessionDataFolderPath = new StringSetting("sessionDataFolderPath");
            public readonly StringSetting DataExportFolderPath = new StringSetting("dataExportFolderPath");
            public readonly BooleanSetting ShowMessageBoxOnTaskScriptError = new BooleanSetting("showMessageBoxOnTaskError");
            public readonly EnumSetting<BatchRvt.RevitProcessingOption> RevitProcessingOption = new EnumSetting<BatchRvt.RevitProcessingOption>("revitProcessingOption");
            public readonly EnumSetting<BatchRvt.CentralFileOpenOption> CentralFileOpenOption = new EnumSetting<BatchRvt.CentralFileOpenOption>("centralFileOpenOption");
            public readonly BooleanSetting DeleteLocalAfter = new BooleanSetting("deleteLocalAfter");
            public readonly IntegerSetting ProgressNumber = new IntegerSetting("progressNumber");
            public readonly IntegerSetting ProgressMax = new IntegerSetting("progressMax");

            public ScriptData()
            {
                this.persistentSettings = new PersistentSettings(
                        new IPersistent[]
                        {
                            this.SessionId,
                            this.RevitFilePath,
                            this.ShowMessageBoxOnTaskScriptError,
                            this.EnableDataExport,
                            this.SessionDataFolderPath,
                            this.DataExportFolderPath,
                            this.RevitProcessingOption,
                            this.CentralFileOpenOption,
                            this.DeleteLocalAfter,
                            this.ProgressNumber,
                            this.ProgressMax
                        }
                    );
            }

            public void Load(JObject jobject)
            {
                this.persistentSettings.Load(jobject);
            }

            public void Store(JObject jobject)
            {
                this.persistentSettings.Store(jobject);
            }

            public bool LoadFromFile(string filePath)
            {
                bool success = false;

                if (File.Exists(filePath))
                {
                    try
                    {
                        var text = File.ReadAllText(filePath);
                        var jobject = JsonUtil.DeserializeFromJson(text);
                        this.persistentSettings.Load(jobject);
                        success = true;
                    }
                    catch (Exception e)
                    {
                        success = false;
                    }
                }

                return success;
            }

            public bool SaveToFile(string filePath)
            {
                bool success = false;

                var jobject = new JObject();

                try
                {
                    this.persistentSettings.Store(jobject);
                    var settingsText = JsonUtil.SerializeToJson(jobject, true);
                    var fileInfo = new FileInfo(filePath);
                    fileInfo.Directory.Create();
                    File.WriteAllText(fileInfo.FullName, settingsText);

                    success = true;
                }
                catch (Exception e)
                {
                    success = false;
                }

                return success;
            }
        }

        public static IEnumerable<ScriptData> LoadManyFromFile(string filePath)
        {
            List<ScriptData> scriptDatas = null;

            if (File.Exists(filePath))
            {
                try
                {
                    var text = File.ReadAllText(filePath);

                    var jarray = JsonUtil.DeserializeArrayFromJson(text);

                    scriptDatas = new List<ScriptData>();

                    foreach (var jtoken in jarray)
                    {
                        var jobject = jtoken as JObject;

                        if (jobject != null)
                        {
                            var scriptData = new ScriptData();

                            scriptData.Load(jobject);

                            scriptDatas.Add(scriptData);
                        }
                    }
                }
                catch (Exception e)
                {
                    scriptDatas = null; // null on failure.
                }
            }

            return scriptDatas;
        }

        public static bool SaveManyToFile(string filePath, IEnumerable<ScriptData> scriptDatas)
        {
            bool success = false;

            try
            {
                var jarray = new JArray();

                foreach (var scriptData in scriptDatas)
                {
                    var jobject = new JObject();

                    scriptData.Store(jobject);

                    jarray.Add(jobject);
                }

                var settingsText = JsonUtil.SerializeToJson(jarray, true);
                
                var fileInfo = new FileInfo(filePath);
                
                fileInfo.Directory.Create();

                File.WriteAllText(fileInfo.FullName, settingsText);

                success = true;
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        public static string GetUniqueScriptDataFilePath()
        {
            return Path.Combine(
                    BatchRvt.GetDataFolderPath(),
                    SCRIPT_DATA_FILENAME_PREFIX + Guid.NewGuid().ToString() + SCRIPT_DATA_FILE_EXTENSION
                );
        }
    }
}
