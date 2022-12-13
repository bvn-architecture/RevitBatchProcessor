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
from System.IO import HandleInheritability
import System.IO.Pipes as Pipes
import System.Security.Principal as Principal
import System.Security.AccessControl as AccessControl

MAX_NUMBER_OF_INSTANCES = 16
PIPE_IO_BUFFER_SIZE = 8192

IN = Pipes.PipeDirection.In
OUT = Pipes.PipeDirection.Out


def CreateNamedPipeServer(pipeName):
    worldsid = Principal.SecurityIdentifier(Principal.WellKnownSidType.WorldSid, None)
    localsid = Principal.SecurityIdentifier(Principal.WellKnownSidType.LocalSid, None)
    worldpr = Pipes.PipeAccessRule(worldsid, Pipes.PipeAccessRights.ReadWrite, AccessControl.AccessControlType.Allow)
    localpr = Pipes.PipeAccessRule(localsid, Pipes.PipeAccessRights.FullControl, AccessControl.AccessControlType.Allow)

    ps = Pipes.PipeSecurity()
    ps.AddAccessRule(worldpr)
    ps.AddAccessRule(localpr)

    return Pipes.NamedPipeServerStream(
        pipeName,
        Pipes.PipeDirection.InOut,
        MAX_NUMBER_OF_INSTANCES,
        Pipes.PipeTransmissionMode.Byte,
        Pipes.PipeOptions.Asynchronous,
        PIPE_IO_BUFFER_SIZE,
        PIPE_IO_BUFFER_SIZE,
        ps
    )


def CreateAnonymousPipeServer(pipeDirection, handleInheritability=None):
    handleInheritability = handleInheritability if handleInheritability is not None else HandleInheritability.None
    outputPipeServer = Pipes.AnonymousPipeServerStream(
        pipeDirection,
        handleInheritability,
        PIPE_IO_BUFFER_SIZE
    )
    return outputPipeServer
