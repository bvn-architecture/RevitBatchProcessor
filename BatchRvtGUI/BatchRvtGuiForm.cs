//
// Revit Batch Processor
//
// Copyright (c) 2019  Daniel Rumery, BVN
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
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using BatchRvtUtil;

namespace BatchRvtGUI
{
    public partial class BatchRvtGuiForm : Form
    {
        public const string WINDOW_TITLE = "Revit Batch Processor";

        private BatchRvtSettings Settings;
        private UIConfig UIConfiguration;
        private bool isBatchRvtRunning = false;

        private const string NEW_TASK_SCRIPT_FILENAME = "new_task_script.py";
        private const string NEW_PREPROCESSING_SCRIPT_FILENAME = "new_pre_processing_script.py";
        private const string NEW_POSTPROCESSING_SCRIPT_FILENAME = "new_post_processing_script.py";

        private enum ScriptType { Python = 0, Dynamo = 1, Any = 2 }
        private enum SaveNewScriptType { TaskScript = 0, PreProcessingScript = 1, PostProcessingScript = 2 }

        private const string TEMPLATE_TASK_SCRIPT_FILENAME = "template_task_script.py";
        private const string TEMPLATE_PREPROCESSING_SCRIPT_FILENAME = "template_pre_processing_script.py";
        private const string TEMPLATE_POSTPROCESSING_SCRIPT_FILENAME = "template_post_processing_script.py";

        private const string PYTHON_SCRIPT_EXTENSION = ".py";
        private const string PYTHON_SCRIPT_FILTER = "Python files (*.py)|*.py";
        private const string DYNAMO_SCRIPT_EXTENSION = ".dyn";
        private const string DYNAMO_SCRIPT_FILTER = "Dynamo files (*.dyn)|*.dyn";
        private const string ANY_SCRIPTS_FILTER = "Script files (*.py;*.dyn)|*.py;*.dyn";

        private const int SETUP_HEIGHT = 642;
        private const int SETUP_INITIAL_WIDTH = 1024;
        private const int SETUP_MINIMUM_WIDTH = 1024;
        private const int SETUP_MAXIMUM_WIDTH = 1600;

        private readonly System.Drawing.Size SETUP_INITIAL_SIZE = new System.Drawing.Size(SETUP_INITIAL_WIDTH, SETUP_HEIGHT);
        private readonly System.Drawing.Size SETUP_MINIMUM_SIZE = new System.Drawing.Size(SETUP_MINIMUM_WIDTH, SETUP_HEIGHT);
        private readonly System.Drawing.Size SETUP_MAXIMUM_SIZE = new System.Drawing.Size(SETUP_MAXIMUM_WIDTH, SETUP_HEIGHT);

        private const int RUNNING_INITIAL_WIDTH = 1024;
        private const int RUNNING_INITIAL_HEIGHT = 875;
        private const int RUNNING_MINIMUM_HEIGHT = 875;
        private const int RUNNING_MINIMUM_WIDTH = 1024;

        private readonly System.Drawing.Size RUNNING_INITIAL_SIZE = new System.Drawing.Size(RUNNING_INITIAL_WIDTH, RUNNING_INITIAL_HEIGHT);
        private readonly System.Drawing.Size RUNNING_MINIMUM_SIZE = new System.Drawing.Size(RUNNING_MINIMUM_WIDTH, RUNNING_MINIMUM_HEIGHT);
        private readonly System.Drawing.Size RUNNING_MAXIMUM_SIZE = new System.Drawing.Size(0, 0); // no maximum size

        private Process batchRvtProcess;
        private Timer readBatchRvtOutput_Timer;
        private const int READ_OUTPUT_INTERVAL_IN_MS = 250;

        private Task<string> pendingOutputReadLineTask;
        private Task<string> pendingErrorReadLineTask;

        public BatchRvtGuiForm()
        {
            InitializeComponent();
            this.Settings = new BatchRvtSettings();

            this.UIConfiguration = new UIConfig(GetUIConfigItems());
        }

