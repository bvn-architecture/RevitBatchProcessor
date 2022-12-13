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

import System
import clr

clr.AddReference("System.Core")
clr.ImportExtensions(System.Linq)
from System.IO import File

import global_test_mode
import time_util
import path_util
import logging_util
import snapshot_data_util
import session_data_util
import revit_file_list
import script_util
from batch_rvt_util import CommandSettings, CommandLineUtil, BatchRvtSettings, BatchRvt, RevitVersion

LOG_NAME = "BatchRvt"


class BatchRvtConfig:

    def __init__(self):

        # BatchRvt settings
        self.SettingsFilePath = None
        self.LogFolderPath = None
        self.LogFilePath = None
        self.SessionId = None
        self.SessionStartTime = None
        self.TaskData = None
        self.TestModeFolderPath = None
        self.SessionDataFolderPath = None
        self.ShowRevitProcessErrorMessages = False

        # General Task Script settings
        self.ScriptFilePath = None
        self.ShowMessageBoxOnTaskError = None
        self.ProcessingTimeOutInMinutes = 0

        # Revit File List settings
        self.RevitFileListFilePath = None
        self.RevitFileList = None
        self.RevitFileListData = None

        # Data Export settings
        self.EnableDataExport = None
        self.DataExportFolderPath = None

        # Pre-processing Script settings
        self.ExecutePreProcessingScript = None
        self.PreProcessingScriptFilePath = None

        # Post-processing Script settings
        self.ExecutePostProcessingScript = None
        self.PostProcessingScriptFilePath = None

        # Central File Processing settings
        self.CentralFileOpenOption = None
        self.DeleteLocalAfter = None
        self.DiscardWorksetsOnDetach = None
        self.WorksetConfigurationOption = None

        # Revit Session settings
        self.RevitSessionOption = None

        # Revit Processing settings
        self.RevitProcessingOption = None

        # Single Revit Task Processing settings
        self.SingleRevitTaskRevitVersion = None

        # Batch Revit File Processing settings
        self.RevitFileProcessingOption = None
        self.IfNotAvailableUseMinimumAvailableRevitVersion = None
        self.BatchRevitTaskRevitVersion = None
        self.AuditOnOpening = False
        self.OpenInUI = False

        return

    def ReadRevitFileListData(self, output):
        revitFileListData = None
        if self.RevitProcessingOption == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing:
            if self.RevitFileList is not None:
                output()
                output("Reading Revit File list from object input.")
                self.RevitFileListData = revit_file_list.FromLines(self.RevitFileList)
                revitFileListData = self.RevitFileListData
            else:
                output()
                output("Reading Revit File list:")
                output()
                output("\t" + (self.RevitFileListFilePath if not str.IsNullOrWhiteSpace(
                    self.RevitFileListFilePath) else "Not specified!"))
                if not File.Exists(self.RevitFileListFilePath):
                    output()
                    output("ERROR: No Revit file list specified or file not found.")
                elif revit_file_list.HasExcelFileExtension(
                        self.RevitFileListFilePath) and not revit_file_list.IsExcelInstalled():
                    output()
                    output(
                        "ERROR: Could not read from the Excel Revit File list. An Excel installation was not detected!")
                else:
                    revitFileListData = revit_file_list.FromFile(self.RevitFileListFilePath)
                    revitFileList = (
                        [revitFilePathData.RevitFilePath for revitFilePathData in revitFileListData]
                        if revitFileListData is not None else None
                    )
                    self.RevitFileList = revitFileList
                    self.RevitFileListData = revitFileListData

            if revitFileListData is None:
                output()
                output("ERROR: Could not read the Revit File list.")
            elif len(revitFileListData) == 0:
                output()
                output("ERROR: Revit File list is empty.")
                revitFileListData = None
        return revitFileListData


