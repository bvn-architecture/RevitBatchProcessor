namespace BatchRvtGUI
{
    partial class RevitFileScanningOptionsUI
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.startScanButton = new System.Windows.Forms.Button();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.otherOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.asteriskNoteLabel = new System.Windows.Forms.Label();
            this.detectRevitFileVersionCheckBox = new System.Windows.Forms.CheckBox();
            this.expandNetworkPathsCheckBox = new System.Windows.Forms.CheckBox();
            this.includeSubfoldersCheckBox = new System.Windows.Forms.CheckBox();
            this.revitFileTypesGroupBox = new System.Windows.Forms.GroupBox();
            this.revitFilesRadioButton = new System.Windows.Forms.RadioButton();
            this.projectFilesRadioButton = new System.Windows.Forms.RadioButton();
            this.familyFilesRadioButton = new System.Windows.Forms.RadioButton();
            this.optionsGroupBox.SuspendLayout();
            this.otherOptionsGroupBox.SuspendLayout();
            this.revitFileTypesGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(309, 171);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // startScanButton
            // 
            this.startScanButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startScanButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.startScanButton.Location = new System.Drawing.Point(165, 171);
            this.startScanButton.Name = "startScanButton";
            this.startScanButton.Size = new System.Drawing.Size(138, 23);
            this.startScanButton.TabIndex = 1;
            this.startScanButton.Text = "Start Scanning";
            this.startScanButton.UseVisualStyleBackColor = true;
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Controls.Add(this.otherOptionsGroupBox);
            this.optionsGroupBox.Controls.Add(this.revitFileTypesGroupBox);
            this.optionsGroupBox.Location = new System.Drawing.Point(12, 12);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(372, 148);
            this.optionsGroupBox.TabIndex = 0;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Options";
            // 
            // otherOptionsGroupBox
            // 
            this.otherOptionsGroupBox.Controls.Add(this.asteriskNoteLabel);
            this.otherOptionsGroupBox.Controls.Add(this.detectRevitFileVersionCheckBox);
            this.otherOptionsGroupBox.Controls.Add(this.expandNetworkPathsCheckBox);
            this.otherOptionsGroupBox.Controls.Add(this.includeSubfoldersCheckBox);
            this.otherOptionsGroupBox.Location = new System.Drawing.Point(165, 19);
            this.otherOptionsGroupBox.Name = "otherOptionsGroupBox";
            this.otherOptionsGroupBox.Size = new System.Drawing.Size(194, 117);
            this.otherOptionsGroupBox.TabIndex = 1;
            this.otherOptionsGroupBox.TabStop = false;
            this.otherOptionsGroupBox.Text = "Additional Options";
            // 
            // asteriskNoteLabel
            // 
            this.asteriskNoteLabel.AutoSize = true;
            this.asteriskNoteLabel.Location = new System.Drawing.Point(21, 90);
            this.asteriskNoteLabel.Name = "asteriskNoteLabel";
            this.asteriskNoteLabel.Size = new System.Drawing.Size(162, 13);
            this.asteriskNoteLabel.TabIndex = 3;
            this.asteriskNoteLabel.Text = "(*) appears as additional columns";
            // 
            // detectRevitFileVersionCheckBox
            // 
            this.detectRevitFileVersionCheckBox.AutoSize = true;
            this.detectRevitFileVersionCheckBox.Location = new System.Drawing.Point(6, 65);
            this.detectRevitFileVersionCheckBox.Name = "detectRevitFileVersionCheckBox";
            this.detectRevitFileVersionCheckBox.Size = new System.Drawing.Size(178, 17);
            this.detectRevitFileVersionCheckBox.TabIndex = 2;
            this.detectRevitFileVersionCheckBox.Text = "Extract Revit File Version Info (*)";
            this.detectRevitFileVersionCheckBox.UseVisualStyleBackColor = true;
            // 
            // expandNetworkPathsCheckBox
            // 
            this.expandNetworkPathsCheckBox.AutoSize = true;
            this.expandNetworkPathsCheckBox.Location = new System.Drawing.Point(6, 42);
            this.expandNetworkPathsCheckBox.Name = "expandNetworkPathsCheckBox";
            this.expandNetworkPathsCheckBox.Size = new System.Drawing.Size(134, 17);
            this.expandNetworkPathsCheckBox.TabIndex = 1;
            this.expandNetworkPathsCheckBox.Text = "Expand Network paths";
            this.expandNetworkPathsCheckBox.UseVisualStyleBackColor = true;
            // 
            // includeSubfoldersCheckBox
            // 
            this.includeSubfoldersCheckBox.AutoSize = true;
            this.includeSubfoldersCheckBox.Location = new System.Drawing.Point(6, 19);
            this.includeSubfoldersCheckBox.Name = "includeSubfoldersCheckBox";
            this.includeSubfoldersCheckBox.Size = new System.Drawing.Size(114, 17);
            this.includeSubfoldersCheckBox.TabIndex = 0;
            this.includeSubfoldersCheckBox.Text = "Include Subfolders";
            this.includeSubfoldersCheckBox.UseVisualStyleBackColor = true;
            // 
            // revitFileTypesGroupBox
            // 
            this.revitFileTypesGroupBox.Controls.Add(this.revitFilesRadioButton);
            this.revitFileTypesGroupBox.Controls.Add(this.projectFilesRadioButton);
            this.revitFileTypesGroupBox.Controls.Add(this.familyFilesRadioButton);
            this.revitFileTypesGroupBox.Location = new System.Drawing.Point(6, 19);
            this.revitFileTypesGroupBox.Name = "revitFileTypesGroupBox";
            this.revitFileTypesGroupBox.Size = new System.Drawing.Size(153, 117);
            this.revitFileTypesGroupBox.TabIndex = 0;
            this.revitFileTypesGroupBox.TabStop = false;
            this.revitFileTypesGroupBox.Text = "Revit File Types";
            // 
            // revitFilesRadioButton
            // 
            this.revitFilesRadioButton.AutoSize = true;
            this.revitFilesRadioButton.Location = new System.Drawing.Point(6, 65);
            this.revitFilesRadioButton.Name = "revitFilesRadioButton";
            this.revitFilesRadioButton.Size = new System.Drawing.Size(135, 17);
            this.revitFilesRadioButton.TabIndex = 2;
            this.revitFilesRadioButton.Text = "Project and Family Files";
            this.revitFilesRadioButton.UseVisualStyleBackColor = true;
            // 
            // projectFilesRadioButton
            // 
            this.projectFilesRadioButton.AutoSize = true;
            this.projectFilesRadioButton.Checked = true;
            this.projectFilesRadioButton.Location = new System.Drawing.Point(6, 19);
            this.projectFilesRadioButton.Name = "projectFilesRadioButton";
            this.projectFilesRadioButton.Size = new System.Drawing.Size(82, 17);
            this.projectFilesRadioButton.TabIndex = 0;
            this.projectFilesRadioButton.TabStop = true;
            this.projectFilesRadioButton.Text = "Project Files";
            this.projectFilesRadioButton.UseVisualStyleBackColor = true;
            // 
            // familyFilesRadioButton
            // 
            this.familyFilesRadioButton.AutoSize = true;
            this.familyFilesRadioButton.Location = new System.Drawing.Point(6, 42);
            this.familyFilesRadioButton.Name = "familyFilesRadioButton";
            this.familyFilesRadioButton.Size = new System.Drawing.Size(78, 17);
            this.familyFilesRadioButton.TabIndex = 1;
            this.familyFilesRadioButton.Text = "Family Files";
            this.familyFilesRadioButton.UseVisualStyleBackColor = true;
            // 
            // RevitFileScanningOptionsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 206);
            this.Controls.Add(this.optionsGroupBox);
            this.Controls.Add(this.startScanButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "RevitFileScanningOptionsUI";
            this.Text = "Revit File Scanning Options";
            this.optionsGroupBox.ResumeLayout(false);
            this.otherOptionsGroupBox.ResumeLayout(false);
            this.otherOptionsGroupBox.PerformLayout();
            this.revitFileTypesGroupBox.ResumeLayout(false);
            this.revitFileTypesGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button startScanButton;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.GroupBox revitFileTypesGroupBox;
        private System.Windows.Forms.RadioButton revitFilesRadioButton;
        private System.Windows.Forms.RadioButton projectFilesRadioButton;
        private System.Windows.Forms.RadioButton familyFilesRadioButton;
        private System.Windows.Forms.CheckBox expandNetworkPathsCheckBox;
        private System.Windows.Forms.CheckBox includeSubfoldersCheckBox;
        private System.Windows.Forms.GroupBox otherOptionsGroupBox;
        private System.Windows.Forms.CheckBox detectRevitFileVersionCheckBox;
        private System.Windows.Forms.Label asteriskNoteLabel;
    }
}