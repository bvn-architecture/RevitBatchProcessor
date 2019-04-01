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
using System.Diagnostics;

namespace BatchRvtUtil
{
    public static class BatchRvtTasks
    {
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
                    logFolderPath: null,
                    fileProcessingTimeOutInMinutes: 0,
                    fallbackToMinimumAvailableRevitVersion: false,
                    taskData: taskData,
                    testModeFolderPath: testModeFolderPath
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
            {
                throw new ArgumentException("useRevitVersion argument must specify a specific Revit version!");
            }

            return RunTask(
                    taskScriptFilePath,
                    null, // Revit File list is N/A for single task processing
                    BatchRvt.RevitProcessingOption.SingleRevitTaskProcessing,
                    useRevitVersion,
                    BatchRvt.CentralFileOpenOption.Detach,  // N/A for single task processing
                    discardWorksetsOnDetach: true, // N/A for single task processing
                    deleteLocalAfter: true, // N/A for single task processing
                    openLogFileWhenDone: openLogFileWhenDone,
                    logFolderPath: null,
                    fileProcessingTimeOutInMinutes: 0, // N/A for single task processing
                    fallbackToMinimumAvailableRevitVersion: false,
                    taskData: taskData,
                    testModeFolderPath: testModeFolderPath
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
                    taskData: taskData,
                    testModeFolderPath: testModeFolderPath
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
            var commandSettingsData = new CommandSettings.Data();

            commandSettingsData.SettingsFilePath = settingsFilePath;
            commandSettingsData.LogFolderPath = logFolderPath;
            commandSettingsData.TaskData = taskData;
            commandSettingsData.TestModeFolderPath = testModeFolderPath;

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
            var batchRvtRevitFileProcessingOption = (
                    useRevitVersion == UseRevitVersion.RevitFileVersion ?
                    BatchRvt.RevitFileProcessingOption.UseFileRevitVersionIfAvailable :
                    BatchRvt.RevitFileProcessingOption.UseSpecificRevitVersion
                );

            // NOTE: can be any version if useRevitVersion is set to RevitFileVersion.
            var taskRevitVersion = (
                    useRevitVersion == UseRevitVersion.Revit2015 ?
                    RevitVersion.SupportedRevitVersion.Revit2015 :
                    useRevitVersion == UseRevitVersion.Revit2016 ?
                    RevitVersion.SupportedRevitVersion.Revit2016 :
                    useRevitVersion == UseRevitVersion.Revit2017 ?
                    RevitVersion.SupportedRevitVersion.Revit2017 :
                    useRevitVersion == UseRevitVersion.Revit2018 ?
                    RevitVersion.SupportedRevitVersion.Revit2018 :
                    RevitVersion.SupportedRevitVersion.Revit2019

                );

            var batchRvtSettings = BatchRvtSettings.Create(
                    taskScriptFilePath,
                    (revitFileListInput as string) ?? string.Empty,
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

        public static string RunTask(
                CommandSettings.Data commandSettingsData,
                bool openLogFileWhenDone
            )
        {
            commandSettingsData = ValidateCommandSettingsData(commandSettingsData);

            var batchRvtFolderPath = BatchRvt.GetBatchRvtFolderPath();

            BatchRvt.ExecuteMonitorScript(batchRvtFolderPath, commandSettingsData);

            var logFilePath = commandSettingsData.GeneratedLogFilePath;

            logFilePath = PostProcessLogFile(logFilePath);

            if (openLogFileWhenDone)
            {
                if (!string.IsNullOrWhiteSpace(logFilePath))
                {
                    Process.Start(logFilePath);
                }
            }

            return logFilePath;
        }

        private static string PostProcessLogFile(string logFilePath)
        {
            if (!string.IsNullOrWhiteSpace(logFilePath))
            {
                var plainTextLogFilePath = Path.Combine(
                        Path.GetDirectoryName(logFilePath),
                        Path.GetFileNameWithoutExtension(logFilePath) + ".txt"
                    );

                File.WriteAllLines(
                        plainTextLogFilePath,
                        LogFile.ReadLinesAsPlainText(logFilePath)
                    );

                logFilePath = plainTextLogFilePath;
            }

            return logFilePath;
        }

        private static CommandSettings.Data ValidateCommandSettingsData(CommandSettings.Data commandSettingsData)
        {
            if (string.IsNullOrWhiteSpace(commandSettingsData.SettingsFilePath))
            {
                commandSettingsData.SettingsFilePath = null;
            }

            if (string.IsNullOrWhiteSpace(commandSettingsData.LogFolderPath))
            {
                commandSettingsData.LogFolderPath = null;
            }

            if (string.IsNullOrWhiteSpace(commandSettingsData.SessionId))
            {
                commandSettingsData.SessionId = null;
            }

            if (string.IsNullOrWhiteSpace(commandSettingsData.TaskData))
            {
                commandSettingsData.TaskData = null;
            }

            if (string.IsNullOrWhiteSpace(commandSettingsData.TestModeFolderPath))
            {
                commandSettingsData.TestModeFolderPath = null;
            }

            var batchRvtSettings = commandSettingsData.Settings;

            if (batchRvtSettings != null)
            {
                if (batchRvtSettings.RevitProcessingOption.GetValue() == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing)
                {
                    if (
                            string.IsNullOrWhiteSpace(batchRvtSettings.RevitFileListFilePath.GetValue())
                            &&
                            commandSettingsData.RevitFileList == null
                        )
                    {
                        throw new ArgumentNullException("No Revit file list was specified for Batch processing mode.");
                    }
                }
            }

            commandSettingsData.GeneratedLogFilePath = null;

            return commandSettingsData;
        }

        // NOTE: Dynamo scripts are not supported in Revit versions earlier than 2016.
        public enum UseRevitVersion { RevitFileVersion = 0, Revit2015 = 1, Revit2016 = 2, Revit2017 = 3, Revit2018 = 4, Revit2019 = 5 }
    }
}
