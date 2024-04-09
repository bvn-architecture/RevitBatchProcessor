#
# Revit Batch Processor
#
# Copyright (c) 2020  Dan Rumery, BVN
#
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.
#
#

import clr
import System
clr.AddReference("System.Core")
clr.ImportExtensions(System.Linq)

from System.IO import File

import global_test_mode
import server_util
import stream_io_util
import revit_process_host
import monitor_revit_process
import snapshot_data_util
import revit_dialog_detection
import exception_util
import time_util
import batch_rvt_util
from batch_rvt_util import ScriptDataUtil

SECONDS_PER_MINUTE = 60
REVIT_PROCESS_EXIT_TIMEOUT_IN_SECONDS = 10 * SECONDS_PER_MINUTE
REVIT_PROGRESS_CHECK_INTERVAL_IN_SECONDS = 5
REVIT_PROCESS_BEGIN_PROCESSING_TIMEOUT_IN_SECONDS = 5 * SECONDS_PER_MINUTE


def ShowSupportedRevitFileInfo(supportedRevitFileInfo):
    message = "\n"
    if supportedRevitFileInfo.IsCloudModel():
        revitCloudModelInfo = supportedRevitFileInfo.GetRevitCloudModelInfo()
        projectGuidText = revitCloudModelInfo.GetProjectGuid().ToString()
        modelGuidText = revitCloudModelInfo.GetModelGuid().ToString()
        message += ("\t" + "CLOUD MODEL\n")
        message += ("\t" + "Project ID: " + projectGuidText + "\n")
        message += ("\t" + "Model ID: " + modelGuidText + "\n")
        revitVersionText = supportedRevitFileInfo.TryGetRevitVersionText()
        revitVersionText = revitVersionText if not str.IsNullOrWhiteSpace(revitVersionText) else "NOT SPECIFIED!"
        message += ("\t" + "Revit version: " + revitVersionText + "\n")
    else:
        revitFileInfo = supportedRevitFileInfo.GetRevitFileInfo()
        revitFilePath = revitFileInfo.GetFullPath()
        fileExists = revitFileInfo.Exists()
        fileSize = revitFileInfo.GetFileSize()
        fileSizeText = str.Format("{0:0.00}MB", fileSize / (1024.0 * 1024.0)) if fileSize is not None else "<UNKNOWN>"
        message += ("\t" + revitFilePath + "\n")
        message += ("\t" + "File exists: " + ("YES" if fileExists else "NO") + "\n")
        message += ("\t" + "File size: " + fileSizeText + "\n")
        if fileExists:
            revitVersionText = revitFileInfo.TryGetRevitVersionText()
            revitVersionText = revitVersionText if not str.IsNullOrWhiteSpace(revitVersionText) else "NOT DETECTED!"
            message += ("\t" + "Revit version: " + revitVersionText + "\n")
    return message

def UsingClientHandle(serverStream, action):
    result = None
    try:
        result = action()
    finally:
        serverStream.DisposeLocalCopyOfClientHandle()
    return result

def ShowRevitScriptOutput(scriptOutputStream, output, pendingReadLineTask=None):
    outputLines, pendingReadLineTask = stream_io_util.ReadAvailableLines(scriptOutputStream, pendingReadLineTask)
    if outputLines.Any():
        for line in outputLines:
            output("\t" + "- " + line)
    return pendingReadLineTask

def ShowRevitProcessOutput(processOutputStream, output, pendingReadLineTask=None):
    outputLines, pendingReadLineTask = stream_io_util.ReadAvailableLines(processOutputStream, pendingReadLineTask)
    if outputLines.Any():
        for line in outputLines:
            if False: # Change to True to see Revit standard output (non-script output)
                output("\t" + "- [ REVIT MESSAGE ] : " + line)
    return pendingReadLineTask

def ShowRevitProcessError(processErrorStream, showRevitProcessErrorMessages, output, pendingReadLineTask=None):
    outputLines, pendingReadLineTask = stream_io_util.ReadAvailableLines(processErrorStream, pendingReadLineTask)
    if outputLines.Any():
        for line in outputLines:
            if line.StartsWith("log4cplus:"): # ignore pesky log4cplus messages (an Autodesk thing?)
                pass
            elif showRevitProcessErrorMessages:
                output("\t" + "- [ REVIT ERROR MESSAGE ] : " + line)
    return pendingReadLineTask

def TerminateHostRevitProcess(hostRevitProcess, output):
    try:
        hostRevitProcess.Kill()
    except Exception, e:
        output()
        output("ERROR: an error occurred while attempting to kill the Revit process!")
        exception_util.LogOutputErrorDetails(e, output)
    return

