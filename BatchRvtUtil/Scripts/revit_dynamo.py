
import clr
import System
from System.IO import IOException

import revit_dynamo_error

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

