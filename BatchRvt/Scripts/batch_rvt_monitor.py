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
from System.IO import Path

import path_util
import revit_file_list
import batch_rvt_monitor_util
import snapshot_data_util
import session_data_util
import session_data_exporter
import exception_util
import time_util
import script_util
from script_util import Output
import batch_rvt_config
import batch_rvt_util
from batch_rvt_util import RevitVersion, ScriptDataUtil, BatchRvt

SCRIPTS_FOLDER_NAME = "Scripts"

def GetBatchRvtFolderPath():
  return AppDomain.CurrentDomain.BaseDirectory

def GetBatchRvtScriptsFolderPath():
  return Path.Combine(GetBatchRvtFolderPath(), SCRIPTS_FOLDER_NAME)

def HasSupportedRevitFilePath(supportedRevitFileInfo):
  fullFilePath = supportedRevitFileInfo.GetRevitFileInfo().GetFullPath()
  return True

def HasSupportedRevitVersion(supportedRevitFileInfo):
  return (
      supportedRevitFileInfo.TryGetRevitVersionNumber() in RevitVersion.GetInstalledRevitVersions()
    )

def GetRevitFileSize(supportedRevitFileInfo):
  return supportedRevitFileInfo.GetRevitFileInfo().GetFileSize()

def HasAllowedRevitVersion(batchRvtConfig, supportedRevitFileInfo):
  hasAllowedRevitVersion = False
  if (batchRvtConfig.RevitFileProcessingOption == BatchRvt.RevitFileProcessingOption.UseSpecificRevitVersion):
    revitVersion = supportedRevitFileInfo.TryGetRevitVersionNumber()
    if revitVersion is None or revitVersion <= batchRvtConfig.BatchRevitTaskRevitVersion:
      hasAllowedRevitVersion = True
  elif HasSupportedRevitVersion(supportedRevitFileInfo):
    hasAllowedRevitVersion = True
  elif batchRvtConfig.IfNotAvailableUseMinimumAvailableRevitVersion:
    hasAllowedRevitVersion = True
  return hasAllowedRevitVersion

def RevitFileExists(supportedRevitFileInfo):
  return supportedRevitFileInfo.GetRevitFileInfo().Exists()

def GetSupportedRevitFiles(batchRvtConfig):
  supportedRevitFileList = None

  revitFileList = batchRvtConfig.ReadRevitFileList(Output)

  if revitFileList is not None:
    supportedRevitFileList = list(
        revit_file_list.SupportedRevitFileInfo(revitFilePath.Trim('"'))
        for revitFilePath in revitFileList
      )

    nonExistentRevitFileList = list(
        supportedRevitFileInfo
        for supportedRevitFileInfo in supportedRevitFileList
        if not RevitFileExists(supportedRevitFileInfo)
      )

    supportedRevitFileList = list(
        supportedRevitFileInfo
        for supportedRevitFileInfo in supportedRevitFileList
        if RevitFileExists(supportedRevitFileInfo)
      )

    unsupportedRevitFileList = list(
        supportedRevitFileInfo
        for supportedRevitFileInfo in supportedRevitFileList
        if not HasAllowedRevitVersion(batchRvtConfig, supportedRevitFileInfo)
      )

    unsupportedRevitFilePathRevitFileList = list(
        supportedRevitFileInfo
        for supportedRevitFileInfo in supportedRevitFileList
        if (
            not HasSupportedRevitFilePath(supportedRevitFileInfo)
          )
      )

    supportedRevitFileList = list(
        supportedRevitFileInfo
        for supportedRevitFileInfo in supportedRevitFileList
        if (
            RevitFileExists(supportedRevitFileInfo)
            and
            HasAllowedRevitVersion(batchRvtConfig, supportedRevitFileInfo)
            and
            HasSupportedRevitFilePath(supportedRevitFileInfo)
          )
      ).OrderBy(lambda supportedRevitFileInfo: GetRevitFileSize(supportedRevitFileInfo)).ToList()

    nonExistentCount = len(nonExistentRevitFileList)
    unsupportedCount = len(unsupportedRevitFileList)
    unsupportedRevitFilePathCount = len(unsupportedRevitFilePathRevitFileList)

    if nonExistentCount > 0:
      Output()
      Output("WARNING: The following Revit Files do not exist (" + str(nonExistentCount) + "):")
      for supportedRevitFileInfo in nonExistentRevitFileList:
        batch_rvt_monitor_util.ShowSupportedRevitFileInfo(supportedRevitFileInfo, Output)

    if unsupportedCount > 0:
      Output()
      Output("WARNING: The following Revit Files are of an unsupported version (" + str(unsupportedCount) + "):")
      for supportedRevitFileInfo in unsupportedRevitFileList:
        batch_rvt_monitor_util.ShowSupportedRevitFileInfo(supportedRevitFileInfo, Output)

    if unsupportedRevitFilePathCount > 0:
      Output()
      Output("WARNING: The following Revit Files have an unsupported file path (" + str(unsupportedRevitFilePathCount) + "):")
      for supportedRevitFileInfo in unsupportedRevitFilePathRevitFileList:
        batch_rvt_monitor_util.ShowSupportedRevitFileInfo(supportedRevitFileInfo, Output)

  return supportedRevitFileList

