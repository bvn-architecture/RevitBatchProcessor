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

import revit_process
import script_environment

def StartHostRevitProcess(revitVersion, batchRvtScriptsFolderPath, scriptFilePath, scriptDataFilePath, scriptOutputPipeHandleString):
  def initEnvironmentVariables(environmentVariables):
    script_environment.InitEnvironmentVariables(
        environmentVariables,
        batchRvtScriptsFolderPath,
        scriptFilePath,
        scriptDataFilePath,
        scriptOutputPipeHandleString
      )
    return
  return revit_process.StartRevitProcess(revitVersion, initEnvironmentVariables)

