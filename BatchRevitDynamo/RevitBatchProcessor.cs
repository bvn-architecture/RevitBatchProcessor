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
                    toggleToExecute,
                    taskScriptFilePath,
                    revitFileList, // Input is a list of Revit file paths.
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
                    toggleToExecute,
                    taskScriptFilePath,
                    revitFileList, // Input is a list of Revit file paths.
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
        public static string RunTaskOnListFromFile(
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
                    toggleToExecute,
                    taskScriptFilePath,
                    revitFileListFilePath,
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
                bool toggleToExecute, // TODO: reconsider if this is needed here.
                string taskScriptFilePath,
                object revitFileListInput,
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
            var batchRvtFolderPath = BatchRvt.GetBatchRvtFolderPath();

            var batchRvtRevitFileProcessingOption = (
                    useRevitVersion == UseRevitVersion.RevitFileVersion ?
                    BatchRvt.RevitFileProcessingOption.UseFileRevitVersionIfAvailable :
                    BatchRvt.RevitFileProcessingOption.UseSpecificRevitVersion
                );

            // NOTE: can be any version if useRevitVersion is set to RevitFileVersion.
            var taskRevitVersion = (
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

            var revitFileListFilePath = revitFileListInput as string;
            var revitFileList = revitFileListInput as IEnumerable<string>;

            if (revitFileListFilePath == null && revitFileList == null)
            {
                throw new ArgumentNullException("Revit file list parameter cannot be null.");
            }

            var batchRvtSettings = BatchRvtSettings.Create(
                    taskScriptFilePath,
                    revitFileListFilePath,
                    batchRvtCentralFileOpenOption,
                    deleteLocalAfter,
                    discardWorksetsOnDetach,
                    BatchRvt.RevitSessionOption.UseSeparateSessionPerFile,
                    batchRvtRevitFileProcessingOption,
                    taskRevitVersion,
                    fileProcessingTimeOutInMinutes,
                    fallbackToMinimumAvailableRevitVersion
                );

            batchRvtSettings.SaveToAppDomainData();

            if (revitFileList != null)
            {
                BatchRvtSettings.SetAppDomainDataRevitFileList(revitFileList);
            }

            if (!string.IsNullOrWhiteSpace(logFolderPath))
            {
                CommandSettings.SetAppDomainDataLogFolderPath(logFolderPath);
            }

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

    // NOTE: Dynamo does not support Revit versions earlier than 2016.
    public enum UseRevitVersion { RevitFileVersion = 0, Revit2016 = 1, Revit2017 = 2, Revit2018 = 3 }
    public enum RevitSessionOption { UseSeparateSessionPerFile = 0, UseSameSessionForFilesOfSameVersion = 1 }
    public enum CentralFileOpenOption { Detach = 0, CreateNewLocal = 1 }
}