def InitializeScriptUtil(batchRvtConfig):
  script_util.SetSessionId(batchRvtConfig)
  script_util.SetExportFolderPath(batchRvtConfig)
  script_util.SetSessionDataFolderPath(batchRvtConfig)
  script_util.SetRevitFileListFilePath(batchRvtConfig)
  return

def ExecutePreProcessingScript(batchRvtConfig, output):
  aborted = False
  try:
    InitializeScriptUtil(batchRvtConfig)
    output()
    output("Pre-processing script operation started.")
    script_util.ExecuteScript(batchRvtConfig.PreProcessingScriptFilePath)
    output()
    output("Pre-processing script operation completed.")
  except Exception, e:
    output()
    output("ERROR: An error occurred while executing the pre-processing script! Operation aborted.")
    exception_util.LogOutputErrorDetails(e, output)
    aborted = True
  return aborted

def ExecutePostProcessingScript(batchRvtConfig, output):
  aborted = False
  try:
    InitializeScriptUtil(batchRvtConfig)
    output()
    output("Post-processing script operation started.")
    script_util.ExecuteScript(batchRvtConfig.PostProcessingScriptFilePath)
    output()
    output("Post-processing script operation completed.")
  except Exception, e:
    output()
    output("ERROR: An error occurred while executing the post-processing script! Operation aborted.")
    exception_util.LogOutputErrorDetails(e, output)
    aborted = True
  return aborted

def RunSingleRevitTask(batchRvtConfig):
  aborted = False

  if not aborted:
    if batchRvtConfig.ExecutePreProcessingScript:
      aborted = ExecutePreProcessingScript(batchRvtConfig, Output)

  if not aborted:
    Output()
    Output("Starting single task operation...")

    revitVersion = batchRvtConfig.SingleRevitTaskRevitVersion
    Output()
    Output("Starting Revit " + RevitVersion.GetRevitVersionText(revitVersion) + " session...")

    scriptData = ScriptDataUtil.ScriptData()
    scriptData.SessionId.SetValue(batchRvtConfig.SessionId)
    scriptData.EnableDataExport.SetValue(batchRvtConfig.EnableDataExport)
    scriptData.SessionDataFolderPath.SetValue(batchRvtConfig.SessionDataFolderPath)
    scriptData.ShowMessageBoxOnTaskScriptError.SetValue(batchRvtConfig.ShowMessageBoxOnTaskError)
    scriptData.RevitProcessingOption.SetValue(batchRvtConfig.RevitProcessingOption)
    scriptData.ProgressNumber.SetValue(1)
    scriptData.ProgressMax.SetValue(1)

    batchRvtScriptsFolderPath = GetBatchRvtScriptsFolderPath()

    batch_rvt_monitor_util.RunScriptedRevitSession(
        revitVersion,
        batchRvtScriptsFolderPath,
        batchRvtConfig.ScriptFilePath,
        [scriptData],
        1,
        batchRvtConfig.ProcessingTimeOutInMinutes,
        Output
      )

  if not aborted:
    if batchRvtConfig.ExecutePostProcessingScript:
      aborted = ExecutePostProcessingScript(batchRvtConfig, Output)

  return aborted

def GetRevitVersionForRevitFileSession(batchRvtConfig, supportedRevitFileInfo):
  revitVersion = RevitVersion.GetMinimumInstalledRevitVersion()
  if (batchRvtConfig.RevitFileProcessingOption == BatchRvt.RevitFileProcessingOption.UseSpecificRevitVersion):
    revitVersion = batchRvtConfig.BatchRevitTaskRevitVersion
  elif HasSupportedRevitVersion(supportedRevitFileInfo):
    revitVersion = supportedRevitFileInfo.TryGetRevitVersionNumber()
  return revitVersion

