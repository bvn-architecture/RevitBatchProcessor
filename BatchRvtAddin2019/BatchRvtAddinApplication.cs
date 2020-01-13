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
using System.ComponentModel;
using System.IO;
using WinForms = System.Windows.Forms;

using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

using BatchRvt.ScriptHost;

namespace BatchRvt.Addin.Revit2019
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [DisplayName("BatchRvtAddin")]
    [Description("BatchRvtAddin")]
    public class BatchRvtAddinApplication : IExternalApplication
    {
        private static Queue<CloudTask> IdlingQueue = new Queue<CloudTask>();

        private static void SetupBatchScriptHost(ControlledApplication controlledApplication)
        {
            var pluginFolderPath = Path.GetDirectoryName(typeof(BatchRvtAddinApplication).Assembly.Location);

            var batchRvtExternalEventHandler = new BatchRvtExternalEventHandler(pluginFolderPath);

            batchRvtExternalEventHandler.Raise();
        }

        private static void SetupCloudScriptHost(UIControlledApplication uiApp, string rvtPath)
        {
            //var keepRunning = !CloudUtil.IsLastFileInBatch();
            var keepRunning = true;
            IdlingQueue.Enqueue(new CloudTask(_uiApp => { CloudUtil.OpenWithWhite(rvtPath, keepRunning); }));
            IdlingQueue.Enqueue(new CloudTask(_uiApp => { SetupBatchScriptHost(uiApp.ControlledApplication); }, true));
            uiApp.Idling += IdleTaskExecution;
        }

        public Result OnStartup(UIControlledApplication uiApplication)
        {
            CloudUtil.GetScriptDataValue<string>("revitFilePath", out var rvtPath);

            if (rvtPath != null && CloudUtil.IsCloudPath(rvtPath))
               SetupCloudScriptHost(uiApplication, rvtPath);
            else
                SetupBatchScriptHost(uiApplication.ControlledApplication);

            return Result.Succeeded;
        }

        private static void IdleTaskExecution(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
            if (!IdlingQueue.Any())
                return;

            var uiApp = sender as UIApplication;
            var task = IdlingQueue.Dequeue();
            if (!task.IsDocumentAction)
                task.Execute(uiApp);
            else
            {
                var doc = uiApp.ActiveUIDocument?.Document;
                if (doc == null && task.Tries > 0)
                {
                    task.Tries--;
                    IdlingQueue.Enqueue(task);
                    return;
                }
                task.Execute(uiApp);
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }

    public class BatchRvtExternalEventHandler : IExternalEventHandler
    {
        private readonly ExternalEvent externalEvent_;
        private readonly string pluginFolderPath_;

        public BatchRvtExternalEventHandler(string pluginFolderPath)
        {
            this.externalEvent_ = ExternalEvent.Create(this);
            this.pluginFolderPath_ = pluginFolderPath;
        }

        public void Execute(UIApplication uiApp)
        {
            try
            {
                ScriptHostUtil.ExecuteBatchScriptHost(this.pluginFolderPath_, uiApp);
            }
            catch (Exception e)
            {
                WinForms.MessageBox.Show(e.ToString(), ScriptHostUtil.BATCH_RVT_ERROR_WINDOW_TITLE);
            }
        }

        public string GetName()
        {
            return "BatchRvt_ExternalEventHandler";
        }

        public ExternalEventRequest Raise()
        {
            return this.externalEvent_.Raise();
        }
    }

    public class CloudTask
    {
        public Action<UIApplication> Action;
        public bool IsDocumentAction;
        public int Tries = 3000;

        public CloudTask(Action<UIApplication> act, bool isDocAct = false)
        {
            Action = act;
            IsDocumentAction = isDocAct;
        }

        public void Execute(UIApplication uiApp)
        {
            Action.Invoke(uiApp);
        }
    }
}

