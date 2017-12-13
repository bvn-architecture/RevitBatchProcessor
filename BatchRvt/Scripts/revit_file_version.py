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

clr.AddReference("System.Core")
import System.Linq
clr.ImportExtensions(System.Linq)
from System.Linq import Enumerable

import System.Reflection as Refl

import System.IO

import System.Text
from System.Text import Encoding

clr.AddReference("WindowsBase")
import System.IO.Packaging as Packaging

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

def GetRevitVersionText(revitFilePath):
  storageRoot = GetStorageRoot(revitFilePath)
  stream = GetBasicFileInfoStream(storageRoot)
  bytes = ReadAllBytes(stream)
  unicodeString = Encoding.Unicode.GetString(bytes)
  start = unicodeString.IndexOf("Autodesk Revit")
  end = unicodeString.IndexOf("\x00", start)
  versionText = unicodeString.Substring(start, end - start)
  return versionText.Substring(0, versionText.LastIndexOf(")") + 1) 