def GroupByRevitVersion(batchRvtConfig, supportedRevitFileList):
  return (
      supportedRevitFileList.GroupBy(
        lambda supportedRevitFileInfo: GetRevitVersionForRevitFileSession(batchRvtConfig, supportedRevitFileInfo)
      ).OrderBy(
        lambda g: g.Key
      ).Select(
        lambda g: (g.Key, g.ToList())
      ).ToList()
    )

def ProcessRevitFiles(batchRvtConfig, supportedRevitFileList):
  aborted = False

  totalFilesCount = len(supportedRevitFileList)
  progressNumber = 1

  for revitVersion, supportedRevitFiles in GroupByRevitVersion(batchRvtConfig, supportedRevitFileList):

    sessionRevitFiles = []
    queuedRevitFiles = list(supportedRevitFiles)

    while queuedRevitFiles.Any():
      scriptDatas = []
      snapshotDataExportFolderPaths = []

      if batchRvtConfig.RevitSessionOption == BatchRvt.RevitSessionOption.UseSameSessionForFilesOfSameVersion:
        sessionRevitFiles = queuedRevitFiles
        queuedRevitFiles = []
      else:
        sessionRevitFiles = [queuedRevitFiles[0]]
        queuedRevitFiles = queuedRevitFiles[1:]

      sessionFilesCount = len(sessionRevitFiles)
      if len(sessionRevitFiles) == 1:
        Output()
        Output(
            "Processing Revit file (" + str(progressNumber) + " of " + str(totalFilesCount) + ")" +
            " in Revit " + RevitVersion.GetRevitVersionText(revitVersion) + " session."
          )
      else:
        Output()
        Output(
            "Processing Revit files (" + str(progressNumber) + " to " + str(progressNumber+sessionFilesCount-1) +
            " of " + str(totalFilesCount) + ")" +
            " in Revit " + RevitVersion.GetRevitVersionText(revitVersion) + " session."
          )

      for supportedRevitFileInfo in sessionRevitFiles:
        batch_rvt_monitor_util.ShowSupportedRevitFileInfo(supportedRevitFileInfo, Output)

      Output()
      Output("Starting Revit " + RevitVersion.GetRevitVersionText(revitVersion) + " session...")

      for index, supportedRevitFileInfo in enumerate(sessionRevitFiles):
        snapshotDataExportFolderPath = str.Empty
        revitFilePath = supportedRevitFileInfo.GetRevitFileInfo().GetFullPath()
        
        if batchRvtConfig.EnableDataExport:
          snapshotDataExportFolderPath = snapshot_data_util.GetSnapshotFolderPath(
              batchRvtConfig.DataExportFolderPath,
              revitFilePath,
              batchRvtConfig.SessionStartTime
            )
          path_util.CreateDirectory(snapshotDataExportFolderPath)
          snapshotDataExportFolderPaths.append(snapshotDataExportFolderPath)

        revitFilePath = supportedRevitFileInfo.GetRevitFileInfo().GetFullPath()
        scriptData = ScriptDataUtil.ScriptData()
        scriptData.SessionId.SetValue(batchRvtConfig.SessionId)
        scriptData.RevitFilePath.SetValue(revitFilePath)
        scriptData.OpenInUI.SetValue(batchRvtConfig.OpenInUI)
        scriptData.EnableDataExport.SetValue(batchRvtConfig.EnableDataExport)
        scriptData.SessionDataFolderPath.SetValue(batchRvtConfig.SessionDataFolderPath)
        scriptData.DataExportFolderPath.SetValue(snapshotDataExportFolderPath)
        scriptData.ShowMessageBoxOnTaskScriptError.SetValue(batchRvtConfig.ShowMessageBoxOnTaskError)
        scriptData.RevitProcessingOption.SetValue(batchRvtConfig.RevitProcessingOption)
        scriptData.CentralFileOpenOption.SetValue(batchRvtConfig.CentralFileOpenOption)
        scriptData.DeleteLocalAfter.SetValue(batchRvtConfig.DeleteLocalAfter)
        scriptData.DiscardWorksetsOnDetach.SetValue(batchRvtConfig.DiscardWorksetsOnDetach)
        scriptData.ProgressNumber.SetValue(progressNumber+index)
        scriptData.ProgressMax.SetValue(totalFilesCount)
        scriptDatas.append(scriptData)

      batchRvtScriptsFolderPath = GetBatchRvtScriptsFolderPath()

      while scriptDatas.Any():
        nextProgressNumber = batch_rvt_monitor_util.RunScriptedRevitSession(
            revitVersion,
            batchRvtScriptsFolderPath,
            batchRvtConfig.ScriptFilePath,
            scriptDatas,
            progressNumber,
            batchRvtConfig.ProcessingTimeOutInMinutes,
            Output
          )

        if nextProgressNumber is None:
          Output()
          Output("WARNING: The Revit session failed to initialize properly! No Revit files were processed in this session!")
          progressNumber += len(scriptDatas)
          break
        else:
          progressNumber = nextProgressNumber

        scriptDatas = (
            scriptDatas
            .Where(lambda scriptData: scriptData.ProgressNumber.GetValue() >= progressNumber)
            .ToList()
          )

        if batchRvtConfig.EnableDataExport:
          Output()
          Output("Consolidating snapshots data.")
          for snapshotDataExportFolderPath in snapshotDataExportFolderPaths:
            snapshot_data_util.ConsolidateSnapshotData(snapshotDataExportFolderPath, Output)
            # NOTE: Have disabled copying of journal files for now because if many files were processed
            #       in the same Revit session, too many copies of a potentially large journal file
            #       will be made. Consider modifying the logic so that the journal file is copied only
            #       once per Revit seesion. Perhaps copy it to the BatchRvt session folder.
            if False:
              try:
                snapshot_data_util.CopySnapshotRevitJournalFile(snapshotDataExportFolderPath, Output)
              except Exception, e:
                Output()
                Output("WARNING: failed to copy the Revit session's journal file to snapshot data folder:")
                Output()
                Output("\t" + snapshotDataExportFolderPath)
                exception_util.LogOutputErrorDetails(e, Output)
        
  return aborted

