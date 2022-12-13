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
import text_file_util
import time_util

SESSION_DATA_FILENAME = "session.json"
SESSION_FILES_DATA_FILENAME = "session_files.json"


def GetSessionData(sessionId, sessionStartTime, sessionEndTime, sessionDataFolderPath, sessionError):
    sessionData = {
        "sessionStartTime": time_util.GetTimestampObject(sessionStartTime),
        "sessionEndTime": time_util.GetTimestampObject(sessionEndTime) if sessionEndTime is not None else None,
        "sessionId": sessionId,
        "sessionFolder": path_util.ExpandedFullNetworkPath(sessionDataFolderPath),
        "sessionError": sessionError,
        "username": environment.GetUserName(),
        "machineName": environment.GetMachineName(),
        "gatewayAddresses": network_util.GetGatewayAddresses(),
        "ipAddresses": network_util.GetIPAddresses()
    }

    return sessionData


def ExportSessionData(sessionId, sessionStartTime, sessionEndTime, sessionDataFolderPath, sessionError):
    sessionData = GetSessionData(
        sessionId,
        sessionStartTime,
        sessionEndTime,
        sessionDataFolderPath,
        sessionError
    )
    serializedSessionData = json_util.SerializeObject(sessionData, True)
    sessionDataFilePath = Path.Combine(sessionDataFolderPath, SESSION_DATA_FILENAME)
    text_file_util.WriteToTextFile(sessionDataFilePath, serializedSessionData)
    return sessionData


def GetSessionFilesData(sessionId, sessionFiles):
    sessionFilesData = {
        "sessionId": sessionId,
        "sessionFiles": [
            (
                path_util.ExpandedFullNetworkPath(filePath)
                if Path.IsPathRooted(filePath)
                # Cloud models 'paths' are not normal file paths so this accounts for that.
                else filePath
            )
            for filePath in sessionFiles
        ]
    }

    return sessionFilesData


def ExportSessionFilesData(sessionDataFolderPath, sessionId, sessionFiles):
    sessionFilesData = GetSessionFilesData(sessionId, sessionFiles)
    serializedSessionFilesData = json_util.SerializeObject(sessionFilesData, True)
    sessionFilesDataFilePath = Path.Combine(sessionDataFolderPath, SESSION_FILES_DATA_FILENAME)
    text_file_util.WriteToTextFile(sessionFilesDataFilePath, serializedSessionFilesData)
    return sessionFilesData
