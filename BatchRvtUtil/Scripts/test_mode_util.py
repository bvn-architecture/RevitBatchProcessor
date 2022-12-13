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

from System.IO import Directory, Path, File

import json_util

TEST_MODE_DATA__SESSION_ID = "sessionId"
TEST_MODE_DATA__REVIT_PROCESS_IDS = "revitProcessIds"


class TestMode:
    def __init__(self, testModeFolderPath):
        self.TestModeFolderPath = testModeFolderPath
        return

    def CreateTestModeFolder(self):
        if not str.IsNullOrWhiteSpace(self.TestModeFolderPath):
            Directory.CreateDirectory(self.TestModeFolderPath)
        if not Directory.Exists(self.TestModeFolderPath):
            raise Exception("ERROR: failed to create the test mode folder!")
        return

    def GetTestModeDataFilePath(self):
        return Path.Combine(self.TestModeFolderPath, "test_mode_data.json")

    def GetTestModeData(self):
        testModeData = None
        testModeDataFilePath = self.GetTestModeDataFilePath()
        if File.Exists(testModeDataFilePath):
            testModeData = json_util.DeserializeToJObject(File.ReadAllText(testModeDataFilePath))
        else:
            testModeData = json_util.JObject()
            testModeData[TEST_MODE_DATA__SESSION_ID] = None
            testModeData[TEST_MODE_DATA__REVIT_PROCESS_IDS] = json_util.JArray()
        return testModeData

    def SaveTestModeData(self, testModeData):
        testModeData = json_util.SerializeObject(testModeData, prettyPrint=True)
        testModeDataFilePath = self.GetTestModeDataFilePath()
        File.WriteAllText(testModeDataFilePath, testModeData)
        return

    def WithTestModeData(self, testModeDataAction):
        testModeData = self.GetTestModeData()
        testModeDataAction(testModeData)
        self.SaveTestModeData(testModeData)
        return

    def ExportRevitProcessId(self, revitProcessId):
        def action(testModeData):
            revitProcessIds = testModeData[TEST_MODE_DATA__REVIT_PROCESS_IDS]
            revitProcessIds.Add(revitProcessId)
            return

        self.WithTestModeData(action)
        return

    def ExportSessionId(self, sessionId):
        def action(testModeData):
            testModeData[TEST_MODE_DATA__SESSION_ID] = sessionId
            return

        self.WithTestModeData(action)
        return


def GetSessionId(testModeData):
    return json_util.GetValueFromJValue(testModeData[TEST_MODE_DATA__SESSION_ID])


def GetRevitProcessIds(testModeData):
    return [
        json_util.GetValueFromJValue(revitProcessId)
        for revitProcessId in testModeData[TEST_MODE_DATA__REVIT_PROCESS_IDS]
    ]
