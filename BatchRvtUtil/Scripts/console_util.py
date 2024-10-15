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

try:
    clr.AddReference("System.Console")
except: pass

def WaitForSpaceBarKeyPress():
    while True:
        keyInfo = System.Console.ReadKey(True)
        if keyInfo.Key == System.ConsoleKey.Spacebar:
            break
    return

def IsInputRedirected():
    return System.Console.IsInputRedirected

def ReadLine():
    return System.Console.ReadLine()

def ReadLines():
    lines = []
    line = System.Console.ReadLine()
    while line is not None:
        lines.append(line)
        line = System.Console.ReadLine()
    return lines

