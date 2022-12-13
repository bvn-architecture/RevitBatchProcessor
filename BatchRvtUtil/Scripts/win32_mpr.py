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

import System
import System.Runtime.InteropServices as Interop

import win32_pinvoke

MPR_MODULE_NAME = "mpr.dll"

WNETGETCONNECTION_FUNCTION_NAME = "WNetGetConnection"

REMOTE_NAME_MAX_LENGTH = 1024

NO_ERROR = 0

WNetGetConnectionWinApiFunc = win32_pinvoke.GetWinApiFunctionUnicode(
    WNETGETCONNECTION_FUNCTION_NAME,
    MPR_MODULE_NAME,
    System.Int32,  # Return value
    System.String,  # [in] lpLocalName
    System.Text.StringBuilder,  # [out] lpRemoteName
    System.IntPtr  # [in, out] lpnLength
)


def WNetGetConnection(localName):
    remoteName = None
    length = REMOTE_NAME_MAX_LENGTH
    remoteNameBuffer = System.Text.StringBuilder(REMOTE_NAME_MAX_LENGTH)
    pLength = Interop.Marshal.AllocHGlobal(Interop.Marshal.SizeOf(length))
    Interop.Marshal.StructureToPtr(length, pLength, False)
    result = WNetGetConnectionWinApiFunc(localName, remoteNameBuffer, pLength)
    outLength = Interop.Marshal.PtrToStructure[int](pLength)
    Interop.Marshal.FreeHGlobal(pLength)
    if result == NO_ERROR:
        remoteName = remoteNameBuffer.ToString()
    return remoteName
