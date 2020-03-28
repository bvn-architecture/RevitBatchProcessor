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
from System import AppDomain
from System.IO import IOException, Path

BATCH_RVT_UTIL_ASSEMBLY_NAME = "BatchRvtUtil"
BATCH_RVT_SCRIPT_HOST_ASSEMBLY_NAME = "BatchRvtScriptHost"

def GetExistingLoadedAssembly(assemblyName):
  return (
      AppDomain.CurrentDomain.GetAssemblies()
      .FirstOrDefault(lambda assembly: assembly.GetName().Name == assemblyName)
    )

def AddBatchRvtUtilAssemblyReference():
  try:
    clr.AddReference(BATCH_RVT_UTIL_ASSEMBLY_NAME)
  except IOException, e: # Can occur if PyRevit is installed. Need to use AddReferenceToFileAndPath() in this case.
    batchRvtScriptHostAssembly = GetExistingLoadedAssembly(BATCH_RVT_SCRIPT_HOST_ASSEMBLY_NAME)
    clr.AddReference(batchRvtScriptHostAssembly)
    from BatchRvt.ScriptHost import ScriptHostUtil
    environmentVariables = ScriptHostUtil.GetEnvironmentVariables()
    batchRvtFolderPath = ScriptHostUtil.GetBatchRvtFolderPath(environmentVariables)
    clr.AddReferenceToFileAndPath(Path.Combine(batchRvtFolderPath, BATCH_RVT_UTIL_ASSEMBLY_NAME))
  return

AddBatchRvtUtilAssemblyReference()

import BatchRvtUtil
from BatchRvtUtil import *

