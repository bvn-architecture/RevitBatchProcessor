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
from System import ArgumentException, NotSupportedException, StringSplitOptions, Guid
from System.IO import PathTooLongException

import text_file_util
import csv_util
import console_util
import path_util
import revit_file_version
import batch_rvt_util
from batch_rvt_util import RevitVersion

class RevitFilePathData:
    def __init__(self, revitFilePath, associatedData):
        self.RevitFilePath = revitFilePath.Trim()
        self.AssociatedData = [value.Trim() for value in associatedData]
        return

def FirstOrDefault(items, default=None):
    for item in items:
        return item
    return default

def GetRevitFileListData(rows):
    return [
            RevitFilePathData(row[0], row[1:])
            for row in rows
            if not str.IsNullOrWhiteSpace(FirstOrDefault(row)) # Ignores rows where the first column's cell value is empty.
        ]

def FromTextFile(textFilePath):
    rows = text_file_util.GetRowsFromTextFile(textFilePath)
    return GetRevitFileListData(rows)

def FromText(text):
    rows = text_file_util.GetRowsFromText(text)
    return GetRevitFileListData(rows)

def FromLines(lines):
    rows = text_file_util.GetRowsFromLines(lines)
    return GetRevitFileListData(rows)

def FromCSVFile(csvFilePath):
        rows = csv_util.GetRowsFromCSVFile(csvFilePath)
        return GetRevitFileListData(rows)

def IsExcelInstalled():
    return System.Type.GetTypeFromProgID("Excel.Application") is not None

def HasExcelFileExtension(filePath):
    return any(path_util.HasFileExtension(filePath, extension) for extension in [".xlsx", ".xls"])

def FromExcelFile(excelFilePath):
    import excel_util
    return GetRevitFileListData(excel_util.ReadRowsTextFromWorkbook(excelFilePath))

def FromConsole():
    return FromLines(console_util.ReadLines())

class RevitCloudModelInfo:
    def __init__(self, cloudModelDescriptor):
        self.cloudModelDescriptor = cloudModelDescriptor
        self.projectGuid = None
        self.modelGuid = None
        self.revitVersionText = None
        self.isValid = False
        parts = self.GetCloudModelDescriptorParts(cloudModelDescriptor)
        numberOfParts = len(parts)
        if numberOfParts > 1 :
            revitVersionPart = str.Empty
            otherParts = parts
            if numberOfParts > 2 :
                revitVersionPart = parts[0]
                otherParts = parts[1:]
            self.projectGuid = self.SafeParseGuidText(otherParts[0])
            self.modelGuid = self.SafeParseGuidText(otherParts[1])
            if RevitVersion.IsSupportedRevitVersionNumber(revitVersionPart):
                self.revitVersionText = revitVersionPart
            self.isValid = (
                    self.projectGuid is not None
                    and
                    self.modelGuid is not None
                )
        return

    def IsValid(self):
        return self.isValid

    def GetProjectGuid(self):
        return self.projectGuid

    def GetModelGuid(self):
        return self.modelGuid

    def GetRevitVersionText(self):
        return self.revitVersionText

    def GetCloudModelDescriptorParts(self, cloudModelDescriptor):
        return cloudModelDescriptor.Split([" "].ToArray[str](), StringSplitOptions.RemoveEmptyEntries)

    def SafeParseGuidText(self, guidText):
        parsed, guid = Guid.TryParse(guidText)
        return guid if parsed else None

    def GetCloudModelDescriptor(self):
        return self.cloudModelDescriptor

class RevitFileInfo():
    def __init__(self, revitFilePath):
        self.cloudModelInfo = RevitCloudModelInfo(revitFilePath)
        pathException = None
        try:
            revitFilePath = path_util.GetFullPath(revitFilePath)
        except ArgumentException, e: # Catch exceptions such as 'Illegal characters in path.'
            pathException = e
        except NotSupportedException, e: # Catch exceptions such as 'The given path's format is not supported.'
            pathException = e
        except PathTooLongException, e: # Catch exceptions such as 'The specified path, file name, or both are too long.'
            pathException = e
        self.revitFilePath = revitFilePath
        self.pathException = pathException
        return

    def IsCloudModel(self):
        return self.GetRevitCloudModelInfo().IsValid()

    def GetRevitCloudModelInfo(self):
        return self.cloudModelInfo

    def IsValidFilePath(self):
        return self.pathException is None

    def IsFilePathTooLong(self):
        return isinstance(self.pathException, PathTooLongException)

    def GetFullPath(self):
        return (
                self.revitFilePath if not self.IsCloudModel()
                else
                self.GetRevitCloudModelInfo().GetCloudModelDescriptor()
            )

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

def FromFile(settingsFilePath):
    revitFileListData = None
    if text_file_util.HasTextFileExtension(settingsFilePath):
        revitFileListData = FromTextFile(settingsFilePath)
    elif csv_util.HasCSVFileExtension(settingsFilePath):
        revitFileListData = FromCSVFile(settingsFilePath)
    elif HasExcelFileExtension(settingsFilePath):
        revitFileListData = FromExcelFile(settingsFilePath)
    return revitFileListData

class SupportedRevitFileInfo():
    def __init__(self, revitFilePathData):
        self.revitFileInfo = RevitFileInfo(revitFilePathData.RevitFilePath)
        self.revitFilePathData = revitFilePathData
        revitVersionText = None
        revitVersionNumber = None
        if self.revitFileInfo.IsCloudModel():
            revitVersionText = self.revitFileInfo.GetRevitCloudModelInfo().GetRevitVersionText()
            if not str.IsNullOrWhiteSpace(revitVersionText):
                if RevitVersion.IsSupportedRevitVersionNumber(revitVersionText):
                    revitVersionNumber = RevitVersion.GetSupportedRevitVersion(revitVersionText)
        else:
            revitVersionText = self.revitFileInfo.TryGetRevitVersionText()
            if not str.IsNullOrWhiteSpace(revitVersionText):
                if any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2015):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2015
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2016):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2016
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2017):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2017
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2018):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2018
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2019):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2019
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2020):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2020
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2021):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2021
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2022):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2022
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2023):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2023
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2024):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2024
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2025):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2025
                elif any(revitVersionText.StartsWith(prefix) for prefix in revit_file_version.REVIT_VERSION_TEXT_PREFIXES_2026):
                    revitVersionNumber = RevitVersion.SupportedRevitVersion.Revit2026
        self.revitVersionText = revitVersionText
        self.revitVersionNumber = revitVersionNumber
        return

    def TryGetRevitVersionNumber(self):
        return self.revitVersionNumber

    def TryGetRevitVersionText(self):
        return self.revitVersionText

    def GetRevitFileInfo(self):
        return self.revitFileInfo

    def GetRevitFilePathData(self):
        return self.revitFilePathData

    def IsCloudModel(self):
        return self.GetRevitFileInfo().IsCloudModel()

    def GetRevitCloudModelInfo(self):
        return self.GetRevitFileInfo().GetRevitCloudModelInfo()

