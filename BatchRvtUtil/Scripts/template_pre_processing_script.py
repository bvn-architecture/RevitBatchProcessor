import System
import clr

clr.AddReference("System.Core")
clr.ImportExtensions(System.Linq)

import script_util
from script_util import Output

sessionId = script_util.GetSessionId()

# NOTE: this only make sense for batch Revit file processing mode.
revitFileListFilePath = script_util.GetRevitFileListFilePath()

# NOTE: these only make sense for data export mode.
sessionDataFolderPath = script_util.GetSessionDataFolderPath()
dataExportFolderPath = script_util.GetExportFolderPath()

# TODO: some real work!
Output()
Output("This pre-processing script is running!")
