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

try:
    clr.AddReference("System.Diagnostics.Process")
except: pass

clr.AddReference("System.Windows.Forms")
from System.Windows.Forms import Application
from System.IO import File

import global_test_mode
import thread_util
import script_environment
import client_util
import stream_io_util
import script_host_error
import path_util
import time_util
import revit_script_util
import revit_file_util
import snapshot_data_exporter
import snapshot_data_util
import exception_util
import revit_session
import stream_io_util
import script_util
import revit_dynamo
import revit_dynamo_error
import revit_process_host
from batch_rvt_util import BatchRvt, RevitVersion
from revit_script_util import ScriptDataUtil


END_SESSION_DELAY_IN_SECONDS = 5
CLOSE_MAIN_WINDOW_ATTEMPTS = 10

def GetCurrentProcess():
    return System.Diagnostics.Process.GetCurrentProcess()

def RunSingleTaskScript(scriptFilePath):
    aborted = False

    showMessageBoxOnTaskError = revit_script_util.GetShowMessageBoxOnTaskError()
    output = revit_script_util.Output

    try:
        def executeTaskScript():
            output()
            output("Task script operation started.")
            script_util.ExecuteScript(scriptFilePath)
            output()
            output("Task script operation completed.")
            return
        result = script_host_error.WithErrorHandling(
                executeTaskScript,
                "ERROR: An error occurred while executing the task script! Operation aborted.",
                output,
                showMessageBoxOnTaskError
            )
    except Exception, e:
        aborted = True
        raise
    finally:
        # Ensure aborted message is shown in the event of an exception.
        if aborted:
            output()
            output("Operation aborted.")

    if aborted:
        output()
        output("Operation aborted.")
    else:
        output()
        output("Operation completed.")

    return aborted

