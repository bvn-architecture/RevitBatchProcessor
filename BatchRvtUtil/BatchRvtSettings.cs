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
using System.IO;
using Newtonsoft.Json.Linq;

namespace BatchRvtUtil;

public class BatchRvtSettings : IPersistent
{
    public const string SETTINGS_FILE_EXTENSION = ".json";
    public const string SETTINGS_FILE_FILTER = "BatchRvt Settings files (*.json)|*.json";
    private const string BATCHRVTGUI_SETTINGS_FILENAME = "BatchRvtGui.Settings" + SETTINGS_FILE_EXTENSION;
    public const string BATCHRVT_SETTINGS_FILENAME = "BatchRvt.Settings" + SETTINGS_FILE_EXTENSION;
    public readonly BooleanSetting AuditOnOpening = new("auditOnOpening");

    public readonly EnumSetting<RevitVersion.SupportedRevitVersion> BatchRevitTaskRevitVersion =
        new("batchRevitTaskRevitVersion");

    // Central File Processing settings
    public readonly EnumSetting<BatchRvt.CentralFileOpenOption> CentralFileOpenOption = new("centralFileOpenOption");

    public readonly StringSetting DataExportFolderPath = new("dataExportFolderPath");
    public readonly BooleanSetting DeleteLocalAfter = new("deleteLocalAfter");
    public readonly BooleanSetting DiscardWorksetsOnDetach = new("discardWorksetsOnDetach");

    // Data Export settings
    public readonly BooleanSetting EnableDataExport = new("enableDataExport");

    // Post-processing Script settings
    public readonly BooleanSetting ExecutePostProcessingScript = new("executePostProcessingScript");

    // Pre-processing Script settings
    public readonly BooleanSetting ExecutePreProcessingScript = new("executePreProcessingScript");

    public readonly BooleanSetting IfNotAvailableUseMinimumAvailableRevitVersion =
        new("ifNotAvailableUseMinimumAvailableRevitVersion");

    private readonly BooleanSetting OpenInUI = new("openInUI");

    private readonly PersistentSettings persistentSettings;
    public readonly StringSetting PostProcessingScriptFilePath = new("PostProcessingScriptFilePath");
    public readonly StringSetting PreProcessingScriptFilePath = new("preProcessingScriptFilePath");
    public readonly IntegerSetting ProcessingTimeOutInMinutes = new("processingTimeOutInMinutes");

    // Revit File List settings
    public readonly StringSetting RevitFileListFilePath = new("revitFileListFilePath");

    // Batch Revit File Processing settings
    public readonly EnumSetting<BatchRvt.RevitFileProcessingOption> RevitFileProcessingOption =
        new("revitFileProcessingOption");

    // Revit Processing settings
    public readonly EnumSetting<BatchRvt.RevitProcessingOption> RevitProcessingOption = new("revitProcessingOption");

    // Revit Session settings
    public readonly EnumSetting<BatchRvt.RevitSessionOption> RevitSessionOption = new("revitSessionOption");

    // UI settings
    public readonly BooleanSetting ShowAdvancedSettings = new("showAdvancedSettings");

    public readonly BooleanSetting ShowMessageBoxOnTaskScriptError = new("showMessageBoxOnTaskScriptError");

    public readonly BooleanSetting ShowRevitProcessErrorMessages = new("showRevitProcessErrorMessages");

    // Single Revit Task Processing settings
    public readonly EnumSetting<RevitVersion.SupportedRevitVersion> SingleRevitTaskRevitVersion =
        new("singleRevitTaskRevitVersion");

    // General Task Script settings
    public readonly StringSetting TaskScriptFilePath = new("taskScriptFilePath");

    public readonly EnumSetting<BatchRvt.WorksetConfigurationOption> WorksetConfigurationOption =
        new("worksetConfigurationOption");

    public BatchRvtSettings()
    {
        persistentSettings = new PersistentSettings(
            new IPersistent[]
            {
                TaskScriptFilePath,
                ShowMessageBoxOnTaskScriptError,
                ProcessingTimeOutInMinutes,
                ShowRevitProcessErrorMessages,
                RevitFileListFilePath,
                EnableDataExport,
                DataExportFolderPath,
                ExecutePreProcessingScript,
                PreProcessingScriptFilePath,
                ExecutePostProcessingScript,
                PostProcessingScriptFilePath,
                CentralFileOpenOption,
                DeleteLocalAfter,
                DiscardWorksetsOnDetach,
                WorksetConfigurationOption,
                RevitSessionOption,
                RevitProcessingOption,
                SingleRevitTaskRevitVersion,
                RevitFileProcessingOption,
                IfNotAvailableUseMinimumAvailableRevitVersion,
                BatchRevitTaskRevitVersion,
                OpenInUI,
                AuditOnOpening,
                ShowAdvancedSettings
            }
        );
    }

    public void Load(JObject jobject)
    {
        persistentSettings.Load(jobject);
    }

    public void Store(JObject jobject)
    {
        persistentSettings.Store(jobject);
    }


    public bool LoadFromFile(string filePath = null)
    {
        
        filePath = string.IsNullOrWhiteSpace(filePath) ? GetDefaultSettingsFilePath() : filePath;

        if (!File.Exists(filePath)) return false;
        try
        {
            var text = File.ReadAllText(filePath);
            var jobject = JsonUtil.DeserializeFromJson(text);
            Load(jobject);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
        
    }

    public bool SaveToFile(string filePath = null)
    {
        var success = false;

        filePath = string.IsNullOrWhiteSpace(filePath) ? GetDefaultSettingsFilePath() : filePath;

        var jobject = new JObject();

        try
        {
            Store(jobject);
            var settingsText = JsonUtil.SerializeToJson(jobject, true);
            var fileInfo = new FileInfo(filePath);
            fileInfo.Directory?.Create();
            File.WriteAllText(fileInfo.FullName, settingsText);

            success = true;
        }
        catch (Exception e)
        {
            success = false;
        }

        return success;
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
        batchRvtSettings.IfNotAvailableUseMinimumAvailableRevitVersion.SetValue(
            fallbackToMinimumAvailableRevitVersion);
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