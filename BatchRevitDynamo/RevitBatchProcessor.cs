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
            return BatchRvtTasks.RunTask(
                    taskScriptFilePath,
                    revitFileList, // Input is a list of Revit file paths.
                    (BatchRvtTasks.UseRevitVersion)useRevitVersion,
                    (BatchRvt.CentralFileOpenOption)centralFileOpenOption,
                    discardWorksetsOnDetach,
                    deleteLocalAfter,
                    openLogFileWhenDone,
                    taskData: null,
                    testModeFolderPath: null
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
            return BatchRvtTasks.RunSingleTask(
                    taskScriptFilePath,
                    (BatchRvtTasks.UseRevitVersion)useRevitVersion,
                    openLogFileWhenDone,
                    taskData: null,
                    testModeFolderPath: null
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
            return BatchRvtTasks.RunTaskAdvanced(
                    taskScriptFilePath,
                    revitFileList,
                    (BatchRvtTasks.UseRevitVersion)useRevitVersion,
                    (BatchRvt.CentralFileOpenOption)centralFileOpenOption,
                    discardWorksetsOnDetach,
                    deleteLocalAfter,
                    openLogFileWhenDone,
                    logFolderPath,
                    fileProcessingTimeOutInMinutes,
                    fallbackToMinimumAvailableRevitVersion,
                    taskData: null,
                    testModeFolderPath: null
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
            return BatchRvtTasks.RunTaskFromSettingsFile(
                    settingsFilePath,
                    logFolderPath,
                    openLogFileWhenDone
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
            return BatchRvtTasks.RunTask(
                    taskScriptFilePath,
                    revitFileListFilePath,
                    BatchRvt.RevitProcessingOption.BatchRevitFileProcessing,
                    (BatchRvtTasks.UseRevitVersion)useRevitVersion,
                    (BatchRvt.CentralFileOpenOption)centralFileOpenOption,
                    discardWorksetsOnDetach,
                    deleteLocalAfter,
                    openLogFileWhenDone,
                    logFolderPath,
                    fileProcessingTimeOutInMinutes,
                    fallbackToMinimumAvailableRevitVersion,
                    taskData: null,
                    testModeFolderPath: null
                );
        }

        // NOTE: Dynamo scripts are not supported in Revit versions earlier than 2016.
        public enum UseRevitVersion {
                RevitFileVersion = 0,
                Revit2015 = 1,
                Revit2016 = 2,
                Revit2017 = 3,
                Revit2018 = 4,
                Revit2019 = 5,
                Revit2020 = 6,
                Revit2021 = 7
        }
        public enum RevitSessionOption { UseSeparateSessionPerFile = 0, UseSameSessionForFilesOfSameVersion = 1 }
        public enum CentralFileOpenOption { Detach = 0, CreateNewLocal = 1 }
    }
}
