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

from System.IO import StreamReader, StreamWriter, FileStream, FileMode, FileAccess, FileShare, IOException

from System.Threading.Tasks import TaskStatus


def WithIgnoredIOException(action):
    result = None
    try:
        result = action()
    except IOException, e:
        pass
    return result


def GetSafeWriteLine(streamWriter):
    def safeWriteLine(msg):
        WithIgnoredIOException(lambda: streamWriter.WriteLine(msg))
        return

    return safeWriteLine


def ReadAvailableLines(streamReader, pendingReadLineTask=None):
    nextPendingReadLineTask = None
    lines = []

    if pendingReadLineTask is not None:
        readLineTask = pendingReadLineTask
    else:
        readLineTask = streamReader.ReadLineAsync()

    reachedEndOfStream = False

    while readLineTask.Status == TaskStatus.RanToCompletion and not reachedEndOfStream:
        line = readLineTask.Result
        if line is not None:
            lines.append(line)
            readLineTask = streamReader.ReadLineAsync()
        else:
            reachedEndOfStream = True

    if reachedEndOfStream:
        nextPendingReadLineTask = None
    elif readLineTask.Status == TaskStatus.Faulted:
        nextPendingReadLineTask = None
    elif readLineTask.Status == TaskStatus.Canceled:
        nextPendingReadLineTask = None
    else:
        nextPendingReadLineTask = readLineTask

    return lines, nextPendingReadLineTask


def GetStreamReader(stream):
    reader = StreamReader(stream)
    return reader


def GetStreamWriter(stream, autoFlush=True):
    writer = StreamWriter(stream)
    writer.AutoFlush = autoFlush
    return writer


def UsingStream(stream, action):
    result = None
    try:
        result = action()
    finally:
        def safeCloseAndDispose():
            stream.Close()
            stream.Dispose()
            return

        # NOTE: the reason for using WithIgnoredIOException() here is that Close() can throw an IOException (Pipe is broken).
        WithIgnoredIOException(safeCloseAndDispose)
    return result


def CreateFile(filePath, overwrite=False):
    fileMode = FileMode.Create if overwrite else FileMode.CreateNew
    fileStream = FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
    return fileStream


def OpenFile(filePath, readonly=True):
    fileAccess = FileAccess.Read if readonly else FileAccess.ReadWrite
    fileStream = FileStream(filePath, FileMode.Open, fileAccess, FileShare.ReadWrite)
    return fileStream
