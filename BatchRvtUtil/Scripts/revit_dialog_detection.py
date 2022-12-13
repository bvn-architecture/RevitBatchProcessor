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

import global_test_mode
import script_host_error
import ui_automation_util
import win32_user32

REVIT_DIALOG_MESSAGE_HANDLER_PREFIX = "[ REVIT DIALOG BOX HANDLER ]"

MODEL_UPGRADE_WINDOW_TITLE = "Model Upgrade"
LOAD_LINK_WINDOW_TITLE = "Load Link"
CHANGES_NOT_SAVED_TITLE = "Changes Not Saved"
CLOSE_PROJECT_WITHOUT_SAVING_TITLE = "Close Project Without Saving"
CERRAR_PROYECTO_SIN_GUARDAR = "Cerrar proyecto sin guardar"
SAVE_FILE_WINDOW_TITLE = "Save File"
EDITABLE_ELEMENTS_TITLE = "Editable Elements"
AUTODESK_CUSTOMER_INVOLVEMENT_PROGRAM_TITLE = "Autodesk Customer Involvement Program"
CAMBIOS_NO_GUARDADOS_TITLE = "Cambios no guardados"
ELEMENTS_LOST_ON_IMPORT_TITLE = "Elements Lost on Import"
REFERENCIAS_TITLE = "Referencias sin resolver"
CAMBIOS_TITLE = "Cambios locales no sincronizados con archivo central"
CHANGES_TITLE = "Local Changes Not Synchronized with Central"
NWC_TITLE = "Navisworks NWC Exporter"
REVIT_TITLE = "Revit"
OPENING_WORKSETS_TITLES = [
    "Worksets",
    "Opening Worksets"
]

DIRECTUI_CLASS_NAME = "DirectUIHWND"
CTRLNOTIFYSINK_CLASS_NAME = "CtrlNotifySink"
BUTTON_CLASS_NAME = "Button"
STATIC_CONTROL_CLASS_NAME = "Static"
CANCELAR_BUTTON_TEXT = "Cancelar"
CLOSE_BUTTON_TEXT = "Close"
CERRAR_BUTTON_TEXT = "Cerrar"
ACEPTAR_BUTTON_TEXT = "Aceptar"
CEDER_TODO_BUTTON_TEXT = "Ceder todos los elementos y subproyectos"
CEDER_BUTTON_TEXT = "Ceder los elementos y subproyectos"
OK_BUTTON_TEXT = "OK"
IGNORAR_BUTTON_TEXT = "Ignorar y abrir el proyecto"
NO_GUARDAR_PROYECTO_BUTTON_TEXT = "No guardar el proyecto"
NO_BUTTON_TEXT = "No"
YES_BUTTON_TEXT = "Yes"
ALWAYS_LOAD_BUTTON_TEXT = "Always Load"
CANCEL_LINK_BUTTON_TEXT = "Cancel Link"
DO_NOT_SAVE_THE_PROJECT_TEXT = "Do not save the project"
CERRAR_ARCHIVO_LOCAL_BUTTON_TEXT = "Cerrar el archivo local"
CLOSE_LOCAL_FILE_BUTTON_TEXT = "Close the local file"

RELINQUISH_ALL_ELEMENTS_AND_WORKSETS_TEXT = "Relinquish all elements and worksets"
RELINQUISH_ELEMENTS_AND_WORKSETS_TEXT = "Relinquish elements and worksets"

HAVE_REPORTED_BATCH_RVT_ERROR_WINDOW_DETECTION = [False]


class RevitDialogInfo:
    def __init__(self, dialogHwnd):
        self.Window = ui_automation_util.WindowInfo(dialogHwnd)
        self.Win32Buttons = []
        self.Buttons = []
        for win32Button in win32_user32.FindWindows(dialogHwnd, BUTTON_CLASS_NAME, None):
            buttonInfo = ui_automation_util.WindowInfo(win32Button)
            self.Win32Buttons.append(buttonInfo)
        for directUI in win32_user32.FindWindows(dialogHwnd, DIRECTUI_CLASS_NAME, None):
            for ctrlNotifySink in win32_user32.FindWindows(directUI, CTRLNOTIFYSINK_CLASS_NAME, None):
                for button in win32_user32.FindWindows(ctrlNotifySink, BUTTON_CLASS_NAME, None):
                    buttonInfo = ui_automation_util.WindowInfo(button)
                    self.Buttons.append(buttonInfo)
        return


