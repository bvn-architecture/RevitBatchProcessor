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

from System.IO import File, StringReader
from System.Text import Encoding
import path_util
import stream_io_util

TXT_FILE_EXTENSION = ".txt"

def ReadAllLines(filePath):
    lines = File.ReadAllLines(filePath)
    if len(lines) > 0: # Workaround for a potential lack of detection of Unicode txt files.
        if lines[0].Contains("\x00"):
            lines = File.ReadAllLines(filePath, Encoding.Unicode)
    return lines

def HasTextFileExtension(filePath):
    return path_util.HasFileExtension(filePath, TXT_FILE_EXTENSION)

def GetLinesFromText(text):
    reader = StringReader(text)
    lines = []
    try:
        line = reader.ReadLine()
        while line is not None:
            lines.append(line)
            line = reader.ReadLine()
    finally:
        reader.Dispose()
    return lines

def GetRowsFromLines(lines):
    return list(line.Split("\t") for line in lines)

def GetRowsFromText(text):
    lines = GetLinesFromText(text)
    return GetRowsFromLines(lines)

def GetRowsFromTextFile(filePath):
    lines = ReadAllLines(filePath)
    return GetRowsFromLines(lines)

def WriteToTextFile(textFilePath, text):
    path_util.CreateDirectoryForFilePath(textFilePath)
    
    fileStream = stream_io_util.CreateFile(textFilePath, True)
    
    def fileStreamAction():
        textWriter = stream_io_util.GetStreamWriter(fileStream)
        
        def textWriterAction():
            textWriter.Write(text)
            return
        
        stream_io_util.UsingStream(textWriter, textWriterAction)
        return

    stream_io_util.UsingStream(fileStream, fileStreamAction)
    return

def WriteLinesToTextFile(textFilePath, lines):
    path_util.CreateDirectoryForFilePath(textFilePath)
    
    fileStream = stream_io_util.CreateFile(textFilePath, True)
    
    def fileStreamAction():
        textWriter = stream_io_util.GetStreamWriter(fileStream)
        
        def textWriterAction():
            for line in lines:
                textWriter.WriteLine(line)
            return
        
        stream_io_util.UsingStream(textWriter, textWriterAction)
        return

    stream_io_util.UsingStream(fileStream, fileStreamAction)
    return

def ReadFromTextFile(textFilePath):
    fileStream = stream_io_util.OpenFile(textFilePath, True)
    
    def fileStreamAction():
        textReader = stream_io_util.GetStreamReader(fileStream)
        
        def textReaderAction():
            text = textReader.ReadToEnd()
            return text

        text = stream_io_util.UsingStream(textReader, textReaderAction)
        return text

    text = stream_io_util.UsingStream(fileStream, fileStreamAction)
    return text

