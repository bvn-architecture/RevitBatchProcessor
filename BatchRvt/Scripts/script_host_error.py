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
from System import AppDomain
from System.Text import StringBuilder
clr.AddReference("System.Windows.Forms")
from System.Windows.Forms import MessageBox

import winforms_util
import exception_util

# NOTE: must be the same as BATCH_RVT_ERROR_WINDOW_TITLE defined in ScriptHostUtil.cs in BatchRvtScriptHost project.
BATCH_RVT_ERROR_WINDOW_TITLE = "BatchRvt Script Error"

SCRIPT_HOST_ERROR_DATA_VARIABLE = "revit_script_host"

def SetDataInCurrentDomain(name, data):
  AppDomain.CurrentDomain.SetData(name, data)
  return

def WithErrorHandling(action, errorMessage, output=None, showErrorMessageBox=False):
  result = None

  try:
    result = action()

  except Exception, e:

    if output is not None:
      output()
      output(errorMessage)
      exception_util.LogOutputErrorDetails(e, output)

    if showErrorMessageBox:
      mainWindowHandle = winforms_util.WindowHandleWrapper.GetMainWindowHandle()
      fullMessage = StringBuilder()
      fullMessage.AppendLine(errorMessage)
      fullMessage.AppendLine()
      fullMessage.AppendLine(exception_util.GetExceptionDetails(e))
      MessageBox.Show(mainWindowHandle, fullMessage.ToString(), BATCH_RVT_ERROR_WINDOW_TITLE)

    SetDataInCurrentDomain(SCRIPT_HOST_ERROR_DATA_VARIABLE, e)

  return result

