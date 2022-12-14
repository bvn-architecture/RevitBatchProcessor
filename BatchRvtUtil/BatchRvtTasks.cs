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
using System.Diagnostics;
using System.IO;

namespace BatchRvtUtil;

public static class BatchRvtTasks
{
    // NOTE: Dynamo scripts are not supported in Revit versions earlier than 2016.
    public enum UseRevitVersion
    {
        RevitFileVersion = 0,
        Revit2015 = 1,
        Revit2016 = 2,
        Revit2017 = 3,
        Revit2018 = 4,
        Revit2019 = 5,
        Revit2020 = 6,
        Revit2021 = 7,
        Revit2022 = 8,
        Revit2023 = 9
    }

    public static string RunTask(
        string taskScriptFilePath,
        IEnumerable<string> revitFileList,
        UseRevitVersion useRevitVersion,
        BatchRvt.CentralFileOpenOption centralFileOpenOption,
        bool discardWorksetsOnDetach,
        bool deleteLocalAfter,
        bool openLogFileWhenDone,
        string taskData,
        string testModeFolderPath
    )
    {
        return RunTask(
            taskScriptFilePath,
            revitFileList, // Input is a list of Revit file paths.
            BatchRvt.RevitProcessingOption.BatchRevitFileProcessing,
            useRevitVersion,
            centralFileOpenOption,
            discardWorksetsOnDetach,
            deleteLocalAfter,
            openLogFileWhenDone,
            null,
            0,
            false,
            taskData,
            testModeFolderPath
        );
    }

    public static string RunSingleTask(
        string taskScriptFilePath,
        UseRevitVersion useRevitVersion,
        bool openLogFileWhenDone,
        string taskData,
        string testModeFolderPath
    )
    {
        if (useRevitVersion == UseRevitVersion.RevitFileVersion)
            throw new ArgumentException("useRevitVersion argument must specify a specific Revit version!");

        return RunTask(
            taskScriptFilePath,
            null, // Revit File list is N/A for single task processing
            BatchRvt.RevitProcessingOption.SingleRevitTaskProcessing,
            useRevitVersion,
            BatchRvt.CentralFileOpenOption.Detach, // N/A for single task processing
            true, // N/A for single task processing
            true, // N/A for single task processing
            openLogFileWhenDone,
            null,
            0, // N/A for single task processing
            false,
            taskData,
            testModeFolderPath
        );
    }

    public static string RunTaskAdvanced(
        string taskScriptFilePath,
        IEnumerable<string> revitFileList,
        UseRevitVersion useRevitVersion,
        BatchRvt.CentralFileOpenOption centralFileOpenOption,
        bool discardWorksetsOnDetach,
        bool deleteLocalAfter,
        bool openLogFileWhenDone,
        string logFolderPath,
        int fileProcessingTimeOutInMinutes,
        bool fallbackToMinimumAvailableRevitVersion,
        string taskData,
        string testModeFolderPath
    )
    {
        return RunTask(
            taskScriptFilePath,
            revitFileList, // Input is a list of Revit file paths.
            BatchRvt.RevitProcessingOption.BatchRevitFileProcessing,
            useRevitVersion,
            centralFileOpenOption,
            discardWorksetsOnDetach,
            deleteLocalAfter,
            openLogFileWhenDone,
            logFolderPath,
            fileProcessingTimeOutInMinutes,
            fallbackToMinimumAvailableRevitVersion,
            taskData,
            testModeFolderPath
        );
    }

    public static string RunTaskFromSettingsFile(
        string settingsFilePath,
        string logFolderPath,
        bool openLogFileWhenDone,
        string taskData = null,
        string testModeFolderPath = null
    )
    {
        var commandSettingsData = new CommandSettings.Data
        {
            SettingsFilePath = settingsFilePath,
            LogFolderPath = logFolderPath,
            TaskData = taskData,
            TestModeFolderPath = testModeFolderPath
        };

        return RunTask(commandSettingsData, openLogFileWhenDone);
    }

