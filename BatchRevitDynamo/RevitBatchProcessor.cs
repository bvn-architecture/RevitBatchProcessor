using System;
using System.Collections.Generic;
using System.Linq;
using BatchRvtUtil;

namespace BatchRevitDynamo
{
    public static class RevitBatchProcessor
    {
        /// <summary>
        /// Runs a Revit Batch Processing task.
        /// </summary>
        /// <param name="taskScriptFilePath"></param>
        /// <param name="revitFileListFilePath"></param>
        /// <param name="useRevitVersion"></param>
        /// <param name="revitSessionOption"></param>
        /// <param name="centralFileOpenOption"></param>
        /// <param name="deleteLocalAfter"></param>
        /// <param name="discardWorksetsOnDetach"></param>
        /// <returns>Full path to the generated log file.</returns>
        public static string RunTask(
                string taskScriptFilePath,
                string revitFileListFilePath,
                UseRevitVersion useRevitVersion = UseRevitVersion.RevitFileVersion,
                RevitSessionOption revitSessionOption = RevitSessionOption.UseSeparateSessionPerFile,
                CentralFileOpenOption centralFileOpenOption = CentralFileOpenOption.Detach,
                bool deleteLocalAfter = true,
                bool discardWorksetsOnDetach = false
            )
        {
            return null; // TODO: implement!
        }
    }

    // NOTE: Dynamo does not support Revit versions earlier than 2016.
    public enum UseRevitVersion { RevitFileVersion = 0, Revit2016 = 1, Revit2017 = 2, Revit2018 = 3 }
    public enum RevitSessionOption { UseSeparateSessionPerFile = 0, UseSameSessionForFilesOfSameVersion = 1 }
    public enum CentralFileOpenOption { Detach = 0, CreateNewLocal = 1 }

}
