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

clr.AddReference("System.Core")
import System.Linq
clr.ImportExtensions(System.Linq)
from System.Linq import Enumerable

import System.Reflection as Refl

import System.IO
from System.IO import IOException

from System.Reflection import TargetInvocationException

import System.Text
from System.Text import Encoding

clr.AddReference("WindowsBase")
import System.IO.Packaging as Packaging

import util

STORAGE_ROOT_TYPE_NAME = "System.IO.Packaging.StorageRoot"
STORAGE_ROOT_OPEN_METHOD_NAME = "Open"
BASIC_FILE_INFO_STREAM_NAME = "BasicFileInfo"

def GetWindowsBaseAssembly():
    return clr.GetClrType(Packaging.StorageInfo).Assembly

def GetStorageRootType():
    return GetWindowsBaseAssembly().GetType(STORAGE_ROOT_TYPE_NAME, True, False)

def InvokeStorageRootMember(storageRoot, methodName, *methodArgs):
    return GetStorageRootType().InvokeMember(
            methodName,
            Refl.BindingFlags.Static | Refl.BindingFlags.Instance |
            Refl.BindingFlags.Public | Refl.BindingFlags.NonPublic |
            Refl.BindingFlags.InvokeMethod,
            None,
            storageRoot,
            methodArgs.ToArray[object](),
        )

def GetStorageRoot(filePath):
    storageRoot = InvokeStorageRootMember(
                None,
                STORAGE_ROOT_OPEN_METHOD_NAME,
                filePath,
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read,
                System.IO.FileShare.Read
            ) if not str.IsNullOrWhiteSpace(filePath) else None
    return storageRoot

def GetBasicFileInfoStream(storageRoot):
    return storageRoot.GetStreamInfo(BASIC_FILE_INFO_STREAM_NAME).GetStream()

def CreateByteBuffer(length):
    return Enumerable.Repeat[System.Byte](0, length).ToArray()

def ReadAllBytes(stream):
    length = int(stream.Length)
    buffer = CreateByteBuffer(length)
    readCount = stream.Read(buffer, 0, length)
    return buffer.Take(readCount).ToArray()

def GetRevitVersionText_OldMethod(revitFilePath):
    storageRoot = GetStorageRoot(revitFilePath)
    stream = GetBasicFileInfoStream(storageRoot)
    bytes = ReadAllBytes(stream)
    unicodeString = Encoding.Unicode.GetString(bytes)
    start = unicodeString.IndexOf("Autodesk Revit")
    end = unicodeString.IndexOf("\x00", start)
    versionText = unicodeString.Substring(start, end - start)
    return versionText.Substring(0, versionText.LastIndexOf(")") + 1)

def GetBasicFileInfoBytes(revitFilePath):
    storageRoot = GetStorageRoot(revitFilePath)
    stream = GetBasicFileInfoStream(storageRoot)
    bytes = ReadAllBytes(stream)
    return bytes

def GetRevitFileVersionInfoText(revitFilePath):
    revitVersionInfoText = str.Empty
    bytes = GetBasicFileInfoBytes(revitFilePath)
    asciiString = Encoding.ASCII.GetString(bytes)
    TEXT_MARKER = '\r\n' # Most common delimiter around the text section.
    TEXT_MARKER_ALT = '\x04\r\x00\n\x00' # Alternative delimiter (occasionally encountered... not sure why though).
    textMarker = TEXT_MARKER
    textMarkerIndices = util.FindAllIndicesOf(asciiString, textMarker)
    numberOfTextMarkerIndices = len(textMarkerIndices)
    if numberOfTextMarkerIndices != 2:
        textMarker = TEXT_MARKER_ALT
        textMarkerIndices = util.FindAllIndicesOf(asciiString, textMarker)
        numberOfTextMarkerIndices = len(textMarkerIndices)
    if numberOfTextMarkerIndices == 2:
        startTextIndex = textMarkerIndices[0] + len(textMarker)
        endTextIndex = textMarkerIndices[1]
        textBytes = bytes[startTextIndex:endTextIndex]
        revitVersionInfoText = Encoding.Unicode.GetString(bytes[startTextIndex:endTextIndex])
    return revitVersionInfoText

def TryGetRevitFileVersionInfoText(revitFilePath):
    revitVersionInfoText = str.Empty
    try:
        revitVersionInfoText = GetRevitFileVersionInfoText(revitFilePath)
    except TargetInvocationException, e:
        revitVersionInfoText = str.Empty
    except IOException, e:
        revitVersionInfoText = str.Empty
    return revitVersionInfoText

