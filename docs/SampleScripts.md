# Sample Scripts

The RevitBatchProcessor takes three types of scripts: a task script and (optionally) a pre-processing and post-processing script. These scripts are written in Iron-Python (a .NET variant of Python). With a small amount of additional code the task script can also execute your Dynamo script!

## Task scripts

Some simple Task scripts to demonstrate how they work:

'Hello World' task script:

```python
'''Output "Hello Revit world!" to the log.'''

# This section is common to all of these scripts. 
import clr
import System

clr.AddReference("RevitAPI")
clr.AddReference("RevitAPIUI")
from Autodesk.Revit.DB import *

import revit_script_util
from revit_script_util import Output

sessionId = revit_script_util.GetSessionId()
uiapp = revit_script_util.GetUIApplication()

# NOTE: these only make sense for batch Revit file processing mode.
doc = revit_script_util.GetScriptDocument()
revitFilePath = revit_script_util.GetRevitFilePath()

# The code above is boilerplate, everything below is yours!

Output()
Output("Hello Revit world!")
```

## Dynamo Scripts

Task script to execute a Dynamo script:

```python
import clr
import System

clr.AddReference("RevitAPI")
clr.AddReference("RevitAPIUI")
from Autodesk.Revit.DB import *

import revit_script_util
from revit_script_util import Output

import revit_dynamo_util

# Change this variable to the path of your Dynamo workspace file.
# (Note that the Dynamo script must have been saved in 'Automatic' mode.)
DYNAMO_SCRIPT_FILE_PATH = r"C:\DynamoScripts\MyDynamoWorkspace.dyn"

sessionId = revit_script_util.GetSessionId()
uiapp = revit_script_util.GetUIApplication()

# NOTE: these only make sense for batch Revit file processing mode.
doc = revit_script_util.GetScriptDocument()
revitFilePath = revit_script_util.GetRevitFilePath()

# Dynamo requires an active UIDocument, not just a loaded Document!
# So we use UIApplication.OpenAndActivateDocument() here.
Output()
Output("Activating the document for Dynamo script automation.")
uidoc = uiapp.OpenAndActivateDocument(doc.PathName)

Output()
Output("Executing Dynamo script.")
# One line to execute the Dynamo script!
revit_dynamo_util.ExecuteDynamoScript(uiapp, DYNAMO_SCRIPT_FILE_PATH)

Output()
Output("Finished Dynamo script.")
```

## Pre-processing scripts

TODO: list some examples

## Post-processing scripts

TODO: list some examples

