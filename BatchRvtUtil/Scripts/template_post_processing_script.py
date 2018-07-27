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
Output("This post-processing script is running!")

