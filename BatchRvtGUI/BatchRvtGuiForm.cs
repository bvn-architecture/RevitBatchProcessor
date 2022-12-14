//
// Revit Batch Processor
//
// Copyright (c) 2020  Daniel Rumery, BVN
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BatchRvtUtil;

namespace BatchRvtGUI;

public partial class BatchRvtGuiForm : Form
{
    private const string WINDOW_TITLE = "Revit Batch Processor";

    private const string NEW_TASK_SCRIPT_FILENAME = "new_task_script.py";
    private const string NEW_PREPROCESSING_SCRIPT_FILENAME = "new_pre_processing_script.py";
    private const string NEW_POSTPROCESSING_SCRIPT_FILENAME = "new_post_processing_script.py";

    private const string TEMPLATE_TASK_SCRIPT_FILENAME = "template_task_script.py";
    private const string TEMPLATE_PREPROCESSING_SCRIPT_FILENAME = "template_pre_processing_script.py";
    private const string TEMPLATE_POSTPROCESSING_SCRIPT_FILENAME = "template_post_processing_script.py";

    private const string PYTHON_SCRIPT_EXTENSION = ".py";
    private const string PYTHON_SCRIPT_FILTER = "Python files (*.py)|*.py";
    private const string DYNAMO_SCRIPT_EXTENSION = ".dyn";
    private const string DYNAMO_SCRIPT_FILTER = "Dynamo files (*.dyn)|*.dyn";
    private const string ANY_SCRIPTS_FILTER = "Script files (*.py;*.dyn)|*.py;*.dyn";

    private const int INITIAL_WIDTH = 1040;

    private const int SETUP_HEIGHT = 685;
    private const int SETUP_INITIAL_WIDTH = INITIAL_WIDTH;
    private const int SETUP_MINIMUM_WIDTH = INITIAL_WIDTH;
    private const int SETUP_MAXIMUM_WIDTH = 1600;

    private const int RUNNING_INITIAL_WIDTH = INITIAL_WIDTH;
    private const int RUNNING_INITIAL_HEIGHT = 875;
    private const int RUNNING_MINIMUM_HEIGHT = 875;
    private const int RUNNING_MINIMUM_WIDTH = INITIAL_WIDTH;

    private const int ADVANCED_SETTINGS_VISIBLE_SIZE_DIFFERENCE = 275;
    private const int READ_OUTPUT_INTERVAL_IN_MS = 10;

    private readonly Size RUNNING_INITIAL_SIZE = new(RUNNING_INITIAL_WIDTH, RUNNING_INITIAL_HEIGHT);
    private readonly Size RUNNING_MAXIMUM_SIZE = new(0, 0); // no maximum size
    private readonly Size RUNNING_MINIMUM_SIZE = new(RUNNING_MINIMUM_WIDTH, RUNNING_MINIMUM_HEIGHT);

    private readonly Size SETUP_INITIAL_SIZE = new(SETUP_INITIAL_WIDTH, SETUP_HEIGHT);
    private readonly Size SETUP_MAXIMUM_SIZE = new(SETUP_MAXIMUM_WIDTH, SETUP_HEIGHT);
    private readonly Size SETUP_MINIMUM_SIZE = new(SETUP_MINIMUM_WIDTH, SETUP_HEIGHT);
    private readonly UIConfig UIConfiguration;

    private Process batchRvtProcess;
    private bool isBatchRvtRunning;
    private bool isUsingRunningSize;
    private Task<string> pendingErrorReadLineTask;

    private Task<string> pendingOutputReadLineTask;
    private Timer readBatchRvtOutput_Timer;

    private BatchRvtSettings Settings;

    public BatchRvtGuiForm()
    {
        InitializeComponent();
        Settings = new BatchRvtSettings();

        UIConfiguration = new UIConfig(GetUIConfigItems());
    }

