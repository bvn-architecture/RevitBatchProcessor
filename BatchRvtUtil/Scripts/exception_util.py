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
from System.Text import StringBuilder

import global_test_mode
import exceptions

EXCEPTION_MESSAGE_HANDLER_PREFIX = "[ EXCEPTION MESSAGE ]"

def GetInterpretedFrameInfo(clsExceptionData):
    interpretedFrameInfo = None
    for kv in clsExceptionData:
        entryName = None
        try:
            entryName = kv.Key.Name
        except:
            pass
        if entryName == "InterpretedFrameInfo":
            interpretedFrameInfo = kv.Value
            break
    return interpretedFrameInfo

def GetClrException(exception):
    return (
            exception.clsException if isinstance(exception, exceptions.Exception)
            else
            exception if isinstance(exception, System.Exception)
            else
            None
        )

def LogOutputErrorDetails(exception, output_, verbose=True):
    output = global_test_mode.PrefixedOutputForGlobalTestMode(output_, EXCEPTION_MESSAGE_HANDLER_PREFIX)
    exceptionMessage = (
            str(exception.message) if isinstance(exception, exceptions.Exception)
            else
            str(exception.Message) if isinstance(exception, System.Exception)
            else
            str.Empty
        )
    output()
    output("Exception: [" + type(exception).__name__ + "] " + exceptionMessage)
    try:
        clsException = GetClrException(exception)
        if clsException is not None:
            clsExceptionType = clr.GetClrType(type(clsException))
            output(".NET exception: [" + str(clsExceptionType.Name) + "] " + str(clsException.Message))
            if verbose:
                interpretedFrameInfo = GetInterpretedFrameInfo(clsException.Data)
                if interpretedFrameInfo is not None:
                    output()
                    output("Further exception information:")
                    output()
                    for i in interpretedFrameInfo:
                        if str(i) != "CallSite.Target":
                            output("\t" + str(i))
    except:
        output("Could not obtain further exception information.")
    return

def GetExceptionDetails(exception):
    exceptionDetails = StringBuilder()
    def output(message=""):
        exceptionDetails.AppendLine(message)
        return
    LogOutputErrorDetails(exception, output)
    return exceptionDetails.ToString()

