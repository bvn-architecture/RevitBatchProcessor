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

BATCHRVT_SCRIPTS_FOLDER_PATH__ENVIRONMENT_VARIABLE_NAME = "BATCHRVT__SCRIPTS_FOLDER_PATH"
SCRIPT_FILE_PATH__ENVIRONMENT_VARIABLE_NAME = r"BATCHRVT__SCRIPT_FILE_PATH"
SCRIPT_DATA_FILE_PATH__ENVIRONMENT_VARIABLE_NAME = r"BATCHRVT__SCRIPT_DATA_FILE_PATH"
SCRIPT_OUTPUT_PIPE_HANDLE_STRING__ENVIRONMENT_VARIABLE_NAME = r"BATCHRVT__SCRIPT_OUTPUT_PIPE_HANDLE_STRING"

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

def SetScriptOutputPipeHandleString(environmentVariables, scriptOutputPipeHandleString):
  SetEnvironmentVariable(
      environmentVariables,
      SCRIPT_OUTPUT_PIPE_HANDLE_STRING__ENVIRONMENT_VARIABLE_NAME,
      scriptOutputPipeHandleString
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

def GetScriptOutputPipeHandleString(environmentVariables):
  return GetEnvironmentVariable(
        environmentVariables,
        SCRIPT_OUTPUT_PIPE_HANDLE_STRING__ENVIRONMENT_VARIABLE_NAME
      )

def InitEnvironmentVariables(environmentVariables, batchRvtScriptsFolderPath, scriptFilePath, scriptDataFilePath, scriptOutputPipeHandleString):
  SetBatchRvtScriptsFolderPath(environmentVariables, batchRvtScriptsFolderPath)
  SetScriptFilePath(environmentVariables, scriptFilePath)
  SetScriptDataFilePath(environmentVariables, scriptDataFilePath)
  SetScriptOutputPipeHandleString(environmentVariables, scriptOutputPipeHandleString)
  return