    private IEnumerable<IUIConfigItem> GetUIConfigItems()
    {
        var iuConfigItems = new IUIConfigItem[]
        {
            // General Task Script settings
            new UIConfigItem(
                () =>
                {
                    UpdateTaskScript(Settings.TaskScriptFilePath.GetValue());
                    showMessageBoxOnTaskScriptErrorCheckBox.Checked =
                        Settings.ShowMessageBoxOnTaskScriptError.GetValue();
                },
                () =>
                {
                    Settings.TaskScriptFilePath.SetValue(taskScriptTextBox.Text);
                    Settings.ShowMessageBoxOnTaskScriptError.SetValue(showMessageBoxOnTaskScriptErrorCheckBox
                        .Checked);
                }
            ),

            // Revit File List settings
            new UIConfigItem(
                () => { revitFileListTextBox.Text = Settings.RevitFileListFilePath.GetValue(); },
                () => { Settings.RevitFileListFilePath.SetValue(revitFileListTextBox.Text); }
            ),

            // Data Export settings
            new UIConfigItem(
                () =>
                {
                    enableDataExportCheckBox.Checked = Settings.EnableDataExport.GetValue();
                    dataExportFolderTextBox.Text = Settings.DataExportFolderPath.GetValue();
                    UpdateDataExportControls();
                },
                () =>
                {
                    Settings.EnableDataExport.SetValue(enableDataExportCheckBox.Checked);
                    Settings.DataExportFolderPath.SetValue(dataExportFolderTextBox.Text);
                }
            ),

            // Pre-processing Script settings
            new UIConfigItem(
                () =>
                {
                    executePreProcessingScriptCheckBox.Checked = Settings.ExecutePreProcessingScript.GetValue();
                    preProcessingScriptTextBox.Text = Settings.PreProcessingScriptFilePath.GetValue();
                    UpdatePreProcessingScriptControls();
                },
                () =>
                {
                    Settings.ExecutePreProcessingScript.SetValue(executePreProcessingScriptCheckBox.Checked);
                    Settings.PreProcessingScriptFilePath.SetValue(preProcessingScriptTextBox.Text);
                }
            ),

            // Post-processing Script settings
            new UIConfigItem(
                () =>
                {
                    executePostProcessingScriptCheckBox.Checked = Settings.ExecutePostProcessingScript.GetValue();
                    postProcessingScriptTextBox.Text = Settings.PostProcessingScriptFilePath.GetValue();
                    UpdatePostProcessingScriptControls();
                },
                () =>
                {
                    Settings.ExecutePostProcessingScript.SetValue(executePostProcessingScriptCheckBox.Checked);
                    Settings.PostProcessingScriptFilePath.SetValue(postProcessingScriptTextBox.Text);
                }
            ),

            // Central File Processing settings
            new UIConfigItem(
                () =>
                {
                    detachFromCentralRadioButton.Checked = Settings.CentralFileOpenOption.GetValue() ==
                                                           BatchRvt.CentralFileOpenOption.Detach;
                    createNewLocalRadioButton.Checked = Settings.CentralFileOpenOption.GetValue() ==
                                                        BatchRvt.CentralFileOpenOption.CreateNewLocal;
                    deleteLocalAfterCheckBox.Checked = Settings.DeleteLocalAfter.GetValue();
                    discardWorksetsCheckBox.Checked = Settings.DiscardWorksetsOnDetach.GetValue();
                    closeAllWorksetsRadioButton.Checked = Settings.WorksetConfigurationOption.GetValue() ==
                                                          BatchRvt.WorksetConfigurationOption.CloseAllWorksets;
                    openAllWorksetsRadioButton.Checked = Settings.WorksetConfigurationOption.GetValue() ==
                                                         BatchRvt.WorksetConfigurationOption.OpenAllWorksets;
                    openLastViewedWorksetsRadioButton.Checked = Settings.WorksetConfigurationOption.GetValue() ==
                                                                BatchRvt.WorksetConfigurationOption.OpenLastViewed;
                    UpdateCentralFileProcessingControls();
                },
                () =>
                {
                    Settings.CentralFileOpenOption.SetValue(
                        createNewLocalRadioButton.Checked
                            ? BatchRvt.CentralFileOpenOption.CreateNewLocal
                            : BatchRvt.CentralFileOpenOption.Detach
                    );
                    Settings.DeleteLocalAfter.SetValue(deleteLocalAfterCheckBox.Checked);
                    Settings.DiscardWorksetsOnDetach.SetValue(discardWorksetsCheckBox.Checked);
                    Settings.WorksetConfigurationOption.SetValue(
                        closeAllWorksetsRadioButton.Checked ? BatchRvt.WorksetConfigurationOption.CloseAllWorksets :
                        openAllWorksetsRadioButton.Checked ? BatchRvt.WorksetConfigurationOption.OpenAllWorksets :
                        BatchRvt.WorksetConfigurationOption.OpenLastViewed
                    );
                }
            ),

            // Revit Session settings
            new UIConfigItem(
                () =>
                {
                    useSeparateRevitSessionRadioButton.Checked = Settings.RevitSessionOption.GetValue() ==
                                                                 BatchRvt.RevitSessionOption
                                                                     .UseSeparateSessionPerFile;
                    useSameRevitSessionRadioButton.Checked = Settings.RevitSessionOption.GetValue() ==
                                                             BatchRvt.RevitSessionOption
                                                                 .UseSameSessionForFilesOfSameVersion;
                    var processingTimeOutInMinutes = Settings.ProcessingTimeOutInMinutes.GetValue();
                    if (processingTimeOutInMinutes < 0) processingTimeOutInMinutes = 0;
                    perFileProcessingTimeOutCheckBox.Checked = processingTimeOutInMinutes > 0;
                    timeOutNumericUpDown.Value = processingTimeOutInMinutes;
                    UpdateRevitSessionControls();

                    // NOTE: This is done so that the Revit session option is updated according to the script file type.
                    // NOTE: This is a bit hacky!
                    UpdateTaskScript(taskScriptTextBox.Text);
                },
                () =>
                {
                    Settings.RevitSessionOption.SetValue(
                        useSameRevitSessionRadioButton.Checked
                            ? BatchRvt.RevitSessionOption.UseSameSessionForFilesOfSameVersion
                            : BatchRvt.RevitSessionOption.UseSeparateSessionPerFile
                    );

                    var processingTimeOutInMinutes = (int)timeOutNumericUpDown.Value;
                    if (processingTimeOutInMinutes < 0) processingTimeOutInMinutes = 0;

                    Settings.ProcessingTimeOutInMinutes.SetValue(perFileProcessingTimeOutCheckBox.Checked
                        ? processingTimeOutInMinutes
                        : 0);
                }
            ),

            // Revit Processing settings
            new UIConfigItem(
                () =>
                {
                    enableBatchProcessingCheckBox.Checked = Settings.RevitProcessingOption.GetValue() ==
                                                            BatchRvt.RevitProcessingOption.BatchRevitFileProcessing;
                    enableSingleRevitTaskProcessingCheckBox.Checked = Settings.RevitProcessingOption.GetValue() ==
                                                                      BatchRvt.RevitProcessingOption
                                                                          .SingleRevitTaskProcessing;
                    UpdateRevitProcessingControls();
                },
                () =>
                {
                    Settings.RevitProcessingOption.SetValue(
                        enableSingleRevitTaskProcessingCheckBox.Checked
                            ? BatchRvt.RevitProcessingOption.SingleRevitTaskProcessing
                            : BatchRvt.RevitProcessingOption.BatchRevitFileProcessing
                    );
                }
            ),

            // Single Revit Task Processing settings
            new UIConfigItem(
                () =>
                {
                    Populate(
                        singleRevitTaskRevitVersionComboBox,
                        RevitVersion.GetInstalledRevitVersions().Select(RevitVersion.GetRevitVersionText),
                        RevitVersion.GetRevitVersionText(Settings.SingleRevitTaskRevitVersion.GetValue())
                    );
                },
                () =>
                {
                    Settings.SingleRevitTaskRevitVersion.SetValue(
                        RevitVersion.GetSupportedRevitVersion(
                            singleRevitTaskRevitVersionComboBox.SelectedItem as string)
                    );
                }
            ),

            // Batch Revit File Processing settings
            new UIConfigItem(
                () =>
                {
                    useFileRevitVersionRadioButton.Checked = Settings.RevitFileProcessingOption.GetValue() ==
                                                             BatchRvt.RevitFileProcessingOption
                                                                 .UseFileRevitVersionIfAvailable;
                    useSpecificRevitVersionRadioButton.Checked = Settings.RevitFileProcessingOption.GetValue() ==
                                                                 BatchRvt.RevitFileProcessingOption
                                                                     .UseSpecificRevitVersion;
                    useMinimumAvailableVersionCheckBox.Checked =
                        Settings.IfNotAvailableUseMinimumAvailableRevitVersion.GetValue();
                    auditOnOpeningCheckBox.Checked = Settings.AuditOnOpening.GetValue();
                    Populate(
                        specificRevitVersionComboBox,
                        RevitVersion.GetInstalledRevitVersions().Select(RevitVersion.GetRevitVersionText),
                        RevitVersion.GetRevitVersionText(Settings.BatchRevitTaskRevitVersion.GetValue())
                    );
                    UpdateRevitFileProcessingControls();
                },
                () =>
                {
                    Settings.RevitFileProcessingOption.SetValue(
                        useSpecificRevitVersionRadioButton.Checked
                            ? BatchRvt.RevitFileProcessingOption.UseSpecificRevitVersion
                            : BatchRvt.RevitFileProcessingOption.UseFileRevitVersionIfAvailable
                    );
                    Settings.IfNotAvailableUseMinimumAvailableRevitVersion.SetValue(
                        useMinimumAvailableVersionCheckBox.Checked);
                    Settings.BatchRevitTaskRevitVersion.SetValue(
                        RevitVersion.GetSupportedRevitVersion(specificRevitVersionComboBox.SelectedItem as string)
                    );
                    Settings.AuditOnOpening.SetValue(auditOnOpeningCheckBox.Checked);
                }
            ),

            // Show Advanced setting
            new UIConfigItem(
                () =>
                {
                    showAdvancedSettingsCheckBox.Checked = Settings.ShowAdvancedSettings.GetValue();
                    UpdateAdvancedSettings();
                },
                () => { Settings.ShowAdvancedSettings.SetValue(showAdvancedSettingsCheckBox.Checked); }
            )
        };

        return iuConfigItems;
    }