        private IEnumerable<IUIConfigItem> GetUIConfigItems()
        {
            var iuConfigItems = new IUIConfigItem[] {
                    
                    // General Task Script settings
                    new UIConfigItem(
                            () => {
                                this.taskScriptTextBox.Text = this.Settings.TaskScriptFilePath.GetValue();
                                this.showMessageBoxOnTaskScriptErrorCheckBox.Checked = this.Settings.ShowMessageBoxOnTaskScriptError.GetValue();
                            },
                            () => {
                                this.Settings.TaskScriptFilePath.SetValue(this.taskScriptTextBox.Text);
                                this.Settings.ShowMessageBoxOnTaskScriptError.SetValue(this.showMessageBoxOnTaskScriptErrorCheckBox.Checked);
                            }
                        ),

                    // Revit File List settings
                    new UIConfigItem(
                            () => { this.revitFileListTextBox.Text = this.Settings.RevitFileListFilePath.GetValue(); },
                            () => { this.Settings.RevitFileListFilePath.SetValue(this.revitFileListTextBox.Text); }
                        ),

                    // Data Export settings
                    new UIConfigItem(
                            () => {
                                this.enableDataExportCheckBox.Checked = this.Settings.EnableDataExport.GetValue();
                                this.dataExportFolderTextBox.Text = this.Settings.DataExportFolderPath.GetValue();
                                UpdateDataExportControls();
                            },
                            () => {
                                this.Settings.EnableDataExport.SetValue(this.enableDataExportCheckBox.Checked);
                                this.Settings.DataExportFolderPath.SetValue(this.dataExportFolderTextBox.Text);
                            }
                        ),

                    // Pre-processing Script settings
                    new UIConfigItem(
                            () => {
                                this.executePreProcessingScriptCheckBox.Checked = this.Settings.ExecutePreProcessingScript.GetValue();
                                this.preProcessingScriptTextBox.Text = this.Settings.PreProcessingScriptFilePath.GetValue();
                                UpdatePreProcessingScriptControls();
                            },
                            () => {
                                this.Settings.ExecutePreProcessingScript.SetValue(this.executePreProcessingScriptCheckBox.Checked);
                                this.Settings.PreProcessingScriptFilePath.SetValue(this.preProcessingScriptTextBox.Text);
                            }
                        ),

                    // Post-processing Script settings
                    new UIConfigItem(
                            () => {
                                this.executePostProcessingScriptCheckBox.Checked = this.Settings.ExecutePostProcessingScript.GetValue();
                                this.postProcessingScriptTextBox.Text = this.Settings.PostProcessingScriptFilePath.GetValue();
                                UpdatePostProcessingScriptControls();
                            },
                            () => {
                                this.Settings.ExecutePostProcessingScript.SetValue(this.executePostProcessingScriptCheckBox.Checked);
                                this.Settings.PostProcessingScriptFilePath.SetValue(this.postProcessingScriptTextBox.Text);
                            }
                        ),

                    // Central File Processing settings
                    new UIConfigItem(
                            () => {
                                this.detachFromCentralRadioButton.Checked = (this.Settings.CentralFileOpenOption.GetValue() == BatchRvt.CentralFileOpenOption.Detach);
                                this.createNewLocalRadioButton.Checked = (this.Settings.CentralFileOpenOption.GetValue() == BatchRvt.CentralFileOpenOption.CreateNewLocal);
                                this.deleteLocalAfterCheckBox.Checked = this.Settings.DeleteLocalAfter.GetValue();
                                this.discardWorksetsCheckBox.Checked = this.Settings.DiscardWorksetsOnDetach.GetValue();
                                this.closeAllWorksetsRadioButton.Checked = (this.Settings.WorksetConfigurationOption.GetValue() == BatchRvt.WorksetConfigurationOption.CloseAllWorksets);
                                this.openAllWorksetsRadioButton.Checked = (this.Settings.WorksetConfigurationOption.GetValue() == BatchRvt.WorksetConfigurationOption.OpenAllWorksets);
                                this.openLastViewedWorksetsRadioButton.Checked = (this.Settings.WorksetConfigurationOption.GetValue() == BatchRvt.WorksetConfigurationOption.OpenLastViewed);
                                UpdateCentralFileProcessingControls();
                            },
                            () => {
                                this.Settings.CentralFileOpenOption.SetValue(
                                        this.createNewLocalRadioButton.Checked ?
                                        BatchRvt.CentralFileOpenOption.CreateNewLocal :
                                        BatchRvt.CentralFileOpenOption.Detach
                                    );
                                this.Settings.DeleteLocalAfter.SetValue(this.deleteLocalAfterCheckBox.Checked);
                                this.Settings.DiscardWorksetsOnDetach.SetValue(this.discardWorksetsCheckBox.Checked);
                                this.Settings.WorksetConfigurationOption.SetValue(
                                        this.closeAllWorksetsRadioButton.Checked ?
                                        BatchRvt.WorksetConfigurationOption.CloseAllWorksets :
                                        (
                                            this.openAllWorksetsRadioButton.Checked ?
                                            BatchRvt.WorksetConfigurationOption.OpenAllWorksets :
                                            BatchRvt.WorksetConfigurationOption.OpenLastViewed
                                        )
                                    );
                            }
                        ),

                    // Revit Session settings
                    new UIConfigItem(
                            () => {
                                this.useSeparateRevitSessionRadioButton.Checked = (this.Settings.RevitSessionOption.GetValue() == BatchRvt.RevitSessionOption.UseSeparateSessionPerFile);
                                this.useSameRevitSessionRadioButton.Checked = (this.Settings.RevitSessionOption.GetValue() == BatchRvt.RevitSessionOption.UseSameSessionForFilesOfSameVersion);
                                var processingTimeOutInMinutes = this.Settings.ProcessingTimeOutInMinutes.GetValue();
                                if (processingTimeOutInMinutes < 0)
                                {
                                    processingTimeOutInMinutes = 0;
                                }
                                this.perFileProcessingTimeOutCheckBox.Checked = processingTimeOutInMinutes > 0;
                                this.timeOutNumericUpDown.Value = processingTimeOutInMinutes;
                                UpdateRevitSessionControls();
                            },
                            () => {
                                this.Settings.RevitSessionOption.SetValue(
                                        this.useSameRevitSessionRadioButton.Checked ?
                                        BatchRvt.RevitSessionOption.UseSameSessionForFilesOfSameVersion :
                                        BatchRvt.RevitSessionOption.UseSeparateSessionPerFile
                                    );

                                var processingTimeOutInMinutes = (int)this.timeOutNumericUpDown.Value;
                                if (processingTimeOutInMinutes < 0)
                                {
                                    processingTimeOutInMinutes = 0;
                                }

                                this.Settings.ProcessingTimeOutInMinutes.SetValue(this.perFileProcessingTimeOutCheckBox.Checked ? processingTimeOutInMinutes : 0);
                            }
                        ),

                    // Revit Processing settings
                    new UIConfigItem(
                            () => {
                                this.enableBatchProcessingCheckBox.Checked = (this.Settings.RevitProcessingOption.GetValue() == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing);
                                this.enableSingleRevitTaskProcessingCheckBox.Checked = (this.Settings.RevitProcessingOption.GetValue() == BatchRvt.RevitProcessingOption.SingleRevitTaskProcessing);
                                UpdateRevitProcessingControls();
                            },
                            () => {
                                this.Settings.RevitProcessingOption.SetValue(
                                        this.enableSingleRevitTaskProcessingCheckBox.Checked ?
                                        BatchRvt.RevitProcessingOption.SingleRevitTaskProcessing :
                                        BatchRvt.RevitProcessingOption.BatchRevitFileProcessing
                                    );
                            }
                        ),

                    // Single Revit Task Processing settings
                    new UIConfigItem(
                            () => {
                                Populate(
                                        this.singleRevitTaskRevitVersionComboBox,
                                        RevitVersion.GetInstalledRevitVersions().Select(RevitVersion.GetRevitVersionText),
                                        RevitVersion.GetRevitVersionText(this.Settings.SingleRevitTaskRevitVersion.GetValue())
                                    );
                            },
                            () => {
                                this.Settings.SingleRevitTaskRevitVersion.SetValue(
                                        RevitVersion.GetSupportedRevitVersion(this.singleRevitTaskRevitVersionComboBox.SelectedItem as string)
                                    );
                            }
                        ),

                    // Batch Revit File Processing settings
                    new UIConfigItem(
                            () => {
                                this.useFileRevitVersionRadioButton.Checked = (this.Settings.RevitFileProcessingOption.GetValue() == BatchRvt.RevitFileProcessingOption.UseFileRevitVersionIfAvailable);
                                this.useSpecificRevitVersionRadioButton.Checked = (this.Settings.RevitFileProcessingOption.GetValue() == BatchRvt.RevitFileProcessingOption.UseSpecificRevitVersion);
                                this.useMinimumAvailableVersionCheckBox.Checked = this.Settings.IfNotAvailableUseMinimumAvailableRevitVersion.GetValue();
                                this.auditOnOpeningCheckBox.Checked = this.Settings.AuditOnOpening.GetValue();
                                Populate(
                                        this.specificRevitVersionComboBox,
                                        RevitVersion.GetInstalledRevitVersions().Select(RevitVersion.GetRevitVersionText),
                                        RevitVersion.GetRevitVersionText(this.Settings.BatchRevitTaskRevitVersion.GetValue())
                                    );
                                UpdateRevitFileProcessingControls();
                            },
                            () => {
                                this.Settings.RevitFileProcessingOption.SetValue(
                                        this.useSpecificRevitVersionRadioButton.Checked ?
                                        BatchRvt.RevitFileProcessingOption.UseSpecificRevitVersion :
                                        BatchRvt.RevitFileProcessingOption.UseFileRevitVersionIfAvailable
                                    );
                                this.Settings.IfNotAvailableUseMinimumAvailableRevitVersion.SetValue(this.useMinimumAvailableVersionCheckBox.Checked);
                                this.Settings.BatchRevitTaskRevitVersion.SetValue(
                                        RevitVersion.GetSupportedRevitVersion(this.specificRevitVersionComboBox.SelectedItem as string)
                                    );
                                this.Settings.AuditOnOpening.SetValue(this.auditOnOpeningCheckBox.Checked);
                            }
                        ),

                    // Show Advanced setting
                    new UIConfigItem(
                            () => {
                                this.showAdvancedSettingsCheckBox.Checked = this.Settings.ShowAdvancedSettings.GetValue();
                                UpdateAdvancedSettings();
                            },
                            () => {
                                this.Settings.ShowAdvancedSettings.SetValue(this.showAdvancedSettingsCheckBox.Checked);
                            }
                        ),
                };

            return iuConfigItems;
        }