def RunBatchTaskScript(scriptFilePath):
    aborted = False

    uiapp = revit_session.GetSessionUIApplication()
    sessionId = revit_script_util.GetSessionId()
    centralFilePath = None
    cloudProjectId = None
    cloudModelId = None
    isCloudModel = revit_script_util.IsCloudModel()
    if isCloudModel:
        cloudProjectId = revit_script_util.GetCloudProjectId()
        cloudModelId = revit_script_util.GetCloudModelId()
    else:
        centralFilePath = revit_script_util.GetRevitFilePath()
    openInUI = revit_script_util.GetOpenInUI()
    enableDataExport = revit_script_util.GetEnableDataExport()
    dataExportFolderPath = revit_script_util.GetDataExportFolderPath()
    showMessageBoxOnTaskError = revit_script_util.GetShowMessageBoxOnTaskError()
    centralFileOpenOption = revit_script_util.GetCentralFileOpenOption()
    deleteLocalAfter = revit_script_util.GetDeleteLocalAfter()
    discardWorksetsOnDetach = revit_script_util.GetDiscardWorksetsOnDetach()
    worksetConfigurationOption = revit_script_util.GetWorksetConfigurationOption()
    auditOnOpening = revit_script_util.GetAuditOnOpening()
    progressNumber = revit_script_util.GetProgressNumber()
    progressMax = revit_script_util.GetProgressMax()
    output = revit_script_util.Output

    if enableDataExport and not path_util.DirectoryExists(dataExportFolderPath):
        output()
        output("ERROR: data export folder does not exist!")
        if not str.IsNullOrWhiteSpace(dataExportFolderPath):
            output()
            output("\t" + dataExportFolderPath)
        aborted = True
    elif not isCloudModel and not path_util.FileExists(centralFilePath):
        output()
        output("ERROR: Revit project file does not exist!")
        if not str.IsNullOrWhiteSpace(centralFilePath):
            output()
            output("\t" + centralFilePath)
        aborted = True
    else:
        if enableDataExport:
            snapshotStartTime = time_util.GetDateTimeNow()
            snapshotError = None
            snapshotEndTime = None
            revitJournalFilePath = uiapp.Application.RecordingJournalFilename
            snapshotData = snapshot_data_exporter.ExportTemporarySnapshotData(
                    sessionId,
                    centralFilePath,
                    isCloudModel,
                    cloudProjectId,
                    cloudModelId,
                    snapshotStartTime,
                    snapshotEndTime,
                    dataExportFolderPath,
                    revitJournalFilePath,
                    snapshotError
                )

        localFilePath = None
        openCreateNewLocal = False # default is False because the file may not be a workshared Central file.
        isCentralModel = False
        isLocalModel = False
        try:
            if isCloudModel:
                output()
                output("Processing file (" + str(progressNumber) + " of " + str(progressMax) + "): " + "CLOUD MODEL")
                output()
                output("\t" + "Project ID: " + cloudProjectId)
                output("\t" + "Model ID: " + cloudModelId)
            else:
                output()
                output("Processing file (" + str(progressNumber) + " of " + str(progressMax) + "): " + centralFilePath)

            if isCloudModel:
                output()
                output("The file is a Cloud Model.")
            elif revit_file_util.IsWorkshared(centralFilePath):
                if revit_file_util.IsLocalModel(centralFilePath):
                    output()
                    output("WARNING: the file being processed appears to be a Workshared Local file!")
                    isLocalModel = True
                if revit_file_util.IsCentralModel(centralFilePath):
                    output()
                    output("The file is a Central Model file.")
                    isCentralModel = True
                if centralFileOpenOption == BatchRvt.CentralFileOpenOption.CreateNewLocal:
                    openCreateNewLocal = True
            elif path_util.HasFileExtension(centralFilePath, ".rfa"):
                output()
                output("The file is a Family file.")
            else:
                output()
                output("The file is a Non-workshared file.")

            if enableDataExport:
                output()
                output("Export folder is: " + dataExportFolderPath)

            def processDocument(doc):
                revit_script_util.SetScriptDocument(doc)
                
                def executeTaskScript():
                    success = False
                    output()
                    output("Task script operation started.")
                    if path_util.HasFileExtension(scriptFilePath, script_util.DYNAMO_SCRIPT_FILE_EXTENSION):
                        if revit_dynamo.IsDynamoRevitModuleLoaded():
                            revit_dynamo.ExecuteDynamoScript(uiapp, scriptFilePath, showUI=False)
                            success = True
                        else:
                            success = False
                            output()
                            output(revit_dynamo_error.DYNAMO_REVIT_MODULE_NOT_FOUND_ERROR_MESSAGE)
                    else:
                        script_util.ExecuteScript(scriptFilePath)
                        success = True
                    if success:
                        output()
                        output("Task script operation completed.")
                    else:
                        output()
                        output("ERROR: An error occurred while executing the task script! Operation aborted.")
                    return

                result = script_host_error.WithErrorHandling(
                        executeTaskScript,
                        "ERROR: An error occurred while executing the task script! Operation aborted.",
                        output,
                        showMessageBoxOnTaskError
                    )
                return result

            result = None
            activeDoc = None #revit_script_util.GetActiveDocument(uiapp)
            if activeDoc is not None:
                result = processDocument(activeDoc)
            else:
                if isCloudModel:
                    result = revit_script_util.RunCloudDocumentAction(
                            uiapp,
                            openInUI,
                            cloudProjectId,
                            cloudModelId,
                            worksetConfigurationOption,
                            auditOnOpening,
                            processDocument,
                            output
                        )
                elif openCreateNewLocal:
                    revitVersion = RevitVersion.GetSupportedRevitVersion(revit_session.GetSessionRevitVersionNumber())
                    localFilePath = RevitVersion.GetRevitLocalFilePath(revitVersion, centralFilePath)
                    try:
                        if File.Exists(localFilePath):
                            output()
                            output("Deleting existing local file...")
                            File.Delete(localFilePath)
                            output()
                            output("Local file deleted.")
                    except Exception, e:
                        output()
                        output("WARNING: failed to delete the local file!")
                    path_util.CreateDirectoryForFilePath(localFilePath)
                    result = revit_script_util.RunNewLocalDocumentAction(
                            uiapp,
                            openInUI,
                            centralFilePath,
                            localFilePath,
                            worksetConfigurationOption,
                            auditOnOpening,
                            processDocument,
                            output
                        )
                elif isCentralModel or isLocalModel:
                    result = revit_script_util.RunDetachedDocumentAction(
                            uiapp,
                            openInUI,
                            centralFilePath,
                            discardWorksetsOnDetach,
                            worksetConfigurationOption,
                            auditOnOpening,
                            processDocument,
                            output
                        )
                else:
                    result = revit_script_util.RunDocumentAction(uiapp, openInUI, centralFilePath, auditOnOpening, processDocument, output)
        except Exception, e:
            aborted = True
            snapshotError = exception_util.GetExceptionDetails(e)
            raise
        finally:
            if isCloudModel:
                pass # explicitly do nothing for cloud-based models
            elif openCreateNewLocal and deleteLocalAfter:
                try:
                    if File.Exists(localFilePath):
                        output()
                        output("Deleting local file...")
                        File.Delete(localFilePath)
                        output()
                        output("Local file deleted.")
                except Exception, e:
                    output()
                    output("WARNING: failed to delete the local file!")

            if enableDataExport:
                snapshotEndTime = time_util.GetDateTimeNow()
                snapshotData = snapshot_data_exporter.ExportSnapshotData(
                        sessionId,
                        centralFilePath,
                        isCloudModel,
                        cloudProjectId,
                        cloudModelId,
                        snapshotStartTime,
                        snapshotEndTime,
                        dataExportFolderPath,
                        revitJournalFilePath,
                        snapshotError
                    )
                snapshot_data_util.ConsolidateSnapshotData(dataExportFolderPath, output)

            # Ensure aborted message is shown in the event of an exception.
            if aborted:
                output()
                output("Operation aborted.")

    if aborted:
        output()
        output("Operation aborted.")
    else:
        output()
        output("Operation completed.")

    return aborted

