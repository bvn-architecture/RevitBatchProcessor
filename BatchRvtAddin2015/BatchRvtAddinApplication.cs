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
using System.Linq;
using System.ComponentModel;
using WinForms = System.Windows.Forms;

using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

using BatchRvt.ScriptHost;

namespace BatchRvt.Addin.Revit2015
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [DisplayName("BatchRvtAddin")]
    [Description("BatchRvtAddin")]
    public class BatchRvtAddinApplication : IExternalApplication
    {
        private static void SetupBatchScriptHost(ControlledApplication controlledApplication)
        {
            var pluginFolderPath = typeof(BatchRvtAddinApplication).Assembly.Location;

            controlledApplication.ApplicationInitialized +=
                (sender, args) =>
                {
                    var uiApplication = new UIApplication(sender as Application);

                    try
                    {
                        ScriptHostUtil.ExecuteBatchScriptHost(pluginFolderPath, uiApplication);
                    }
                    catch (Exception e)
                    {
                        WinForms.MessageBox.Show(e.ToString(), ScriptHostUtil.BATCH_RVT_ERROR_WINDOW_TITLE);
                    }
                };
        }

        public Result OnStartup(UIControlledApplication uiApplication)
        {
            SetupBatchScriptHost(uiApplication.ControlledApplication);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