def SendButtonClick(buttons, output):
    okButtons = ui_automation_util.FilterControlsByText(buttons, OK_BUTTON_TEXT)
    aceptarButtons = ui_automation_util.FilterControlsByText(buttons, ACEPTAR_BUTTON_TEXT)
    ignorarButtons = ui_automation_util.FilterControlsByText(buttons, IGNORAR_BUTTON_TEXT)
    cederButtons = ui_automation_util.FilterControlsByText(buttons, CEDER_BUTTON_TEXT)
    cerrarButtons = ui_automation_util.FilterControlsByText(buttons, CERRAR_BUTTON_TEXT)
    noGuardarButtons = ui_automation_util.FilterControlsByText(buttons, NO_GUARDAR_PROYECTO_BUTTON_TEXT)
    closeButtons = ui_automation_util.FilterControlsByText(buttons, CLOSE_BUTTON_TEXT)
    noButtons = ui_automation_util.FilterControlsByText(buttons, NO_BUTTON_TEXT)
    alwaysLoadButtons = ui_automation_util.FilterControlsByText(buttons, ALWAYS_LOAD_BUTTON_TEXT)
    cerrarLocalButtons = ui_automation_util.FilterControlsByText(buttons, CERRAR_ARCHIVO_LOCAL_BUTTON_TEXT)
    closeLocalButtons = ui_automation_util.FilterControlsByText(buttons, CLOSE_LOCAL_FILE_BUTTON_TEXT)

    if len(okButtons) == 1:
        targetButton = okButtons[0]
    elif len(closeButtons) == 1:
        targetButton = closeButtons[0]
    elif len(noButtons) == 1:
        targetButton = noButtons[0]
    elif len(alwaysLoadButtons) == 1:
        targetButton = alwaysLoadButtons[0]
    elif len(ignorarButtons) == 1:
        targetButton = ignorarButtons[0]
    elif len(noGuardarButtons) == 1:
        targetButton = noGuardarButtons[0]
    elif len(cederButtons) == 1:
        targetButton = cederButtons[0]
    elif len(cerrarButtons) == 1:
        targetButton = cerrarButtons[0]
    elif len(aceptarButtons) == 1:
        targetButton = aceptarButtons[0]
    elif len(cerrarLocalButtons) == 1:
        targetButton = aceptarButtons[0]
    elif len(closeLocalButtons) == 1:
        targetButton = aceptarButtons[0]

    else:
        output()
        output("WARNING: Could not find suitable button to click.")
        targetButton = None

    if targetButton is None:
        return

    targetButtonText = ui_automation_util.GetButtonText(targetButton)
    output()
    output("Sending button click to '" + targetButtonText + "' button...")
    win32_user32.SendButtonClickMessage(targetButton.Hwnd)
    output()
    output("...sent.")
    return


def DismissRevitDialogBox(title, buttons, targetButtonText, output):
    targetButtons = ui_automation_util.FilterControlsByText(buttons, targetButtonText)
    if len(targetButtons) == 1:
        targetButton = targetButtons[0]
    else:
        output()
        output("WARNING: Could not find suitable button to click for '" + title + "' dialog box!")
        targetButton = None
        for button in buttons:
            buttonText = ui_automation_util.GetButtonText(button)
            output()
            output("\tButton: '" + buttonText + "'")

    if targetButton is None:
        return

    targetButtonText = ui_automation_util.GetButtonText(targetButton)
    output()
    output("Sending button click to '" + targetButtonText + "' button...")
    win32_user32.SendButtonClickMessage(targetButton.Hwnd)
    output()
    output("...sent.")
    return


