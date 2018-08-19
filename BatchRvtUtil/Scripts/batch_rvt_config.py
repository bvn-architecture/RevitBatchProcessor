#
# Revit Batch Processor
#
# Copyright (c) 2017  Dan Rumery, BVN
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
from System import AppDomain
from System.IO import File, Directory

import test_mode_util
import time_util
import path_util
import logging_util
import snapshot_data_util
import session_data_util
import revit_file_list
import batch_rvt_util
import script_util
from batch_rvt_util import CommandSettings, BatchRvtSettings, BatchRvt, RevitVersion

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

    # General Task Script settings
    self.ScriptFilePath = None
    self.ShowMessageBoxOnTaskError = None
    self.ProcessingTimeOutInMinutes = 0

    # Revit File List settings
    self.RevitFileListFilePath = None

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
    self.OpenInUI = False

    return

  def ReadRevitFileList(self, output):
    revitFileList = None
    if self.RevitProcessingOption == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing:
      if BatchRvtSettings.IsAppDomainRevitFileListAvailable():
        output()
        output("Reading Revit File list from object input.")
        revitFileList = list(revitFilePath for revitFilePath in BatchRvtSettings.GetAppDomainRevitFileList())
      else:
        output()
        output("Reading Revit File list:")
        output()
        output("\t" + (self.RevitFileListFilePath if not str.IsNullOrWhiteSpace(self.RevitFileListFilePath) else "Not specified!"))
        if not File.Exists(self.RevitFileListFilePath):
          output()
          output("ERROR: No Revit file list specified or file not found.")
        elif revit_file_list.HasExcelFileExtension(self.RevitFileListFilePath) and not revit_file_list.IsExcelInstalled():
          output()
          output("ERROR: Could not read from the Excel Revit File list. An Excel installation was not detected!")
        else:
          revitFileList = revit_file_list.GetRevitFileList(self.RevitFileListFilePath)
  
      if revitFileList is None:
        output()
        output("ERROR: Could not read the Revit File list.")
      elif len(revitFileList) == 0:
        output()
        output("ERROR: Revit File list is empty.")
        revitFileList = None
    return revitFileList

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

    isDynamoTaskScript = path_util.HasFileExtension(batchRvtConfig.ScriptFilePath, script_util.DYNAMO_SCRIPT_FILE_EXTENSION)

    if isDynamoTaskScript:
      batchRvtConfig.OpenInUI = True # Always open and activate documents in the UI when executing a Dynamo task script.

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
          "Create New Local" if (batchRvtConfig.CentralFileOpenOption == BatchRvt.CentralFileOpenOption.CreateNewLocal)
          else "Detach from Central"
        )
      output()
      output("Central File Processing mode:")
      output()
      output("\t" + centralFileProcessingDescription)
      if (batchRvtConfig.CentralFileOpenOption == BatchRvt.CentralFileOpenOption.CreateNewLocal):
        if (batchRvtConfig.DeleteLocalAfter):
          output()
          output("\t" + "Local File will be deleted after processing.")
      elif (batchRvtConfig.CentralFileOpenOption == BatchRvt.CentralFileOpenOption.Detach):
        if (batchRvtConfig.DiscardWorksetsOnDetach):
          output()
          output("\t" + "Worksets will be discarded upon detach.")

  return aborted

def GetBatchRvtSettings(batchRvtConfig, output):
  aborted = False
  if BatchRvtSettings.IsAppDomainDataAvailable():
    batchRvtSettings = BatchRvtSettings()
    isSettingsLoaded = batchRvtSettings.LoadFromAppDomainData()
    if not isSettingsLoaded:
      output()
      output("ERROR: Could not load settings from the AppDomain data!")
      aborted = True
  elif not File.Exists(batchRvtConfig.SettingsFilePath):
    output()
    output("ERROR: No settings file specified or settings file not found.")
    aborted = True
  else:
    batchRvtSettings = BatchRvtSettings()
    isSettingsLoaded = batchRvtSettings.LoadFromFile(batchRvtConfig.SettingsFilePath)
    if not isSettingsLoaded:
      output()
      output("ERROR: Could not load settings from the settings file!")
      aborted = True
  return batchRvtSettings if not aborted else None

def ConfigureBatchRvt(output):
  aborted = False
  
  batchRvtConfig = BatchRvtConfig()
  
  options = CommandSettings.GetCommandLineOptions()
  
  batchRvtConfig.SettingsFilePath = options[CommandSettings.SETTINGS_FILE_PATH_OPTION]
  logFolderPathFromAppDomainData = CommandSettings.GetAppDomainDataLogFolderPath()
  if logFolderPathFromAppDomainData is not None:
    batchRvtConfig.LogFolderPath = logFolderPathFromAppDomainData
  else:
    batchRvtConfig.LogFolderPath = options[CommandSettings.LOG_FOLDER_PATH_OPTION]
  batchRvtConfig.SessionId = options[CommandSettings.SESSION_ID_OPTION]
  
  batchRvtConfig.SessionId, batchRvtConfig.SessionStartTime = ParseSessionIdAndStartTime(batchRvtConfig.SessionId)

  taskDataFromAppDomainData = CommandSettings.GetAppDomainDataTaskData()
  if taskDataFromAppDomainData is not None:
    batchRvtConfig.TaskData = taskDataFromAppDomainData
  else:
    batchRvtConfig.TaskData = options[CommandSettings.TASK_DATA_OPTION]

  # NOTE: use of output function must occur after the log file initialization
  batchRvtConfig.LogFilePath = InitializeLogging(batchRvtConfig.LogFolderPath, batchRvtConfig.SessionStartTime)

  BatchRvt.SetAppDomainDataLogFilePath(batchRvtConfig.LogFilePath)

  testModeFolderPath = CommandSettings.GetAppDomainDataTestModeFolderPath()
  if testModeFolderPath is not None:
    batchRvtConfig.TestModeFolderPath = testModeFolderPath
  else:
    batchRvtConfig.TestModeFolderPath = options[CommandSettings.TEST_MODE_FOLDER_PATH_OPTION]
  test_mode_util.InitializeTestMode(batchRvtConfig.TestModeFolderPath)

  output()
  output("Session ID: " + batchRvtConfig.SessionId)

  output()
  output("Log File:")
  output()
  output("\t" + batchRvtConfig.LogFilePath)

  if (not RevitVersion.GetInstalledRevitVersions().Any()):
    output()
    output("ERROR: Could not detect the BatchRvt addin for any version of Revit installed on this machine!")
    output()
    output("You must first install the BatchRvt addin for at least one version of Revit.")
    aborted = True

  if not aborted:
    batchRvtSettings = GetBatchRvtSettings(batchRvtConfig, output)
    if batchRvtSettings is None:
      aborted = True

  if not aborted:
    aborted = ConfigureBatchRvtSettings(batchRvtConfig, batchRvtSettings, output)

  return batchRvtConfig if not aborted else None

