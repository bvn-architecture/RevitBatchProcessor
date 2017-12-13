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

import win32_user32
import ui_automation_util
import script_host_error

MODEL_UPGRADE_WINDOW_TITLE = "Model Upgrade"
SAVE_FILE_WINDOW_TITLE = "Save File"

DIRECTUI_CLASS_NAME = "DirectUIHWND"
CTRLNOTIFYSINK_CLASS_NAME = "CtrlNotifySink"
BUTTON_CLASS_NAME = "Button"
CLOSE_BUTTON_TEXT = "Close"
OK_BUTTON_TEXT = "OK"
NO_BUTTON_TEXT = "No"
YES_BUTTON_TEXT = "Yes"
ALWAYS_LOAD_BUTTON_TEXT = "Always Load"


class RevitDialogInfo:
  def __init__(self, dialogHwnd):
    self.Window = ui_automation_util.WindowInfo(dialogHwnd)
    self.Buttons = []
    for directUI in win32_user32.FindWindows(dialogHwnd, DIRECTUI_CLASS_NAME, None):
      for ctrlNotifySink in win32_user32.FindWindows(directUI, CTRLNOTIFYSINK_CLASS_NAME, None):
        for button in win32_user32.FindWindows(ctrlNotifySink, BUTTON_CLASS_NAME, None):
          buttonInfo = ui_automation_util.WindowInfo(button)
          self.Buttons.append(buttonInfo)
    return

def SendButtonClick(buttons, output):
  okButtons = ui_automation_util.FilterControlsByText(buttons, OK_BUTTON_TEXT)
  closeButtons = ui_automation_util.FilterControlsByText(buttons, CLOSE_BUTTON_TEXT)
  noButtons = ui_automation_util.FilterControlsByText(buttons, NO_BUTTON_TEXT)
  alwaysLoadButtons = ui_automation_util.FilterControlsByText(buttons, ALWAYS_LOAD_BUTTON_TEXT)

  if len(okButtons) == 1:
    targetButton = okButtons[0]
  elif len(closeButtons) == 1:
    targetButton = closeButtons[0]
  elif len(noButtons) == 1:
    targetButton = noButtons[0]
  elif len(alwaysLoadButtons) == 1:
    targetButton = alwaysLoadButtons[0]
  else:
    output()
    output("WARNING: Could not find suitable button to click.")
    targetButton = None

  if targetButton is not None:
    targetButtonText = ui_automation_util.GetButtonText(targetButton)
    output()
    output("Sending button click to '" + targetButtonText + "' button...")
    win32_user32.SendButtonClickMessage(targetButton.Hwnd)
    output()
    output("...sent.")
  return

def DismissCheekyRevitDialogBoxes(revitProcessId, output):
  enabledDialogs = ui_automation_util.GetEnabledDialogsInfo(revitProcessId)
  if len(enabledDialogs) > 0:
    for enabledDialog in enabledDialogs:
      revitDialog = RevitDialogInfo(enabledDialog.Hwnd)
      buttons = revitDialog.Buttons
      if enabledDialog.WindowText == MODEL_UPGRADE_WINDOW_TITLE and len(buttons) == 0:
        pass # Do nothing for model upgrade dialog box. It has no buttons and will go away on its own.
      elif enabledDialog.WindowText == script_host_error.BATCH_RVT_ERROR_WINDOW_TITLE:
        pass # Do nothing for BatchRvt error message windows.
      elif enabledDialog.WindowText == SAVE_FILE_WINDOW_TITLE and len(buttons) == 3:
        output()
        output("'" + SAVE_FILE_WINDOW_TITLE + "' dialog box detected.")

        noButtons = ui_automation_util.FilterControlsByText(buttons, NO_BUTTON_TEXT)
        if len(noButtons) == 1:
          targetButton = noButtons[0]
        else:
          output()
          output("WARNING: Could not find suitable button to click for '" + SAVE_FILE_WINDOW_TITLE + "' dialog box!")
          targetButton = None
          for button in buttons:
            buttonText = ui_automation_util.GetButtonText(button)
            output()
            output("\tButton: '" + buttonText +"'")

        if targetButton is not None:
          targetButtonText = ui_automation_util.GetButtonText(targetButton)
          output()
          output("Sending button click to '" + targetButtonText + "' button...")
          win32_user32.SendButtonClickMessage(targetButton.Hwnd)
          output()
          output("...sent.")
      
      else:
        output()
        output("WARNING: unhandled Revit dialog box detected!")
        output()
        output("\tDialog box title: '" + enabledDialog.WindowText + "'")

        for button in buttons:
          buttonText = ui_automation_util.GetButtonText(button)
          output()
          output("\tButton: '" + buttonText +"'")
        SendButtonClick(buttons, output)
  return