def DismissCheekyRevitDialogBoxes(revitProcessId, output_):
    output = global_test_mode.PrefixedOutputForGlobalTestMode(output_, REVIT_DIALOG_MESSAGE_HANDLER_PREFIX)
    enabledDialogs = ui_automation_util.GetEnabledDialogsInfo(revitProcessId)
    if len(enabledDialogs) > 0:
        for enabledDialog in enabledDialogs:
            revitDialog = RevitDialogInfo(enabledDialog.Hwnd)
            buttons = revitDialog.Buttons
            win32Buttons = revitDialog.Win32Buttons
            if enabledDialog.WindowText == MODEL_UPGRADE_WINDOW_TITLE and len(buttons) == 0:
                pass  # Do nothing for model upgrade dialog box. It has no buttons and will go away on its own.
            elif (
                    enabledDialog.WindowText == LOAD_LINK_WINDOW_TITLE
                    and
                    len(win32Buttons) == 1
                    and
                    ui_automation_util.GetButtonText(win32Buttons[0]) == CANCEL_LINK_BUTTON_TEXT
            ):
                pass  # Do nothing for this dialog box. It will go away on its own.
            elif enabledDialog.WindowText == script_host_error.BATCH_RVT_ERROR_WINDOW_TITLE:
                # Report dialog detection but do nothing for BatchRvt error message windows.
                if not HAVE_REPORTED_BATCH_RVT_ERROR_WINDOW_DETECTION[0]:
                    output()
                    output("WARNING: Revit Batch Processor error window detected! Processing has halted!")
                    HAVE_REPORTED_BATCH_RVT_ERROR_WINDOW_DETECTION[0] = True
                    staticControls = list(ui_automation_util.WindowInfo(hwnd) for hwnd in
                                          win32_user32.FindWindows(enabledDialog.Hwnd, STATIC_CONTROL_CLASS_NAME, None))
                    if len(staticControls) > 0:
                        output()
                        output("Dialog has the following static control text:")
                        for staticControl in staticControls:
                            staticControlText = staticControl.WindowText
                            if not str.IsNullOrWhiteSpace(staticControlText):
                                output()
                                output(staticControlText)
            elif enabledDialog.WindowText == CHANGES_NOT_SAVED_TITLE and len(buttons) == 4:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, DO_NOT_SAVE_THE_PROJECT_TEXT, output)
            elif enabledDialog.WindowText == CLOSE_PROJECT_WITHOUT_SAVING_TITLE and len(buttons) == 3:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, RELINQUISH_ALL_ELEMENTS_AND_WORKSETS_TEXT,
                                      output)
            elif enabledDialog.WindowText == SAVE_FILE_WINDOW_TITLE and len(buttons) == 3:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, NO_BUTTON_TEXT, output)
            elif enabledDialog.WindowText == EDITABLE_ELEMENTS_TITLE and len(buttons) == 3:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, RELINQUISH_ELEMENTS_AND_WORKSETS_TEXT, output)
            elif enabledDialog.WindowText == CAMBIOS_NO_GUARDADOS_TITLE and len(buttons) == 4:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, NO_GUARDAR_PROYECTO_BUTTON_TEXT, output)
            elif enabledDialog.WindowText == CERRAR_PROYECTO_SIN_GUARDAR and len(buttons) == 3:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, CEDER_TODO_BUTTON_TEXT, output)
            elif enabledDialog.WindowText == REFERENCIAS_TITLE and len(buttons) == 2:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, IGNORAR_BUTTON_TEXT, output)
            elif enabledDialog.WindowText == NWC_TITLE and len(buttons) == 1:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, ACEPTAR_BUTTON_TEXT, output)
            elif enabledDialog.WindowText == REVIT_TITLE and len(buttons) == 1:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, ACEPTAR_BUTTON_TEXT, output)
            elif enabledDialog.WindowText == CAMBIOS_TITLE and len(buttons) == 3:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, CERRAR_ARCHIVO_LOCAL_BUTTON_TEXT, output)
            elif enabledDialog.WindowText == CHANGES_TITLE and len(buttons) == 3:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, CLOSE_LOCAL_FILE_BUTTON_TEXT, output)
            elif enabledDialog.WindowText == ELEMENTS_LOST_ON_IMPORT_TITLE and len(buttons) == 1:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                DismissRevitDialogBox(enabledDialog.WindowText, buttons, CERRAR_BUTTON_TEXT, output)
                #######
            elif enabledDialog.WindowText in ["Revit", str.Empty] and len(buttons) == 0 and len(win32Buttons) > 0:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                staticControls = list(ui_automation_util.WindowInfo(hwnd) for hwnd in
                                      win32_user32.FindWindows(enabledDialog.Hwnd, STATIC_CONTROL_CLASS_NAME, None))
                if len(staticControls) > 0:
                    output()
                    output("Dialog has the following static control text:")
                    for staticControl in staticControls:
                        staticControlText = staticControl.WindowText
                        if not str.IsNullOrWhiteSpace(staticControlText):
                            output()
                            output(staticControlText)
                SendButtonClick(win32Buttons, output)
            elif enabledDialog.WindowText == AUTODESK_CUSTOMER_INVOLVEMENT_PROGRAM_TITLE and len(buttons) == 0 and len(
                    win32Buttons) > 0:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                output()
                output("Sending close message...")
                win32_user32.SendCloseMessage(enabledDialog.Hwnd)
                output()
                output("...sent.")
            elif enabledDialog.WindowText in OPENING_WORKSETS_TITLES and len(buttons) == 0 and len(win32Buttons) > 0:
                output()
                output("'" + enabledDialog.WindowText + "' dialog box detected.")
                output()
                SendButtonClick(win32Buttons, output)
            else:
                output()
                output("Revit dialog box detected!")
                output()
                output("\tDialog box title: '" + enabledDialog.WindowText + "'")

                buttons = buttons if len(buttons) > 0 else win32Buttons
                for button in buttons:
                    buttonText = ui_automation_util.GetButtonText(button)
                    output()
                    output("\tButton: '" + buttonText + "'")
                SendButtonClick(buttons, output)
    return