def ParseSessionIdAndStartTime(sessionId):
    # NOTE: If the session ID looks too much like a serialized json date/time then the deserializer will
    #       deserialize it into a date/time object, which is undesirable in this case.
    #       Hence the addition of the surrounding angle brackets to thwart this behaviour.
    haveSessionId = (sessionId is not None and (sessionId.StartsWith("<") and sessionId.EndsWith(">")))

    if haveSessionId:
        sessionStartTime = time_util.GetDateTimeUtcFromISO8601FormattedDate(sessionId[1:-1])
    else:
        sessionStartTime = time_util.GetDateTimeUtcNow()
        sessionId = "<" + time_util.GetISO8601FormattedUtcDate(sessionStartTime) + ">"
    return sessionId, sessionStartTime


def InitializeLogging(logFolderPath, sessionStartTime):
    logFolderPath = logFolderPath if not str.IsNullOrWhiteSpace(logFolderPath) else BatchRvt.GetDataFolderPath()
    path_util.CreateDirectory(logFolderPath)
    logName = LOG_NAME + "_" + snapshot_data_util.GetSnapshotFolderName(sessionStartTime.ToLocalTime())
    logging_util.InitializeLogging(logName, logFolderPath)
    logFilePath = logging_util.GetLogFilePath()
    return logFilePath


