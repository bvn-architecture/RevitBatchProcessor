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
from System.Diagnostics import Process

import revit_process
import script_environment
import time_util

PROCESS_UNIQUE_ID_DELIMITER = "|"


def GetUniqueIdForProcess(process):
    return str.Join(
        PROCESS_UNIQUE_ID_DELIMITER,
        process.Id.ToString(),
        time_util.GetISO8601FormattedUtcDate(process.StartTime)
    )


def IsBatchRvtProcessRunning(batchRvtProcessUniqueId):
    def IsBatchRvtProcess(process):
        isTargetProcess = False
        try:
            isTargetProcess = (GetUniqueIdForProcess(process) == batchRvtProcessUniqueId)
        except Exception, e:
            isTargetProcess = False
        return isTargetProcess

    batchRvtProcess = Process.GetProcesses().FirstOrDefault(IsBatchRvtProcess)
    return (batchRvtProcess is not None)


def StartHostRevitProcess(
        revitVersion,
        batchRvtScriptsFolderPath,
        scriptFilePath,
        scriptDataFilePath,
        progressNumber,
        scriptOutputPipeHandleString,
        testModeFolderPath
):
    batchRvtProcessUniqueId = GetUniqueIdForProcess(Process.GetCurrentProcess())

    def initEnvironmentVariables(environmentVariables):
        script_environment.InitEnvironmentVariables(
            environmentVariables,
            batchRvtScriptsFolderPath,
            scriptFilePath,
            scriptDataFilePath,
            progressNumber,
            scriptOutputPipeHandleString,
            batchRvtProcessUniqueId,
            testModeFolderPath
        )
        return

    return revit_process.StartRevitProcess(revitVersion, initEnvironmentVariables)
