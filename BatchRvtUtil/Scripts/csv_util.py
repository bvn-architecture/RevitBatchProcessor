#
# Revit Batch Processor
#
# Copyright (c) 2019  Dan Rumery, BVN
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

from System.Text import Encoding
from System.IO import File
import path_util

CSV_FILE_EXTENSION = ".csv"

def ReadAllLines(filePath):
  lines = File.ReadAllLines(filePath)
  if len(lines) > 0: # Workaround for a potential lack of detection of Unicode txt files.
    if lines[0].Contains("\x00"):
      lines = File.ReadAllLines(filePath, Encoding.Unicode)
  return lines

def GetRowsFromLines(lines):
  return [line.split(',') for line in lines]

def WriteToCSVFile(rows, csvFilePath, delimiter, encoding):
  lines = [str.Join(delimiter, [str(value) for value in row]) for row in rows]
  File.WriteAllLines(csvFilePath, lines, encoding)
  return

def WriteToTabDelimitedTxtFile(rows, txtFilePath, encoding=Encoding.UTF8):
  WriteToCSVFile(rows, txtFilePath, "\t", encoding)
  return

def HasCSVFileExtension(filePath):
  return path_util.HasFileExtension(filePath, CSV_FILE_EXTENSION)

def GetRowsFromCSVFile(csvFilePath):
  lines = ReadAllLines(csvFilePath)
  return GetRowsFromLines(lines)