def ConfigureBatchRvtSettings(batchRvtConfig, batchRvtSettings, output):
    aborted = False

    # General Task Script settings
    batchRvtConfig.ScriptFilePath = batchRvtSettings.TaskScriptFilePath.GetValue()
    batchRvtConfig.ShowMessageBoxOnTaskError = batchRvtSettings.ShowMessageBoxOnTaskScriptError.GetValue()
    batchRvtConfig.ProcessingTimeOutInMinutes = batchRvtSettings.ProcessingTimeOutInMinutes.GetValue()
    batchRvtConfig.ShowRevitProcessErrorMessages = batchRvtSettings.ShowRevitProcessErrorMessages.GetValue()

    # Revit File List settings
    batchRvtConfig.RevitFileListFilePath = batchRvtSettings.RevitFileListFilePath.GetValue()

    # Data Export settings
    batchRvtConfig.EnableDataExport = batchRvtSettings.EnableDataExport.GetValue()
    batchRvtConfig.DataExportFolderPath = batchRvtSettings.DataExportFolderPath.GetValue()

    # Pre-processing Script settings
    batchRvtConfig.ExecutePreProcessingScript = batchRvtSettings.ExecutePreProcessingScript.GetValue()
    batchRvtConfig.PreProcessingScriptFilePath = batchRvtSettings.PreProcessingScriptFilePath.GetValue()

    # Post-processing Script settings
    batchRvtConfig.ExecutePostProcessingScript = batchRvtSettings.ExecutePostProcessingScript.GetValue()
    batchRvtConfig.PostProcessingScriptFilePath = batchRvtSettings.PostProcessingScriptFilePath.GetValue()

    # Central File Processing settings
    batchRvtConfig.CentralFileOpenOption = batchRvtSettings.CentralFileOpenOption.GetValue()
    batchRvtConfig.DeleteLocalAfter = batchRvtSettings.DeleteLocalAfter.GetValue()
    batchRvtConfig.DiscardWorksetsOnDetach = batchRvtSettings.DiscardWorksetsOnDetach.GetValue()
    batchRvtConfig.WorksetConfigurationOption = batchRvtSettings.WorksetConfigurationOption.GetValue()

    # Revit Session settings
    batchRvtConfig.RevitSessionOption = batchRvtSettings.RevitSessionOption.GetValue()

    # Revit Processing settings
    batchRvtConfig.RevitProcessingOption = batchRvtSettings.RevitProcessingOption.GetValue()

    # Single Revit Task Processing settings
    batchRvtConfig.SingleRevitTaskRevitVersion = batchRvtSettings.SingleRevitTaskRevitVersion.GetValue()

    # Batch Revit File Processing settings
    batchRvtConfig.RevitFileProcessingOption = batchRvtSettings.RevitFileProcessingOption.GetValue()
    batchRvtConfig.IfNotAvailableUseMinimumAvailableRevitVersion = batchRvtSettings.IfNotAvailableUseMinimumAvailableRevitVersion.GetValue()
    batchRvtConfig.BatchRevitTaskRevitVersion = batchRvtSettings.BatchRevitTaskRevitVersion.GetValue()
    batchRvtConfig.AuditOnOpening = batchRvtSettings.AuditOnOpening.GetValue()
    batchRvtConfig.OpenInUI = batchRvtSettings.OpenInUI.GetValue()

    if not File.Exists(batchRvtConfig.ScriptFilePath):
        output()
        output("ERROR: No script file specified or script file not found.")
        aborted = True
    else:
        output()
        output("Task Script:")
        output()
        output("\t" + batchRvtConfig.ScriptFilePath)

        isDynamoTaskScript = path_util.HasFileExtension(batchRvtConfig.ScriptFilePath,
                                                        script_util.DYNAMO_SCRIPT_FILE_EXTENSION)

        if isDynamoTaskScript:
            batchRvtConfig.OpenInUI = True  # Always open and activate documents in the UI when executing a Dynamo task script.

        if isDynamoTaskScript:
            # Always use a separate Revit session for each Revit file when executing a Dynamo task script.
            # This restriction is due a limitation on closing documents that are active in the UI (which Dynamo requires).
            batchRvtConfig.RevitSessionOption = BatchRvt.RevitSessionOption.UseSeparateSessionPerFile

        if batchRvtConfig.ShowMessageBoxOnTaskError:
            output()
            output("Show Message Box on Task Script Error is enabled.")

        if batchRvtConfig.ProcessingTimeOutInMinutes > 0:
            output()
            output("Revit Task / File processing time-out setting:")
            output()
            output(
                "\t" + str(batchRvtConfig.ProcessingTimeOutInMinutes) +
                " " + ("minute" if batchRvtConfig.ProcessingTimeOutInMinutes == 1 else "minutes")
            )

        revitProcessingModeDescription = (
            "Batch Revit File processing" if batchRvtConfig.RevitProcessingOption == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing
            else "Single Revit Task processing"
        )
        output()
        output("Revit Processing mode:")
        output()
        output("\t" + revitProcessingModeDescription)

        if batchRvtConfig.EnableDataExport:
            if str.IsNullOrWhiteSpace(batchRvtConfig.DataExportFolderPath):
                output()
                output("ERROR: No data export folder specified.")
                aborted = True
            else:
                output()
                output("Data Export is enabled.")
                output()
                output("Data Export Folder:")
                output()
                output("\t" + batchRvtConfig.DataExportFolderPath)

                batchRvtConfig.SessionDataFolderPath = (
                    session_data_util.GetSessionFolderPath(
                        batchRvtConfig.DataExportFolderPath,
                        batchRvtConfig.SessionStartTime
                    )
                )

                output()
                output("Session Folder:")
                output()
                output("\t" + batchRvtConfig.SessionDataFolderPath)

        if batchRvtConfig.ExecutePreProcessingScript:
            if not File.Exists(batchRvtConfig.PreProcessingScriptFilePath):
                output()
                output("ERROR: Pre-processing script file does not exist.")
                aborted = True
            else:
                output()
                output("Pre-Processing Script:")
                output()
                output("\t" + batchRvtConfig.PreProcessingScriptFilePath)

        if batchRvtConfig.ExecutePostProcessingScript:
            if not File.Exists(batchRvtConfig.PostProcessingScriptFilePath):
                output()
                output("ERROR: Post-processing script file does not exist.")
                aborted = True
            else:
                output()
                output("Post-Processing Script:")
                output()
                output("\t" + batchRvtConfig.PostProcessingScriptFilePath)

        if batchRvtConfig.RevitProcessingOption == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing:
            centralFileProcessingDescription = (
                "Create New Local" if (
                            batchRvtConfig.CentralFileOpenOption == BatchRvt.CentralFileOpenOption.CreateNewLocal)
                else "Detach from Central"
            )
            output()
            output("Central File Processing mode:")
            output()
            output("\t" + centralFileProcessingDescription)
            usingWorksetConfigurationOption = False
            if (batchRvtConfig.CentralFileOpenOption == BatchRvt.CentralFileOpenOption.CreateNewLocal):
                usingWorksetConfigurationOption = True
                if (batchRvtConfig.DeleteLocalAfter):
                    output()
                    output("\t" + "Local File will be deleted after processing.")
            elif (batchRvtConfig.CentralFileOpenOption == BatchRvt.CentralFileOpenOption.Detach):
                if (batchRvtConfig.DiscardWorksetsOnDetach):
                    usingWorksetConfigurationOption = False
                    output()
                    output("\t" + "Worksets will be discarded upon detach.")
                else:
                    usingWorksetConfigurationOption = True

            if usingWorksetConfigurationOption:
                worksetConfigurationOptionDescription = (
                    "Close All" if batchRvtConfig.WorksetConfigurationOption == BatchRvt.WorksetConfigurationOption.CloseAllWorksets else
                    "Open All" if batchRvtConfig.WorksetConfigurationOption == BatchRvt.WorksetConfigurationOption.OpenAllWorksets else
                    "Open Last Viewed"
                )
                output()
                output("\t" + "Worksets Configuration: " + worksetConfigurationOptionDescription)

            if batchRvtConfig.AuditOnOpening:
                output()
                output("\t" + "Revit files will be audited on opening.")

    return aborted