        private double GetDisplaySettingPercentage()
        {
            var graphics = this.CreateGraphics();
            var dpiX = graphics.DpiX;
            return dpiX / 96f;
        }

        private static System.Drawing.Size Scale(System.Drawing.Size size, double scale)
        {
            return new System.Drawing.Size((int)(size.Width * scale), (int)(size.Height * scale));
        }

        private void AdjustWindowSizeForDisplaySetting()
        {
            var displaySettingPercentage = GetDisplaySettingPercentage();

            this.MinimumSize = Scale(this.MinimumSize, displaySettingPercentage);
            this.MaximumSize = Scale(this.MaximumSize, displaySettingPercentage);
            this.Size = Scale(this.Size, displaySettingPercentage);
        }

        private void BatchRvtGuiForm_Load(object sender, EventArgs e)
        {
            this.Text = WINDOW_TITLE;

            this.TopMost = false;
            this.alwaysOnTopCheckbox.Checked = this.TopMost;
            this.batchRvtOutputGroupBox.Visible = false;

            this.MinimumSize = SETUP_MINIMUM_SIZE;
            this.MaximumSize = SETUP_MAXIMUM_SIZE;
            this.Size = SETUP_INITIAL_SIZE;
            this.MaximizeBox = false;

            AdjustWindowSizeForDisplaySetting();

            bool isLoaded = LoadSettings();

            // TODO: show error message if load failed!!
        }

        private bool LoadSettings(string filePath = null)
        {
            var newBatchRvtSettings = new BatchRvtSettings();

            bool isLoaded = newBatchRvtSettings.LoadFromFile(filePath);

            if (isLoaded)
            {
                this.Settings = newBatchRvtSettings;
            }

            this.UIConfiguration.UpdateUI();

            VerifyExcelInstallation(this.revitFileListTextBox.Text);

            return isLoaded;
        }

