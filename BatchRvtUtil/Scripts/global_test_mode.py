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

import test_mode_util

GLOBAL_TEST_MODE = [None]


def GetGlobalTestMode():
    return GLOBAL_TEST_MODE[0]


def IsGlobalTestMode():
    return GetGlobalTestMode() is not None


def InitializeGlobalTestMode(testModeFolderPath):
    if IsGlobalTestMode():
        raise Exception("ERROR: Global test mode is already initialized!")
    if not str.IsNullOrWhiteSpace(testModeFolderPath):
        globalTestMode = test_mode_util.TestMode(testModeFolderPath)
        globalTestMode.CreateTestModeFolder()
        GLOBAL_TEST_MODE[0] = globalTestMode
    return


def ExportSessionId(sessionId):
    if IsGlobalTestMode():
        globalTestMode = GetGlobalTestMode()
        globalTestMode.ExportSessionId(sessionId)
    return


def ExportRevitProcessId(revitProcessId):
    if IsGlobalTestMode():
        globalTestMode = GetGlobalTestMode()
        globalTestMode.ExportRevitProcessId(revitProcessId)
    return


def PrefixedOutputForGlobalTestMode(output_, prefixForTestMode):
    if IsGlobalTestMode():
        def output(m=""):
            output_(prefixForTestMode + " " + m)
            return
    else:
        output = output_
    return output