def GetBatchRvtSettings(settingsFilePath, output):
    aborted = False
    batchRvtSettings = None
    if not File.Exists(settingsFilePath):
        output()
        output("ERROR: No settings file specified or settings file not found.")
        aborted = True
    else:
        batchRvtSettings = BatchRvtSettings()
        isSettingsLoaded = batchRvtSettings.LoadFromFile(settingsFilePath)
        if not isSettingsLoaded:
            output()
            output("ERROR: Could not load settings from the settings file!")
            aborted = True
    return batchRvtSettings if not aborted else None


def GetCommandSettingsOption(aborted, commandLineOptions, commandSettingsOption, output, invalidValueErrorMessage=None,
                             missingValueErrorMessage=None):
    # NOTE: option==None doesn't necessarily mean aborted==True. Specifically, if an optional option is not specified, option==None but aborted==False.
    option = None
    if CommandLineUtil.HasCommandLineOption(commandSettingsOption):
        optionValue = commandLineOptions[commandSettingsOption]
        if optionValue is None:
            output()
            if invalidValueErrorMessage is not None:
                output(invalidValueErrorMessage)
            else:
                output(
                    "ERROR: Invalid " + CommandLineUtil.OptionSwitchPrefix + commandSettingsOption + " option value!")
            aborted = True
        else:
            option = optionValue
    elif CommandLineUtil.HasCommandLineOption(commandSettingsOption, False):
        output()
        if missingValueErrorMessage is not None:
            output(missingValueErrorMessage)
        else:
            output("ERROR: Missing " + CommandLineUtil.OptionSwitchPrefix + commandSettingsOption + " option value!")
        aborted = True
    return option, aborted