    public static string RunTask(
        string taskScriptFilePath,
        object revitFileListInput,
        BatchRvt.RevitProcessingOption revitProcessingOption,
        UseRevitVersion useRevitVersion,
        BatchRvt.CentralFileOpenOption centralFileOpenOption,
        bool discardWorksetsOnDetach,
        bool deleteLocalAfter,
        bool openLogFileWhenDone,
        string logFolderPath,
        int fileProcessingTimeOutInMinutes,
        bool fallbackToMinimumAvailableRevitVersion,
        string taskData,
        string testModeFolderPath
    )
    {
        var batchRvtRevitFileProcessingOption = useRevitVersion == UseRevitVersion.RevitFileVersion
            ? BatchRvt.RevitFileProcessingOption.UseFileRevitVersionIfAvailable
            : BatchRvt.RevitFileProcessingOption.UseSpecificRevitVersion;

        // NOTE: can be any version if useRevitVersion is set to RevitFileVersion.
        var taskRevitVersion = GetVersion(useRevitVersion);

        var batchRvtSettings = BatchRvtSettings.Create(
            taskScriptFilePath,
            revitFileListInput as string ?? string.Empty,
            revitProcessingOption,
            centralFileOpenOption,
            deleteLocalAfter,
            discardWorksetsOnDetach,
            BatchRvt.RevitSessionOption.UseSeparateSessionPerFile,
            batchRvtRevitFileProcessingOption,
            taskRevitVersion,
            fileProcessingTimeOutInMinutes,
            fallbackToMinimumAvailableRevitVersion
        );

        var commandSettingsData = new CommandSettings.Data();

        commandSettingsData.RevitFileList = revitFileListInput as IEnumerable<string>;
        commandSettingsData.Settings = batchRvtSettings;
        commandSettingsData.LogFolderPath = logFolderPath;
        commandSettingsData.TaskData = taskData;
        commandSettingsData.TestModeFolderPath = testModeFolderPath;

        return RunTask(commandSettingsData, openLogFileWhenDone);
    }

    private static RevitVersion.SupportedRevitVersion GetVersion(UseRevitVersion useRevitVersion)
    {
        return (RevitVersion.SupportedRevitVersion)Enum.Parse(typeof(UseRevitVersion), useRevitVersion.ToString());
    }

    private static string RunTask(
        CommandSettings.Data commandSettingsData,
        bool openLogFileWhenDone
    )
    {
        commandSettingsData = ValidateCommandSettingsData(commandSettingsData);

        var batchRvtFolderPath = BatchRvt.GetBatchRvtFolderPath();

        BatchRvt.ExecuteMonitorScript(batchRvtFolderPath, commandSettingsData);

        var logFilePath = commandSettingsData.GeneratedLogFilePath;

        logFilePath = PostProcessLogFile(logFilePath);

        if (!openLogFileWhenDone) return logFilePath;
        if (!string.IsNullOrWhiteSpace(logFilePath))
            Process.Start(logFilePath);

        return logFilePath;
    }

    private static string PostProcessLogFile(string logFilePath)
    {
        if (string.IsNullOrWhiteSpace(logFilePath)) return logFilePath;
        var plainTextLogFilePath = Path.Combine(
            Path.GetDirectoryName(logFilePath) ?? string.Empty,
            Path.GetFileNameWithoutExtension(logFilePath) + ".txt"
        );

        File.WriteAllLines(
            plainTextLogFilePath,
            LogFile.ReadLinesAsPlainText(logFilePath)
        );

        logFilePath = plainTextLogFilePath;

        return logFilePath;
    }

    private static CommandSettings.Data ValidateCommandSettingsData(CommandSettings.Data commandSettingsData)
    {
        if (string.IsNullOrWhiteSpace(commandSettingsData.SettingsFilePath))
            commandSettingsData.SettingsFilePath = null;

        if (string.IsNullOrWhiteSpace(commandSettingsData.LogFolderPath)) commandSettingsData.LogFolderPath = null;

        if (string.IsNullOrWhiteSpace(commandSettingsData.SessionId)) commandSettingsData.SessionId = null;

        if (string.IsNullOrWhiteSpace(commandSettingsData.TaskData)) commandSettingsData.TaskData = null;

        if (string.IsNullOrWhiteSpace(commandSettingsData.TestModeFolderPath))
            commandSettingsData.TestModeFolderPath = null;

        var batchRvtSettings = commandSettingsData.Settings;

        if (batchRvtSettings != null && batchRvtSettings.RevitProcessingOption.GetValue() ==
                                     BatchRvt.RevitProcessingOption.BatchRevitFileProcessing
                                     &&
                                     string.IsNullOrWhiteSpace(batchRvtSettings.RevitFileListFilePath.GetValue())
                                     &&
                                     commandSettingsData.RevitFileList == null)
        {
            throw new ArgumentNullException(@"No Revit file list was specified for Batch processing mode.");
        }
            

        commandSettingsData.GeneratedLogFilePath = null;

        return commandSettingsData;
    }
}