        private void VerifyExcelInstallation(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (ExcelUtil.HasExcelExtension(filePath) && !ExcelUtil.IsExcelInstalled())
                {
                    MessageBox.Show(
                            "WARNING: An Excel installation was not detected! Support for Excel files requires an Excel installation.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                }
            }
        }

        private bool SaveSettings(string filePath = null)
        {
            this.UIConfiguration.UpdateConfig();

            bool isSaved = this.Settings.SaveToFile(filePath);

            return isSaved;
        }

        private void BatchRvtGuiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.isBatchRvtRunning)
            {
                var message = new StringBuilder();

                message.AppendLine("Do you want to terminate the currently running task?");

                var dialogResult = MessageBox.Show(
                        message.ToString(),
                        this.Text,
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Asterisk,
                        MessageBoxDefaultButton.Button3
                    );

                if (dialogResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        this.batchRvtProcess.Kill();
                    }
                    catch (Exception ex)
                    {
                        // TODO: report failure to kill the process?
                    }
                }
            }

            if (!e.Cancel)
            {
                var message = new StringBuilder();

                message.AppendLine("Do you want to save the current settings as default?");

                var dialogResult = MessageBox.Show(
                        message.ToString(),
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    );

                if (dialogResult == DialogResult.Yes)
                {
                    bool isSaved = SaveSettings();

                    // TODO: show error message if save failed!!
                }

                if (readBatchRvtOutput_Timer != null)
                {
                    this.readBatchRvtOutput_Timer.Stop();
                    this.readBatchRvtOutput_Timer.Dispose();
                }
            }
        }

        private void alwaysOnTopCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = this.alwaysOnTopCheckbox.Checked;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public static void ShowErrorMessageBox(string errorMessage)
        {
            MessageBox.Show(errorMessage, BatchRvtGuiForm.WINDOW_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool ValidateConfig()
        {
            bool validated = false;

            if (!this.enableSingleRevitTaskProcessingCheckBox.Checked && !this.enableBatchProcessingCheckBox.Checked)
            {
                ShowErrorMessageBox("ERROR: You must select either Batch Revit File Processing or Single Revit Task Processing!");
            }
            else if (!File.Exists(this.Settings.TaskScriptFilePath.GetValue()))
            {
                ShowErrorMessageBox("ERROR: You must select an existing Task Python script!");
            }
            else if (
                    (this.Settings.RevitProcessingOption.GetValue() == BatchRvt.RevitProcessingOption.BatchRevitFileProcessing)
                    &&
                    !File.Exists(this.Settings.RevitFileListFilePath.GetValue())
                )
            {
                ShowErrorMessageBox("ERROR: You must select an existing Revit File List!");
            }
            else if (
                    this.Settings.EnableDataExport.GetValue()
                    &&
                    !Directory.Exists(this.Settings.DataExportFolderPath.GetValue())
                )
            {
                ShowErrorMessageBox("ERROR: You must select an existing Data Export folder!");
            }
            else if (
                    this.Settings.ExecutePreProcessingScript.GetValue()
                    &&
                    !File.Exists(this.Settings.PreProcessingScriptFilePath.GetValue())
                )
            {
                ShowErrorMessageBox("ERROR: You must select an existing Pre-Processing Python script!");
            }
            else if (
                    this.Settings.ExecutePostProcessingScript.GetValue()
                    &&
                    !File.Exists(this.Settings.PostProcessingScriptFilePath.GetValue())
                )
            {
                ShowErrorMessageBox("ERROR: You must select an existing Post-Processing Python script!");
            }
            else
            {
                validated = true;
            }

            return validated;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            this.UIConfiguration.UpdateConfig();

            bool validated = ValidateConfig();
            
            if (validated)
            {
                bool isSaved = SaveSettings();

                // TODO: show error message if save failed!!

                var settingsFilePath = BatchRvtSettings.GetDefaultSettingsFilePath();
                
                this.batchRvtProcess = BatchRvt.StartBatchRvt(settingsFilePath);

                this.readBatchRvtOutput_Timer = new Timer() { Interval = READ_OUTPUT_INTERVAL_IN_MS };
                this.readBatchRvtOutput_Timer.Tick += readBatchRvtOutput_Timer_Tick;

                this.isBatchRvtRunning = true;
                this.settingsGroupBox.Enabled = false;
                this.importSettingsButton.Enabled = false;
                this.startButton.Enabled = false;
                this.startButton.Text = "Running...";
                this.batchRvtOutputGroupBox.Visible = true;
                this.MinimumSize = RUNNING_MINIMUM_SIZE;
                this.MaximumSize = RUNNING_MAXIMUM_SIZE;
                this.Size = RUNNING_INITIAL_SIZE;
                this.MaximizeBox = true;

                UpdateAdvancedSettings();

                AdjustWindowSizeForDisplaySetting();
                
                readBatchRvtOutput_Timer.Start();
            }
        }

        private void readBatchRvtOutput_Timer_Tick(object sender, EventArgs e)
        {
            var linesAndPendingTask = StreamIOUtil.ReadAvailableLines(this.batchRvtProcess.StandardOutput, this.pendingOutputReadLineTask);
            this.pendingOutputReadLineTask = linesAndPendingTask.Item2;
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
                {
                    this.batchRvtOutputTextBox.AppendText(fullLine);
                }
            }

            linesAndPendingTask = StreamIOUtil.ReadAvailableLines(this.batchRvtProcess.StandardError, this.pendingErrorReadLineTask);
            this.pendingErrorReadLineTask = linesAndPendingTask.Item2;
            lines = linesAndPendingTask.Item1;

            foreach (var line in lines)
            {
                if (line.StartsWith("log4cplus:")) // ignore pesky log4cplus messages (an Autodesk thing?)
                {
                    continue;
                }

                this.batchRvtOutputTextBox.AppendText("[ REVIT ERROR MESSAGE ] : " + line + Environment.NewLine);
            }

            if (isBatchRvtRunning)
            {
                this.batchRvtProcess.Refresh();
                if (this.batchRvtProcess.HasExited)
                {
                    this.isBatchRvtRunning = false;
                    this.startButton.Text = "Done!";
                }
            }
        }

        private void browseScriptButton_Click(object sender, EventArgs e)
        {
            BrowseForExistingScriptFile(
                    "Select Task Python script",
                    scriptFilePath => { this.taskScriptTextBox.Text = scriptFilePath; },
                    ScriptType.Any,
                    PathUtil.GetExistingFileDirectoryPath(this.taskScriptTextBox.Text)
                );
        }

        private void BrowseForSave(string dialogTitle, Action<string> fileAction, string defaultExt, string filter, string initialDirectory = null, string initialFileName = null)
        {
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.DefaultExt = defaultExt;
            saveFileDialog.Filter = filter;
            saveFileDialog.Title = dialogTitle;

            if (!string.IsNullOrWhiteSpace(initialDirectory))
            {
                saveFileDialog.InitialDirectory = initialDirectory;
            }

            if (!string.IsNullOrWhiteSpace(initialFileName))
            {
                saveFileDialog.FileName = initialFileName;
            }

            var dialogResult = saveFileDialog.ShowDialog(this);

            if (dialogResult == DialogResult.OK)
            {
                var selectedFilePath = saveFileDialog.FileName;

                if (!string.IsNullOrWhiteSpace(selectedFilePath))
                {
                    fileAction(selectedFilePath);
                }
            }

            return;
        }

        private void BrowseForFile(string dialogTitle, Action<string> fileAction, string defaultExt, string filter, bool checkFileExists, string initialDirectory = null)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = defaultExt;
            openFileDialog.Filter = filter;
            openFileDialog.CheckFileExists = checkFileExists;
            openFileDialog.ReadOnlyChecked = true;
            openFileDialog.Multiselect = false;
            openFileDialog.Title = dialogTitle;

            if (!string.IsNullOrWhiteSpace(initialDirectory))
            {
                openFileDialog.InitialDirectory = initialDirectory;
            }

            var dialogResult = openFileDialog.ShowDialog(this);

            if (dialogResult == DialogResult.OK)
            {
                var selectedFilePath = openFileDialog.FileName;

                if (!string.IsNullOrWhiteSpace(selectedFilePath))
                {
                    fileAction(selectedFilePath);
                }
            }

            return;
        }

        private void BrowseForExistingScriptFile(
                string dialogTitle,
                Action<string> scriptFileAction,
                ScriptType scriptType,
                string initialDirectory = null
            )
        {
            var scriptDefaultExtension = (scriptType == ScriptType.Dynamo) ? DYNAMO_SCRIPT_EXTENSION : PYTHON_SCRIPT_EXTENSION;
            var scriptFilter = (
                    (scriptType == ScriptType.Dynamo) ? DYNAMO_SCRIPT_FILTER :
                    (scriptType == ScriptType.Python) ? PYTHON_SCRIPT_FILTER :
                    ANY_SCRIPTS_FILTER
                );

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

            string initialDirectory = PathUtil.GetExistingFileDirectoryPath(this.revitFileListTextBox.Text);

            if (initialDirectory != null)
            {
                openFileDialog.InitialDirectory = initialDirectory;
            }

            var dialogResult = openFileDialog.ShowDialog(this);

            if (dialogResult == DialogResult.OK)
            {
                var selectedFilePath = openFileDialog.FileName;

                if (!string.IsNullOrWhiteSpace(selectedFilePath))
                {
                    this.revitFileListTextBox.Text = selectedFilePath;

                    VerifyExcelInstallation(selectedFilePath);
                }
            }
        }

        private void browseDataExportFolderButton_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            folderBrowserDialog.Description = "Select Data Export folder";

            var currentFolderPath = this.dataExportFolderTextBox.Text;

            if (Directory.Exists(currentFolderPath))
            {
                folderBrowserDialog.SelectedPath = currentFolderPath;
            }

            var dialogResult = folderBrowserDialog.ShowDialog(this);

            if (dialogResult == DialogResult.OK)
            {
                var selectedFolderPath = folderBrowserDialog.SelectedPath;

                if (!string.IsNullOrWhiteSpace(selectedFolderPath))
                {
                    this.dataExportFolderTextBox.Text = selectedFolderPath;
                }
            }
        }

        private void batchRvtOutputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.A))
            {
                this.batchRvtOutputTextBox.SelectAll();
                e.SuppressKeyPress = true;
            }
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
                    scriptFilePath => { this.preProcessingScriptTextBox.Text = scriptFilePath; },
                    ScriptType.Python,
                    PathUtil.GetExistingFileDirectoryPath(this.preProcessingScriptTextBox.Text)
                );
        }

        private void postProcessingScriptBrowseButton_Click(object sender, EventArgs e)
        {
            BrowseForExistingScriptFile(
                    "Select Post-Processing Python script",
                    scriptFilePath => { this.postProcessingScriptTextBox.Text = scriptFilePath; },
                    ScriptType.Python,
                    PathUtil.GetExistingFileDirectoryPath(this.postProcessingScriptTextBox.Text)
                );
        }

        private void enableBatchProcessingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.enableBatchProcessingCheckBox.Checked)
            {
                this.enableSingleRevitTaskProcessingCheckBox.Checked = false;
            }

            UpdateRevitProcessingControls();
        }

        private void enableSingleRevitTaskProcessingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.enableSingleRevitTaskProcessingCheckBox.Checked)
            {
                this.enableBatchProcessingCheckBox.Checked = false;
            }

            UpdateRevitProcessingControls();
        }

        private void UpdateRevitProcessingControls()
        {
            bool batchTaskEnabled = this.enableBatchProcessingCheckBox.Checked;
            this.revitFileListLabel.Enabled = batchTaskEnabled;
            this.revitFileListTextBox.Enabled = batchTaskEnabled;
            this.browseRevitFileListButton.Enabled = batchTaskEnabled;

            this.centralFileProcessingGroupBox.Enabled = batchTaskEnabled;
            this.revitFileProcessingGroupBox.Enabled = batchTaskEnabled;
            this.revitSessionGroupBox.Enabled = batchTaskEnabled;

            bool singleTaskEnabled = this.enableSingleRevitTaskProcessingCheckBox.Checked;
            this.singleRevitTaskRevitVersionLabel.Enabled = singleTaskEnabled;
            this.singleRevitTaskRevitVersionComboBox.Enabled = singleTaskEnabled;
        }

        private void perFileProcessingTimeOutCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRevitSessionControls();
        }

        private void UpdateRevitSessionControls()
        {
            bool perFileProcessingTimeOutEnabled = this.perFileProcessingTimeOutCheckBox.Checked;

            if (perFileProcessingTimeOutEnabled)
            {
                // If time-out option is enabled but the current numeric value is 0, set a sensible default / initial value for the time-out.
                if (this.timeOutNumericUpDown.Value == 0)
                {
                    this.timeOutNumericUpDown.Value = 15;
                }
            }
            this.timeOutNumericUpDown.Enabled = perFileProcessingTimeOutEnabled;
        }

        private void UpdatePreProcessingScriptControls()
        {
            var isChecked = this.executePreProcessingScriptCheckBox.Checked;
            this.preProcessingScriptTextBox.Enabled = isChecked;
            this.preProcessingScriptBrowseButton.Enabled = isChecked;
            this.preProcessingScriptNewScriptButton.Enabled = isChecked;
        }

        private void UpdatePostProcessingScriptControls()
        {
            var isChecked = this.executePostProcessingScriptCheckBox.Checked;
            this.postProcessingScriptTextBox.Enabled = isChecked;
            this.postProcessingScriptBrowseButton.Enabled = isChecked;
            this.postProcessingScriptNewScriptButton.Enabled = isChecked;
        }

        private static void Populate<T>(ComboBox comboBox, IEnumerable<T> items, T selectedItem)
        {
            var itemsList = items.ToList();

            comboBox.Items.Clear();

            foreach (var item in itemsList)
            {
                comboBox.Items.Add(item);
            }

            int selectedIndex = itemsList.IndexOf(selectedItem);

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
            this.deleteLocalAfterCheckBox.Enabled = this.createNewLocalRadioButton.Checked;
            this.discardWorksetsCheckBox.Enabled = this.detachFromCentralRadioButton.Checked;
            this.worksetConfigurationGroupBox.Enabled = !(this.detachFromCentralRadioButton.Checked && this.discardWorksetsCheckBox.Checked);
        }

        private void useFileRevitVersionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRevitFileProcessingControls();
        }

        private void UpdateRevitFileProcessingControls()
        {
            this.useMinimumAvailableVersionCheckBox.Enabled = this.useFileRevitVersionRadioButton.Checked;
            this.specificRevitVersionComboBox.Enabled = this.useSpecificRevitVersionRadioButton.Checked;
        }

        private void enableDataExportCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDataExportControls();
        }

        private void UpdateDataExportControls()
        {
            var isChecked = this.enableDataExportCheckBox.Checked;
            this.dataExportBaseFolderLabel.Enabled = isChecked;
            this.dataExportFolderTextBox.Enabled = isChecked;
            this.browseDataExportFolderButton.Enabled = isChecked;
        }

        private void useSpecificRevitVersionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRevitFileProcessingControls();
        }

        private void importSettingsButton_Click(object sender, EventArgs e)
        {
            BrowseForFile(
                "Import BatchRvt Settings file",
                scriptFilePath => { this.LoadSettings(scriptFilePath); },
                BatchRvtSettings.SETTINGS_FILE_EXTENSION,
                BatchRvtSettings.SETTINGS_FILE_FILTER,
                true
            );
        }

        private void exportSettingsButton_Click(object sender, EventArgs e)
        {
            BrowseForSave(
                "Export BatchRvt Settings file",
                scriptFilePath => { this.SaveSettings(scriptFilePath); },
                BatchRvtSettings.SETTINGS_FILE_EXTENSION,
                BatchRvtSettings.SETTINGS_FILE_FILTER,
                initialFileName: BatchRvtSettings.BATCHRVT_SETTINGS_FILENAME
            );
        }

        private void taskScriptNewScriptButton_Click(object sender, EventArgs e)
        {
            BrowseForSaveScriptFile(
                    "Save New Task Script",
                    scriptFilePath => {
                        bool isSaved = this.SaveNewScript(scriptFilePath, SaveNewScriptType.TaskScript );

                        if (isSaved)
                        {
                            this.taskScriptTextBox.Text = scriptFilePath;
                        }
                        else
                        {
                            ShowErrorMessageBox("ERROR: Failed to Save the new script!");
                        }
                    },
                    ScriptType.Python,
                    PathUtil.GetExistingFileDirectoryPath(this.taskScriptTextBox.Text),
                    NEW_TASK_SCRIPT_FILENAME
                );
        }

        private void preProcessingScriptNewScriptButton_Click(object sender, EventArgs e)
        {
            BrowseForSaveScriptFile(
                    "Save New Pre-Processing Script",
                    scriptFilePath => {
                        bool isSaved = this.SaveNewScript(scriptFilePath, SaveNewScriptType.PreProcessingScript );

                        if (isSaved)
                        {
                            this.preProcessingScriptTextBox.Text = scriptFilePath;
                        }
                        else
                        {
                            ShowErrorMessageBox("ERROR: Failed to Save the new script!");
                        }
                    },
                    ScriptType.Python,
                    PathUtil.GetExistingFileDirectoryPath(this.preProcessingScriptTextBox.Text),
                    NEW_PREPROCESSING_SCRIPT_FILENAME
                );
        }

        private void postProcessingScriptNewScriptButton_Click(object sender, EventArgs e)
        {
            BrowseForSaveScriptFile(
                    "Save New Post-Processing Script",
                    scriptFilePath => {
                        bool isSaved = this.SaveNewScript(scriptFilePath, SaveNewScriptType.PostProcessingScript);

                        if (isSaved)
                        {
                            this.postProcessingScriptTextBox.Text = scriptFilePath;
                        }
                        else
                        {
                            ShowErrorMessageBox("ERROR: Failed to Save the new script!");
                        }
                    },
                    ScriptType.Python,
                    PathUtil.GetExistingFileDirectoryPath(this.postProcessingScriptTextBox.Text),
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
            var scriptDefaultExtension = (scriptType == ScriptType.Dynamo) ? DYNAMO_SCRIPT_EXTENSION : PYTHON_SCRIPT_EXTENSION;
            var scriptFilter = (scriptType == ScriptType.Dynamo) ? DYNAMO_SCRIPT_FILTER : PYTHON_SCRIPT_FILTER;

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
            bool success = false;

            string scriptTemplateFileName = null;

            if (saveNewScriptType == SaveNewScriptType.TaskScript)
            {
                scriptTemplateFileName = TEMPLATE_TASK_SCRIPT_FILENAME;
            }
            else if (saveNewScriptType == SaveNewScriptType.PreProcessingScript)
            {
                scriptTemplateFileName = TEMPLATE_PREPROCESSING_SCRIPT_FILENAME;
            }
            else if (saveNewScriptType == SaveNewScriptType.PostProcessingScript)
            {
                scriptTemplateFileName = TEMPLATE_POSTPROCESSING_SCRIPT_FILENAME;
            }

            var scriptTemplateFilePath = Path.Combine(BatchRvt.GetBatchRvtScriptsFolderPath(), scriptTemplateFileName);

            var scriptContents = File.ReadAllText(scriptTemplateFilePath);

            try
            {
                File.WriteAllText(scriptFilePath, scriptContents);
                success = true;
            }
            catch (Exception e)
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
            var isChecked = this.showAdvancedSettingsCheckBox.Checked;
            this.singleRevitTaskProcessingGroupBox.Visible = isChecked;
            this.dataExportGroupBox.Visible = isChecked;
            this.showMessageBoxOnTaskScriptErrorCheckBox.Visible = isChecked;
            this.preAndPostProcessingGroupBox.Visible = isChecked;

            // TODO: this stuff is messy... refactor it !! Perhaps use FlowLayouyPanel or TableLayoutPanel to better manage the layout changes.

            this.taskScriptGroupBox.Size = new System.Drawing.Size(this.taskScriptGroupBox.Size.Width, isChecked ? 76 : 76 - 24);
            this.batchRevitFileProcessingGroupBox.Location = new System.Drawing.Point(this.batchRevitFileProcessingGroupBox.Location.X, isChecked ? 180 : 180 - 103);

            int advancedSettingsHiddenSizeReductionAmount = 273;
            this.settingsGroupBox.Size = new System.Drawing.Size(this.settingsGroupBox.Size.Width, isChecked ? 553 : 553 - advancedSettingsHiddenSizeReductionAmount);
            // Lower area buttons.
            int advancedSettingsLowerAreaButtonYLocation = 571;
            this.importSettingsButton.Location = new System.Drawing.Point(this.importSettingsButton.Location.X, isChecked ? advancedSettingsLowerAreaButtonYLocation : advancedSettingsLowerAreaButtonYLocation - advancedSettingsHiddenSizeReductionAmount);
            this.exportSettingsButton.Location = new System.Drawing.Point(this.exportSettingsButton.Location.X, isChecked ? advancedSettingsLowerAreaButtonYLocation : advancedSettingsLowerAreaButtonYLocation - advancedSettingsHiddenSizeReductionAmount);
            this.exportSettingsButton.Location = new System.Drawing.Point(this.exportSettingsButton.Location.X, isChecked ? advancedSettingsLowerAreaButtonYLocation : advancedSettingsLowerAreaButtonYLocation - advancedSettingsHiddenSizeReductionAmount);
            this.startButton.Location = new System.Drawing.Point(this.startButton.Location.X, isChecked ? advancedSettingsLowerAreaButtonYLocation : advancedSettingsLowerAreaButtonYLocation - advancedSettingsHiddenSizeReductionAmount);
            this.closeButton.Location = new System.Drawing.Point(this.closeButton.Location.X, isChecked ? advancedSettingsLowerAreaButtonYLocation : advancedSettingsLowerAreaButtonYLocation - advancedSettingsHiddenSizeReductionAmount);

            // Lower area checkboxes.
            int advancedSettingsLowerAreaCheckBoxesYLocation = 575;
            this.showAdvancedSettingsCheckBox.Location = new System.Drawing.Point(this.showAdvancedSettingsCheckBox.Location.X, isChecked ? advancedSettingsLowerAreaCheckBoxesYLocation : advancedSettingsLowerAreaCheckBoxesYLocation - advancedSettingsHiddenSizeReductionAmount);
            this.alwaysOnTopCheckbox.Location = new System.Drawing.Point(this.alwaysOnTopCheckbox.Location.X, isChecked ? advancedSettingsLowerAreaCheckBoxesYLocation : advancedSettingsLowerAreaCheckBoxesYLocation - advancedSettingsHiddenSizeReductionAmount);

            int minimumWindowHeight = this.isBatchRvtRunning ? RUNNING_MINIMUM_HEIGHT : SETUP_HEIGHT;
            this.MinimumSize = new System.Drawing.Size(SETUP_MINIMUM_WIDTH, isChecked ? minimumWindowHeight : minimumWindowHeight - advancedSettingsHiddenSizeReductionAmount);

            if (!this.isBatchRvtRunning)
            {
                this.Size = new System.Drawing.Size(this.Size.Width, isChecked ? SETUP_HEIGHT : SETUP_HEIGHT - advancedSettingsHiddenSizeReductionAmount);
                this.MaximumSize = new System.Drawing.Size(SETUP_MAXIMUM_WIDTH, isChecked ? SETUP_HEIGHT : SETUP_HEIGHT - advancedSettingsHiddenSizeReductionAmount);
            }

            this.batchRvtOutputGroupBox.Location = new System.Drawing.Point(12, isChecked ? 600 : 600 - advancedSettingsHiddenSizeReductionAmount);
            this.batchRvtOutputGroupBox.Size = new System.Drawing.Size(this.batchRvtOutputGroupBox.Size.Width, this.ClientSize.Height - 12 - this.batchRvtOutputGroupBox.Location.Y);
        }

        private void timeOutNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (this.perFileProcessingTimeOutCheckBox.Checked && this.timeOutNumericUpDown.Value == 0)
            {
                this.perFileProcessingTimeOutCheckBox.Checked = false;
                UpdateRevitSessionControls();
            }
        }

        private void timeOutNumericUpDown_Leave(object sender, EventArgs e)
        {
            // Detect if the numeric time-out value was left blank and set it to 0.
            if (string.IsNullOrWhiteSpace(this.timeOutNumericUpDown.Controls[1].Text))
            {
                this.timeOutNumericUpDown.Value = 0;
            }
        }

        private void newRevitFileListButton_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            folderBrowserDialog.Description = "Select a folder containing Revit files";

            // TODO: remember last folder path and assign it to folderBrowserDialog.SelectedPath

            var dialogResult = folderBrowserDialog.ShowDialog(this);

            if (dialogResult == DialogResult.OK)
            {
                var selectedFolderPath = folderBrowserDialog.SelectedPath;

                if (!string.IsNullOrWhiteSpace(selectedFolderPath))
                {
                    var revitFileScanningOptionsUI = new RevitFileScanningOptionsUI();

                    var optionsDialogResult = revitFileScanningOptionsUI.ShowDialog(this);

                    if (optionsDialogResult == DialogResult.OK)
                    {
                        var selectedRevitFileType = revitFileScanningOptionsUI.GetSelectedRevitFileType();

                        bool includeSubfolders = revitFileScanningOptionsUI.IncludeSubfolders();

                        var selectedSearchOption = includeSubfolders ?
                            SearchOption.AllDirectories :
                            SearchOption.TopDirectoryOnly;

                        bool expandNetworkPaths = revitFileScanningOptionsUI.ExpandNetworkPaths();
                        bool extractRevitVersionInfo = revitFileScanningOptionsUI.ExtractRevitVersionInfo();

                        var rows = Enumerable.Empty<IEnumerable<string>>();

                        Action<Func<string, bool>> revitFileScanningProgressReporter =
                            (progressReporter) => {
                                rows = RevitFileScanning.FindAndExtractRevitFilesInfoWithProgressReporting(
                                    selectedFolderPath,
                                    selectedSearchOption,
                                    selectedRevitFileType,
                                    expandNetworkPaths,
                                    extractRevitVersionInfo,
                                    progressReporter
                                );
                            };

                        var revitFileScanningProgressUI = new RevitFileScanningProgressUI(revitFileScanningProgressReporter);

                        var scanningDialogResult = revitFileScanningProgressUI.ShowDialog(this);

                        if (scanningDialogResult == DialogResult.OK)
                        {
                            var initialDirectory = PathUtil.GetExistingFileDirectoryPath(this.revitFileListTextBox.Text);

                            BrowseForSave(
                                    "Save New Revit file list",
                                    revitFileListPath => {

                                        bool isSaved = false;

                                        try
                                        {
                                            TextFileUtil.WriteToTabDelimitedTxtFile(rows, revitFileListPath);
                                            isSaved = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            isSaved = false;
                                        }

                                        if (isSaved)
                                        {
                                            this.revitFileListTextBox.Text = revitFileListPath;
                                        }
                                        else
                                        {
                                            ShowErrorMessageBox("ERROR: Failed to Save the new Revit file list!");
                                        }
                                    },
                                    TextFileUtil.TEXT_FILE_EXTENSION,
                                    TextFileUtil.TEXT_FILE_FILTER,
                                    initialDirectory,
                                    initialFileName: "revit_file_list.txt"
                                );
                        }
                    }
                }
            }
        }

        private void discardWorksetsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCentralFileProcessingControls();
        }
    }
}