def ShowCommandLineHelp(output):
    output()
    output("Help:")
    output()
    output("\t" + "Usage (using a settings file):")
    output()
    output("\t\t" + "BatchRvt.exe --settings_file <SETTINGS FILE PATH> [--log_folder <LOG FOLDER PATH>]")
    output()
    output("\t" + "Example:")
    output()
    output("\t\t" + "BatchRvt.exe --settings_file BatchRvt.Settings.json --log_folder .")
    output()
    output()
    output("\t" + "Usage (without a settings file):")
    output()
    output("\t\t" + "BatchRvt.exe --file_list <REVIT FILE LIST PATH> --task_script <TASK SCRIPT FILE PATH>")
    output()
    output(
        "\t" + "(NOTE: this mode operates in batch mode only; by default operates in detach mode for central files.)")
    output()
    output()
    output("\t" + "Additional command-line options:")
    output()
    output("\t\t" + "--revit_version <REVIT VERSION>")
    output()
    output("\t\t" + "--log_folder <LOG FOLDER PATH>")
    output()
    output("\t\t" + "--detach | --create_new_local")
    output()
    output("\t\t" + "--worksets <open_all | close_all>")
    output()
    output("\t\t" + "--audit")
    output()
    output("\t\t" + "--per_file_timeout <PER-FILE PROCESSING TIMEOUT IN MINUTES>")
    output()
    output("\t\t" + "--help")
    output()
    output()
    output("\t" + "Examples:")
    output()
    output("\t\t" + "BatchRvt.exe --task_script MyDynamoWorkspace.dyn --file_list RevitFileList.xlsx")
    output()
    output("\t\t" + "BatchRvt.exe --task_script MyDynamoWorkspace.dyn --file_list RevitFileList.xlsx --detach --audit")
    output()
    output(
        "\t\t" + "BatchRvt.exe --task_script MyTask.py --file_list RevitFileList.txt --create_new_local --worksets open_all")
    output()
    output(
        "\t\t" + "BatchRvt.exe --task_script MyTask.py --file_list RevitFileList.xlsx --revit_version 2019 --detach --worksets close_all")
    output()
    output("\t\t" + "BatchRvt.exe --task_script MyTask.py --file_list RevitFileList.txt --per_file_timeout 15")
    output()
    return


def ShowInvalidOptionsError(invalidOptions, output):
    output()
    output("ERROR: unknown command-line option(s):")
    output()
    for invalidOption in invalidOptions:
        output("\t" + CommandLineUtil.OptionSwitchPrefix + invalidOption)
    output()
    output("See --help option for command-line usage.")
    return


def InitializeBatchRvtSettings(commandSettingsData, batchRvtConfig, revitFileListOption, taskScriptFilePathOption,
                               output):
    batchRvtSettings = None
    if commandSettingsData is not None and commandSettingsData.Settings is not None:
        # Initialize from in-memory object.
        batchRvtSettings = commandSettingsData.Settings
    elif batchRvtConfig.SettingsFilePath is not None:
        # Initialize from settings file specified at the command-line.
        batchRvtSettings = GetBatchRvtSettings(batchRvtConfig.SettingsFilePath, output)
    elif revitFileListOption is not None and taskScriptFilePathOption is not None:
        # Initialize settings for non-settings-file mode.
        batchRvtSettings = BatchRvtSettings()
        batchRvtSettings.RevitProcessingOption.SetValue(BatchRvt.RevitProcessingOption.BatchRevitFileProcessing)
        batchRvtSettings.RevitSessionOption.SetValue(
            BatchRvt.RevitSessionOption.UseSameSessionForFilesOfSameVersion)  # TODO: reconsider default?
        batchRvtSettings.RevitFileProcessingOption.SetValue(
            BatchRvt.RevitFileProcessingOption.UseFileRevitVersionIfAvailable)
        batchRvtSettings.IfNotAvailableUseMinimumAvailableRevitVersion.SetValue(False)  # TODO: reconsider default?
        batchRvtSettings.AuditOnOpening.SetValue(False)
        batchRvtSettings.ProcessingTimeOutInMinutes.SetValue(0)
    else:
        output()
        output("ERROR: No settings file specified or settings file not found.")
    return batchRvtSettings


