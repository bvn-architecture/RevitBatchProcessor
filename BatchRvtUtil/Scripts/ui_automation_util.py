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

import win32_user32

DIALOG_WINDOW_CLASS_NAME = "#32770"

class WindowInfo:
    def __init__(self, hwnd):
        self.Hwnd = hwnd
        self.IsWindowEnabled = win32_user32.IsWindowEnabled(hwnd)
        self.OwnerWindow = win32_user32.GetOwnerWindow(hwnd)
        self.ParentWindow = win32_user32.GetParentWindow(hwnd)
        self.DialogControlId = win32_user32.GetDialogControlId(hwnd)
        self.WindowClassName = win32_user32.GetWindowClassName(hwnd)
        self.WindowText = win32_user32.GetWindowText(hwnd)
        return

def GetEnabledDialogsInfo(processId):
    return list(
            WindowInfo(hwnd)
            for hwnd in win32_user32.GetTopLevelWindows(DIALOG_WINDOW_CLASS_NAME, None, processId)
            if win32_user32.IsWindowEnabled(hwnd)
        )

def TextWithoutAmpersands(text):
    return text.Replace("&", str.Empty).Replace(u"&", str.Empty)

def GetButtonText(buttonInfo):
    buttonText = buttonInfo.WindowText
    return TextWithoutAmpersands(buttonText)

def FilterControlsByText(controls, controlText):
    targetControls = list(
            control
            for control in controls
            if TextWithoutAmpersands(control.WindowText).Trim().ToLower() == controlText.Trim().ToLower()
        )
    return targetControls

