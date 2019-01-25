using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatchRvtGUI
{
    public partial class RevitFileScanningOptionsUI : Form
    {
        public enum RevitFileType { Project, Family, ProjectAndFamily }

        public RevitFileScanningOptionsUI()
        {
            InitializeComponent();
        }

        public RevitFileType GetSelectedRevitFileType()
        {
            var revitFileType = RevitFileType.Project;

            if (this.projectFilesRadioButton.Checked)
            {
                revitFileType = RevitFileType.Project;
            }
            else if (this.familyFilesRadioButton.Checked)
            {
                revitFileType = RevitFileType.Family;
            }
            else if (this.revitFilesRadioButton.Checked)
            {
                revitFileType = RevitFileType.ProjectAndFamily;
            }

            return revitFileType;
        }

        public bool IncludeSubfolders()
        {
            return this.includeSubfoldersCheckBox.Checked;
        }

        public bool ExpandNetworkPaths()
        {
            return this.expandNetworkPathsCheckBox.Checked;
        }

        public bool ExtractRevitVersionInfo()
        {
            return this.detectRevitFileVersionCheckBox.Checked;
        }
    }
}