    private void UpdateTaskScript(string scriptFilePath)
    {
        taskScriptTextBox.Text = scriptFilePath;

        var scriptType = GetScriptType(scriptFilePath);

        if (scriptType == ScriptType.Dynamo)
        {
            useSeparateRevitSessionRadioButton.Checked = true;
            useSameRevitSessionRadioButton.Checked = false;
            useSeparateRevitSessionRadioButton.Enabled = false;
            useSameRevitSessionRadioButton.Enabled = false;
        }
        else
        {
            useSeparateRevitSessionRadioButton.Enabled = true;
            useSameRevitSessionRadioButton.Enabled = true;
        }
    }

    private double GetDisplaySettingPercentage()
    {
        var graphics = CreateGraphics();
        var dpiX = graphics.DpiX;
        return dpiX / 96f;
    }

    private static int Scale(int value, double scale)
    {
        return (int)(value * scale);
    }

    private static Size Scale(Size size, double scale)
    {
        return new Size(Scale(size.Width, scale), Scale(size.Height, scale));
    }

    private void AdjustWindowSizeForDisplaySetting()
    {
        var displaySettingPercentage = GetDisplaySettingPercentage();

        MinimumSize = Scale(MinimumSize, displaySettingPercentage);
        MaximumSize = Scale(MaximumSize, displaySettingPercentage);
        Size = Scale(Size, displaySettingPercentage);
    }

    private void BatchRvtGuiForm_Load(object sender, EventArgs e)
    {
        Text = WINDOW_TITLE;

        TopMost = false;
        alwaysOnTopCheckbox.Checked = TopMost;
        batchRvtOutputGroupBox.Visible = false;

        MinimumSize = SETUP_MINIMUM_SIZE;
        MaximumSize = SETUP_MAXIMUM_SIZE;
        Size = SETUP_INITIAL_SIZE;
        MaximizeBox = false;

        AdjustWindowSizeForDisplaySetting();

        var isLoaded = LoadSettings();

        // TODO: show error message if load failed!!
    }

