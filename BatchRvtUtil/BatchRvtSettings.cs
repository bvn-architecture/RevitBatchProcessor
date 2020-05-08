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
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;

namespace BatchRvtUtil
{
    public class BatchRvtSettings : IPersistent
    {
        public const string SETTINGS_FILE_EXTENSION = ".json";
        public const string SETTINGS_FILE_FILTER = "BatchRvt Settings files (*.json)|*.json";
        public const string BATCHRVTGUI_SETTINGS_FILENAME = "BatchRvtGui.Settings" + SETTINGS_FILE_EXTENSION;
        public const string BATCHRVT_SETTINGS_FILENAME = "BatchRvt.Settings" + SETTINGS_FILE_EXTENSION;

        private readonly PersistentSettings persistentSettings;

        // General Task Script settings
        public readonly StringSetting TaskScriptFilePath = new StringSetting("taskScriptFilePath");
        public readonly BooleanSetting ShowMessageBoxOnTaskScriptError = new BooleanSetting("showMessageBoxOnTaskScriptError");
        public readonly IntegerSetting ProcessingTimeOutInMinutes = new IntegerSetting("processingTimeOutInMinutes");
        public readonly BooleanSetting ShowRevitProcessErrorMessages = new BooleanSetting("showRevitProcessErrorMessages");

        // Revit File List settings
        public readonly StringSetting RevitFileListFilePath = new StringSetting("revitFileListFilePath");

        // Data Export settings
        public readonly BooleanSetting EnableDataExport = new BooleanSetting("enableDataExport");
        public readonly StringSetting DataExportFolderPath = new StringSetting("dataExportFolderPath");

        // Pre-processing Script settings
        public readonly BooleanSetting ExecutePreProcessingScript = new BooleanSetting("executePreProcessingScript");
        public readonly StringSetting PreProcessingScriptFilePath = new StringSetting("preProcessingScriptFilePath");
        
        // Post-processing Script settings
        public readonly BooleanSetting ExecutePostProcessingScript = new BooleanSetting("executePostProcessingScript");
        public readonly StringSetting PostProcessingScriptFilePath = new StringSetting("PostProcessingScriptFilePath");
        
        // Central File Processing settings
        public readonly EnumSetting<BatchRvt.CentralFileOpenOption> CentralFileOpenOption = new EnumSetting<BatchRvt.CentralFileOpenOption>("centralFileOpenOption");
        public readonly BooleanSetting DeleteLocalAfter = new BooleanSetting("deleteLocalAfter");
        public readonly BooleanSetting DiscardWorksetsOnDetach = new BooleanSetting("discardWorksetsOnDetach");
        public readonly EnumSetting<BatchRvt.WorksetConfigurationOption> WorksetConfigurationOption = new EnumSetting<BatchRvt.WorksetConfigurationOption>("worksetConfigurationOption");

        // Revit Session settings
        public readonly EnumSetting<BatchRvt.RevitSessionOption> RevitSessionOption = new EnumSetting<BatchRvt.RevitSessionOption>("revitSessionOption");

        // Revit Processing settings
        public readonly EnumSetting<BatchRvt.RevitProcessingOption> RevitProcessingOption = new EnumSetting<BatchRvt.RevitProcessingOption>("revitProcessingOption");

        // Single Revit Task Processing settings
        public readonly EnumSetting<RevitVersion.SupportedRevitVersion> SingleRevitTaskRevitVersion = new EnumSetting<RevitVersion.SupportedRevitVersion>("singleRevitTaskRevitVersion");

        // Batch Revit File Processing settings
        public readonly EnumSetting<BatchRvt.RevitFileProcessingOption> RevitFileProcessingOption = new EnumSetting<BatchRvt.RevitFileProcessingOption>("revitFileProcessingOption");
        public readonly BooleanSetting IfNotAvailableUseMinimumAvailableRevitVersion = new BooleanSetting("ifNotAvailableUseMinimumAvailableRevitVersion");
        public readonly EnumSetting<RevitVersion.SupportedRevitVersion> BatchRevitTaskRevitVersion = new EnumSetting<RevitVersion.SupportedRevitVersion>("batchRevitTaskRevitVersion");
        public readonly BooleanSetting OpenInUI = new BooleanSetting("openInUI");
        public readonly BooleanSetting AuditOnOpening = new BooleanSetting("auditOnOpening");

        // UI settings
        public readonly BooleanSetting ShowAdvancedSettings = new BooleanSetting("showAdvancedSettings");