def InitializeBatchRvtConfig(commandSettingsData, commandLineOptions):
    batchRvtConfig = BatchRvtConfig()

    if commandSettingsData is not None:
        batchRvtConfig.SettingsFilePath = commandSettingsData.SettingsFilePath
    else:
        batchRvtConfig.SettingsFilePath = commandLineOptions[CommandSettings.SETTINGS_FILE_PATH_OPTION]

    if commandSettingsData is not None:
        batchRvtConfig.LogFolderPath = commandSettingsData.LogFolderPath
    else:
        batchRvtConfig.LogFolderPath = commandLineOptions[CommandSettings.LOG_FOLDER_PATH_OPTION]

    batchRvtConfig.SessionId = commandLineOptions[CommandSettings.SESSION_ID_OPTION]

    batchRvtConfig.SessionId, batchRvtConfig.SessionStartTime = ParseSessionIdAndStartTime(batchRvtConfig.SessionId)

    if commandSettingsData is not None:
        batchRvtConfig.TaskData = commandSettingsData.TaskData
    else:
        batchRvtConfig.TaskData = commandLineOptions[CommandSettings.TASK_DATA_OPTION]

    # NOTE: use of output function must occur after the log file initialization
    batchRvtConfig.LogFilePath = InitializeLogging(batchRvtConfig.LogFolderPath, batchRvtConfig.SessionStartTime)

    if commandSettingsData is not None:
        commandSettingsData.GeneratedLogFilePath = batchRvtConfig.LogFilePath

    testModeFolderPath = None
    if commandSettingsData is not None:
        testModeFolderPath = commandSettingsData.TestModeFolderPath
    else:
        testModeFolderPath = commandLineOptions[CommandSettings.TEST_MODE_FOLDER_PATH_OPTION]

    batchRvtConfig.TestModeFolderPath = (
        path_util.GetFullPath(testModeFolderPath)
        if not str.IsNullOrWhiteSpace(testModeFolderPath)
        else None
    )

    if commandSettingsData is not None:
        if commandSettingsData.RevitFileList is not None:
            # NOTE: list is constructed here because although the source object is an IEnumerable<string> it may not be a list.
            batchRvtConfig.RevitFileList = list(revitFilePath for revitFilePath in commandSettingsData.RevitFileList)

    return batchRvtConfig