    private bool LoadSettings(string filePath = null)
    {
        var newBatchRvtSettings = new BatchRvtSettings();

        var isLoaded = newBatchRvtSettings.LoadFromFile(filePath);

        if (isLoaded) Settings = newBatchRvtSettings;

        UIConfiguration.UpdateUI();

        VerifyExcelInstallation(revitFileListTextBox.Text);

        return isLoaded;
    }

    private void VerifyExcelInstallation(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return;
        if (ExcelUtil.HasExcelExtension(filePath) && !ExcelUtil.IsExcelInstalled())
            MessageBox.Show(
                "WARNING: An Excel installation was not detected! Support for Excel files requires an Excel installation.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
    }

    private bool SaveSettings(string filePath = null)
    {
        UIConfiguration.UpdateConfig();

        var isSaved = Settings.SaveToFile(filePath);

        return isSaved;
    }

    private void BatchRvtGuiForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!isBatchRvtRunning)
        {
            return;
        }

        var message = new StringBuilder();

        message.AppendLine("Do you want to terminate the currently running task?");

        var dialogResult = MessageBox.Show(
            message.ToString(),
            Text,
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Asterisk,
            MessageBoxDefaultButton.Button3
        );

        if (dialogResult == DialogResult.Cancel)
            e.Cancel = true;
        else if (dialogResult == DialogResult.Yes)
            try
            {
                batchRvtProcess.Kill();
            }
            catch (Exception)
            {
                // TODO: report failure to kill the process?
            }
        

        if (e.Cancel)
        {
            return;
        }
        
           

        message.AppendLine("Do you want to save the current settings as default?");

        var dialogResult2 = MessageBox.Show(
            message.ToString(),
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button1
        );

        if (dialogResult2 == DialogResult.Yes)
        {
            var isSaved = SaveSettings();

            // TODO: show error message if save failed!!
        }

