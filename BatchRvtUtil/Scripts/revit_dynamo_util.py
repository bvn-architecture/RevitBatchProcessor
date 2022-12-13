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

clr.AddReference("System.Xml")
from System.Xml import XmlDocument, XmlException

from System.IO import File, Path, IOException

import revit_dynamo_error
import text_file_util
import json_util

try:
    clr.AddReference("DynamoRevitDS")
except IOException, e:
    raise Exception(revit_dynamo_error.DYNAMO_REVIT_MODULE_NOT_FOUND_ERROR_MESSAGE)

from Dynamo.Applications import DynamoRevit, DynamoRevitCommandData

DYNAMO_RUNTYPE_AUTOMATIC = "Automatic"
DYNAMO_RUNTYPE_MANUAL = "Manual"
DYNAMO_WORKSPACE_XML_NODE = "Workspace"
DYNAMO_JOBJECT_VIEW = "View"
DYNAMO_JOBJECT_DYNAMO = "Dynamo"
DYNAMO_RUNTYPE_ATTRIBUTE = "RunType"
DYNAMO_HAS_RUN_WITHOUT_CRASH_ATTRIBUTE = "HasRunWithoutCrash"
DYNAMO_ATTRIBUTE_XML_VALUE_TRUE = "True"
DYNAMO_ATTRIBUTE_XML_VALUE_FALSE = "False"


def WithLoadedXmlDocument(xmlDocumentFilePath, action):
    result = None
    doc = XmlDocument()
    try:
        doc.Load(xmlDocumentFilePath)
        result = action(doc)
    except XmlException, e:
        result = None
    except Exception, e:
        result = None
    return result


def WithTextFileJsonObject(textFilePath, action):
    result = None
    try:
        textFileContents = text_file_util.ReadFromTextFile(textFilePath)
        jobject = None
        try:
            jobject = json_util.DeserializeToJObject(textFileContents)
            result = action(jobject)
        except json_util.JSON.JsonReaderException, e:
            result = None
    except IOException, e:
        result = None
    except Exception, e:
        result = None
    return result


def WithDynamoWorkspaceJsonObject(dynamoScriptFilePath, action, writeBackToFile=False):
    def jobjectAction(jobject):
        viewJObject = jobject[DYNAMO_JOBJECT_VIEW]
        dynamoJObject = viewJObject[DYNAMO_JOBJECT_DYNAMO]
        result = action(jobject, dynamoJObject)
        if writeBackToFile:
            serializedJObjectText = json_util.ToString(jobject, prettyPrint=True)
            text_file_util.WriteToTextFile(dynamoScriptFilePath, serializedJObjectText)
        return result

    result = WithTextFileJsonObject(dynamoScriptFilePath, jobjectAction)
    return result


def IsDynamoWorkspaceJsonFile(dynamoScriptFilePath):
    def action(jobject, dynamoJObject):
        return dynamoJObject is not None

    result = WithDynamoWorkspaceJsonObject(dynamoScriptFilePath, action)
    return result == True


def WithDynamoWorkspaceXmlNode(dynamoScriptFilePath, action):
    def docAction(doc):
        dynamoWorkspaceXmlNode = doc[DYNAMO_WORKSPACE_XML_NODE]
        result = action(doc, dynamoWorkspaceXmlNode)
        return result

    result = WithLoadedXmlDocument(dynamoScriptFilePath, docAction)
    return result


def IsDynamoWorkspaceXmlFile(dynamoScriptFilePath):
    def action(doc, dynamoWorkspaceXmlNode):
        return dynamoWorkspaceXmlNode is not None

    result = WithDynamoWorkspaceXmlNode(dynamoScriptFilePath, action)
    return result == True


def SetDynamoScriptRunType(dynamoScriptFilePath, runType):
    prevRunType = None
    if IsDynamoWorkspaceXmlFile(dynamoScriptFilePath):
        def action(doc, dynamoWorkspaceXmlNode):
            dynamoRunTypeAttribute = dynamoWorkspaceXmlNode.Attributes[DYNAMO_RUNTYPE_ATTRIBUTE]
            prevRunType = dynamoRunTypeAttribute.Value
            dynamoRunTypeAttribute.Value = runType
            dynamoHasRunWithoutCrashAttribute = dynamoWorkspaceXmlNode.Attributes[
                DYNAMO_HAS_RUN_WITHOUT_CRASH_ATTRIBUTE]
            if dynamoHasRunWithoutCrashAttribute is not None:
                dynamoHasRunWithoutCrashAttribute.Value = DYNAMO_ATTRIBUTE_XML_VALUE_TRUE
            doc.Save(dynamoScriptFilePath)
            return prevRunType

        prevRunType = WithDynamoWorkspaceXmlNode(dynamoScriptFilePath, action)
    elif IsDynamoWorkspaceJsonFile(dynamoScriptFilePath):
        def action(jobject, dynamoJObject):
            prevRunTypeJValue = dynamoJObject[DYNAMO_RUNTYPE_ATTRIBUTE]
            prevRunType = json_util.GetValueFromJValue(prevRunTypeJValue)
            dynamoJObject[DYNAMO_RUNTYPE_ATTRIBUTE] = runType
            dynamoHasRunWithoutCrashJValue = dynamoJObject[DYNAMO_HAS_RUN_WITHOUT_CRASH_ATTRIBUTE]
            if dynamoHasRunWithoutCrashJValue is not None:
                dynamoJObject[DYNAMO_HAS_RUN_WITHOUT_CRASH_ATTRIBUTE] = True
            return prevRunType

        prevRunType = WithDynamoWorkspaceJsonObject(dynamoScriptFilePath, action, writeBackToFile=True)
    return prevRunType


