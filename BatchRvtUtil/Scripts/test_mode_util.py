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
from System.IO import Directory

IS_TEST_MODE = [False]
TEST_MODE_FOLDER_PATH = [None]

def InitializeTestMode(testModeFolderPath):
  if not str.IsNullOrWhiteSpace(testModeFolderPath):
    Directory.CreateDirectory(testModeFolderPath)
    TEST_MODE_FOLDER_PATH[0] = testModeFolderPath
    IS_TEST_MODE[0] = True
  return IS_TEST_MODE[0]

def IsTestMode():
  return IS_TEST_MODE[0]

def GetTestModeFolderPath():
  return TEST_MODE_FOLDER_PATH[0]

def PrefixedOutputForTestMode(output_, prefixForTestMode):
  if IsTestMode():
    def output(m=""):
      output_(prefixForTestMode + " " + m)
      return
  else:
    output = output_
  return output