        if (readBatchRvtOutput_Timer == null)
        {
            return;
        }
        readBatchRvtOutput_Timer.Stop();
        readBatchRvtOutput_Timer.Dispose();

    }

    private void alwaysOnTopCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        TopMost = alwaysOnTopCheckbox.Checked;
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    public static void ShowErrorMessageBox(string errorMessage)
    {
        MessageBox.Show(errorMessage, WINDOW_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private bool ValidateConfig()
    {
        var validated = false;

        if (!enableSingleRevitTaskProcessingCheckBox.Checked && !enableBatchProcessingCheckBox.Checked)
            ShowErrorMessageBox(
                "ERROR: You must select either Batch Revit File Processing or Single Revit Task Processing!");
        else if (!File.Exists(Settings.TaskScriptFilePath.GetValue()))
            ShowErrorMessageBox("ERROR: You must select an existing Task script!");
        else if (
            Settings.RevitProcessingOption.GetValue() == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing
            &&
            !File.Exists(Settings.RevitFileListFilePath.GetValue())
        )
            ShowErrorMessageBox("ERROR: You must select an existing Revit File List!");
        else if (
            Settings.EnableDataExport.GetValue()
            &&
            !Directory.Exists(Settings.DataExportFolderPath.GetValue())
        )
            ShowErrorMessageBox("ERROR: You must select an existing Data Export folder!");
        else if (
            Settings.ExecutePreProcessingScript.GetValue()
            &&
            !File.Exists(Settings.PreProcessingScriptFilePath.GetValue())
        )
            ShowErrorMessageBox("ERROR: You must select an existing Pre-Processing Python script!");
        else if (
            Settings.ExecutePostProcessingScript.GetValue()
            &&
            !File.Exists(Settings.PostProcessingScriptFilePath.GetValue())
        )
            ShowErrorMessageBox("ERROR: You must select an existing Post-Processing Python script!");
        else
            validated = true;

        return validated;
    }

    private void startButton_Click(object sender, EventArgs e)
    {
        UIConfiguration.UpdateConfig();

        var validated = ValidateConfig();

        if (!validated) return;
        var isSaved = SaveSettings();

        // TODO: show error message if save failed!!

        var settingsFilePath = BatchRvtSettings.GetDefaultSettingsFilePath();

        batchRvtProcess = BatchRvt.StartBatchRvt(settingsFilePath);

        readBatchRvtOutput_Timer = new Timer { Interval = READ_OUTPUT_INTERVAL_IN_MS };
        readBatchRvtOutput_Timer.Tick += readBatchRvtOutput_Timer_Tick;

        isBatchRvtRunning = true;
        settingsGroupBox.Enabled = false;
        importSettingsButton.Enabled = false;
        startButton.Enabled = false;
        startButton.Text = @"Running...";
        batchRvtOutputGroupBox.Visible = true;
        MinimumSize = RUNNING_MINIMUM_SIZE;
        MaximumSize = RUNNING_MAXIMUM_SIZE;
        Size = RUNNING_INITIAL_SIZE;
        MaximizeBox = true;
        isUsingRunningSize = true;

        UpdateAdvancedSettings();

        readBatchRvtOutput_Timer.Start();
    }

    private void readBatchRvtOutput_Timer_Tick(object sender, EventArgs e)
    {
        var linesAndPendingTask =
            StreamIOUtil.ReadAvailableLines(batchRvtProcess.StandardOutput, pendingOutputReadLineTask);
        pendingOutputReadLineTask = linesAndPendingTask.Item2;
        var lines = linesAndPendingTask.Item1;

        foreach (var line in lines)
        {
            var fullLine = line + Environment.NewLine;

            if (!BatchRvt.IsBatchRvtLine(line))
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss");

                fullLine = timestamp + " : [ REVIT MESSAGE ] : " + fullLine;
            }

            if (BatchRvt.IsBatchRvtLine(line)) // Do not show non-BatchRvt-related output. (TODO: reconsider?)
                batchRvtOutputTextBox.AppendText(fullLine);
        }

        linesAndPendingTask =
            StreamIOUtil.ReadAvailableLines(batchRvtProcess.StandardError, pendingErrorReadLineTask);
        pendingErrorReadLineTask = linesAndPendingTask.Item2;
        lines = linesAndPendingTask.Item1;

        foreach (var line in lines.Where(line => !line.StartsWith("log4cplus:")).Where(line => Settings.ShowRevitProcessErrorMessages.GetValue()))
        {
            batchRvtOutputTextBox.AppendText("[ REVIT ERROR MESSAGE ] : " + line + Environment.NewLine);
        }

        if (!isBatchRvtRunning) return;
        batchRvtProcess.Refresh();
        if (!batchRvtProcess.HasExited) return;
        isBatchRvtRunning = false;
        startButton.Text = @"Done!";
    }

    private void browseScriptButton_Click(object sender, EventArgs e)
    {
        BrowseForExistingScriptFile(
            "Select Task script",
            UpdateTaskScript,
            ScriptType.Any,
            PathUtil.GetExistingFileDirectoryPath(taskScriptTextBox.Text)
        );
    }

    private void BrowseForSave(string dialogTitle, Action<string> fileAction, string defaultExt, string filter,
        string initialDirectory = null, string initialFileName = null)
    {
        var saveFileDialog = new SaveFileDialog();

        saveFileDialog.DefaultExt = defaultExt;
        saveFileDialog.Filter = filter;
        saveFileDialog.Title = dialogTitle;

        if (!string.IsNullOrWhiteSpace(initialDirectory)) saveFileDialog.InitialDirectory = initialDirectory;

        if (!string.IsNullOrWhiteSpace(initialFileName)) saveFileDialog.FileName = initialFileName;

        var dialogResult = saveFileDialog.ShowDialog(this);

        if (dialogResult != DialogResult.OK) return;

        var selectedFilePath = saveFileDialog.FileName;

        if (!string.IsNullOrWhiteSpace(selectedFilePath)) fileAction(selectedFilePath);
    }

    private void BrowseForFile(string dialogTitle, Action<string> fileAction, string defaultExt, string filter,
        bool checkFileExists, string initialDirectory = null)
    {
        var openFileDialog = new OpenFileDialog();

        openFileDialog.DefaultExt = defaultExt;
        openFileDialog.Filter = filter;
        openFileDialog.CheckFileExists = checkFileExists;
        openFileDialog.ReadOnlyChecked = true;
        openFileDialog.Multiselect = false;
        openFileDialog.Title = dialogTitle;

        if (!string.IsNullOrWhiteSpace(initialDirectory)) openFileDialog.InitialDirectory = initialDirectory;

        var dialogResult = openFileDialog.ShowDialog(this);

        if (dialogResult != DialogResult.OK) return;

        var selectedFilePath = openFileDialog.FileName;

        if (!string.IsNullOrWhiteSpace(selectedFilePath)) fileAction(selectedFilePath);
    }

    private void BrowseForExistingScriptFile(
        string dialogTitle,
        Action<string> scriptFileAction,
        ScriptType scriptType,
        string initialDirectory = null
    )
    {
        var scriptDefaultExtension =
            scriptType == ScriptType.Dynamo ? DYNAMO_SCRIPT_EXTENSION : PYTHON_SCRIPT_EXTENSION;
        var scriptFilter = scriptType == ScriptType.Dynamo ? DYNAMO_SCRIPT_FILTER :
            scriptType == ScriptType.Python ? PYTHON_SCRIPT_FILTER :
            ANY_SCRIPTS_FILTER;

        BrowseForFile(
            dialogTitle,
            scriptFileAction,
            scriptDefaultExtension,
            scriptFilter,
            true,
            initialDirectory
        );
    }

    private void browseRevitFileListButton_Click(object sender, EventArgs e)
    {
        var openFileDialog = new OpenFileDialog();

        openFileDialog.DefaultExt = ".txt";
        openFileDialog.Filter = "Revit File List (*.txt;*.xls;*.xlsx;*.csv)|*.txt;*.xls;*.xlsx;*.csv";
        openFileDialog.CheckFileExists = true;
        openFileDialog.ReadOnlyChecked = true;
        openFileDialog.Multiselect = false;
        openFileDialog.Title = "Select Revit File List";

        var initialDirectory = PathUtil.GetExistingFileDirectoryPath(revitFileListTextBox.Text);

        if (initialDirectory != null) openFileDialog.InitialDirectory = initialDirectory;

        var dialogResult = openFileDialog.ShowDialog(this);

        if (dialogResult != DialogResult.OK) return;
        var selectedFilePath = openFileDialog.FileName;

        if (string.IsNullOrWhiteSpace(selectedFilePath)) return;

        revitFileListTextBox.Text = selectedFilePath;

        VerifyExcelInstallation(selectedFilePath);
    }

    private void browseDataExportFolderButton_Click(object sender, EventArgs e)
    {
        var folderBrowserDialog = new FolderBrowserDialog();

        folderBrowserDialog.Description = "Select Data Export folder";

        var currentFolderPath = dataExportFolderTextBox.Text;

        if (Directory.Exists(currentFolderPath)) folderBrowserDialog.SelectedPath = currentFolderPath;

        var dialogResult = folderBrowserDialog.ShowDialog(this);

        if (dialogResult != DialogResult.OK) return;

        var selectedFolderPath = folderBrowserDialog.SelectedPath;

        if (!string.IsNullOrWhiteSpace(selectedFolderPath)) dataExportFolderTextBox.Text = selectedFolderPath;
    }

    private void batchRvtOutputTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyData != (Keys.Control | Keys.A)) return;

        batchRvtOutputTextBox.SelectAll();
        e.SuppressKeyPress = true;
    }

    private void executePreProcessingScriptCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        UpdatePreProcessingScriptControls();
    }

    private void executePostProcessingScriptCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        UpdatePostProcessingScriptControls();
    }

    private void preProcessingScriptBrowseButton_Click(object sender, EventArgs e)
    {
        BrowseForExistingScriptFile(
            "Select Pre-Processing Python script",
            scriptFilePath => { preProcessingScriptTextBox.Text = scriptFilePath; },
            ScriptType.Python,
            PathUtil.GetExistingFileDirectoryPath(preProcessingScriptTextBox.Text)
        );
    }

    private void postProcessingScriptBrowseButton_Click(object sender, EventArgs e)
    {
        BrowseForExistingScriptFile(
            "Select Post-Processing Python script",
            scriptFilePath => { postProcessingScriptTextBox.Text = scriptFilePath; },
            ScriptType.Python,
            PathUtil.GetExistingFileDirectoryPath(postProcessingScriptTextBox.Text)
        );
    }

    private void enableBatchProcessingCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (enableBatchProcessingCheckBox.Checked) enableSingleRevitTaskProcessingCheckBox.Checked = false;

        UpdateRevitProcessingControls();
    }

    private void enableSingleRevitTaskProcessingCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (enableSingleRevitTaskProcessingCheckBox.Checked) enableBatchProcessingCheckBox.Checked = false;

        UpdateRevitProcessingControls();
    }

    private static ScriptType GetScriptType(string scriptFilePath)
    {
        return PathUtil.HasExtension(scriptFilePath, PYTHON_SCRIPT_EXTENSION) ? ScriptType.Python :
            PathUtil.HasExtension(scriptFilePath, DYNAMO_SCRIPT_EXTENSION) ? ScriptType.Dynamo :
            ScriptType.Any;
    }

    private void UpdateRevitProcessingControls()
    {
        var batchTaskEnabled = enableBatchProcessingCheckBox.Checked;
        revitFileListLabel.Enabled = batchTaskEnabled;
        revitFileListTextBox.Enabled = batchTaskEnabled;
        browseRevitFileListButton.Enabled = batchTaskEnabled;

        centralFileProcessingGroupBox.Enabled = batchTaskEnabled;
        revitFileProcessingGroupBox.Enabled = batchTaskEnabled;
        revitSessionGroupBox.Enabled = batchTaskEnabled;

        var singleTaskEnabled = enableSingleRevitTaskProcessingCheckBox.Checked;
        singleRevitTaskRevitVersionLabel.Enabled = singleTaskEnabled;
        singleRevitTaskRevitVersionComboBox.Enabled = singleTaskEnabled;
    }

    private void perFileProcessingTimeOutCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        UpdateRevitSessionControls();
    }

    private void UpdateRevitSessionControls()
    {
        var perFileProcessingTimeOutEnabled = perFileProcessingTimeOutCheckBox.Checked;

        if (perFileProcessingTimeOutEnabled)
            // If time-out option is enabled but the current numeric value is 0, set a sensible default / initial value for the time-out.
            if (timeOutNumericUpDown.Value == 0)
                timeOutNumericUpDown.Value = 15;
        timeOutNumericUpDown.Enabled = perFileProcessingTimeOutEnabled;
    }

    private void UpdatePreProcessingScriptControls()
    {
        var isChecked = executePreProcessingScriptCheckBox.Checked;
        preProcessingScriptTextBox.Enabled = isChecked;
        preProcessingScriptBrowseButton.Enabled = isChecked;
        preProcessingScriptNewScriptButton.Enabled = isChecked;
    }

    private void UpdatePostProcessingScriptControls()
    {
        var isChecked = executePostProcessingScriptCheckBox.Checked;
        postProcessingScriptTextBox.Enabled = isChecked;
        postProcessingScriptBrowseButton.Enabled = isChecked;
        postProcessingScriptNewScriptButton.Enabled = isChecked;
    }

    private static void Populate<T>(ComboBox comboBox, IEnumerable<T> items, T selectedItem)
    {
        var itemsList = items.ToList();

        comboBox.Items.Clear();

        foreach (var item in itemsList) comboBox.Items.Add(item);

        var selectedIndex = itemsList.IndexOf(selectedItem);

        comboBox.SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
    }

    private void createNewLocalRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        UpdateCentralFileProcessingControls();
    }

    private void detachFromCentralRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        UpdateCentralFileProcessingControls();
    }

    private void UpdateCentralFileProcessingControls()
    {
        deleteLocalAfterCheckBox.Enabled = createNewLocalRadioButton.Checked;
        discardWorksetsCheckBox.Enabled = detachFromCentralRadioButton.Checked;
        worksetConfigurationGroupBox.Enabled =
            !(detachFromCentralRadioButton.Checked && discardWorksetsCheckBox.Checked);
    }

    private void useFileRevitVersionRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        UpdateRevitFileProcessingControls();
    }

    private void UpdateRevitFileProcessingControls()
    {
        useMinimumAvailableVersionCheckBox.Enabled = useFileRevitVersionRadioButton.Checked;
        specificRevitVersionComboBox.Enabled = useSpecificRevitVersionRadioButton.Checked;
    }

    private void enableDataExportCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        UpdateDataExportControls();
    }

    private void UpdateDataExportControls()
    {
        var isChecked = enableDataExportCheckBox.Checked;
        dataExportBaseFolderLabel.Enabled = isChecked;
        dataExportFolderTextBox.Enabled = isChecked;
        browseDataExportFolderButton.Enabled = isChecked;
    }

    private void useSpecificRevitVersionRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        UpdateRevitFileProcessingControls();
    }

    private void importSettingsButton_Click(object sender, EventArgs e)
    {
        BrowseForFile(
            "Import BatchRvt Settings file",
            scriptFilePath => { LoadSettings(scriptFilePath); },
            BatchRvtSettings.SETTINGS_FILE_EXTENSION,
            BatchRvtSettings.SETTINGS_FILE_FILTER,
            true
        );
    }

    private void exportSettingsButton_Click(object sender, EventArgs e)
    {
        BrowseForSave(
            "Export BatchRvt Settings file",
            scriptFilePath => { SaveSettings(scriptFilePath); },
            BatchRvtSettings.SETTINGS_FILE_EXTENSION,
            BatchRvtSettings.SETTINGS_FILE_FILTER,
            initialFileName: BatchRvtSettings.BATCHRVT_SETTINGS_FILENAME
        );
    }

    private void taskScriptNewScriptButton_Click(object sender, EventArgs e)
    {
        BrowseForSaveScriptFile(
            "Save New Task Script",
            scriptFilePath =>
            {
                var isSaved = SaveNewScript(scriptFilePath, SaveNewScriptType.TaskScript);

                if (isSaved)
                    UpdateTaskScript(scriptFilePath);
                else
                    ShowErrorMessageBox("ERROR: Failed to Save the new script!");
            },
            ScriptType.Python,
            PathUtil.GetExistingFileDirectoryPath(taskScriptTextBox.Text),
            NEW_TASK_SCRIPT_FILENAME
        );
    }

    private void preProcessingScriptNewScriptButton_Click(object sender, EventArgs e)
    {
        BrowseForSaveScriptFile(
            "Save New Pre-Processing Script",
            scriptFilePath =>
            {
                var isSaved = SaveNewScript(scriptFilePath, SaveNewScriptType.PreProcessingScript);

                if (isSaved)
                    preProcessingScriptTextBox.Text = scriptFilePath;
                else
                    ShowErrorMessageBox("ERROR: Failed to Save the new script!");
            },
            ScriptType.Python,
            PathUtil.GetExistingFileDirectoryPath(preProcessingScriptTextBox.Text),
            NEW_PREPROCESSING_SCRIPT_FILENAME
        );
    }

    private void postProcessingScriptNewScriptButton_Click(object sender, EventArgs e)
    {
        BrowseForSaveScriptFile(
            "Save New Post-Processing Script",
            scriptFilePath =>
            {
                var isSaved = SaveNewScript(scriptFilePath, SaveNewScriptType.PostProcessingScript);

                if (isSaved)
                    postProcessingScriptTextBox.Text = scriptFilePath;
                else
                    ShowErrorMessageBox("ERROR: Failed to Save the new script!");
            },
            ScriptType.Python,
            PathUtil.GetExistingFileDirectoryPath(postProcessingScriptTextBox.Text),
            NEW_POSTPROCESSING_SCRIPT_FILENAME
        );
    }

    private void BrowseForSaveScriptFile(
        string dialogTitle,
        Action<string> scriptFileAction,
        ScriptType scriptType,
        string initialDirectory = null,
        string initialFileName = null
    )
    {
        var scriptDefaultExtension =
            scriptType == ScriptType.Dynamo ? DYNAMO_SCRIPT_EXTENSION : PYTHON_SCRIPT_EXTENSION;
        var scriptFilter = scriptType == ScriptType.Dynamo ? DYNAMO_SCRIPT_FILTER : PYTHON_SCRIPT_FILTER;

        BrowseForSave(
            dialogTitle,
            scriptFileAction,
            scriptDefaultExtension,
            scriptFilter,
            initialDirectory,
            initialFileName
        );
    }

    private bool SaveNewScript(string scriptFilePath, SaveNewScriptType saveNewScriptType)
    {
        var success = false;

        string scriptTemplateFileName = null;

        if (saveNewScriptType == SaveNewScriptType.TaskScript)
            scriptTemplateFileName = TEMPLATE_TASK_SCRIPT_FILENAME;
        else if (saveNewScriptType == SaveNewScriptType.PreProcessingScript)
            scriptTemplateFileName = TEMPLATE_PREPROCESSING_SCRIPT_FILENAME;
        else if (saveNewScriptType == SaveNewScriptType.PostProcessingScript)
            scriptTemplateFileName = TEMPLATE_POSTPROCESSING_SCRIPT_FILENAME;

        var scriptTemplateFilePath = Path.Combine(BatchRvt.GetBatchRvtScriptsFolderPath(), scriptTemplateFileName);

        var scriptContents = File.ReadAllText(scriptTemplateFilePath);

        try
        {
            File.WriteAllText(scriptFilePath, scriptContents);
            success = true;
        }
        catch (Exception)
        {
            success = false;
        }

        return success;
    }

    private void showAdvancedSettingsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        UpdateAdvancedSettings();
    }

    private void UpdateAdvancedSettings()
    {
        var advancedSettingsIsChecked = showAdvancedSettingsCheckBox.Checked;
        singleRevitTaskProcessingGroupBox.Visible = advancedSettingsIsChecked;
        dataExportGroupBox.Visible = advancedSettingsIsChecked;
        showMessageBoxOnTaskScriptErrorCheckBox.Visible = advancedSettingsIsChecked;
        preAndPostProcessingGroupBox.Visible = advancedSettingsIsChecked;

        var displaySettingPercentage = GetDisplaySettingPercentage();

        var minimumWindowHeight = isUsingRunningSize ? RUNNING_MINIMUM_HEIGHT : SETUP_HEIGHT;

        MinimumSize = Scale(
            new Size(Scale(SETUP_MINIMUM_WIDTH, displaySettingPercentage),
                advancedSettingsIsChecked
                    ? minimumWindowHeight
                    : minimumWindowHeight - ADVANCED_SETTINGS_VISIBLE_SIZE_DIFFERENCE),
            displaySettingPercentage
        );

        if (isUsingRunningSize) return;
        Size = new Size(Size.Width,
            advancedSettingsIsChecked
                ? SETUP_HEIGHT
                : SETUP_HEIGHT - ADVANCED_SETTINGS_VISIBLE_SIZE_DIFFERENCE);
        MaximumSize = Scale(
            new Size(SETUP_MAXIMUM_WIDTH,
                advancedSettingsIsChecked
                    ? SETUP_HEIGHT
                    : SETUP_HEIGHT - ADVANCED_SETTINGS_VISIBLE_SIZE_DIFFERENCE),
            displaySettingPercentage
        );
    }

    private void timeOutNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        if (!perFileProcessingTimeOutCheckBox.Checked || timeOutNumericUpDown.Value != 0) return;
        perFileProcessingTimeOutCheckBox.Checked = false;
        UpdateRevitSessionControls();
    }

    private void timeOutNumericUpDown_Leave(object sender, EventArgs e)
    {
        // Detect if the numeric time-out value was left blank and set it to 0.
        if (string.IsNullOrWhiteSpace(timeOutNumericUpDown.Controls[1].Text)) timeOutNumericUpDown.Value = 0;
    }

    private void newRevitFileListButton_Click(object sender, EventArgs e)
    {
        var folderBrowserDialog = new FolderBrowserDialog
        {
            Site = null,
            Tag = null,
            ShowNewFolderButton = false,
            SelectedPath = "",
            RootFolder = Environment.SpecialFolder.MyComputer,
            Description = @"Select a folder containing Revit files"
        };


        var dialogResult = folderBrowserDialog.ShowDialog(this);

        if (dialogResult != DialogResult.OK) return;

        var selectedFolderPath = folderBrowserDialog.SelectedPath;

        if (string.IsNullOrWhiteSpace(selectedFolderPath)) return;

        RevitFileScanning.RevitFileType selectedRevitFileType;
        SearchOption selectedSearchOption;
        bool expandNetworkPaths;
        bool extractRevitVersionInfo;
        bool ignoreRevitBackupFiles;
        using (var revitFileScanningOptionsUi = new RevitFileScanningOptionsUI())
        {
            var optionsDialogResult = revitFileScanningOptionsUi.ShowDialog(this);

            if (optionsDialogResult != DialogResult.OK) return;

            selectedRevitFileType = revitFileScanningOptionsUi.GetSelectedRevitFileType();

            var includeSubfolders = revitFileScanningOptionsUi.IncludeSubfolders();

            selectedSearchOption = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            expandNetworkPaths = revitFileScanningOptionsUi.ExpandNetworkPaths();
            extractRevitVersionInfo = revitFileScanningOptionsUi.ExtractRevitVersionInfo();
            ignoreRevitBackupFiles = revitFileScanningOptionsUi.IgnoreRevitBackupFiles();
        }

        var rows = Enumerable.Empty<IEnumerable<string>>();

        void RevitFileScanningProgressReporter(Func<string, bool> progressReporter)
        {
            rows = RevitFileScanning.FindAndExtractRevitFilesInfoWithProgressReporting(
                selectedFolderPath,
                selectedSearchOption,
                selectedRevitFileType,
                expandNetworkPaths,
                extractRevitVersionInfo,
                ignoreRevitBackupFiles,
                progressReporter);
        }

        DialogResult scanningDialogResult;
        using (var revitFileScanningProgressUi = new RevitFileScanningProgressUI(RevitFileScanningProgressReporter))
        {
            scanningDialogResult = revitFileScanningProgressUi.ShowDialog(this);
        }

        if (scanningDialogResult != DialogResult.OK) return;

        var initialDirectory = PathUtil.GetExistingFileDirectoryPath(revitFileListTextBox.Text);

        BrowseForSave(
            "Save New Revit file list",
            revitFileListPath =>
            {
                var isSaved = false;

                try
                {
                    TextFileUtil.WriteToTabDelimitedTxtFile(rows, revitFileListPath);
                    isSaved = true;
                }
                catch (Exception)
                {
                    isSaved = false;
                }

                if (isSaved)
                    revitFileListTextBox.Text = revitFileListPath;
                else
                    ShowErrorMessageBox("ERROR: Failed to Save the new Revit file list!");
            },
            TextFileUtil.TEXT_FILE_EXTENSION,
            TextFileUtil.TEXT_FILE_FILTER,
            initialDirectory,
            "revit_file_list.txt"
        );
    }

    private void discardWorksetsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        UpdateCentralFileProcessingControls();
    }

    private enum ScriptType
    {
        Python = 0,
        Dynamo = 1,
        Any = 2
    }

    private enum SaveNewScriptType
    {
        TaskScript = 0,
        PreProcessingScript = 1,
        PostProcessingScript = 2
    }
}