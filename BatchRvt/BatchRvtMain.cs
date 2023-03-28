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
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BatchRvtUtil;

namespace BatchRvtCommand;

public class BatchRvtMain
{
    private const string BatchRvtConsoleTitle = "Batch Revit File Processor";

    private const int WindowWidth = 160;
    private const int WindowHeight = 60;
    private const int BufferWidth = 320;
    private const int BufferHeight = WindowHeight * 50;

    [STAThread]
    private static void Main(string[] args)
    {
        Application.EnableVisualStyles();

        if (HasConsole()) InitConsole();

        var batchRvtFolderPath = GetExecutableFolderPath();
        BatchRvt.ExecuteMonitorScript(batchRvtFolderPath);
    }

    private static void InitConsole()
    {
        Console.Title = BatchRvtConsoleTitle;

        try
        {
            Console.SetWindowSize(
                Math.Min(WindowWidth, Console.LargestWindowWidth),
                Math.Min(WindowHeight, Console.LargestWindowHeight)
            );

            Console.SetBufferSize(BufferWidth, BufferHeight);
        }
        catch (ArgumentOutOfRangeException e)
        {
            // Can occur when output has been redirected via the command-line.
        }
        catch (IOException e)
        {
            // Can occur when output has been redirected via the command-line.
        }
    }

    private static string GetExecutableFolderPath()
    {
        return AppDomain.CurrentDomain.BaseDirectory;
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    private static bool HasConsole()
    {
        return GetConsoleWindow() != IntPtr.Zero;
    }
}