        public BatchRvtSettings()
        {
            this.persistentSettings = new PersistentSettings(
                    new IPersistent[] {
                        this.TaskScriptFilePath,
                        this.ShowMessageBoxOnTaskScriptError,
                        this.ProcessingTimeOutInMinutes,
                        this.ShowRevitProcessErrorMessages,
                        this.RevitFileListFilePath,
                        this.EnableDataExport,
                        this.DataExportFolderPath,
                        this.ExecutePreProcessingScript,
                        this.PreProcessingScriptFilePath,
                        this.ExecutePostProcessingScript,
                        this.PostProcessingScriptFilePath,
                        this.CentralFileOpenOption,
                        this.DeleteLocalAfter,
                        this.DiscardWorksetsOnDetach,
                        this.WorksetConfigurationOption,
                        this.RevitSessionOption,
                        this.RevitProcessingOption,
                        this.SingleRevitTaskRevitVersion,
                        this.RevitFileProcessingOption,
                        this.IfNotAvailableUseMinimumAvailableRevitVersion,
                        this.BatchRevitTaskRevitVersion,
                        this.OpenInUI,
                        this.AuditOnOpening,
                        this.ShowAdvancedSettings
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

        public bool TryLoad(JObject jobject)
        {
            bool success = false;

            try
            {
                this.Load(jobject);
                success = true;
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        public bool TryStore(JObject jobject)
        {
            bool success = false;

            try
            {
                this.Store(jobject);
                success = true;
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        public bool LoadFromFile(string filePath = null)
        {
            bool success = false;

            filePath = string.IsNullOrWhiteSpace(filePath) ? GetDefaultSettingsFilePath() : filePath;

            if (File.Exists(filePath))
            {
                try
                {
                    var text = File.ReadAllText(filePath);
                    var jobject = JsonUtil.DeserializeFromJson(text);
                    this.Load(jobject);
                    success = true;
                }
                catch (Exception e)
                {
                    success = false;
                }
            }

            return success;
        }

        public bool SaveToFile(string filePath = null)
        {
            bool success = false;

            filePath = string.IsNullOrWhiteSpace(filePath) ? GetDefaultSettingsFilePath() : filePath;

            var jobject = new JObject();

            try
            {
                this.Store(jobject);
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

        public string ToJsonString()
        {
            var jobject = new JObject();
            this.Store(jobject);
            return jobject.ToString();
        }

        public static BatchRvtSettings FromJsonString(string batchRvtSettingsJson)
        {
            BatchRvtSettings batchRvtSettings = null;

            try
            {
                var jobject = JsonUtil.DeserializeFromJson(batchRvtSettingsJson);
                batchRvtSettings = new BatchRvtSettings();
                batchRvtSettings.Load(jobject);
            }
            catch (Exception e)
            {
                batchRvtSettings = null;
            }

            return batchRvtSettings;
        }

        public static BatchRvtSettings Create(
                string taskScriptFilePath,
                string revitFileListFilePath,
                BatchRvt.RevitProcessingOption revitProcessingOption,
                BatchRvt.CentralFileOpenOption centralFileOpenOption,
                bool deleteLocalAfter,
                bool discardWorksetsOnDetach,
                BatchRvt.RevitSessionOption revitSessionOption,
                BatchRvt.RevitFileProcessingOption revitFileVersionOption,
                RevitVersion.SupportedRevitVersion taskRevitVersion,
                int fileProcessingTimeOutInMinutes,
                bool fallbackToMinimumAvailableRevitVersion
            )
        {
            var batchRvtSettings = new BatchRvtSettings();

            // General Task Script settings
            batchRvtSettings.TaskScriptFilePath.SetValue(taskScriptFilePath);
            batchRvtSettings.ProcessingTimeOutInMinutes.SetValue(fileProcessingTimeOutInMinutes);

            // Revit Processing settings
            batchRvtSettings.RevitProcessingOption.SetValue(revitProcessingOption);

            // Revit File List settings
            batchRvtSettings.RevitFileListFilePath.SetValue(revitFileListFilePath);

            // Central File Processing settings
            batchRvtSettings.CentralFileOpenOption.SetValue(centralFileOpenOption);
            batchRvtSettings.DeleteLocalAfter.SetValue(deleteLocalAfter);
            batchRvtSettings.DiscardWorksetsOnDetach.SetValue(discardWorksetsOnDetach);

            // Revit Session settings
            batchRvtSettings.RevitSessionOption.SetValue(revitSessionOption);

            // Single Revit Task Processing settings
            batchRvtSettings.SingleRevitTaskRevitVersion.SetValue(taskRevitVersion);

            // Batch Revit File Processing settings
            batchRvtSettings.RevitFileProcessingOption.SetValue(revitFileVersionOption);
            batchRvtSettings.IfNotAvailableUseMinimumAvailableRevitVersion.SetValue(fallbackToMinimumAvailableRevitVersion);
            batchRvtSettings.BatchRevitTaskRevitVersion.SetValue(taskRevitVersion);
            batchRvtSettings.AuditOnOpening.SetValue(false); // TODO: implement this option for this function?

            return batchRvtSettings;
        }

        public static string GetDefaultSettingsFilePath()
        {
            return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "BatchRvt",
                    BATCHRVTGUI_SETTINGS_FILENAME
                );
        }
    }
}