def GetDynamoScriptRunType(dynamoScriptFilePath):
    runType = None
    if IsDynamoWorkspaceXmlFile(dynamoScriptFilePath):
        def action(doc, dynamoWorkspaceXmlNode):
            runType = dynamoWorkspaceXmlNode.Attributes[DYNAMO_RUNTYPE_ATTRIBUTE].Value
            return runType

        runType = WithDynamoWorkspaceXmlNode(dynamoScriptFilePath, action)
    elif IsDynamoWorkspaceJsonFile(dynamoScriptFilePath):
        def action(jobject, dynamoJObject):
            runTypeJValue = dynamoJObject[DYNAMO_RUNTYPE_ATTRIBUTE]
            runType = json_util.GetValueFromJValue(runTypeJValue)
            return runType

        runType = WithDynamoWorkspaceJsonObject(dynamoScriptFilePath, action)
    return runType


def GetDynamoScriptHasRunWithoutCrash(dynamoScriptFilePath):
    hasRunWithoutCrash = None
    if IsDynamoWorkspaceXmlFile(dynamoScriptFilePath):
        def action(doc, dynamoWorkspaceXmlNode):
            hasRunWithoutCrash = dynamoWorkspaceXmlNode.Attributes[DYNAMO_HAS_RUN_WITHOUT_CRASH_ATTRIBUTE].Value
            return hasRunWithoutCrash

        hasRunWithoutCrash = WithDynamoWorkspaceXmlNode(dynamoScriptFilePath, action)
    elif IsDynamoWorkspaceJsonFile(dynamoScriptFilePath):
        def action(jobject, dynamoJObject):
            hasRunWithoutCrashJValue = dynamoJObject[DYNAMO_HAS_RUN_WITHOUT_CRASH_ATTRIBUTE]
            hasRunWithoutCrash = json_util.GetValueFromJValue(hasRunWithoutCrashJValue)
            return hasRunWithoutCrash

        hasRunWithoutCrash = WithDynamoWorkspaceJsonObject(dynamoScriptFilePath, action)
    return hasRunWithoutCrash


# NOTE: Dynamo requires an active UIDocument! The document must be active before executing this function.
#       The Dynamo script must have been saved with the 'Automatic' run mode!
def ExecuteDynamoScriptInternal(uiapp, dynamoScriptFilePath, showUI=False):
    dynamoScriptRunType = GetDynamoScriptRunType(dynamoScriptFilePath)
    if dynamoScriptRunType is None:
        raise Exception("Could not determine the Run mode of this Dynamo script!")
    elif dynamoScriptRunType != DYNAMO_RUNTYPE_AUTOMATIC:
        raise Exception(
            "The Dynamo script has Run mode set to '" + dynamoScriptRunType + "'. " +
            "It must be set to '" + DYNAMO_RUNTYPE_AUTOMATIC + "' in order for Dynamo script automation to work."
        )
    hasRunWithoutCrash = GetDynamoScriptHasRunWithoutCrash(dynamoScriptFilePath)
    if hasRunWithoutCrash is not None:
        if hasRunWithoutCrash != DYNAMO_ATTRIBUTE_XML_VALUE_TRUE and hasRunWithoutCrash != True:
            raise Exception(
                "The Dynamo script has attribute '" + DYNAMO_HAS_RUN_WITHOUT_CRASH_ATTRIBUTE + "' set to '" + str(
                    hasRunWithoutCrash) + "'. " +
                "It must be set to '" + DYNAMO_ATTRIBUTE_XML_VALUE_TRUE + "' in order for Dynamo script automation to work."
            )
    revitVersionNumber = uiapp.Application.VersionNumber
    if revitVersionNumber == "2015":
        raise Exception("Automation of Dynamo scripts is not supported in Revit 2015!")
    elif revitVersionNumber == "2016":
        JOURNAL_KEY__AUTOMATION_MODE = "dynAutomation"
        JOURNAL_KEY__SHOW_UI = "dynShowUI"
        JOURNAL_KEY__DYN_PATH = "dynPath"
    else:
        from Dynamo.Applications import JournalKeys
        JOURNAL_KEY__AUTOMATION_MODE = JournalKeys.AutomationModeKey
        JOURNAL_KEY__SHOW_UI = JournalKeys.ShowUiKey
        JOURNAL_KEY__DYN_PATH = JournalKeys.DynPathKey

    dynamoRevitCommandData = DynamoRevitCommandData()
    dynamoRevitCommandData.Application = uiapp
    dynamoRevitCommandData.JournalData = {
        JOURNAL_KEY__AUTOMATION_MODE: True.ToString(),
        JOURNAL_KEY__SHOW_UI: showUI.ToString(),
        JOURNAL_KEY__DYN_PATH: dynamoScriptFilePath
    }

    dynamoRevit = DynamoRevit()
    externalCommandResult = dynamoRevit.ExecuteCommand(dynamoRevitCommandData)
    return externalCommandResult


def ExecuteDynamoScript(uiapp, dynamoScriptFilePath, showUI=False):
    externalCommandResult = None
    # NOTE: The temporary copy of the Dynamo script file is created in same folder as the original so
    # that any relative paths in the script won't break.
    tempDynamoScriptFilePath = Path.Combine(Path.GetDirectoryName(dynamoScriptFilePath), Path.GetRandomFileName())
    File.Copy(dynamoScriptFilePath, tempDynamoScriptFilePath)
    try:
        SetDynamoScriptRunType(tempDynamoScriptFilePath, DYNAMO_RUNTYPE_AUTOMATIC)
        externalCommandResult = ExecuteDynamoScriptInternal(uiapp, tempDynamoScriptFilePath, showUI)
    finally:
        File.Delete(tempDynamoScriptFilePath)
    return externalCommandResult
