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
from System import ArgumentException

import text_file_util
import console_util
import path_util
import revit_file_version
import batch_rvt_util
from batch_rvt_util import RevitVersion

def GetNonEmptyTrimmedTextValues(textValues):
  return list(
      textValue.Trim() for textValue in textValues
      if not str.IsNullOrWhiteSpace(textValue)
    )

def FirstOrDefault(items, default=None):
  for item in items:
    return item
  return default

def GetCentralFileListFromRows(rows):
  return GetNonEmptyTrimmedTextValues(FirstOrDefault(row) for row in rows)

def FromTextFile(textFilePath):
  rows = text_file_util.GetRowsFromTextFile(textFilePath)
  return GetCentralFileListFromRows(rows)

def FromText(text):
  rows = text_file_util.GetRowsFromText(text)
  return GetCentralFileListFromRows(rows)

def FromLines(lines):
  rows = text_file_util.GetRowsFromLines(lines)
  return GetCentralFileListFromRows(rows)

def IsExcelInstalled():
  return System.Type.GetTypeFromProgID("Excel.Application") is not None

def HasExcelFileExtension(filePath):
  return any(path_util.HasFileExtension(filePath, extension) for extension in [".xlsx", ".xls"])

def FromExcelFile(excelFilePath):
  centralFilePaths = []
  if IsExcelInstalled():
    import excel_util
    centralFilePaths = GetCentralFileListFromRows(excel_util.ReadRowsTextFromWorkbook(excelFilePath))
  else:
    raise Exception("ERROR: An Excel installation was not detected! Support for Excel files requires an Excel installation.")
  return centralFilePaths 

def FromConsole():
  return FromLines(console_util.ReadLines())

class RevitFileInfo():
  def __init__(self, revitFilePath):
    try:
      revitFilePath = path_util.GetFullPath(revitFilePath)
    except ArgumentException, e: # Catch ArgumentException exceptions such as 'Illegal characters in path.'
      pass
    self.revitFilePath = revitFilePath
    return

  def GetFullPath(self):
    return self.revitFilePath

  def GetFileSize(self):
    return path_util.GetFileSize(self.revitFilePath)

  def TryGetRevitVersionText(self):
    revitVersionText = None
    try:
      revitVersionText = revit_file_version.GetRevitVersionText(self.revitFilePath)
    except Exception, e:
      pass
    return revitVersionText

  def Exists(self):
    return path_util.FileExists(self.revitFilePath)

def GetRevitFileList(settingsFilePath):
  revitFileList = None
  if text_file_util.HasTextFileExtension(settingsFilePath):
    revitFileList = FromTextFile(settingsFilePath)
  elif HasExcelFileExtension(settingsFilePath):
    revitFileList = FromExcelFile(settingsFilePath)
  return revitFileList

REVIT_VERSION_TEXT_PREFIXES_2015 = ["Autodesk Revit 2015", "Autodesk Revit Architecture 2015"]
REVIT_VERSION_TEXT_PREFIXES_2016 = ["Autodesk Revit 2016", "Autodesk Revit Architecture 2016"]
REVIT_VERSION_TEXT_PREFIX_2017 = "Autodesk Revit 2017"
REVIT_VERSION_TEXT_PREFIX_2018 = "Autodesk Revit 2018"

class SupportedRevitFileInfo():
  def __init__(self, revitFilePath):
    self.revitFileInfo = RevitFileInfo(revitFilePath)
    revitVersionText = self.revitFileInfo.TryGetRevitVersionText()
    revitVersionNumber = None
    if revitVersionText is not None:
      if any(revitVersionText.StartsWith(prefix) for prefix in REVIT_VERSION_TEXT_PREFIXES_2015):
        revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2015
      elif any(revitVersionText.StartsWith(prefix) for prefix in REVIT_VERSION_TEXT_PREFIXES_2016):
        revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2016
      elif revitVersionText.StartsWith(REVIT_VERSION_TEXT_PREFIX_2017):
        revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2017
      elif revitVersionText.StartsWith(REVIT_VERSION_TEXT_PREFIX_2018):
        revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2018
    self.revitVersionNumber = revitVersionNumber
    return

  def TryGetRevitVersionNumber(self):
    return self.revitVersionNumber

  def GetRevitFileInfo(self):
    return self.revitFileInfo

