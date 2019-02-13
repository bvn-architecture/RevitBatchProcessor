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
from System.IO import Path, File

import batch_rvt_util
from batch_rvt_util import LogFile

LOG_FILE = [None] # Needs to be a list so it can be captured by reference in closures.

def InitializeLogging(logName, logFolderPath):
  LOG_FILE[0] = LogFile(logName, logFolderPath, False)
  return

def GetLogFilePath():
  logFile = LOG_FILE[0]
  return logFile.GetLogFilePath() if logFile is not None else str.Empty

def DumpPlainTextLogFile():
  plainTextLogFilePath = None
  logFilePath = GetLogFilePath()
  if not str.IsNullOrWhiteSpace(logFilePath):
    plainTextLogFilePath = Path.Combine(
        Path.GetDirectoryName(logFilePath),
        Path.GetFileNameWithoutExtension(logFilePath) + ".txt"
      )
    try:
      File.WriteAllLines(
          plainTextLogFilePath,
          LogFile.ReadLinesAsPlainText(logFilePath)
        )
    except Exception, e:
      plainTextLogFilePath = None
  return plainTextLogFilePath

