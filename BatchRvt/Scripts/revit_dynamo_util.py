
import clr
import System

clr.AddReference("DynamoRevitDS")
from Dynamo.Applications import DynamoRevit, DynamoRevitCommandData

# NOTE: Dynamo requires an active UIDocument! The document must be active before executing this function.
#       The Dynamo script must have been saved with the 'Automatic' run mode!
def ExecuteDynamoScript(uiapp, dynamoScriptFilePath, showUI=False):
  revitVersionNumber = uiapp.Application.VersionNumber

  if revitVersionNumber == "2015":
    raise Exception("Automation of Dynamo scripts is not supported in Revit 2015!")
  elif revitVersionNumber == "2016":
    JOURNAL_KEY__AUTOMATION_MODE = "dynAutomation" # NOTE: the typo is intended here because there's a typo in the original source code!
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
      JOURNAL_KEY__AUTOMATION_MODE : True.ToString(),
      JOURNAL_KEY__SHOW_UI : showUI.ToString(),
      JOURNAL_KEY__DYN_PATH : dynamoScriptFilePath
    }

  dynamoRevit = DynamoRevit()
  externalCommandResult = dynamoRevit.ExecuteCommand(dynamoRevitCommandData)
  return externalCommandResult
