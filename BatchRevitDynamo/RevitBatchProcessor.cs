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
using BatchRvtUtil;

namespace BatchRevitDynamo
{
    public static class RevitBatchProcessor
    {
        /// <summary>
        /// Runs a Revit Batch Processing task. The Revit file list is provided as a list of strings.
        /// </summary>
        /// <param name="toggleToExecute"></param>
        /// <param name="taskScriptFilePath"></param>
        /// <param name="revitFileList"></param>
        /// <param name="useRevitVersion"></param>
        /// <param name="centralFileOpenOption"></param>
        /// <param name="discardWorksetsOnDetach"></param>
        /// <param name="deleteLocalAfter"></param>
        /// <param name="openLogFileWhenDone"></param>
        /// <returns></returns>
        public static string RunTask(
                bool toggleToExecute, // TODO: reconsider if this is needed here.
                string taskScriptFilePath,
                IEnumerable<string> revitFileList,
                UseRevitVersion useRevitVersion,
                CentralFileOpenOption centralFileOpenOption,
                bool discardWorksetsOnDetach,
                bool deleteLocalAfter,
                bool openLogFileWhenDone
            )
        {
            return RunTaskInternal(
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
                    fallbackToMinimumAvailableRevitVersion: false
                );
        }

        /// <summary>
        /// Runs a single (non-batch) Revit task. Any file processing is left up to the task script to manage!
        /// </summary>
        /// <param name="toggleToExecute"></param>
        /// <param name="taskScriptFilePath"></param>
        /// <param name="useRevitVersion"></param>
        /// <param name="openLogFileWhenDone"></param>
        /// <returns></returns>
        public static string RunSingleTask(
                bool toggleToExecute, // TODO: reconsider if this is needed here.
                string taskScriptFilePath,
                UseRevitVersion useRevitVersion,
                bool openLogFileWhenDone
            )
        {
            if (useRevitVersion == UseRevitVersion.RevitFileVersion)
            {
                throw new ArgumentException("useRevitVersion argument must specify a specific Revit version!");
            }

            return RunTaskInternal(
                    taskScriptFilePath,
                    null, // Revit File list is N/A for single task processing
                    BatchRvt.RevitProcessingOption.SingleRevitTaskProcessing,
                    useRevitVersion,
                    CentralFileOpenOption.Detach,  // N/A for single task processing
                    discardWorksetsOnDetach: true, // N/A for single task processing
                    deleteLocalAfter: true, // N/A for single task processing
                    openLogFileWhenDone: openLogFileWhenDone,
                    logFolderPath: null,
                    fileProcessingTimeOutInMinutes: 0, // N/A for single task processing
                    fallbackToMinimumAvailableRevitVersion: false
                );
        }

        /// <summary>
        /// Runs a Revit Batch Processing task, with advanced options available. The Revit file list is provided as a list of strings.
        /// </summary>
        /// <param name="toggleToExecute"></param>
        /// <param name="taskScriptFilePath"></param>
        /// <param name="revitFileList"></param>
        /// <param name="useRevitVersion"></param>
        /// <param name="centralFileOpenOption"></param>
        /// <param name="discardWorksetsOnDetach"></param>
        /// <param name="deleteLocalAfter"></param>
        /// <param name="openLogFileWhenDone"></param>
        /// <param name="logFolderPath"></param>
        /// <param name="fileProcessingTimeOutInMinutes"></param>
        /// <param name="fallbackToMinimumAvailableRevitVersion"></param>
        /// <returns></returns>
        public static string RunTaskAdvanced(
                bool toggleToExecute, // TODO: reconsider if this is needed here.
                string taskScriptFilePath,
                IEnumerable<string> revitFileList,
                UseRevitVersion useRevitVersion,
                CentralFileOpenOption centralFileOpenOption,
                bool discardWorksetsOnDetach,
                bool deleteLocalAfter,
                bool openLogFileWhenDone,
                string logFolderPath,
                int fileProcessingTimeOutInMinutes,
                bool fallbackToMinimumAvailableRevitVersion
            )
        {
            return RunTaskInternal(
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
                    fallbackToMinimumAvailableRevitVersion
                );
        }

        /// <summary>
        /// Runs a Revit Batch Processing task, with settings provided by the specified settings file.
        /// </summary>
        /// <param name="toggleToExecute"></param>
        /// <param name="settingsFilePath"></param>
        /// <param name="logFolderPath"></param>
        /// <param name="openLogFileWhenDone"></param>
        /// <returns></returns>
        public static string RunTaskFromSettingsFile(
                bool toggleToExecute, // TODO: reconsider if this is needed here.
                string settingsFilePath,
                string logFolderPath,
                bool openLogFileWhenDone
            )
        {
            var batchRvtSettings = new BatchRvtSettings();

            var settingsLoaded = batchRvtSettings.LoadFromFile(settingsFilePath);

            if (!settingsLoaded)
            {
                throw new InvalidOperationException("Failed to load settings from the file.");
            }

            return RunTaskInternal(null, batchRvtSettings, logFolderPath, openLogFileWhenDone);
        }

