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
using System.ComponentModel;
using System.Windows.Forms;

namespace BatchRvtGUI;

public partial class RevitFileScanningProgressUI : Form
{
    private readonly Action<Func<string, bool>> actionWithProgressReporting_;
    private BackgroundWorker backgroundWorker_;
    private string currentProgressMessage_ = string.Empty;
    private Timer progressUpdateTimer_;
    private bool scanningCancelled_;
    private bool scanningCompleted_;

    public RevitFileScanningProgressUI(Action<Func<string, bool>> actionWithProgressReporting)
    {
        InitializeComponent();

        actionWithProgressReporting_ = actionWithProgressReporting;
    }

    private void RevitFileScanningProgressUI_Load(object sender, EventArgs e)
    {
        backgroundWorker_ = new BackgroundWorker();
        backgroundWorker_.DoWork += BackgroundWorker__DoWork;
        backgroundWorker_.ProgressChanged += BackgroundWorker__ProgressChanged;
        backgroundWorker_.RunWorkerCompleted += BackgroundWorker__RunWorkerCompleted;
        backgroundWorker_.WorkerReportsProgress = true;
        backgroundWorker_.RunWorkerAsync();

        /*progressUpdateTimer_ = new Timer();
        progressUpdateTimer_.Interval = 50;
        progressUpdateTimer_.Tick += ProgressUpdateTimer__Tick;

        progressUpdateTimer_.Start();*/
    }


    /*private void ProgressUpdateTimer__Tick(object sender, EventArgs e)
    {
        progressLabel.Text = currentProgressMessage_;
    }*/

    private void BackgroundWorker__RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        // TODO: determine if the work was cancelled or terminated due to an error
        //       and return an appropriate dialog result.
        if (scanningCancelled_) return;
        scanningCompleted_ = true;
        DialogResult = DialogResult.OK;
    }

    private void BackgroundWorker__ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        var progressMessage = e.UserState as string;

        currentProgressMessage_ = progressMessage;
        ////Test
        progressLabel.Text = currentProgressMessage_;
    }

    private void BackgroundWorker__DoWork(object sender, DoWorkEventArgs e)
    {
        bool ProgressReporter(string progressMessage)
        {
            backgroundWorker_.ReportProgress(0, progressMessage);
            return scanningCancelled_;
        }

        actionWithProgressReporting_(ProgressReporter);
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        CancelWithConfirmationPrompt();
    }

    private void CancelWithConfirmationPrompt()
    {
        var dialogResult = MessageBox.Show(
            this,
            @"Are you sure you want to cancel the scan?",
            string.Empty,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Asterisk,
            MessageBoxDefaultButton.Button2
        );

        if (dialogResult != DialogResult.Yes) return;
        // TODO: stop the scanning operation?
        scanningCancelled_ = true;
        DialogResult = DialogResult.Cancel;
    }

    private void RevitFileScanningProgressUI_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
            if (!scanningCompleted_ && !scanningCancelled_)
            {
                // NOTE: this scenario occurs when the user closes the progress window using Alt+F4.
                CancelWithConfirmationPrompt();

                if (!scanningCancelled_) e.Cancel = true;
            }

        //if (!e.Cancel) progressUpdateTimer_.Stop();
    }
}