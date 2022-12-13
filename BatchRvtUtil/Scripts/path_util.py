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

import sys

from System import Environment, ArgumentException, StringComparison, Char
from System.IO import Path, File, Directory, FileInfo, DirectoryInfo, PathTooLongException

import win32_mpr


def AddSearchPath(searchPath):
    # TODO: only add the path if it's not already added.
    sys.path.append(searchPath)
    return


def GetUserDesktopFolderPath():
    return Environment.GetFolderPath(Environment.SpecialFolder.Desktop)


def GetLocalAppDataFolderPath():
    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)


def GetFileExtension(filePath):
    return Path.GetExtension(filePath)


def HasFileExtension(filePath, extension):
    return GetFileExtension(filePath).ToLower() == extension.ToLower()


def FileExists(filePath):
    return File.Exists(filePath)


def GetFullPath(path):
    return Path.GetFullPath(path)


def DirectoryExists(folderPath):
    return Directory.Exists(folderPath)


def CreateDirectory(folderPath):
    directoryInfo = Directory.CreateDirectory(folderPath)
    return directoryInfo


def CreateDirectoryForFilePath(filePath):
    directoryInfo = CreateDirectory(Path.GetDirectoryName(filePath))
    return directoryInfo


def GetFileSize(filePath):
    fileSize = None
    try:
        fileSize = FileInfo(filePath).Length
    except Exception, e:
        pass
    return fileSize


def GetLastWriteTimeUtc(filePath):
    lastWriteTime = None
    try:
        lastWriteTime = File.GetLastWriteTimeUtc(filePath)
    except Exception, e:
        pass
    return lastWriteTime


def GetDriveLetter(path):
    driveLetter = Path.GetPathRoot(path).Split(":")[0]
    return driveLetter.ToUpper() if len(driveLetter) == 1 else None


def GetDriveRemoteName(path):
    driveRemoteName = None
    driveLetter = GetDriveLetter(path)
    if driveLetter is not None:
        driveRemoteName = win32_mpr.WNetGetConnection(driveLetter + ":")
    return driveRemoteName


def GetFullNetworkPath(path):
    fullNetworkPath = None
    try:
        if not Path.IsPathRooted(path):
            raise ArgumentException("A full file path must be specified.", "path")
        pathRoot = Path.GetPathRoot(path)
        driveRemoteName = GetDriveRemoteName(pathRoot)
        if driveRemoteName is not None:
            pathWithoutRoot = path.Substring(pathRoot.Length)
            fullNetworkPath = Path.Combine(driveRemoteName, pathWithoutRoot)
    except PathTooLongException, e:
        fullNetworkPath = None
    return fullNetworkPath


def ExpandedFullNetworkPath(path):
    expandedPath = GetFullNetworkPath(path)
    if expandedPath is None:
        expandedPath = path
    return expandedPath


def DirectoryHasName(directoryInfo, name):
    return str.Equals(directoryInfo.Name, name, StringComparison.OrdinalIgnoreCase)


def GetDirectoryParts(folderPath):
    parts = []
    current = DirectoryInfo(folderPath)
    while current is not None:
        parts.insert(0, current.Name)
        current = current.Parent
    return parts


def IsProjectYearFolderName(folderName):
    return (
            len(folderName) == 2 and
            Char.IsDigit(folderName[0]) and
            Char.IsDigit(folderName[1])
    )


def GetProjectFolderNameFromRevitProjectFilePath(revitProjectFilePath):
    projectFolderName = None
    # NOTE: called Path.GetDirectoryName() before expanding the path (rather than after) in
    # order to reduce the risk of hitting .NET path length limit (260 characters?)
    folderPath = ExpandedFullNetworkPath(Path.GetDirectoryName(revitProjectFilePath))
    parts = GetDirectoryParts(folderPath)
    numberOfParts = len(parts)
    if numberOfParts > (2):
        if IsProjectYearFolderName(parts[1]):
            projectFolderName = parts[2]
    return projectFolderName