def ConfigureBatchRvt(commandSettingsData, output):
    aborted = False

    commandLineOptions = CommandSettings.GetCommandLineOptions()
    batchRvtConfig = InitializeBatchRvtConfig(commandSettingsData, commandLineOptions)

    global_test_mode.InitializeGlobalTestMode(batchRvtConfig.TestModeFolderPath)
    global_test_mode.ExportSessionId(batchRvtConfig.SessionId)

    output()
    output("Session ID: " + batchRvtConfig.SessionId)

    output()
    output("Log File:")
    output()
    output("\t" + batchRvtConfig.LogFilePath)

    haveHelpOption = commandLineOptions[CommandSettings.HELP_OPTION]
    if not CommandLineUtil.HaveArguments() or haveHelpOption:
        ShowCommandLineHelp(output)
        aborted = True

    if not aborted:
        invalidOptions = CommandSettings.GetInvalidOptions()
        if invalidOptions.Any():
            ShowInvalidOptionsError(invalidOptions, output)
            aborted = True

    revitVersionOption, aborted = GetCommandSettingsOption(
        aborted,
        commandLineOptions,
        CommandSettings.REVIT_VERSION_OPTION,
        output
    )
    if not aborted and revitVersionOption is not None:
        output()
        output("Using specific Revit version: " + RevitVersion.GetRevitVersionText(revitVersionOption))

    revitFileListOption, aborted = GetCommandSettingsOption(
        aborted,
        commandLineOptions,
        CommandSettings.REVIT_FILE_LIST_OPTION,
        output,
        invalidValueErrorMessage="ERROR: Revit file list not found.",
        missingValueErrorMessage="ERROR: Missing Revit file list option value!"
    )

    taskScriptFilePathOption, aborted = GetCommandSettingsOption(
        aborted,
        commandLineOptions,
        CommandSettings.TASK_SCRIPT_FILE_PATH_OPTION,
        output,
        invalidValueErrorMessage="ERROR: Task script file not found.",
        missingValueErrorMessage="ERROR: Missing Task script file option value!"
    )

    centralFileOpenOption = None
    if not aborted:
        haveDetachOption = CommandLineUtil.HasCommandLineOption(CommandSettings.DETACH_OPTION, False)
        haveCreateNewLocalOption = CommandLineUtil.HasCommandLineOption(CommandSettings.CREATE_NEW_LOCAL_OPTION, False)
        if haveDetachOption and haveCreateNewLocalOption:
            output()
            output(
                "ERROR: You cannot specify both " +
                CommandLineUtil.OptionSwitchPrefix + CommandSettings.DETACH_OPTION +
                " and " +
                CommandLineUtil.OptionSwitchPrefix + CommandSettings.CREATE_NEW_LOCAL_OPTION +
                " options simultaneously."
            )
            aborted = True
        elif haveDetachOption or haveCreateNewLocalOption:
            centralFileOpenOption = (
                BatchRvt.CentralFileOpenOption.CreateNewLocal if haveCreateNewLocalOption
                else BatchRvt.CentralFileOpenOption.Detach  # default
            )

    worksetsOption, aborted = GetCommandSettingsOption(
        aborted,
        commandLineOptions,
        CommandSettings.WORKSETS_OPTION,
        output
    )

    auditOnOpeningOption = None
    if not aborted:
        haveAuditOnOpeningOption = CommandLineUtil.HasCommandLineOption(CommandSettings.AUDIT_ON_OPENING_OPTION, False)
        if haveAuditOnOpeningOption:
            auditOnOpeningOption = haveAuditOnOpeningOption

    perFileTimeoutOption, aborted = GetCommandSettingsOption(
        aborted,
        commandLineOptions,
        CommandSettings.PER_FILE_PROCESSING_TIMEOUT_OPTION,
        output
    )

    if (not RevitVersion.GetInstalledRevitVersions().Any()):
        output()
        output("ERROR: Could not detect the BatchRvt addin for any version of Revit installed on this machine!")
        output()
        output("You must first install the BatchRvt addin for at least one version of Revit.")
        aborted = True

    if not aborted:
        batchRvtSettings = InitializeBatchRvtSettings(commandSettingsData, batchRvtConfig, revitFileListOption,
                                                      taskScriptFilePathOption, output)
        if batchRvtSettings is None:
            aborted = True

    if not aborted:
        # Handles command-line overrides
        if revitVersionOption is not None:
            batchRvtSettings.RevitFileProcessingOption.SetValue(
                BatchRvt.RevitFileProcessingOption.UseSpecificRevitVersion)
            batchRvtSettings.BatchRevitTaskRevitVersion.SetValue(revitVersionOption)
        if revitFileListOption is not None:
            batchRvtSettings.RevitFileListFilePath.SetValue(revitFileListOption)
        if taskScriptFilePathOption is not None:
            batchRvtSettings.TaskScriptFilePath.SetValue(taskScriptFilePathOption)
        if centralFileOpenOption is not None:
            batchRvtSettings.CentralFileOpenOption.SetValue(centralFileOpenOption)
        if worksetsOption is not None:
            batchRvtSettings.WorksetConfigurationOption.SetValue(worksetsOption)
        if auditOnOpeningOption is not None:
            batchRvtSettings.AuditOnOpening.SetValue(auditOnOpeningOption)
        if perFileTimeoutOption is not None:
            batchRvtSettings.ProcessingTimeOutInMinutes.SetValue(perFileTimeoutOption)
        aborted = ConfigureBatchRvtSettings(batchRvtConfig, batchRvtSettings, output)

    return batchRvtConfig if not aborted else None
