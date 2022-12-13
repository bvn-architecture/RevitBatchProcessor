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
from System.Diagnostics import Process

clr.AddReference("System.Windows.Forms")
from System.Windows.Forms import IWin32Window, Cursor

clr.AddReference("System.Drawing")
from System.Drawing import Point


class WindowHandleWrapper(IWin32Window):
    def __init__(self, hwnd):
        self.hwnd = hwnd

    def get_Handle(self):
        return self.hwnd

    @staticmethod
    def GetMainWindowHandle():
        return WindowHandleWrapper(Process.GetCurrentProcess().MainWindowHandle)


def SetMousePosition(x, y):
    Cursor.Position = Point(x, y)
    return
