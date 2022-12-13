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

from System.Diagnostics import Process, ProcessStartInfo
from System.Text import Encoding

DIR_COMMAND_DIRECTORIES = "dir /ad /b /on"
DIR_COMMAND_FILES = "dir /a-d /b /on"


# NOTE: the pushd/popd commands are needed in order for dir command to respect relative base folder paths
#       when invoked from this script. Otherwise it always uses the current directory, for whatever reason.
#       note that setting the ProcessStartInfo.WorkingDirectory property doesn't alleviate this problem!
def GetDirFoldersCommand(baseFolderPath, folderPattern, includeSubfolders):
    return (
            'pushd "' + baseFolderPath + '" && ' +  # '&&' is used so that the command fails if the folder doesn't exist.
            DIR_COMMAND_DIRECTORIES + (' /s' if includeSubfolders else '') + ' "' + folderPattern + '" & ' +
            'popd'
    )


# NOTE: the pushd/popd commands are needed in order for dir command to respect relative base folder paths
#       when invoked from this script. Otherwise it always uses the current directory, for whatever reason.
#       note that setting the ProcessStartInfo.WorkingDirectory property doesn't alleviate this problem!
def GetDirFilesCommand(baseFolderPath, filePattern, includeSubfolders):
    return (
            'pushd "' + baseFolderPath + '" && ' +  # '&&' is used so that the command fails if the folder doesn't exist.
            DIR_COMMAND_FILES + (' /s' if includeSubfolders else '') + ' "' + filePattern + '" & ' +
            'popd'
    )


def StartCmdProcess(commandLine):
    # NOTE: do not call Process.WaitForExit() until redirected streams have been entirely read from / closed.
    #       doing so can lead to a deadlock when the child process is waiting on being able to write to output / error
    #       stream and the parent process is waiting for the child to exit! See Microsoft's documentation for more info.
    # NOTE: if redirecting both output and error streams, one should be read asynchronously to avoid a deadlock where
    #       the child process is waiting to write to one of the streams and the parent is waiting for data from the other
    #       stream. See Microsoft's documentation for more info.
    psi = ProcessStartInfo('cmd.exe', '/U /S /C " ' + commandLine + ' "')
    psi.UseShellExecute = False
    psi.CreateNoWindow = True
    psi.RedirectStandardInput = False
    psi.RedirectStandardError = False  # See notes above if enabling this alongside redirect output stream.
    psi.RedirectStandardOutput = True
    psi.StandardOutputEncoding = Encoding.Unicode
    p = Process.Start(psi)
    return p


def ReadProcessOutputLines(process):
    output = process.StandardOutput
    line = output.ReadLine()
    while line is not None:
        yield line
        line = output.ReadLine()
    return


def FindFiles(baseFolderPath, filePattern, includeSubfolders=False):
    cmd = GetDirFilesCommand(baseFolderPath, filePattern, includeSubfolders)
    p = StartCmdProcess(cmd)
    return ReadProcessOutputLines(p)


def FindFolders(baseFolderPath, folderPattern, includeSubfolders=False):
    cmd = GetDirFoldersCommand(baseFolderPath, folderPattern, includeSubfolders)
    p = StartCmdProcess(cmd)
    return ReadProcessOutputLines(p)
