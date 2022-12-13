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

from System import NullReferenceException
from System.Diagnostics import Process

BATCHRVT_SCRIPTS_FOLDER_PATH__ENVIRONMENT_VARIABLE_NAME = "BATCHRVT__SCRIPTS_FOLDER_PATH"
SCRIPT_FILE_PATH__ENVIRONMENT_VARIABLE_NAME = r"BATCHRVT__SCRIPT_FILE_PATH"
SCRIPT_DATA_FILE_PATH__ENVIRONMENT_VARIABLE_NAME = r"BATCHRVT__SCRIPT_DATA_FILE_PATH"
PROGRESS_NUMBER__ENVIRONMENT_VARIABLE_NAME = r"BATCHRVT__PROGRESS_NUMBER"
SCRIPT_OUTPUT_PIPE_HANDLE_STRING__ENVIRONMENT_VARIABLE_NAME = r"BATCHRVT__SCRIPT_OUTPUT_PIPE_HANDLE_STRING"
BATCHRVT_PROCESS_UNIQUE_ID__ENVIRONMENT_VARIABLE_NAME = r"BATCHRVT__PROCESS_UNIQUE_ID"
BATCHRVT_TEST_MODE_FOLDER_PATH__ENVIRONMENT_VARIABLE_NAME = r"BATCHRVT__TEST_MODE_FOLDER_PATH"


def GetEnvironmentVariable(environmentVariables, variableName):
    return environmentVariables.Item[variableName]


def SetEnvironmentVariable(environmentVariables, variableName, value):
    environmentVariables.Item[variableName] = value
    return


def SetBatchRvtScriptsFolderPath(environmentVariables, batchRvtScriptsFolderPath):
    SetEnvironmentVariable(
        environmentVariables,
        BATCHRVT_SCRIPTS_FOLDER_PATH__ENVIRONMENT_VARIABLE_NAME,
        batchRvtScriptsFolderPath
    )
    return


def SetScriptFilePath(environmentVariables, scriptFilePath):
    SetEnvironmentVariable(
        environmentVariables,
        SCRIPT_FILE_PATH__ENVIRONMENT_VARIABLE_NAME,
        scriptFilePath
    )
    return


def SetScriptDataFilePath(environmentVariables, scriptDataFilePath):
    SetEnvironmentVariable(
        environmentVariables,
        SCRIPT_DATA_FILE_PATH__ENVIRONMENT_VARIABLE_NAME,
        scriptDataFilePath
    )
    return


def SetProgressNumber(environmentVariables, progressNumber):
    SetEnvironmentVariable(
        environmentVariables,
        PROGRESS_NUMBER__ENVIRONMENT_VARIABLE_NAME,
        str(progressNumber)
    )


def SetScriptOutputPipeHandleString(environmentVariables, scriptOutputPipeHandleString):
    SetEnvironmentVariable(
        environmentVariables,
        SCRIPT_OUTPUT_PIPE_HANDLE_STRING__ENVIRONMENT_VARIABLE_NAME,
        scriptOutputPipeHandleString
    )
    return


def SetBatchRvtProcessUniqueId(environmentVariables, batchRvtProcessUniqueId):
    SetEnvironmentVariable(
        environmentVariables,
        BATCHRVT_PROCESS_UNIQUE_ID__ENVIRONMENT_VARIABLE_NAME,
        batchRvtProcessUniqueId
    )
    return


def SetTestModeFolderPath(environmentVariables, testModeFolderPath):
    SetEnvironmentVariable(
        environmentVariables,
        BATCHRVT_TEST_MODE_FOLDER_PATH__ENVIRONMENT_VARIABLE_NAME,
        testModeFolderPath
    )
    return


def GetBatchRvtScriptsFolderPath(environmentVariables):
    return GetEnvironmentVariable(
        environmentVariables,
        BATCHRVT_SCRIPTS_FOLDER_PATH__ENVIRONMENT_VARIABLE_NAME
    )


def GetScriptFilePath(environmentVariables):
    return GetEnvironmentVariable(
        environmentVariables,
        SCRIPT_FILE_PATH__ENVIRONMENT_VARIABLE_NAME
    )


def GetScriptDataFilePath(environmentVariables):
    return GetEnvironmentVariable(
        environmentVariables,
        SCRIPT_DATA_FILE_PATH__ENVIRONMENT_VARIABLE_NAME
    )


def GetProgressNumber(environmentVariables):
    progressNumber = GetEnvironmentVariable(
        environmentVariables,
        PROGRESS_NUMBER__ENVIRONMENT_VARIABLE_NAME
    )
    return int(progressNumber)


def GetScriptOutputPipeHandleString(environmentVariables):
    return GetEnvironmentVariable(
        environmentVariables,
        SCRIPT_OUTPUT_PIPE_HANDLE_STRING__ENVIRONMENT_VARIABLE_NAME
    )


def GetBatchRvtProcessUniqueId(environmentVariables):
    return GetEnvironmentVariable(
        environmentVariables,
        BATCHRVT_PROCESS_UNIQUE_ID__ENVIRONMENT_VARIABLE_NAME
    )


def GetTestModeFolderPath(environmentVariables):
    return GetEnvironmentVariable(
        environmentVariables,
        BATCHRVT_TEST_MODE_FOLDER_PATH__ENVIRONMENT_VARIABLE_NAME
    )


def InitEnvironmentVariables(
        environmentVariables,
        batchRvtScriptsFolderPath,
        scriptFilePath,
        scriptDataFilePath,
        progressNumber,
        scriptOutputPipeHandleString,
        batchRvtProcessUniqueId,
        testModeFolderPath
):
    SetBatchRvtScriptsFolderPath(environmentVariables, batchRvtScriptsFolderPath)
    SetScriptFilePath(environmentVariables, scriptFilePath)
    SetScriptDataFilePath(environmentVariables, scriptDataFilePath)
    SetProgressNumber(environmentVariables, progressNumber)
    SetScriptOutputPipeHandleString(environmentVariables, scriptOutputPipeHandleString)
    SetBatchRvtProcessUniqueId(environmentVariables, batchRvtProcessUniqueId)
    SetTestModeFolderPath(environmentVariables, testModeFolderPath)
    return


def GetEnvironmentVariables():
    environmentVariables = None
    # NOTE: Have encountered (at least once) a NullReferenceException upon accessing the EnvironmentVariables property!
    try:
        environmentVariables = Process.GetCurrentProcess().StartInfo.EnvironmentVariables
    except NullReferenceException, e:
        environmentVariables = None
    return environmentVariables