def RunBatchRevitTasks(batchRvtConfig):
  aborted = False

  if not aborted:
    if batchRvtConfig.ExecutePreProcessingScript:
      aborted = ExecutePreProcessingScript(batchRvtConfig, Output)

  if not aborted:
    supportedRevitFileList = GetSupportedRevitFiles(batchRvtConfig)
    if supportedRevitFileList is None:
      aborted = True

  if not aborted:
    supportedCount = len(supportedRevitFileList)
    if supportedCount == 0:
      Output()
      Output("ERROR: All specified Revit Files are of an unsupported version or have an unsupported file path.")
      aborted = True

  if not aborted:
    Output()
    Output("Revit Files for processing (" + str(supportedCount) + "):")
    for supportedRevitFileInfo in supportedRevitFileList:
      batch_rvt_monitor_util.ShowSupportedRevitFileInfo(supportedRevitFileInfo, Output)

    if batchRvtConfig.EnableDataExport:
      session_data_exporter.ExportSessionFilesData(
          batchRvtConfig.SessionDataFolderPath,
          batchRvtConfig.SessionId,
          [
            supportedRevitFileInfo.GetRevitFileInfo().GetFullPath()
            for supportedRevitFileInfo in supportedRevitFileList
          ]
        )

  if not aborted:
    Output()
    Output("Starting batch operation...")
    aborted = ProcessRevitFiles(batchRvtConfig, supportedRevitFileList)

  if not aborted:
    if batchRvtConfig.ExecutePostProcessingScript:
      aborted = ExecutePostProcessingScript(batchRvtConfig, Output)

  return aborted

def Main():
  aborted = False

  batchRvtConfig = batch_rvt_config.ConfigureBatchRvt(Output)

  if batchRvtConfig is None:
    aborted = True
  else:
    if batchRvtConfig.EnableDataExport:
      path_util.CreateDirectory(batchRvtConfig.SessionDataFolderPath)

      session_data_exporter.ExportSessionData(
          batchRvtConfig.SessionId,
          batchRvtConfig.SessionStartTime,
          None,
          batchRvtConfig.SessionDataFolderPath,
          None
        )

    sessionError = None
    try:
      if batchRvtConfig.RevitProcessingOption == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing:
        aborted = RunBatchRevitTasks(batchRvtConfig)
      else:
        aborted = RunSingleRevitTask(batchRvtConfig)
    except Exception, e:
      sessionError = exception_util.GetExceptionDetails(e)
      raise
    finally:
      if batchRvtConfig.EnableDataExport:
        sessionEndTime = time_util.GetDateTimeNow()

        session_data_exporter.ExportSessionData(
            batchRvtConfig.SessionId,
            batchRvtConfig.SessionStartTime,
            sessionEndTime,
            batchRvtConfig.SessionDataFolderPath,
            sessionError
          )
  
  Output()
  if aborted:
    Output("Operation aborted.")
  else:
    Output("Operation completed.")
  Output()
  return

try:
  Main()
except Exception, e:
  exception_util.LogOutputErrorDetails(e, Output)
  raise
