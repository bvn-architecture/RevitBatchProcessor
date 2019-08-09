namespace BatchRvtGUI
{
    partial class BatchRvtGuiForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchRvtGuiForm));
            this.revitFileListTextBox = new System.Windows.Forms.TextBox();
            this.dataExportFolderTextBox = new System.Windows.Forms.TextBox();
            this.browseRevitFileListButton = new System.Windows.Forms.Button();
            this.browseDataExportFolderButton = new System.Windows.Forms.Button();
            this.revitFileListLabel = new System.Windows.Forms.Label();
            this.settingsGroupBox = new System.Windows.Forms.GroupBox();
            this.settingsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.taskScriptGroupBox = new System.Windows.Forms.GroupBox();
            this.showMessageBoxOnTaskScriptErrorCheckBox = new System.Windows.Forms.CheckBox();
            this.taskScriptLabel = new System.Windows.Forms.Label();
            this.taskScriptTextBox = new System.Windows.Forms.TextBox();
            this.taskScriptNewScriptButton = new System.Windows.Forms.Button();
            this.browseScriptButton = new System.Windows.Forms.Button();
            this.dataExportGroupBox = new System.Windows.Forms.GroupBox();
            this.dataExportBaseFolderLabel = new System.Windows.Forms.Label();
            this.enableDataExportCheckBox = new System.Windows.Forms.CheckBox();
            this.singleRevitTaskProcessingGroupBox = new System.Windows.Forms.GroupBox();
            this.singleRevitTaskRevitVersionComboBox = new System.Windows.Forms.ComboBox();
            this.enableSingleRevitTaskProcessingCheckBox = new System.Windows.Forms.CheckBox();
            this.singleRevitTaskRevitVersionLabel = new System.Windows.Forms.Label();
            this.preAndPostProcessingGroupBox = new System.Windows.Forms.GroupBox();
            this.executePostProcessingScriptCheckBox = new System.Windows.Forms.CheckBox();
            this.executePreProcessingScriptCheckBox = new System.Windows.Forms.CheckBox();
            this.postProcessingScriptTextBox = new System.Windows.Forms.TextBox();
            this.postProcessingScriptNewScriptButton = new System.Windows.Forms.Button();
            this.preProcessingScriptNewScriptButton = new System.Windows.Forms.Button();
            this.postProcessingScriptBrowseButton = new System.Windows.Forms.Button();
            this.preProcessingScriptTextBox = new System.Windows.Forms.TextBox();
            this.preProcessingScriptBrowseButton = new System.Windows.Forms.Button();
            this.batchRevitFileProcessingGroupBox = new System.Windows.Forms.GroupBox();
            this.revitSessionGroupBox = new System.Windows.Forms.GroupBox();
            this.timeOutNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.perFileProcessingTimeOutCheckBox = new System.Windows.Forms.CheckBox();
            this.useSameRevitSessionRadioButton = new System.Windows.Forms.RadioButton();
            this.useSeparateRevitSessionRadioButton = new System.Windows.Forms.RadioButton();
            this.enableBatchProcessingCheckBox = new System.Windows.Forms.CheckBox();
            this.newRevitFileListButton = new System.Windows.Forms.Button();
            this.centralFileProcessingGroupBox = new System.Windows.Forms.GroupBox();
            this.worksetConfigurationGroupBox = new System.Windows.Forms.GroupBox();
            this.openLastViewedWorksetsRadioButton = new System.Windows.Forms.RadioButton();
            this.openAllWorksetsRadioButton = new System.Windows.Forms.RadioButton();
            this.closeAllWorksetsRadioButton = new System.Windows.Forms.RadioButton();
            this.discardWorksetsCheckBox = new System.Windows.Forms.CheckBox();
            this.deleteLocalAfterCheckBox = new System.Windows.Forms.CheckBox();
            this.detachFromCentralRadioButton = new System.Windows.Forms.RadioButton();
            this.createNewLocalRadioButton = new System.Windows.Forms.RadioButton();
            this.revitFileProcessingGroupBox = new System.Windows.Forms.GroupBox();
            this.auditOnOpeningCheckBox = new System.Windows.Forms.CheckBox();
            this.useMinimumAvailableVersionCheckBox = new System.Windows.Forms.CheckBox();
            this.useSpecificRevitVersionRadioButton = new System.Windows.Forms.RadioButton();
            this.specificRevitVersionComboBox = new System.Windows.Forms.ComboBox();
            this.useFileRevitVersionRadioButton = new System.Windows.Forms.RadioButton();
            this.alwaysOnTopCheckbox = new System.Windows.Forms.CheckBox();
            this.startButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.batchRvtOutputGroupBox = new System.Windows.Forms.GroupBox();
            this.batchRvtOutputTextBox = new System.Windows.Forms.TextBox();
            this.importSettingsButton = new System.Windows.Forms.Button();
            this.exportSettingsButton = new System.Windows.Forms.Button();
            this.showAdvancedSettingsCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.otherControlsTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.settingsGroupBox.SuspendLayout();
            this.settingsTableLayoutPanel.SuspendLayout();
            this.taskScriptGroupBox.SuspendLayout();
            this.dataExportGroupBox.SuspendLayout();
            this.singleRevitTaskProcessingGroupBox.SuspendLayout();
            this.preAndPostProcessingGroupBox.SuspendLayout();
            this.batchRevitFileProcessingGroupBox.SuspendLayout();
            this.revitSessionGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeOutNumericUpDown)).BeginInit();
            this.centralFileProcessingGroupBox.SuspendLayout();
            this.worksetConfigurationGroupBox.SuspendLayout();
            this.revitFileProcessingGroupBox.SuspendLayout();
            this.batchRvtOutputGroupBox.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.otherControlsTableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // revitFileListTextBox
            // 
            this.revitFileListTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.revitFileListTextBox.Location = new System.Drawing.Point(85, 42);
            this.revitFileListTextBox.Name = "revitFileListTextBox";
            this.revitFileListTextBox.ReadOnly = true;
            this.revitFileListTextBox.Size = new System.Drawing.Size(949, 20);
            this.revitFileListTextBox.TabIndex = 2;
            // 
            // dataExportFolderTextBox
            // 
            this.dataExportFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataExportFolderTextBox.Location = new System.Drawing.Point(137, 45);
            this.dataExportFolderTextBox.Name = "dataExportFolderTextBox";
            this.dataExportFolderTextBox.ReadOnly = true;
            this.dataExportFolderTextBox.Size = new System.Drawing.Size(997, 20);
            this.dataExportFolderTextBox.TabIndex = 2;
            // 
            // browseRevitFileListButton
            // 
            this.browseRevitFileListButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseRevitFileListButton.Location = new System.Drawing.Point(1040, 40);
            this.browseRevitFileListButton.Name = "browseRevitFileListButton";
            this.browseRevitFileListButton.Size = new System.Drawing.Size(75, 23);
            this.browseRevitFileListButton.TabIndex = 3;
            this.browseRevitFileListButton.Text = "Browse ...";
            this.browseRevitFileListButton.UseVisualStyleBackColor = true;
            this.browseRevitFileListButton.Click += new System.EventHandler(this.browseRevitFileListButton_Click);
            // 
            // browseDataExportFolderButton
            // 
            this.browseDataExportFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseDataExportFolderButton.Location = new System.Drawing.Point(1140, 43);
            this.browseDataExportFolderButton.Name = "browseDataExportFolderButton";
            this.browseDataExportFolderButton.Size = new System.Drawing.Size(75, 23);
            this.browseDataExportFolderButton.TabIndex = 3;
            this.browseDataExportFolderButton.Text = "Browse ...";
            this.browseDataExportFolderButton.UseVisualStyleBackColor = true;
            this.browseDataExportFolderButton.Click += new System.EventHandler(this.browseDataExportFolderButton_Click);
            // 
            // revitFileListLabel
            // 
            this.revitFileListLabel.AutoSize = true;
            this.revitFileListLabel.Location = new System.Drawing.Point(6, 45);
            this.revitFileListLabel.Name = "revitFileListLabel";
            this.revitFileListLabel.Size = new System.Drawing.Size(73, 13);
            this.revitFileListLabel.TabIndex = 1;
            this.revitFileListLabel.Text = "Revit File List:";
            // 
            // settingsGroupBox
            // 
            this.settingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsGroupBox.AutoSize = true;
            this.settingsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settingsGroupBox.Controls.Add(this.settingsTableLayoutPanel);
            this.settingsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.settingsGroupBox.Name = "settingsGroupBox";
            this.settingsGroupBox.Size = new System.Drawing.Size(1240, 582);
            this.settingsGroupBox.TabIndex = 0;
            this.settingsGroupBox.TabStop = false;
            this.settingsGroupBox.Text = "Settings";
            // 
            // settingsTableLayoutPanel
            // 
            this.settingsTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsTableLayoutPanel.AutoSize = true;
            this.settingsTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settingsTableLayoutPanel.ColumnCount = 1;
            this.settingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.settingsTableLayoutPanel.Controls.Add(this.taskScriptGroupBox, 0, 0);
            this.settingsTableLayoutPanel.Controls.Add(this.dataExportGroupBox, 0, 4);
            this.settingsTableLayoutPanel.Controls.Add(this.singleRevitTaskProcessingGroupBox, 0, 1);
            this.settingsTableLayoutPanel.Controls.Add(this.preAndPostProcessingGroupBox, 0, 3);
            this.settingsTableLayoutPanel.Controls.Add(this.batchRevitFileProcessingGroupBox, 0, 2);
            this.settingsTableLayoutPanel.Location = new System.Drawing.Point(7, 20);
            this.settingsTableLayoutPanel.Name = "settingsTableLayoutPanel";
            this.settingsTableLayoutPanel.RowCount = 5;
            this.settingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.settingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.settingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.settingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.settingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.settingsTableLayoutPanel.Size = new System.Drawing.Size(1227, 543);
            this.settingsTableLayoutPanel.TabIndex = 5;
            // 
            // taskScriptGroupBox
            // 
            this.taskScriptGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.taskScriptGroupBox.AutoSize = true;
            this.taskScriptGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.taskScriptGroupBox.Controls.Add(this.showMessageBoxOnTaskScriptErrorCheckBox);
            this.taskScriptGroupBox.Controls.Add(this.taskScriptLabel);
            this.taskScriptGroupBox.Controls.Add(this.taskScriptTextBox);
            this.taskScriptGroupBox.Controls.Add(this.taskScriptNewScriptButton);
            this.taskScriptGroupBox.Controls.Add(this.browseScriptButton);
            this.taskScriptGroupBox.Location = new System.Drawing.Point(3, 3);
            this.taskScriptGroupBox.Name = "taskScriptGroupBox";
            this.taskScriptGroupBox.Size = new System.Drawing.Size(1221, 83);
            this.taskScriptGroupBox.TabIndex = 0;
            this.taskScriptGroupBox.TabStop = false;
            this.taskScriptGroupBox.Text = "Task Script";
            // 
            // showMessageBoxOnTaskScriptErrorCheckBox
            // 
            this.showMessageBoxOnTaskScriptErrorCheckBox.AutoSize = true;
            this.showMessageBoxOnTaskScriptErrorCheckBox.Location = new System.Drawing.Point(12, 47);
            this.showMessageBoxOnTaskScriptErrorCheckBox.Name = "showMessageBoxOnTaskScriptErrorCheckBox";
            this.showMessageBoxOnTaskScriptErrorCheckBox.Size = new System.Drawing.Size(217, 17);
            this.showMessageBoxOnTaskScriptErrorCheckBox.TabIndex = 4;
            this.showMessageBoxOnTaskScriptErrorCheckBox.Text = "Show Message Box on Task Script Error";
            this.showMessageBoxOnTaskScriptErrorCheckBox.UseVisualStyleBackColor = true;
            this.showMessageBoxOnTaskScriptErrorCheckBox.Visible = false;
            // 
            // taskScriptLabel
            // 
            this.taskScriptLabel.AutoSize = true;
            this.taskScriptLabel.Location = new System.Drawing.Point(6, 24);
            this.taskScriptLabel.Name = "taskScriptLabel";
            this.taskScriptLabel.Size = new System.Drawing.Size(121, 13);
            this.taskScriptLabel.TabIndex = 0;
            this.taskScriptLabel.Text = "Task Script (*.py; *.dyn):";
            // 
            // taskScriptTextBox
            // 
            this.taskScriptTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.taskScriptTextBox.Location = new System.Drawing.Point(133, 21);
            this.taskScriptTextBox.Name = "taskScriptTextBox";
            this.taskScriptTextBox.ReadOnly = true;
            this.taskScriptTextBox.Size = new System.Drawing.Size(901, 20);
            this.taskScriptTextBox.TabIndex = 1;
            // 
            // taskScriptNewScriptButton
            // 
            this.taskScriptNewScriptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.taskScriptNewScriptButton.Location = new System.Drawing.Point(1121, 19);
            this.taskScriptNewScriptButton.Name = "taskScriptNewScriptButton";
            this.taskScriptNewScriptButton.Size = new System.Drawing.Size(94, 23);
            this.taskScriptNewScriptButton.TabIndex = 3;
            this.taskScriptNewScriptButton.Text = "New Script ...";
            this.taskScriptNewScriptButton.UseVisualStyleBackColor = true;
            this.taskScriptNewScriptButton.Click += new System.EventHandler(this.taskScriptNewScriptButton_Click);
            // 
            // browseScriptButton
            // 
            this.browseScriptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseScriptButton.Location = new System.Drawing.Point(1040, 19);
            this.browseScriptButton.Name = "browseScriptButton";
            this.browseScriptButton.Size = new System.Drawing.Size(75, 23);
            this.browseScriptButton.TabIndex = 2;
            this.browseScriptButton.Text = "Browse ...";
            this.browseScriptButton.UseVisualStyleBackColor = true;
            this.browseScriptButton.Click += new System.EventHandler(this.browseScriptButton_Click);
            // 
            // dataExportGroupBox
            // 
            this.dataExportGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataExportGroupBox.Controls.Add(this.dataExportBaseFolderLabel);
            this.dataExportGroupBox.Controls.Add(this.enableDataExportCheckBox);
            this.dataExportGroupBox.Controls.Add(this.browseDataExportFolderButton);
            this.dataExportGroupBox.Controls.Add(this.dataExportFolderTextBox);
            this.dataExportGroupBox.Location = new System.Drawing.Point(3, 459);
            this.dataExportGroupBox.Name = "dataExportGroupBox";
            this.dataExportGroupBox.Size = new System.Drawing.Size(1221, 81);
            this.dataExportGroupBox.TabIndex = 4;
            this.dataExportGroupBox.TabStop = false;
            this.dataExportGroupBox.Text = "Data Export";
            this.dataExportGroupBox.Visible = false;
            // 
            // dataExportBaseFolderLabel
            // 
            this.dataExportBaseFolderLabel.AutoSize = true;
            this.dataExportBaseFolderLabel.Location = new System.Drawing.Point(6, 48);
            this.dataExportBaseFolderLabel.Name = "dataExportBaseFolderLabel";
            this.dataExportBaseFolderLabel.Size = new System.Drawing.Size(125, 13);
            this.dataExportBaseFolderLabel.TabIndex = 1;
            this.dataExportBaseFolderLabel.Text = "Data Export Base Folder:";
            // 
            // enableDataExportCheckBox
            // 
            this.enableDataExportCheckBox.AutoSize = true;
            this.enableDataExportCheckBox.Location = new System.Drawing.Point(6, 23);
            this.enableDataExportCheckBox.Name = "enableDataExportCheckBox";
            this.enableDataExportCheckBox.Size = new System.Drawing.Size(118, 17);
            this.enableDataExportCheckBox.TabIndex = 0;
            this.enableDataExportCheckBox.Text = "Enable Data Export";
            this.enableDataExportCheckBox.UseVisualStyleBackColor = true;
            this.enableDataExportCheckBox.CheckedChanged += new System.EventHandler(this.enableDataExportCheckBox_CheckedChanged);
            // 
            // singleRevitTaskProcessingGroupBox
            // 
            this.singleRevitTaskProcessingGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.singleRevitTaskProcessingGroupBox.Controls.Add(this.singleRevitTaskRevitVersionComboBox);
            this.singleRevitTaskProcessingGroupBox.Controls.Add(this.enableSingleRevitTaskProcessingCheckBox);
            this.singleRevitTaskProcessingGroupBox.Controls.Add(this.singleRevitTaskRevitVersionLabel);
            this.singleRevitTaskProcessingGroupBox.Location = new System.Drawing.Point(3, 92);
            this.singleRevitTaskProcessingGroupBox.Name = "singleRevitTaskProcessingGroupBox";
            this.singleRevitTaskProcessingGroupBox.Size = new System.Drawing.Size(1221, 73);
            this.singleRevitTaskProcessingGroupBox.TabIndex = 1;
            this.singleRevitTaskProcessingGroupBox.TabStop = false;
            this.singleRevitTaskProcessingGroupBox.Text = "Single Revit Task Processing";
            this.singleRevitTaskProcessingGroupBox.Visible = false;
            // 
            // singleRevitTaskRevitVersionComboBox
            // 
            this.singleRevitTaskRevitVersionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.singleRevitTaskRevitVersionComboBox.FormattingEnabled = true;
            this.singleRevitTaskRevitVersionComboBox.Location = new System.Drawing.Point(100, 42);
            this.singleRevitTaskRevitVersionComboBox.Name = "singleRevitTaskRevitVersionComboBox";
            this.singleRevitTaskRevitVersionComboBox.Size = new System.Drawing.Size(129, 21);
            this.singleRevitTaskRevitVersionComboBox.TabIndex = 2;
            // 
            // enableSingleRevitTaskProcessingCheckBox
            // 
            this.enableSingleRevitTaskProcessingCheckBox.AutoSize = true;
            this.enableSingleRevitTaskProcessingCheckBox.Location = new System.Drawing.Point(6, 19);
            this.enableSingleRevitTaskProcessingCheckBox.Name = "enableSingleRevitTaskProcessingCheckBox";
            this.enableSingleRevitTaskProcessingCheckBox.Size = new System.Drawing.Size(298, 17);
            this.enableSingleRevitTaskProcessingCheckBox.TabIndex = 0;
            this.enableSingleRevitTaskProcessingCheckBox.Text = "Enable Single Revit Task Processing (Python scripts only)";
            this.enableSingleRevitTaskProcessingCheckBox.UseVisualStyleBackColor = true;
            this.enableSingleRevitTaskProcessingCheckBox.CheckedChanged += new System.EventHandler(this.enableSingleRevitTaskProcessingCheckBox_CheckedChanged);
            // 
            // singleRevitTaskRevitVersionLabel
            // 
            this.singleRevitTaskRevitVersionLabel.AutoSize = true;
            this.singleRevitTaskRevitVersionLabel.Location = new System.Drawing.Point(21, 45);
            this.singleRevitTaskRevitVersionLabel.Name = "singleRevitTaskRevitVersionLabel";
            this.singleRevitTaskRevitVersionLabel.Size = new System.Drawing.Size(73, 13);
            this.singleRevitTaskRevitVersionLabel.TabIndex = 1;
            this.singleRevitTaskRevitVersionLabel.Text = "Revit Version:";
            // 
            // preAndPostProcessingGroupBox
            // 
            this.preAndPostProcessingGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.preAndPostProcessingGroupBox.Controls.Add(this.executePostProcessingScriptCheckBox);
            this.preAndPostProcessingGroupBox.Controls.Add(this.executePreProcessingScriptCheckBox);
            this.preAndPostProcessingGroupBox.Controls.Add(this.postProcessingScriptTextBox);
            this.preAndPostProcessingGroupBox.Controls.Add(this.postProcessingScriptNewScriptButton);
            this.preAndPostProcessingGroupBox.Controls.Add(this.preProcessingScriptNewScriptButton);
            this.preAndPostProcessingGroupBox.Controls.Add(this.postProcessingScriptBrowseButton);
            this.preAndPostProcessingGroupBox.Controls.Add(this.preProcessingScriptTextBox);
            this.preAndPostProcessingGroupBox.Controls.Add(this.preProcessingScriptBrowseButton);
            this.preAndPostProcessingGroupBox.Location = new System.Drawing.Point(3, 371);
            this.preAndPostProcessingGroupBox.Name = "preAndPostProcessingGroupBox";
            this.preAndPostProcessingGroupBox.Size = new System.Drawing.Size(1221, 82);
            this.preAndPostProcessingGroupBox.TabIndex = 3;
            this.preAndPostProcessingGroupBox.TabStop = false;
            this.preAndPostProcessingGroupBox.Text = "Pre/Post-Processing";
            this.preAndPostProcessingGroupBox.Visible = false;
            // 
            // executePostProcessingScriptCheckBox
            // 
            this.executePostProcessingScriptCheckBox.AutoSize = true;
            this.executePostProcessingScriptCheckBox.Location = new System.Drawing.Point(6, 50);
            this.executePostProcessingScriptCheckBox.Name = "executePostProcessingScriptCheckBox";
            this.executePostProcessingScriptCheckBox.Size = new System.Drawing.Size(204, 17);
            this.executePostProcessingScriptCheckBox.TabIndex = 4;
            this.executePostProcessingScriptCheckBox.Text = "Execute Post-Processing Script (*.py):";
            this.executePostProcessingScriptCheckBox.UseVisualStyleBackColor = true;
            this.executePostProcessingScriptCheckBox.CheckedChanged += new System.EventHandler(this.executePostProcessingScriptCheckBox_CheckedChanged);
            // 
            // executePreProcessingScriptCheckBox
            // 
            this.executePreProcessingScriptCheckBox.AutoSize = true;
            this.executePreProcessingScriptCheckBox.Location = new System.Drawing.Point(6, 21);
            this.executePreProcessingScriptCheckBox.Name = "executePreProcessingScriptCheckBox";
            this.executePreProcessingScriptCheckBox.Size = new System.Drawing.Size(199, 17);
            this.executePreProcessingScriptCheckBox.TabIndex = 0;
            this.executePreProcessingScriptCheckBox.Text = "Execute Pre-Processing Script (*.py):";
            this.executePreProcessingScriptCheckBox.UseVisualStyleBackColor = true;
            this.executePreProcessingScriptCheckBox.CheckedChanged += new System.EventHandler(this.executePreProcessingScriptCheckBox_CheckedChanged);
            // 
            // postProcessingScriptTextBox
            // 
            this.postProcessingScriptTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.postProcessingScriptTextBox.Location = new System.Drawing.Point(216, 48);
            this.postProcessingScriptTextBox.Name = "postProcessingScriptTextBox";
            this.postProcessingScriptTextBox.ReadOnly = true;
            this.postProcessingScriptTextBox.Size = new System.Drawing.Size(818, 20);
            this.postProcessingScriptTextBox.TabIndex = 5;
            // 
            // postProcessingScriptNewScriptButton
            // 
            this.postProcessingScriptNewScriptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.postProcessingScriptNewScriptButton.Location = new System.Drawing.Point(1121, 46);
            this.postProcessingScriptNewScriptButton.Name = "postProcessingScriptNewScriptButton";
            this.postProcessingScriptNewScriptButton.Size = new System.Drawing.Size(94, 23);
            this.postProcessingScriptNewScriptButton.TabIndex = 7;
            this.postProcessingScriptNewScriptButton.Text = "New Script ...";
            this.postProcessingScriptNewScriptButton.UseVisualStyleBackColor = true;
            this.postProcessingScriptNewScriptButton.Click += new System.EventHandler(this.postProcessingScriptNewScriptButton_Click);
            // 
            // preProcessingScriptNewScriptButton
            // 
            this.preProcessingScriptNewScriptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.preProcessingScriptNewScriptButton.Location = new System.Drawing.Point(1121, 17);
            this.preProcessingScriptNewScriptButton.Name = "preProcessingScriptNewScriptButton";
            this.preProcessingScriptNewScriptButton.Size = new System.Drawing.Size(94, 23);
            this.preProcessingScriptNewScriptButton.TabIndex = 3;
            this.preProcessingScriptNewScriptButton.Text = "New Script ...";
            this.preProcessingScriptNewScriptButton.UseVisualStyleBackColor = true;
            this.preProcessingScriptNewScriptButton.Click += new System.EventHandler(this.preProcessingScriptNewScriptButton_Click);
            // 
            // postProcessingScriptBrowseButton
            // 
            this.postProcessingScriptBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.postProcessingScriptBrowseButton.Location = new System.Drawing.Point(1040, 46);
            this.postProcessingScriptBrowseButton.Name = "postProcessingScriptBrowseButton";
            this.postProcessingScriptBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.postProcessingScriptBrowseButton.TabIndex = 6;
            this.postProcessingScriptBrowseButton.Text = "Browse ...";
            this.postProcessingScriptBrowseButton.UseVisualStyleBackColor = true;
            this.postProcessingScriptBrowseButton.Click += new System.EventHandler(this.postProcessingScriptBrowseButton_Click);
            // 
            // preProcessingScriptTextBox
            // 
            this.preProcessingScriptTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.preProcessingScriptTextBox.Location = new System.Drawing.Point(216, 19);
            this.preProcessingScriptTextBox.Name = "preProcessingScriptTextBox";
            this.preProcessingScriptTextBox.ReadOnly = true;
            this.preProcessingScriptTextBox.Size = new System.Drawing.Size(818, 20);
            this.preProcessingScriptTextBox.TabIndex = 1;
            // 
            // preProcessingScriptBrowseButton
            // 
            this.preProcessingScriptBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.preProcessingScriptBrowseButton.Location = new System.Drawing.Point(1040, 17);
            this.preProcessingScriptBrowseButton.Name = "preProcessingScriptBrowseButton";
            this.preProcessingScriptBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.preProcessingScriptBrowseButton.TabIndex = 2;
            this.preProcessingScriptBrowseButton.Text = "Browse ...";
            this.preProcessingScriptBrowseButton.UseVisualStyleBackColor = true;
            this.preProcessingScriptBrowseButton.Click += new System.EventHandler(this.preProcessingScriptBrowseButton_Click);
            // 
            // batchRevitFileProcessingGroupBox
            // 
            this.batchRevitFileProcessingGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.batchRevitFileProcessingGroupBox.Controls.Add(this.revitSessionGroupBox);
            this.batchRevitFileProcessingGroupBox.Controls.Add(this.enableBatchProcessingCheckBox);
            this.batchRevitFileProcessingGroupBox.Controls.Add(this.revitFileListLabel);
            this.batchRevitFileProcessingGroupBox.Controls.Add(this.newRevitFileListButton);
            this.batchRevitFileProcessingGroupBox.Controls.Add(this.revitFileListTextBox);
            this.batchRevitFileProcessingGroupBox.Controls.Add(this.centralFileProcessingGroupBox);
            this.batchRevitFileProcessingGroupBox.Controls.Add(this.browseRevitFileListButton);
            this.batchRevitFileProcessingGroupBox.Controls.Add(this.revitFileProcessingGroupBox);
            this.batchRevitFileProcessingGroupBox.Location = new System.Drawing.Point(3, 171);
            this.batchRevitFileProcessingGroupBox.Name = "batchRevitFileProcessingGroupBox";
            this.batchRevitFileProcessingGroupBox.Size = new System.Drawing.Size(1221, 194);
            this.batchRevitFileProcessingGroupBox.TabIndex = 2;
            this.batchRevitFileProcessingGroupBox.TabStop = false;
            this.batchRevitFileProcessingGroupBox.Text = "Batch Revit File Processing";
            // 
            // revitSessionGroupBox
            // 
            this.revitSessionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.revitSessionGroupBox.Controls.Add(this.timeOutNumericUpDown);
            this.revitSessionGroupBox.Controls.Add(this.perFileProcessingTimeOutCheckBox);
            this.revitSessionGroupBox.Controls.Add(this.useSameRevitSessionRadioButton);
            this.revitSessionGroupBox.Controls.Add(this.useSeparateRevitSessionRadioButton);
            this.revitSessionGroupBox.Location = new System.Drawing.Point(625, 68);
            this.revitSessionGroupBox.Name = "revitSessionGroupBox";
            this.revitSessionGroupBox.Size = new System.Drawing.Size(590, 97);
            this.revitSessionGroupBox.TabIndex = 7;
            this.revitSessionGroupBox.TabStop = false;
            this.revitSessionGroupBox.Text = "Revit Session";
            // 
            // timeOutNumericUpDown
            // 
            this.timeOutNumericUpDown.Location = new System.Drawing.Point(230, 65);
            this.timeOutNumericUpDown.Name = "timeOutNumericUpDown";
            this.timeOutNumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.timeOutNumericUpDown.TabIndex = 3;
            this.timeOutNumericUpDown.ValueChanged += new System.EventHandler(this.timeOutNumericUpDown_ValueChanged);
            this.timeOutNumericUpDown.Leave += new System.EventHandler(this.timeOutNumericUpDown_Leave);
            // 
            // perFileProcessingTimeOutCheckBox
            // 
            this.perFileProcessingTimeOutCheckBox.AutoSize = true;
            this.perFileProcessingTimeOutCheckBox.Location = new System.Drawing.Point(6, 67);
            this.perFileProcessingTimeOutCheckBox.Name = "perFileProcessingTimeOutCheckBox";
            this.perFileProcessingTimeOutCheckBox.Size = new System.Drawing.Size(218, 17);
            this.perFileProcessingTimeOutCheckBox.TabIndex = 2;
            this.perFileProcessingTimeOutCheckBox.Text = "Per-File processing Time-out (in minutes):";
            this.perFileProcessingTimeOutCheckBox.UseVisualStyleBackColor = true;
            this.perFileProcessingTimeOutCheckBox.CheckedChanged += new System.EventHandler(this.perFileProcessingTimeOutCheckBox_CheckedChanged);
            // 
            // useSameRevitSessionRadioButton
            // 
            this.useSameRevitSessionRadioButton.AutoSize = true;
            this.useSameRevitSessionRadioButton.Location = new System.Drawing.Point(6, 42);
            this.useSameRevitSessionRadioButton.Name = "useSameRevitSessionRadioButton";
            this.useSameRevitSessionRadioButton.Size = new System.Drawing.Size(329, 17);
            this.useSameRevitSessionRadioButton.TabIndex = 1;
            this.useSameRevitSessionRadioButton.TabStop = true;
            this.useSameRevitSessionRadioButton.Text = "Use same Revit session for Revit files of the same Revit Version.";
            this.useSameRevitSessionRadioButton.UseVisualStyleBackColor = true;
            // 
            // useSeparateRevitSessionRadioButton
            // 
            this.useSeparateRevitSessionRadioButton.AutoSize = true;
            this.useSeparateRevitSessionRadioButton.Location = new System.Drawing.Point(6, 19);
            this.useSeparateRevitSessionRadioButton.Name = "useSeparateRevitSessionRadioButton";
            this.useSeparateRevitSessionRadioButton.Size = new System.Drawing.Size(246, 17);
            this.useSeparateRevitSessionRadioButton.TabIndex = 0;
            this.useSeparateRevitSessionRadioButton.TabStop = true;
            this.useSeparateRevitSessionRadioButton.Text = "Use separate Revit session for each Revit File.";
            this.useSeparateRevitSessionRadioButton.UseVisualStyleBackColor = true;
            // 
            // enableBatchProcessingCheckBox
            // 
            this.enableBatchProcessingCheckBox.AutoSize = true;
            this.enableBatchProcessingCheckBox.Location = new System.Drawing.Point(6, 19);
            this.enableBatchProcessingCheckBox.Name = "enableBatchProcessingCheckBox";
            this.enableBatchProcessingCheckBox.Size = new System.Drawing.Size(192, 17);
            this.enableBatchProcessingCheckBox.TabIndex = 0;
            this.enableBatchProcessingCheckBox.Text = "Enable Batch Revit File Processing";
            this.enableBatchProcessingCheckBox.UseVisualStyleBackColor = true;
            this.enableBatchProcessingCheckBox.CheckedChanged += new System.EventHandler(this.enableBatchProcessingCheckBox_CheckedChanged);
            // 
            // newRevitFileListButton
            // 
            this.newRevitFileListButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newRevitFileListButton.Location = new System.Drawing.Point(1121, 40);
            this.newRevitFileListButton.Name = "newRevitFileListButton";
            this.newRevitFileListButton.Size = new System.Drawing.Size(94, 23);
            this.newRevitFileListButton.TabIndex = 4;
            this.newRevitFileListButton.Text = "New List ...";
            this.newRevitFileListButton.UseVisualStyleBackColor = true;
            this.newRevitFileListButton.Click += new System.EventHandler(this.newRevitFileListButton_Click);
            // 
            // centralFileProcessingGroupBox
            // 
            this.centralFileProcessingGroupBox.Controls.Add(this.worksetConfigurationGroupBox);
            this.centralFileProcessingGroupBox.Controls.Add(this.discardWorksetsCheckBox);
            this.centralFileProcessingGroupBox.Controls.Add(this.deleteLocalAfterCheckBox);
            this.centralFileProcessingGroupBox.Controls.Add(this.detachFromCentralRadioButton);
            this.centralFileProcessingGroupBox.Controls.Add(this.createNewLocalRadioButton);
            this.centralFileProcessingGroupBox.Location = new System.Drawing.Point(6, 68);
            this.centralFileProcessingGroupBox.Name = "centralFileProcessingGroupBox";
            this.centralFileProcessingGroupBox.Size = new System.Drawing.Size(298, 120);
            this.centralFileProcessingGroupBox.TabIndex = 5;
            this.centralFileProcessingGroupBox.TabStop = false;
            this.centralFileProcessingGroupBox.Text = "Central File Processing";
            // 
            // worksetConfigurationGroupBox
            // 
            this.worksetConfigurationGroupBox.Controls.Add(this.openLastViewedWorksetsRadioButton);
            this.worksetConfigurationGroupBox.Controls.Add(this.openAllWorksetsRadioButton);
            this.worksetConfigurationGroupBox.Controls.Add(this.closeAllWorksetsRadioButton);
            this.worksetConfigurationGroupBox.Location = new System.Drawing.Point(152, 19);
            this.worksetConfigurationGroupBox.Name = "worksetConfigurationGroupBox";
            this.worksetConfigurationGroupBox.Size = new System.Drawing.Size(140, 95);
            this.worksetConfigurationGroupBox.TabIndex = 4;
            this.worksetConfigurationGroupBox.TabStop = false;
            this.worksetConfigurationGroupBox.Text = "Workset Configuration";
            // 
            // openLastViewedWorksetsRadioButton
            // 
            this.openLastViewedWorksetsRadioButton.AutoSize = true;
            this.openLastViewedWorksetsRadioButton.Location = new System.Drawing.Point(6, 65);
            this.openLastViewedWorksetsRadioButton.Name = "openLastViewedWorksetsRadioButton";
            this.openLastViewedWorksetsRadioButton.Size = new System.Drawing.Size(112, 17);
            this.openLastViewedWorksetsRadioButton.TabIndex = 2;
            this.openLastViewedWorksetsRadioButton.TabStop = true;
            this.openLastViewedWorksetsRadioButton.Text = "Open Last Viewed";
            this.openLastViewedWorksetsRadioButton.UseVisualStyleBackColor = true;
            // 
            // openAllWorksetsRadioButton
            // 
            this.openAllWorksetsRadioButton.AutoSize = true;
            this.openAllWorksetsRadioButton.Location = new System.Drawing.Point(6, 42);
            this.openAllWorksetsRadioButton.Name = "openAllWorksetsRadioButton";
            this.openAllWorksetsRadioButton.Size = new System.Drawing.Size(113, 17);
            this.openAllWorksetsRadioButton.TabIndex = 1;
            this.openAllWorksetsRadioButton.TabStop = true;
            this.openAllWorksetsRadioButton.Text = "Open All Worksets";
            this.openAllWorksetsRadioButton.UseVisualStyleBackColor = true;
            // 
            // closeAllWorksetsRadioButton
            // 
            this.closeAllWorksetsRadioButton.AutoSize = true;
            this.closeAllWorksetsRadioButton.Location = new System.Drawing.Point(6, 19);
            this.closeAllWorksetsRadioButton.Name = "closeAllWorksetsRadioButton";
            this.closeAllWorksetsRadioButton.Size = new System.Drawing.Size(113, 17);
            this.closeAllWorksetsRadioButton.TabIndex = 0;
            this.closeAllWorksetsRadioButton.TabStop = true;
            this.closeAllWorksetsRadioButton.Text = "Close All Worksets";
            this.closeAllWorksetsRadioButton.UseVisualStyleBackColor = true;
            // 
            // discardWorksetsCheckBox
            // 
            this.discardWorksetsCheckBox.AutoSize = true;
            this.discardWorksetsCheckBox.Location = new System.Drawing.Point(24, 88);
            this.discardWorksetsCheckBox.Name = "discardWorksetsCheckBox";
            this.discardWorksetsCheckBox.Size = new System.Drawing.Size(110, 17);
            this.discardWorksetsCheckBox.TabIndex = 3;
            this.discardWorksetsCheckBox.Text = "Discard Worksets";
            this.discardWorksetsCheckBox.UseVisualStyleBackColor = true;
            this.discardWorksetsCheckBox.CheckedChanged += new System.EventHandler(this.discardWorksetsCheckBox_CheckedChanged);
            // 
            // deleteLocalAfterCheckBox
            // 
            this.deleteLocalAfterCheckBox.AutoSize = true;
            this.deleteLocalAfterCheckBox.Location = new System.Drawing.Point(24, 42);
            this.deleteLocalAfterCheckBox.Name = "deleteLocalAfterCheckBox";
            this.deleteLocalAfterCheckBox.Size = new System.Drawing.Size(111, 17);
            this.deleteLocalAfterCheckBox.TabIndex = 1;
            this.deleteLocalAfterCheckBox.Text = "Delete Local After";
            this.deleteLocalAfterCheckBox.UseVisualStyleBackColor = true;
            // 
            // detachFromCentralRadioButton
            // 
            this.detachFromCentralRadioButton.AutoSize = true;
            this.detachFromCentralRadioButton.Location = new System.Drawing.Point(6, 65);
            this.detachFromCentralRadioButton.Name = "detachFromCentralRadioButton";
            this.detachFromCentralRadioButton.Size = new System.Drawing.Size(119, 17);
            this.detachFromCentralRadioButton.TabIndex = 2;
            this.detachFromCentralRadioButton.TabStop = true;
            this.detachFromCentralRadioButton.Text = "Detach from Central";
            this.detachFromCentralRadioButton.UseVisualStyleBackColor = true;
            this.detachFromCentralRadioButton.CheckedChanged += new System.EventHandler(this.detachFromCentralRadioButton_CheckedChanged);
            // 
            // createNewLocalRadioButton
            // 
            this.createNewLocalRadioButton.AutoSize = true;
            this.createNewLocalRadioButton.Location = new System.Drawing.Point(6, 19);
            this.createNewLocalRadioButton.Name = "createNewLocalRadioButton";
            this.createNewLocalRadioButton.Size = new System.Drawing.Size(110, 17);
            this.createNewLocalRadioButton.TabIndex = 0;
            this.createNewLocalRadioButton.TabStop = true;
            this.createNewLocalRadioButton.Text = "Create New Local";
            this.createNewLocalRadioButton.UseVisualStyleBackColor = true;
            this.createNewLocalRadioButton.CheckedChanged += new System.EventHandler(this.createNewLocalRadioButton_CheckedChanged);
            // 
            // revitFileProcessingGroupBox
            // 
            this.revitFileProcessingGroupBox.Controls.Add(this.auditOnOpeningCheckBox);
            this.revitFileProcessingGroupBox.Controls.Add(this.useMinimumAvailableVersionCheckBox);
            this.revitFileProcessingGroupBox.Controls.Add(this.useSpecificRevitVersionRadioButton);
            this.revitFileProcessingGroupBox.Controls.Add(this.specificRevitVersionComboBox);
            this.revitFileProcessingGroupBox.Controls.Add(this.useFileRevitVersionRadioButton);
            this.revitFileProcessingGroupBox.Location = new System.Drawing.Point(310, 68);
            this.revitFileProcessingGroupBox.Name = "revitFileProcessingGroupBox";
            this.revitFileProcessingGroupBox.Size = new System.Drawing.Size(309, 120);
            this.revitFileProcessingGroupBox.TabIndex = 6;
            this.revitFileProcessingGroupBox.TabStop = false;
            this.revitFileProcessingGroupBox.Text = "Revit File Processing";
            // 
            // auditOnOpeningCheckBox
            // 
            this.auditOnOpeningCheckBox.AutoSize = true;
            this.auditOnOpeningCheckBox.Location = new System.Drawing.Point(6, 90);
            this.auditOnOpeningCheckBox.Name = "auditOnOpeningCheckBox";
            this.auditOnOpeningCheckBox.Size = new System.Drawing.Size(108, 17);
            this.auditOnOpeningCheckBox.TabIndex = 4;
            this.auditOnOpeningCheckBox.Text = "Audit on Opening";
            this.auditOnOpeningCheckBox.UseVisualStyleBackColor = true;
            // 
            // useMinimumAvailableVersionCheckBox
            // 
            this.useMinimumAvailableVersionCheckBox.AutoSize = true;
            this.useMinimumAvailableVersionCheckBox.Location = new System.Drawing.Point(25, 42);
            this.useMinimumAvailableVersionCheckBox.Name = "useMinimumAvailableVersionCheckBox";
            this.useMinimumAvailableVersionCheckBox.Size = new System.Drawing.Size(275, 17);
            this.useMinimumAvailableVersionCheckBox.TabIndex = 1;
            this.useMinimumAvailableVersionCheckBox.Text = "If not available, use minimum available Revit Version.";
            this.useMinimumAvailableVersionCheckBox.UseVisualStyleBackColor = true;
            // 
            // useSpecificRevitVersionRadioButton
            // 
            this.useSpecificRevitVersionRadioButton.AutoSize = true;
            this.useSpecificRevitVersionRadioButton.Location = new System.Drawing.Point(6, 65);
            this.useSpecificRevitVersionRadioButton.Name = "useSpecificRevitVersionRadioButton";
            this.useSpecificRevitVersionRadioButton.Size = new System.Drawing.Size(152, 17);
            this.useSpecificRevitVersionRadioButton.TabIndex = 2;
            this.useSpecificRevitVersionRadioButton.TabStop = true;
            this.useSpecificRevitVersionRadioButton.Text = "Use specific Revit Version:";
            this.useSpecificRevitVersionRadioButton.UseVisualStyleBackColor = true;
            this.useSpecificRevitVersionRadioButton.CheckedChanged += new System.EventHandler(this.useSpecificRevitVersionRadioButton_CheckedChanged);
            // 
            // specificRevitVersionComboBox
            // 
            this.specificRevitVersionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.specificRevitVersionComboBox.FormattingEnabled = true;
            this.specificRevitVersionComboBox.Location = new System.Drawing.Point(164, 64);
            this.specificRevitVersionComboBox.Name = "specificRevitVersionComboBox";
            this.specificRevitVersionComboBox.Size = new System.Drawing.Size(129, 21);
            this.specificRevitVersionComboBox.TabIndex = 3;
            // 
            // useFileRevitVersionRadioButton
            // 
            this.useFileRevitVersionRadioButton.AutoSize = true;
            this.useFileRevitVersionRadioButton.Location = new System.Drawing.Point(6, 19);
            this.useFileRevitVersionRadioButton.Name = "useFileRevitVersionRadioButton";
            this.useFileRevitVersionRadioButton.Size = new System.Drawing.Size(185, 17);
            this.useFileRevitVersionRadioButton.TabIndex = 0;
            this.useFileRevitVersionRadioButton.TabStop = true;
            this.useFileRevitVersionRadioButton.Text = "Use file Revit Version (if available)";
            this.useFileRevitVersionRadioButton.UseVisualStyleBackColor = true;
            this.useFileRevitVersionRadioButton.CheckedChanged += new System.EventHandler(this.useFileRevitVersionRadioButton_CheckedChanged);
            // 
            // alwaysOnTopCheckbox
            // 
            this.alwaysOnTopCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.alwaysOnTopCheckbox.AutoSize = true;
            this.alwaysOnTopCheckbox.Location = new System.Drawing.Point(919, 3);
            this.alwaysOnTopCheckbox.Name = "alwaysOnTopCheckbox";
            this.alwaysOnTopCheckbox.Size = new System.Drawing.Size(92, 23);
            this.alwaysOnTopCheckbox.TabIndex = 4;
            this.alwaysOnTopCheckbox.Text = "Always on top";
            this.alwaysOnTopCheckbox.UseVisualStyleBackColor = true;
            this.alwaysOnTopCheckbox.CheckedChanged += new System.EventHandler(this.alwaysOnTopCheckbox_CheckedChanged);
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.startButton.Location = new System.Drawing.Point(1018, 3);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(137, 23);
            this.startButton.TabIndex = 5;
            this.startButton.Text = "Start Processing";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(1162, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // batchRvtOutputGroupBox
            // 
            this.batchRvtOutputGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.batchRvtOutputGroupBox.Controls.Add(this.batchRvtOutputTextBox);
            this.batchRvtOutputGroupBox.Location = new System.Drawing.Point(3, 626);
            this.batchRvtOutputGroupBox.Name = "batchRvtOutputGroupBox";
            this.batchRvtOutputGroupBox.Size = new System.Drawing.Size(1240, 178);
            this.batchRvtOutputGroupBox.TabIndex = 7;
            this.batchRvtOutputGroupBox.TabStop = false;
            this.batchRvtOutputGroupBox.Text = "Progress";
            // 
            // batchRvtOutputTextBox
            // 
            this.batchRvtOutputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.batchRvtOutputTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.batchRvtOutputTextBox.Location = new System.Drawing.Point(6, 19);
            this.batchRvtOutputTextBox.Multiline = true;
            this.batchRvtOutputTextBox.Name = "batchRvtOutputTextBox";
            this.batchRvtOutputTextBox.ReadOnly = true;
            this.batchRvtOutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.batchRvtOutputTextBox.Size = new System.Drawing.Size(1228, 153);
            this.batchRvtOutputTextBox.TabIndex = 0;
            this.batchRvtOutputTextBox.TabStop = false;
            this.batchRvtOutputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.batchRvtOutputTextBox_KeyDown);
            // 
            // importSettingsButton
            // 
            this.importSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.importSettingsButton.Location = new System.Drawing.Point(3, 3);
            this.importSettingsButton.Name = "importSettingsButton";
            this.importSettingsButton.Size = new System.Drawing.Size(110, 23);
            this.importSettingsButton.TabIndex = 1;
            this.importSettingsButton.Text = "Import Settings ...";
            this.importSettingsButton.UseVisualStyleBackColor = true;
            this.importSettingsButton.Click += new System.EventHandler(this.importSettingsButton_Click);
            // 
            // exportSettingsButton
            // 
            this.exportSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.exportSettingsButton.Location = new System.Drawing.Point(120, 3);
            this.exportSettingsButton.Name = "exportSettingsButton";
            this.exportSettingsButton.Size = new System.Drawing.Size(110, 23);
            this.exportSettingsButton.TabIndex = 2;
            this.exportSettingsButton.Text = "Export Settings ...";
            this.exportSettingsButton.UseVisualStyleBackColor = true;
            this.exportSettingsButton.Click += new System.EventHandler(this.exportSettingsButton_Click);
            // 
            // showAdvancedSettingsCheckBox
            // 
            this.showAdvancedSettingsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.showAdvancedSettingsCheckBox.AutoSize = true;
            this.showAdvancedSettingsCheckBox.Location = new System.Drawing.Point(238, 3);
            this.showAdvancedSettingsCheckBox.Name = "showAdvancedSettingsCheckBox";
            this.showAdvancedSettingsCheckBox.Size = new System.Drawing.Size(146, 23);
            this.showAdvancedSettingsCheckBox.TabIndex = 3;
            this.showAdvancedSettingsCheckBox.Text = "Show Advanced Settings";
            this.showAdvancedSettingsCheckBox.UseVisualStyleBackColor = true;
            this.showAdvancedSettingsCheckBox.CheckedChanged += new System.EventHandler(this.showAdvancedSettingsCheckBox_CheckedChanged);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.settingsGroupBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.batchRvtOutputGroupBox, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.otherControlsTableLayout, 0, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1246, 807);
            this.tableLayoutPanel.TabIndex = 5;
            // 
            // otherControlsTableLayout
            // 
            this.otherControlsTableLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.otherControlsTableLayout.ColumnCount = 7;
            this.otherControlsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 117F));
            this.otherControlsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 118F));
            this.otherControlsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 157F));
            this.otherControlsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.otherControlsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 106F));
            this.otherControlsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 144F));
            this.otherControlsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.otherControlsTableLayout.Controls.Add(this.importSettingsButton, 0, 0);
            this.otherControlsTableLayout.Controls.Add(this.showAdvancedSettingsCheckBox, 2, 0);
            this.otherControlsTableLayout.Controls.Add(this.closeButton, 6, 0);
            this.otherControlsTableLayout.Controls.Add(this.startButton, 5, 0);
            this.otherControlsTableLayout.Controls.Add(this.alwaysOnTopCheckbox, 4, 0);
            this.otherControlsTableLayout.Controls.Add(this.exportSettingsButton, 1, 0);
            this.otherControlsTableLayout.Location = new System.Drawing.Point(3, 591);
            this.otherControlsTableLayout.Name = "otherControlsTableLayout";
            this.otherControlsTableLayout.RowCount = 1;
            this.otherControlsTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.otherControlsTableLayout.Size = new System.Drawing.Size(1240, 29);
            this.otherControlsTableLayout.TabIndex = 7;
            // 
            // BatchRvtGuiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1270, 831);
            this.Controls.Add(this.tableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BatchRvtGuiForm";
            this.ShowIcon = false;
            this.Text = "Revit Batch Processor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BatchRvtGuiForm_FormClosing);
            this.Load += new System.EventHandler(this.BatchRvtGuiForm_Load);
            this.settingsGroupBox.ResumeLayout(false);
            this.settingsGroupBox.PerformLayout();
            this.settingsTableLayoutPanel.ResumeLayout(false);
            this.settingsTableLayoutPanel.PerformLayout();
            this.taskScriptGroupBox.ResumeLayout(false);
            this.taskScriptGroupBox.PerformLayout();
            this.dataExportGroupBox.ResumeLayout(false);
            this.dataExportGroupBox.PerformLayout();
            this.singleRevitTaskProcessingGroupBox.ResumeLayout(false);
            this.singleRevitTaskProcessingGroupBox.PerformLayout();
            this.preAndPostProcessingGroupBox.ResumeLayout(false);
            this.preAndPostProcessingGroupBox.PerformLayout();
            this.batchRevitFileProcessingGroupBox.ResumeLayout(false);
            this.batchRevitFileProcessingGroupBox.PerformLayout();
            this.revitSessionGroupBox.ResumeLayout(false);
            this.revitSessionGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeOutNumericUpDown)).EndInit();
            this.centralFileProcessingGroupBox.ResumeLayout(false);
            this.centralFileProcessingGroupBox.PerformLayout();
            this.worksetConfigurationGroupBox.ResumeLayout(false);
            this.worksetConfigurationGroupBox.PerformLayout();
            this.revitFileProcessingGroupBox.ResumeLayout(false);
            this.revitFileProcessingGroupBox.PerformLayout();
            this.batchRvtOutputGroupBox.ResumeLayout(false);
            this.batchRvtOutputGroupBox.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.otherControlsTableLayout.ResumeLayout(false);
            this.otherControlsTableLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox revitFileListTextBox;
        private System.Windows.Forms.TextBox dataExportFolderTextBox;
        private System.Windows.Forms.Button browseRevitFileListButton;
        private System.Windows.Forms.Button browseDataExportFolderButton;
        private System.Windows.Forms.Label revitFileListLabel;
        private System.Windows.Forms.GroupBox settingsGroupBox;
        private System.Windows.Forms.CheckBox alwaysOnTopCheckbox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.GroupBox batchRvtOutputGroupBox;
        private System.Windows.Forms.TextBox batchRvtOutputTextBox;
        private System.Windows.Forms.RadioButton detachFromCentralRadioButton;
        private System.Windows.Forms.RadioButton createNewLocalRadioButton;
        private System.Windows.Forms.CheckBox deleteLocalAfterCheckBox;
        private System.Windows.Forms.GroupBox centralFileProcessingGroupBox;
        private System.Windows.Forms.GroupBox revitFileProcessingGroupBox;
        private System.Windows.Forms.GroupBox batchRevitFileProcessingGroupBox;
        private System.Windows.Forms.RadioButton useSpecificRevitVersionRadioButton;
        private System.Windows.Forms.RadioButton useFileRevitVersionRadioButton;
        private System.Windows.Forms.ComboBox specificRevitVersionComboBox;
        private System.Windows.Forms.CheckBox useMinimumAvailableVersionCheckBox;
        private System.Windows.Forms.GroupBox preAndPostProcessingGroupBox;
        private System.Windows.Forms.CheckBox executePostProcessingScriptCheckBox;
        private System.Windows.Forms.CheckBox executePreProcessingScriptCheckBox;
        private System.Windows.Forms.TextBox postProcessingScriptTextBox;
        private System.Windows.Forms.Button postProcessingScriptBrowseButton;
        private System.Windows.Forms.TextBox preProcessingScriptTextBox;
        private System.Windows.Forms.Button preProcessingScriptBrowseButton;
        private System.Windows.Forms.GroupBox taskScriptGroupBox;
        private System.Windows.Forms.CheckBox showMessageBoxOnTaskScriptErrorCheckBox;
        private System.Windows.Forms.Label taskScriptLabel;
        private System.Windows.Forms.TextBox taskScriptTextBox;
        private System.Windows.Forms.Button browseScriptButton;
        private System.Windows.Forms.GroupBox dataExportGroupBox;
        private System.Windows.Forms.CheckBox enableDataExportCheckBox;
        private System.Windows.Forms.Label dataExportBaseFolderLabel;
        private System.Windows.Forms.CheckBox enableBatchProcessingCheckBox;
        private System.Windows.Forms.GroupBox singleRevitTaskProcessingGroupBox;
        private System.Windows.Forms.ComboBox singleRevitTaskRevitVersionComboBox;
        private System.Windows.Forms.Label singleRevitTaskRevitVersionLabel;
        private System.Windows.Forms.CheckBox enableSingleRevitTaskProcessingCheckBox;
        private System.Windows.Forms.GroupBox revitSessionGroupBox;
        private System.Windows.Forms.RadioButton useSameRevitSessionRadioButton;
        private System.Windows.Forms.RadioButton useSeparateRevitSessionRadioButton;
        private System.Windows.Forms.Button importSettingsButton;
        private System.Windows.Forms.Button exportSettingsButton;
        private System.Windows.Forms.Button taskScriptNewScriptButton;
        private System.Windows.Forms.Button preProcessingScriptNewScriptButton;
        private System.Windows.Forms.Button postProcessingScriptNewScriptButton;
        private System.Windows.Forms.CheckBox perFileProcessingTimeOutCheckBox;
        private System.Windows.Forms.CheckBox showAdvancedSettingsCheckBox;
        private System.Windows.Forms.NumericUpDown timeOutNumericUpDown;
        private System.Windows.Forms.CheckBox discardWorksetsCheckBox;
        private System.Windows.Forms.Button newRevitFileListButton;
        private System.Windows.Forms.GroupBox worksetConfigurationGroupBox;
        private System.Windows.Forms.RadioButton openLastViewedWorksetsRadioButton;
        private System.Windows.Forms.RadioButton openAllWorksetsRadioButton;
        private System.Windows.Forms.RadioButton closeAllWorksetsRadioButton;
        private System.Windows.Forms.CheckBox auditOnOpeningCheckBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel otherControlsTableLayout;
        private System.Windows.Forms.TableLayoutPanel settingsTableLayoutPanel;
    }
}

