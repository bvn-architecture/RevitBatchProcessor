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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BatchRvtUtil;

namespace BatchRvtGUI
{
    public partial class RevitFileScanningOptionsUI : Form
    {
        public RevitFileScanningOptionsUI()
        {
            InitializeComponent();
        }

        public RevitFileScanning.RevitFileType GetSelectedRevitFileType()
        {
            var revitFileType = RevitFileScanning.RevitFileType.Project;

            if (this.projectFilesRadioButton.Checked)
            {
                revitFileType = RevitFileScanning.RevitFileType.Project;
            }
            else if (this.familyFilesRadioButton.Checked)
            {
                revitFileType = RevitFileScanning.RevitFileType.Family;
            }
            else if (this.revitFilesRadioButton.Checked)
            {
                revitFileType = RevitFileScanning.RevitFileType.ProjectAndFamily;
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
