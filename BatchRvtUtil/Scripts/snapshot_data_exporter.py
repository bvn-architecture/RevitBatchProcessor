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

from System.IO import Path

import environment
import json_util
import network_util
import path_util
import revit_file_util
import snapshot_data_util
import text_file_util
import time_util


def GetSnapshotData(
        sessionId,
        revitFilePath,
        isCloudModel,
        cloudProjectId,
        cloudModelId,
        snapshotStartTime,
        snapshotEndTime,
        snapshotFolderPath,
        revitJournalFilePath,
        snapshotError
):
    if isCloudModel:
        projectFolderName = None
        projectModelFolderPath = None
        modelFolder = None
        modelName = None
        modelFileLastModified = None
        modelFileSize = None
        modelRevitVersion = None
        modelRevitVersionDetails = None
    else:
        projectFolderName = path_util.GetProjectFolderNameFromRevitProjectFilePath(revitFilePath)
        projectModelFolderPath = Path.GetDirectoryName(revitFilePath)
        modelFolder = path_util.ExpandedFullNetworkPath(projectModelFolderPath)
        modelName = snapshot_data_util.GetRevitModelName(revitFilePath)
        modelFileLastModified = path_util.GetLastWriteTimeUtc(revitFilePath)
        modelFileSize = path_util.GetFileSize(revitFilePath)
        modelRevitVersion = revit_file_util.GetRevitFileVersion(revitFilePath)
        modelRevitVersionDetails = snapshot_data_util.GetRevitFileVersionDetails(revitFilePath)

    snapshotData = {
        "isCloudModel": isCloudModel,
        "cloudProjectId": cloudProjectId,
        "cloudModelId": cloudModelId,
        "projectFolderName": projectFolderName,
        "modelFolder": modelFolder,
        "modelName": modelName,
        "modelFileLastModified": (
            time_util.GetTimestampObject(modelFileLastModified)
            if modelFileLastModified is not None else None
        ),
        "modelFileSize": modelFileSize,
        "modelRevitVersion": modelRevitVersion,
        "modelRevitVersionDetails": modelRevitVersionDetails,
        "snapshotStartTime": (
            time_util.GetTimestampObject(snapshotStartTime)
            if snapshotStartTime is not None else None
        ),
        "snapshotEndTime": (
            time_util.GetTimestampObject(snapshotEndTime)
            if snapshotEndTime is not None else None
        ),
        "sessionId": sessionId,
        "snapshotFolder": path_util.ExpandedFullNetworkPath(snapshotFolderPath),
        "snapshotError": snapshotError,
        "username": environment.GetUserName(),
        "machineName": environment.GetMachineName(),
        "gatewayAddresses": network_util.GetGatewayAddresses(),
        "ipAddresses": network_util.GetIPAddresses(),
        snapshot_data_util.SNAPSHOT_DATA__REVIT_JOURNAL_FILE: revitJournalFilePath
    }

    return snapshotData


def ExportSnapshotDataInternal(
        snapshotDataFilePath,
        sessionId,
        revitProjectFilePath,
        isCloudModel,
        cloudProjectId,
        cloudModelId,
        snapshotStartTime,
        snapshotEndTime,
        dataExportFolderPath,
        revitJournalFilePath,
        snapshotError
):
    snapshotData = GetSnapshotData(
        sessionId,
        revitProjectFilePath,
        isCloudModel,
        cloudProjectId,
        cloudModelId,
        snapshotStartTime,
        snapshotEndTime,
        dataExportFolderPath,
        revitJournalFilePath,
        snapshotError
    )
    serializedSnapshotData = json_util.SerializeObject(snapshotData, True)
    text_file_util.WriteToTextFile(snapshotDataFilePath, serializedSnapshotData)
    return snapshotData


def ExportSnapshotData(
        sessionId,
        revitProjectFilePath,
        isCloudModel,
        cloudProjectId,
        cloudModelId,
        snapshotStartTime,
        snapshotEndTime,
        dataExportFolderPath,
        revitJournalFilePath,
        snapshotError
):
    snapshotData = ExportSnapshotDataInternal(
        snapshot_data_util.GetSnapshotDataFilePath(dataExportFolderPath),
        sessionId,
        revitProjectFilePath,
        isCloudModel,
        cloudProjectId,
        cloudModelId,
        snapshotStartTime,
        snapshotEndTime,
        dataExportFolderPath,
        revitJournalFilePath,
        snapshotError
    )
    return snapshotData


def ExportTemporarySnapshotData(
        sessionId,
        revitProjectFilePath,
        isCloudModel,
        cloudProjectId,
        cloudModelId,
        snapshotStartTime,
        snapshotEndTime,
        dataExportFolderPath,
        revitJournalFilePath,
        snapshotError
):
    snapshotData = ExportSnapshotDataInternal(
        snapshot_data_util.GetTemporarySnapshotDataFilePath(dataExportFolderPath),
        sessionId,
        revitProjectFilePath,
        isCloudModel,
        cloudProjectId,
        cloudModelId,
        snapshotStartTime,
        snapshotEndTime,
        dataExportFolderPath,
        revitJournalFilePath,
        snapshotError
    )
    return snapshotData
