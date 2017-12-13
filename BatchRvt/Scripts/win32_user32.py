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
import System
from System.Text import StringBuilder

import win32_pinvoke
import System.Runtime.InteropServices as Interop

USER32_MODULE_NAME = "user32.dll"

GW_OWNER = 4
GA_PARENT = 1

Win32_FindWindowEx = win32_pinvoke.GetWinApiFunctionAnsi("FindWindowEx", USER32_MODULE_NAME, System.IntPtr, System.IntPtr, System.IntPtr, System.String, System.String)
Win32_GetWindowText = win32_pinvoke.GetWinApiFunctionAnsi("GetWindowText", USER32_MODULE_NAME, System.Int32, System.IntPtr, StringBuilder, System.Int32)
Win32_GetClassName = win32_pinvoke.GetWinApiFunctionAnsi("GetClassName", USER32_MODULE_NAME, System.Int32, System.IntPtr, StringBuilder, System.Int32)
Win32_GetDlgCtrlId = win32_pinvoke.GetWinApiFunction("GetDlgCtrlID", USER32_MODULE_NAME, System.Int32, System.IntPtr)
Win32_IsWindowEnabled = win32_pinvoke.GetWinApiFunction("IsWindowEnabled", USER32_MODULE_NAME, System.Int32, System.IntPtr)
Win32_GetWindow = win32_pinvoke.GetWinApiFunction("GetWindow", USER32_MODULE_NAME, System.IntPtr, System.IntPtr, System.Int32)
Win32_GetAncestor = win32_pinvoke.GetWinApiFunction("GetAncestor", USER32_MODULE_NAME, System.IntPtr, System.IntPtr, System.Int32)
Win32_GetWindowThreadProcessId = win32_pinvoke.GetWinApiFunction(
    "GetWindowThreadProcessId",
    USER32_MODULE_NAME,
    System.Int32, # Return value
    System.IntPtr, # hWnd
    System.IntPtr # [out] lpdwProcessId
  )
Win32_SendMessage = win32_pinvoke.GetWinApiFunction("SendMessage", USER32_MODULE_NAME, System.IntPtr, System.IntPtr, System.Int32, System.IntPtr, System.IntPtr)
Win32_GetDlgItem = win32_pinvoke.GetWinApiFunction("GetDlgItem", USER32_MODULE_NAME, System.IntPtr, System.IntPtr, System.Int32)
Win32_SetFocus = win32_pinvoke.GetWinApiFunction("SetFocus", USER32_MODULE_NAME, System.IntPtr, System.IntPtr)
Win32_PostMessage = win32_pinvoke.GetWinApiFunction("PostMessage", USER32_MODULE_NAME, System.Int32, System.IntPtr, System.Int32, System.IntPtr, System.IntPtr)
Win32_IsWindowVisible = win32_pinvoke.GetWinApiFunction("IsWindowVisible", USER32_MODULE_NAME, System.Int32, System.IntPtr)
Win32_GetDesktopWindow = win32_pinvoke.GetWinApiFunction("GetDesktopWindow", USER32_MODULE_NAME, System.IntPtr)

def FindWindows(parentHwnd, className, windowTitle):
  hwnd = System.IntPtr.Zero
  while True:
    hwnd = Win32_FindWindowEx(parentHwnd, hwnd, className, windowTitle)
    if hwnd == System.IntPtr.Zero:
      break
    else:
      yield hwnd
  return

STRING_BUFFER_SIZE = 1025

def GetWindowText(hwnd):
  s = StringBuilder()
  s.EnsureCapacity(STRING_BUFFER_SIZE)
  numberOfChars = Win32_GetWindowText(hwnd, s, STRING_BUFFER_SIZE)
  return s.ToString()

def GetWindowClassName(hwnd):
  s = StringBuilder()
  s.EnsureCapacity(STRING_BUFFER_SIZE)
  numberOfChars = Win32_GetClassName(hwnd, s, STRING_BUFFER_SIZE)
  return s.ToString()

def GetDialogControlId(hwnd):
  return Win32_GetDlgCtrlId(hwnd)

def GetOwnerWindow(hwnd):
  return Win32_GetWindow(hwnd, GW_OWNER)

def GetParentWindow(hwnd):
  return Win32_GetAncestor(hwnd, GA_PARENT)

def IsWindowEnabled(hwnd):
  return Win32_IsWindowEnabled(hwnd) != 0

def SendButtonClickMessage(hwnd):
  BM_CLICK = 0x00F5
  Win32_SendMessage(hwnd, BM_CLICK, System.IntPtr.Zero, System.IntPtr.Zero)
  return

def GetWindowThreadProcessId(hwnd):
  processId = 0
  pProcessId = Interop.Marshal.AllocHGlobal(Interop.Marshal.SizeOf(processId))
  Interop.Marshal.StructureToPtr(processId, pProcessId, False)
  threadId = Win32_GetWindowThreadProcessId(hwnd, pProcessId)
  if threadId != 0:
    processId = Interop.Marshal.PtrToStructure[System.Int32](pProcessId)
  Interop.Marshal.FreeHGlobal(pProcessId)
  return threadId, processId

def GetWindowProcessId(hwnd):
  threadId, processId = GetWindowThreadProcessId(hwnd)
  return processId

def GetWindowThreadId(hwnd):
  threadId, processId = GetWindowThreadProcessId(hwnd)
  return threadId

def GetTopLevelWindows(className, windowTitle, processId=None):
  return list(
      hwnd for hwnd in FindWindows(None, className, windowTitle)
      if processId is None or GetWindowProcessId(hwnd) == processId
    )
