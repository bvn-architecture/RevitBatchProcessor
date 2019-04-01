#
# Revit Batch Processor
#
# Copyright (c) 2019  Dan Rumery, BVN
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
from System import Guid
clr.AddReference("System.Core")
import System.Linq
clr.ImportExtensions(System.Linq)

import System.Reflection as Refl
from System.Reflection import Emit
import System.Runtime.InteropServices as Interop

WIN32_PINVOKE_DYNAMIC_ASSEMBLY_NAME = "WIN_API_ASSEMBLY"

PINVOKE_METHOD_ATTRIBUTES = (
        Refl.MethodAttributes.Public |
        Refl.MethodAttributes.Static |
        Refl.MethodAttributes.HideBySig |
        Refl.MethodAttributes.PinvokeImpl
    )

PUBLIC_STATIC_BINDING_FLAGS = Refl.BindingFlags.Static | Refl.BindingFlags.Public

WIN_API_CALLING_CONVENTION = Interop.CallingConvention.StdCall

assemblyBuilder = (
    Emit.AssemblyBuilder.DefineDynamicAssembly(
        Refl.AssemblyName(WIN32_PINVOKE_DYNAMIC_ASSEMBLY_NAME),
        Emit.AssemblyBuilderAccess.Run
      )
  )

MODULE_BUILDER = assemblyBuilder.DefineDynamicModule("WIN_API_MODULE_" + Guid.NewGuid().ToString())

def GetWinApiFunctionImpl(
        functionName,
        moduleName,
        charSet,
        returnType,
        *parameterTypes
    ):
    tbuilder = MODULE_BUILDER.DefineType("WIN_API_TYPE" + "_" + moduleName + "_" + functionName)
    mbuilder = tbuilder.DefinePInvokeMethod(
            functionName,
            moduleName,
            PINVOKE_METHOD_ATTRIBUTES,
            Refl.CallingConventions.Standard,
            clr.GetClrType(returnType),
            [clr.GetClrType(t) for t in parameterTypes].ToArray[System.Type](),
            WIN_API_CALLING_CONVENTION,
            charSet
        )
    mbuilder.SetImplementationFlags(mbuilder.MethodImplementationFlags | Refl.MethodImplAttributes.PreserveSig)
    winApiType = tbuilder.CreateType()
    methodInfo = winApiType.GetMethod(functionName, PUBLIC_STATIC_BINDING_FLAGS)
    def WinApiFunction(*parameters):
        return methodInfo.Invoke(None, parameters.ToArray[System.Object]())
    return WinApiFunction

def GetWinApiFunction(functionName, moduleName, returnType, *parameterTypes):
    return GetWinApiFunctionImpl(functionName, moduleName, Interop.CharSet.Auto, returnType, *parameterTypes)

def GetWinApiFunctionAnsi(functionName, moduleName, returnType, *parameterTypes):
    return GetWinApiFunctionImpl(functionName, moduleName, Interop.CharSet.Ansi, returnType, *parameterTypes)

def GetWinApiFunctionUnicode(functionName, moduleName, returnType, *parameterTypes):
    return GetWinApiFunctionImpl(functionName, moduleName, Interop.CharSet.Unicode, returnType, *parameterTypes)

