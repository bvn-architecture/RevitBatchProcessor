
import clr
import System
clr.AddReference("System.Core")
clr.ImportExtensions(System.Linq)

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
projectFolderName = revit_script_util.GetProjectFolderName()

# NOTE: these only make sense for data export mode.
sessionDataFolderPath = revit_script_util.GetSessionDataFolderPath()
dataExportFolderPath = revit_script_util.GetDataExportFolderPath()

# TODO: some real work!
Output()
Output("This task script is running!")

