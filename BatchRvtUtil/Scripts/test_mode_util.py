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
from System.IO import Directory, Path, File
from System.Diagnostics import Process

import json_util

IS_TEST_MODE = [False]
TEST_MODE_FOLDER_PATH = [None]

TEST_MODE_DATA__SESSION_ID = "sessionId"
TEST_MODE_DATA__REVIT_PROCESS_IDS = "revitProcessIds"

def InitializeTestMode(testModeFolderPath):
  if not str.IsNullOrWhiteSpace(testModeFolderPath):
    Directory.CreateDirectory(testModeFolderPath)
    TEST_MODE_FOLDER_PATH[0] = testModeFolderPath
    IS_TEST_MODE[0] = True
  else:
    IS_TEST_MODE[0] = False
    TEST_MODE_FOLDER_PATH[0] = None
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

def GetTestModeDataFilePath():
  return Path.Combine(GetTestModeFolderPath(), "test_mode_data.json")

def GetTestModeData():
  testModeData = None
  if IsTestMode():
    testModeDataFilePath = GetTestModeDataFilePath()
    if File.Exists(testModeDataFilePath):
      testModeData = json_util.DeserializeToJObject(File.ReadAllText(testModeDataFilePath))
    else:
      testModeData = json_util.JObject()
      testModeData[TEST_MODE_DATA__SESSION_ID] = None
      testModeData[TEST_MODE_DATA__REVIT_PROCESS_IDS] = json_util.JArray()
  return testModeData

def SaveTestModeData(testModeData):
  testModeData = json_util.SerializeObject(testModeData, prettyPrint=True)
  testModeDataFilePath = GetTestModeDataFilePath()
  File.WriteAllText(testModeDataFilePath, testModeData)
  return

def WithTestModeData(testModeDataAction):
  if IsTestMode():
    testModeData = GetTestModeData()
    testModeDataAction(testModeData)
    SaveTestModeData(testModeData)
  return

def ExportRevitProcessId(revitProcessId):
  def action(testModeData):
    revitProcessIds = testModeData[TEST_MODE_DATA__REVIT_PROCESS_IDS]
    revitProcessIds.Add(revitProcessId)
    return
  WithTestModeData(action)
  return

def ExportSessionId(sessionId):
  def action(testModeData):
    testModeData[TEST_MODE_DATA__SESSION_ID] = sessionId
    return
  WithTestModeData(action)
  return