def RunScriptedRevitSession(
        revitVersion,
        batchRvtScriptsFolderPath,
        scriptFilePath,
        scriptDatas,
        progressNumber,
        processingTimeOutInMinutes,
        showRevitProcessErrorMessages,
        testModeFolderPath,
        output
    ):
    scriptDataFilePath = ScriptDataUtil.GetUniqueScriptDataFilePath()
    ScriptDataUtil.SaveManyToFile(scriptDataFilePath, scriptDatas)
    progressRecordFilePath = ScriptDataUtil.GetProgressRecordFilePath(scriptDataFilePath)

    serverStream = server_util.CreateAnonymousPipeServer(
            server_util.IN,
            server_util.HandleInheritability.Inheritable
        )
    
    def serverStreamAction():
        scriptOutputStreamReader = stream_io_util.GetStreamReader(serverStream)
        
        def streamReaderAction():
            scriptOutputPipeHandleString = serverStream.GetClientHandleAsString()

            def clientHandleAction():
                hostRevitProcess = revit_process_host.StartHostRevitProcess(
                        revitVersion,
                        batchRvtScriptsFolderPath,
                        scriptFilePath,
                        scriptDataFilePath,
                        progressNumber,
                        scriptOutputPipeHandleString,
                        testModeFolderPath
                    )
                return hostRevitProcess

            hostRevitProcess = UsingClientHandle(serverStream, clientHandleAction)

            hostRevitProcessId = hostRevitProcess.Id

            global_test_mode.ExportRevitProcessId(hostRevitProcessId)

            snapshotDataFilePaths = [
                    snapshot_data_util.GetSnapshotDataFilePath(scriptData.DataExportFolderPath.GetValue())
                    for scriptData in scriptDatas
                ]

            pendingReadLineTask = [None] # Needs to be a list so it can be captured by reference in closures.
            pendingProcessOutputReadLineTask = [None] # As above.
            pendingProcessErrorReadLineTask = [None] # As above.
            
            snapshotDataFilesExistTimestamp = [None] # Needs to be a list so it can be captured by reference in closures.

            currentProgressRecordNumber = [0]  # Needs to be a list so it can be captured by reference in closures.
            progressRecordCheckTimeUtc = [time_util.GetDateTimeUtcNow()] # As above.
            progressRecordChangedTimeUtc = [time_util.GetDateTimeUtcNow()] # As above.

            def monitoringAction():
                pendingProcessOutputReadLineTask[0] = ShowRevitProcessOutput(hostRevitProcess.StandardOutput, output, pendingProcessOutputReadLineTask[0])
                pendingProcessErrorReadLineTask[0] = ShowRevitProcessError(hostRevitProcess.StandardError, showRevitProcessErrorMessages, output, pendingProcessErrorReadLineTask[0])
                pendingReadLineTask[0] = ShowRevitScriptOutput(scriptOutputStreamReader, output, pendingReadLineTask[0])
                
                if time_util.GetSecondsElapsedSinceUtc(progressRecordCheckTimeUtc[0]) > REVIT_PROGRESS_CHECK_INTERVAL_IN_SECONDS:
                    progressRecordCheckTimeUtc[0] = time_util.GetDateTimeUtcNow()
                    progressRecordNumber = ScriptDataUtil.GetProgressNumber(progressRecordFilePath)
                    if progressRecordNumber is not None:
                        if currentProgressRecordNumber[0] != progressRecordNumber:
                            # Progress update detected.
                            currentProgressRecordNumber[0] = progressRecordNumber
                            progressRecordChangedTimeUtc[0] = time_util.GetDateTimeUtcNow()

                if processingTimeOutInMinutes > 0:
                    if currentProgressRecordNumber[0] != 0:
                        if time_util.GetSecondsElapsedSinceUtc(progressRecordChangedTimeUtc[0]) > (processingTimeOutInMinutes * SECONDS_PER_MINUTE):
                            output()
                            output("WARNING: Timed-out waiting for Revit task / file to be processed. Forcibly terminating the Revit process...")
                            TerminateHostRevitProcess(hostRevitProcess, output)

                if currentProgressRecordNumber[0] == 0:
                    if time_util.GetSecondsElapsedSinceUtc(progressRecordChangedTimeUtc[0]) > REVIT_PROCESS_BEGIN_PROCESSING_TIMEOUT_IN_SECONDS:
                        output()
                        output("WARNING: Timed-out waiting for Revit script host to begin task / file processing. Forcibly terminating the Revit process...")
                        TerminateHostRevitProcess(hostRevitProcess, output)

                if snapshotDataFilesExistTimestamp[0] is not None:
                    if time_util.GetSecondsElapsedSinceUtc(snapshotDataFilesExistTimestamp[0]) > REVIT_PROCESS_EXIT_TIMEOUT_IN_SECONDS:
                        output()
                        output("WARNING: Timed-out waiting for the Revit process to exit. Forcibly terminating the Revit process...")
                        TerminateHostRevitProcess(hostRevitProcess, output)
                elif snapshotDataFilePaths.All(lambda snapshotDataFilePath: File.Exists(snapshotDataFilePath)):
                    output()
                    output("Detected snapshot data files. Waiting for Revit process to exit...")
                    snapshotDataFilesExistTimestamp[0] = time_util.GetDateTimeUtcNow()

                try:
                    revit_dialog_detection.DismissCheekyRevitDialogBoxes(hostRevitProcessId, output)
                except Exception, e:
                    output()
                    output("WARNING: an error occurred in the cheeky Revit dialog box dismisser!")
                    exception_util.LogOutputErrorDetails(e, output)
                
                return

            monitor_revit_process.MonitorHostRevitProcess(hostRevitProcess, monitoringAction, output)
            return
        
        stream_io_util.UsingStream(scriptOutputStreamReader, streamReaderAction)
        return
    
    stream_io_util.UsingStream(serverStream, serverStreamAction)

    lastProgressNumber = ScriptDataUtil.GetProgressNumber(progressRecordFilePath)
    nextProgressNumber = (lastProgressNumber + 1) if lastProgressNumber is not None else None

    return nextProgressNumber