def ExtractRevitVersionInfoFromText(revitVersionInfoText):
    REVIT_BUILD_PROPERTY = "Revit Build:"
    FORMAT_PROPERTY = "Format:"
    BUILD_PROPERTY = "Build:"
    revitVersionDescription = str.Empty
    lines = util.ReadLinesFromText(revitVersionInfoText)
    indexedLines = [[index, line] for index, line in enumerate(lines)]
    # Revit 2019 (and onwards?) has 'Build' and 'Format' properties instead of 'Revit Build'
    formatLine = indexedLines.SingleOrDefault(lambda l: l[1].StartsWith(FORMAT_PROPERTY))
    buildLine = indexedLines.SingleOrDefault(lambda l: l[1].StartsWith(BUILD_PROPERTY))
    if buildLine is not None:
        buildLineText = buildLine[1]
        buildLineText = buildLineText[len(BUILD_PROPERTY):]
        formatLineText = str.Empty
        if formatLine is not None:
            formatLineText = formatLine[1]
            formatLineText = formatLineText[len(FORMAT_PROPERTY):]
            revitVersionDescription = "Autodesk Revit " + formatLineText.Trim() + " (Build: " + buildLineText.Trim() + ")"
    else:
        revitBuildLine = indexedLines.SingleOrDefault(lambda l: l[1].StartsWith(REVIT_BUILD_PROPERTY))
        revitBuildLineText = str.Empty
        if revitBuildLine is None:
            # In rare cases the Revit Build *value* is on the next line for some reason!
            # In this scenario it seems to always be followed immediately (no spaces) by the 'Last Save Path:' property specifier
            revitBuildLine = indexedLines.SingleOrDefault(lambda l: l[1].Contains(REVIT_BUILD_PROPERTY))
            if revitBuildLine is not None:
                lineNumber = revitBuildLine[0]
                revitBuildLine = indexedLines[lineNumber+1]
                revitBuildLineText = revitBuildLine[1]
                indexOfLastSavePath = revitBuildLineText.IndexOf("Last Save Path:")
                revitBuildLineText = revitBuildLineText[:indexOfLastSavePath] if indexOfLastSavePath != -1 else revitBuildLineText
        else:
            revitBuildLineText = revitBuildLine[1]
            revitBuildLineText = revitBuildLineText[len(REVIT_BUILD_PROPERTY):]
        revitVersionDescription = revitBuildLineText.Trim()
    return revitVersionDescription

def GetRevitVersionText(revitFilePath):
    revitVersionInfoText = TryGetRevitFileVersionInfoText(revitFilePath)
    revitVersionText = ExtractRevitVersionInfoFromText(revitVersionInfoText)
    return revitVersionText

def GenerateRevitVersionTextPrefixes(revitVersionNumberText, includeDisciplineVersions=False):
    REVIT_VERSION_TEXT_PREFIXES = [
            "Autodesk Revit",
            "Autodesk Revit LT",
            # Very old versions (e.g. 2010) may have the following prefixes.
            "Revit",
            "Revit LT"
        ]
    if includeDisciplineVersions:
        REVIT_VERSION_TEXT_PREFIXES.extend([
                "Autodesk Revit Architecture",
                "Autodesk Revit MEP",
                "Autodesk Revit Structure",
                # Very old versions (e.g. 2010) may have the following prefixes.
                "Revit Architecture",
                "Revit MEP",
                "Revit Structure"
            ])
    return [str.Join(" ", prefix, revitVersionNumberText) for prefix in REVIT_VERSION_TEXT_PREFIXES]

REVIT_VERSION_TEXT_PREFIXES_2010 = GenerateRevitVersionTextPrefixes("2010", includeDisciplineVersions=True)
REVIT_VERSION_TEXT_PREFIXES_2011 = GenerateRevitVersionTextPrefixes("2011", includeDisciplineVersions=True)
REVIT_VERSION_TEXT_PREFIXES_2012 = GenerateRevitVersionTextPrefixes("2012", includeDisciplineVersions=True)
REVIT_VERSION_TEXT_PREFIXES_2013 = GenerateRevitVersionTextPrefixes("2013", includeDisciplineVersions=True)
REVIT_VERSION_TEXT_PREFIXES_2014 = GenerateRevitVersionTextPrefixes("2014", includeDisciplineVersions=True)
REVIT_VERSION_TEXT_PREFIXES_2015 = GenerateRevitVersionTextPrefixes("2015", includeDisciplineVersions=True)
REVIT_VERSION_TEXT_PREFIXES_2016 = GenerateRevitVersionTextPrefixes("2016", includeDisciplineVersions=True)
REVIT_VERSION_TEXT_PREFIXES_2017 = GenerateRevitVersionTextPrefixes("2017")
REVIT_VERSION_TEXT_PREFIXES_2018 = GenerateRevitVersionTextPrefixes("2018")
REVIT_VERSION_TEXT_PREFIXES_2019 = GenerateRevitVersionTextPrefixes("2019")
REVIT_VERSION_TEXT_PREFIXES_2020 = GenerateRevitVersionTextPrefixes("2020")
REVIT_VERSION_TEXT_PREFIXES_2021 = GenerateRevitVersionTextPrefixes("2021")
REVIT_VERSION_TEXT_PREFIXES_2022 = GenerateRevitVersionTextPrefixes("2022")
REVIT_VERSION_TEXT_PREFIXES_2023 = GenerateRevitVersionTextPrefixes("2023")
REVIT_VERSION_TEXT_PREFIXES_2024 = GenerateRevitVersionTextPrefixes("2024")
REVIT_VERSION_TEXT_PREFIXES_2025 = GenerateRevitVersionTextPrefixes("2025")

def GetRevitVersionNumberTextFromRevitVersionText(revitVersionText):
    revitVersionNumberText = None
    if not str.IsNullOrWhiteSpace(revitVersionText):
        def StartsWithOneOfPrefixes(text, prefixes):
            return any(text.StartsWith(prefix) for prefix in prefixes)
        if StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2010):
            revitVersionNumberText = "2010"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2011):
            revitVersionNumberText = "2011"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2012):
            revitVersionNumberText = "2012"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2013):
            revitVersionNumberText = "2013"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2014):
            revitVersionNumberText = "2014"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2015):
            revitVersionNumberText = "2015"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2016):
            revitVersionNumberText = "2016"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2017):
            revitVersionNumberText = "2017"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2018):
            revitVersionNumberText = "2018"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2019):
            revitVersionNumberText = "2019"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2020):
            revitVersionNumberText = "2020"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2021):
            revitVersionNumberText = "2021"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2022):
            revitVersionNumberText = "2022"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2023):
            revitVersionNumberText = "2023"
        elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2024):
            revitVersionNumberText = "2024"
		elif StartsWithOneOfPrefixes(revitVersionText, REVIT_VERSION_TEXT_PREFIXES_2025):
            revitVersionNumberText = "2025"
    return revitVersionNumberText