        /// <summary>
        /// Runs a Revit Batch Processing task, with advanced options available. The Revit file list is provided by the specified Excel or text file path.
        /// </summary>
        /// <param name="toggleToExecute"></param>
        /// <param name="taskScriptFilePath"></param>
        /// <param name="revitFileListFilePath"></param>
        /// <param name="useRevitVersion"></param>
        /// <param name="centralFileOpenOption"></param>
        /// <param name="discardWorksetsOnDetach"></param>
        /// <param name="deleteLocalAfter"></param>
        /// <param name="openLogFileWhenDone"></param>
        /// <param name="logFolderPath"></param>
        /// <param name="fileProcessingTimeOutInMinutes"></param>
        /// <param name="fallbackToMinimumAvailableRevitVersion"></param>
        /// 
        /// <returns></returns>
        public static string RunTaskOnListFile(
                bool toggleToExecute, // TODO: reconsider if this is needed here.
                string taskScriptFilePath,
                string revitFileListFilePath, // Input is a file path to a list of Revit file paths.
                UseRevitVersion useRevitVersion,
                CentralFileOpenOption centralFileOpenOption,
                bool discardWorksetsOnDetach,
                bool deleteLocalAfter,
                bool openLogFileWhenDone,
                string logFolderPath,
                int fileProcessingTimeOutInMinutes,
                bool fallbackToMinimumAvailableRevitVersion
            )
        {
            return RunTaskInternal(
                    taskScriptFilePath,
                    revitFileListFilePath,
                    BatchRvt.RevitProcessingOption.BatchRevitFileProcessing,
                    useRevitVersion,
                    centralFileOpenOption,
                    discardWorksetsOnDetach,
                    deleteLocalAfter,
                    openLogFileWhenDone,
                    logFolderPath,
                    fileProcessingTimeOutInMinutes,
                    fallbackToMinimumAvailableRevitVersion
                );
        }

        private static string RunTaskInternal(
                string taskScriptFilePath,
                object revitFileListInput,
                BatchRvt.RevitProcessingOption revitProcessingOption,
                UseRevitVersion useRevitVersion,
                CentralFileOpenOption centralFileOpenOption,
                bool discardWorksetsOnDetach,
                bool deleteLocalAfter,
                bool openLogFileWhenDone,
                string logFolderPath,
                int fileProcessingTimeOutInMinutes,
                bool fallbackToMinimumAvailableRevitVersion
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
                    RevitVersion.SupportedRevitVersion.Revit2018
                );

            var batchRvtCentralFileOpenOption = (
                    centralFileOpenOption == CentralFileOpenOption.CreateNewLocal ?
                    BatchRvt.CentralFileOpenOption.CreateNewLocal :
                    BatchRvt.CentralFileOpenOption.Detach
                );

            var batchRvtSettings = BatchRvtSettings.Create(
                    taskScriptFilePath,
                    (revitFileListInput as string) ?? string.Empty,
                    revitProcessingOption,
                    batchRvtCentralFileOpenOption,
                    deleteLocalAfter,
                    discardWorksetsOnDetach,
                    BatchRvt.RevitSessionOption.UseSeparateSessionPerFile,
                    batchRvtRevitFileProcessingOption,
                    taskRevitVersion,
                    fileProcessingTimeOutInMinutes,
                    fallbackToMinimumAvailableRevitVersion
                );

            return RunTaskInternal(revitFileListInput, batchRvtSettings, logFolderPath, openLogFileWhenDone);
        }

        private static string RunTaskInternal(
                object revitFileListInput,
                BatchRvtSettings batchRvtSettings,
                string logFolderPath,
                bool openLogFileWhenDone
            )
        {
            var revitFileListFilePath = revitFileListInput as string;
            var revitFileList = revitFileListInput as IEnumerable<string>;

            if (
                    batchRvtSettings.RevitProcessingOption.GetValue() == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing
                    &&
                    string.IsNullOrWhiteSpace(revitFileListFilePath)
                    &&
                    string.IsNullOrWhiteSpace(batchRvtSettings.RevitFileListFilePath.GetValue())
                    &&
                    revitFileList == null
                )
            {
                throw new ArgumentNullException("Revit file list parameter cannot be null.");
            }

            if (revitFileListFilePath != null)
            {
                batchRvtSettings.RevitFileListFilePath.SetValue(revitFileListFilePath);
            }

            batchRvtSettings.SaveToAppDomainData();

            if (revitFileList != null)
            {
                BatchRvtSettings.SetAppDomainDataRevitFileList(revitFileList);
            }

            if (!string.IsNullOrWhiteSpace(logFolderPath))
            {
                CommandSettings.SetAppDomainDataLogFolderPath(logFolderPath);
            }

            var batchRvtFolderPath = BatchRvt.GetBatchRvtFolderPath();

            BatchRvt.ExecuteMonitorScript(batchRvtFolderPath);

            var logFilePath = BatchRvt.GetAppDomainDataLogFilePath();

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

                if (openLogFileWhenDone)
                {
                    Process.Start(logFilePath);
                }
            }

            return logFilePath;
        }
    }

    // NOTE: Dynamo scripts are not supported in Revit versions earlier than 2016.
    public enum UseRevitVersion { RevitFileVersion = 0, Revit2015 = 1, Revit2016 = 2, Revit2017 = 3, Revit2018 = 4 }
    public enum RevitSessionOption { UseSeparateSessionPerFile = 0, UseSameSessionForFilesOfSameVersion = 1 }
    public enum CentralFileOpenOption { Detach = 0, CreateNewLocal = 1 }
}
