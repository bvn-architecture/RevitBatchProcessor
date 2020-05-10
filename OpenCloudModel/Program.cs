using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using TestStack.White;

namespace OpenCloudModel
{
    class Program
    {
        static void Main(string[] args)
        {
            //int rvtProcessId = 1168;
            //var rvtPath = @"BIM360:\\ADN intheworks\DimitarTest\Project Files\rac_advanced_sample_project.rvt";
            //Open(rvtProcessId, rvtPath);
            var keepRunning = true;
            while (keepRunning)
            {
                try
                {
                    using (var pipeServer = new NamedPipeServerStream("__OPEN_CLOUD_MODEL__", PipeDirection.In))
                    {
                        Console.WriteLine("Waiting for connection...");
                        pipeServer.WaitForConnection();
                        using (var sr = new StreamReader(pipeServer))
                        {
                            _ = int.TryParse(sr.ReadLine(), out int rvtProcessId);
                            var rvtPath = sr.ReadLine();
                            _ = bool.TryParse(sr.ReadLine(), out keepRunning);

                            Console.WriteLine("Opening '{0}'", rvtPath);
                            Open(rvtProcessId, rvtPath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("=====================================");
                    Console.WriteLine("ERROR: {0}", ex.Message);
                    Console.WriteLine();
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("=====================================");
                    Console.WriteLine();
                }
            }
        }

        private static void Open(int rvtProcessId, string rvtPath)
        {
            var app = Application.Attach(rvtProcessId);
            
            var mainWin = app.GetWindows().FirstOrDefault(w => w.Title.StartsWith("Autodesk Revit"));
            var openBtnSc = TestStack.White.UIItems.Finders.SearchCriteria.ByAutomationId("ID_Open");
            var openBtn = mainWin?.Get(openBtnSc);
            openBtn?.Invoke();
            mainWin.WaitWhileBusy();
            //var openWin = app.GetWindow("Open");
            var openWin = app.GetWindows().FirstOrDefault(w => w.Title == "Open");
            var bim360BtnSc = TestStack.White.UIItems.Finders.SearchCriteria.ByText("BIM 360");
            var bim360btn = openWin?.Get(bim360BtnSc);
            mainWin.Focus();
            openWin.Focus();
            bim360btn?.Click();

            var folderViewSc = TestStack.White.UIItems.Finders.SearchCriteria.ByText("Folder View");
            foreach (var n in rvtPath.Split('\\').Skip(1))
            {
                if (String.IsNullOrWhiteSpace(n))
                    continue;

                openWin = app.GetWindows().FirstOrDefault(w => w.Title == "Open");
                var fv = openWin.Items.FirstOrDefault(i => i.Name == ""
                                                        && i.ToString().Contains(", ControlType:datagrid,"));
                if (fv != null && fv is TestStack.White.UIItems.ListView filegrid)
                {
                    var cell = GetCell(filegrid, n);
                    if (cell != null)
                    {
                        mainWin.Focus();
                        openWin.Focus();
                        cell.DoubleClick();
                    }
                    else
                        throw new Exception("Invalid cloud model path.");
                }
                else
                    throw new Exception("Could not find file grid.");

            }
            //TestStack.White.Configuration.CoreAppXmlConfiguration.Instance.BusyTimeout = 120000;
            //mainWin.WaitWhileBusy();
        }

        static TestStack.White.UIItems.ListViewCell GetCell(TestStack.White.UIItems.ListView datagrid, string name)
        {
            var items = datagrid.Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == name)
                {
                    return datagrid.Cell("Name", i);
                }
            }
            return null;
        }
    }
}
