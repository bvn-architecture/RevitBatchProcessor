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
from System.IO import IOException


def IsDynamoRevitModuleLoaded():
    isLoaded = False
    try:
        clr.AddReference("DynamoRevitDS")
        isLoaded = True
    except IOException, e:
        isLoaded = False
    return isLoaded


def ExecuteDynamoScript(uiapp, dynamoScriptFilePath, showUI=False):
    import revit_dynamo_util
    result = revit_dynamo_util.ExecuteDynamoScript(uiapp, dynamoScriptFilePath, showUI)
    return result
