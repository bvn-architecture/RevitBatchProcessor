//
// Revit Batch Processor
//
// Copyright (c) 2017  Daniel Rumery, BVN
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

namespace BatchRvtGUI
{
    public partial class RevitFileScanningProgressUI : Form
    {
        Action<Func<string, bool>> actionWithProgressReporting_ = null;
        BackgroundWorker backgroundWorker_ = null;
        bool scanningCancelled_ = false;
        bool scanningCompleted_ = false;
        string currentProgressMessage_ = string.Empty;
        Timer progressUpdateTimer_ = null;

        public RevitFileScanningProgressUI(Action<Func<string, bool>> actionWithProgressReporting)
        {
            InitializeComponent();

            this.actionWithProgressReporting_ = actionWithProgressReporting;
        }

        private void RevitFileScanningProgressUI_Load(object sender, EventArgs e)
        {
            this.backgroundWorker_ = new BackgroundWorker();
            this.backgroundWorker_.DoWork += BackgroundWorker__DoWork;
            this.backgroundWorker_.ProgressChanged += BackgroundWorker__ProgressChanged;
            this.backgroundWorker_.RunWorkerCompleted += BackgroundWorker__RunWorkerCompleted;
            this.backgroundWorker_.WorkerReportsProgress = true;
            this.backgroundWorker_.RunWorkerAsync();

            this.progressUpdateTimer_ = new Timer();
            this.progressUpdateTimer_.Interval = 50;
            this.progressUpdateTimer_.Tick += ProgressUpdateTimer__Tick;

            this.progressUpdateTimer_.Start();
        }

        private void ProgressUpdateTimer__Tick(object sender, EventArgs e)
        {
            this.progressLabel.Text = this.currentProgressMessage_;
        }

        private void BackgroundWorker__RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // TODO: determine if the work was cancelled or terminated due to an error
            //       and return an appropriate dialog result.
            if (!scanningCancelled_)
            {
                this.scanningCompleted_ = true;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void BackgroundWorker__ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progressMessage = e.UserState as string;

            this.currentProgressMessage_ = progressMessage;
        }

        private void BackgroundWorker__DoWork(object sender, DoWorkEventArgs e)
        {
            Func<string, bool> progressReporter = (progressMessage) => {
                    this.backgroundWorker_.ReportProgress(0, progressMessage);
                    return this.scanningCancelled_;
                };

            actionWithProgressReporting_(progressReporter);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            CancelWithConfirmationPrompt();
        }

        private void CancelWithConfirmationPrompt()
        {
            var dialogResult = MessageBox.Show(
                    this,
                    "Are you sure you want to cancel the scan?",
                    string.Empty,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Asterisk,
                    MessageBoxDefaultButton.Button2
                );

            if (dialogResult == DialogResult.Yes)
            {
                // TODO: stop the scanning operation?
                this.scanningCancelled_ = true;
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void RevitFileScanningProgressUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!this.scanningCompleted_ && !this.scanningCancelled_)
                {
                    // NOTE: this scenario occurs when the user closes the progress window using Alt+F4.
                    CancelWithConfirmationPrompt();

                    if (!this.scanningCancelled_)
                    {
                        e.Cancel = true;
                    }
                }
            }

            if (!e.Cancel)
            {
                this.progressUpdateTimer_.Stop();
            }
        }
    }
}