def ShutdownSession(process):
    for i in xrange(CLOSE_MAIN_WINDOW_ATTEMPTS):
        process.Refresh()
        process.CloseMainWindow()
        Application.DoEvents()
    return

def DoEvents(seconds):
    for i in xrange(seconds):
        Application.DoEvents()
        thread_util.SleepForSeconds(1)
    return

def GetRevitProcessingOptionForSession(scriptDatas):
    oldScriptData = revit_script_util.GetCurrentScriptData()
    revit_script_util.SetCurrentScriptData(scriptDatas[0])
    revitProcessingOption = revit_script_util.GetRevitProcessingOption()
    revit_script_util.SetCurrentScriptData(oldScriptData)
    return revitProcessingOption

def DoRevitSessionProcessing(
        scriptFilePath,
        scriptDataFilePath,
        progressNumber,
        batchRvtProcessUniqueId,
        output
    ):
    results = []
    revit_script_util.SetUIApplication(revit_session.GetSessionUIApplication())
    revit_script_util.SetScriptDataFilePath(scriptDataFilePath)
    scriptDatas = (
            revit_script_util.LoadScriptDatas()
            .Where(lambda scriptData: scriptData.ProgressNumber.GetValue() >= progressNumber)
            .OrderBy(lambda scriptData: scriptData.ProgressNumber.GetValue())
            .ToList()
        )
    if len(scriptDatas) > 0:
        revitProcessingOption = GetRevitProcessingOptionForSession(scriptDatas)
        if revitProcessingOption == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing:
            for scriptData in scriptDatas:
                revit_script_util.SetCurrentScriptData(scriptData)
                if not revit_process_host.IsBatchRvtProcessRunning(batchRvtProcessUniqueId):
                    script_host_error.ShowScriptErrorMessageBox("ERROR: The BatchRvt process appears to have terminated! Operation aborted.")
                    break
                progressRecordFilePath = ScriptDataUtil.GetProgressRecordFilePath(scriptDataFilePath)
                progressRecorded = ScriptDataUtil.SetProgressNumber(progressRecordFilePath, scriptData.ProgressNumber.GetValue())
                if not progressRecorded:
                    output()
                    output("WARNING: Failed to update the session progress record file!")
                result = script_host_error.WithErrorHandling(
                        lambda: RunBatchTaskScript(scriptFilePath),
                        "ERROR: An error occurred while processing the file!",
                        output,
                        False
                    )
                results.append(result)
        else:
            scriptData = scriptDatas[0]
            revit_script_util.SetCurrentScriptData(scriptData)
            result = RunSingleTaskScript(scriptFilePath)
            results.append(result)
    else:
        raise Exception("ERROR: received no script data!")
    return results

def Main():
    environmentVariables = script_environment.GetEnvironmentVariables()
    outputPipeHandleString = script_environment.GetScriptOutputPipeHandleString(environmentVariables)
    scriptFilePath = script_environment.GetScriptFilePath(environmentVariables)
    scriptDataFilePath = script_environment.GetScriptDataFilePath(environmentVariables)
    progressNumber = script_environment.GetProgressNumber(environmentVariables)
    batchRvtProcessUniqueId = script_environment.GetBatchRvtProcessUniqueId(environmentVariables)
    testModeFolderPath = script_environment.GetTestModeFolderPath(environmentVariables)
    global_test_mode.InitializeGlobalTestMode(testModeFolderPath)

    if outputPipeHandleString is not None and scriptFilePath is not None:

        outputStream = client_util.CreateAnonymousPipeClient(client_util.OUT, outputPipeHandleString)

        def outputStreamAction():
            outputStreamWriter = stream_io_util.GetStreamWriter(outputStream)

            def outputStreamWriterAction():
                revit_script_util.SetOutputFunction(stream_io_util.GetSafeWriteLine(outputStreamWriter))
                result = script_host_error.WithErrorHandling(
                        lambda: DoRevitSessionProcessing(
                                scriptFilePath,
                                scriptDataFilePath,
                                progressNumber,
                                batchRvtProcessUniqueId,
                                revit_script_util.Output
                            ),
                        "ERROR: An error occurred while executing the script host! Operation aborted.",
                        output=revit_script_util.Output,
                        showErrorMessageBox=False
                    )
                return result

            stream_io_util.UsingStream(outputStreamWriter, outputStreamWriterAction)
            return

        stream_io_util.UsingStream(outputStream, outputStreamAction)

    return

def TerminateSession():
    currentProcess = GetCurrentProcess()
    DoEvents(END_SESSION_DELAY_IN_SECONDS)
    ShutdownSession(currentProcess)
    DoEvents(END_SESSION_DELAY_IN_SECONDS)
    currentProcess.Kill()
    return

script_host_error.WithErrorHandling(
        Main,
        "ERROR: An error occurred while executing the script host! Operation aborted.",
        showErrorMessageBox=False
    )

TerminateSession()
