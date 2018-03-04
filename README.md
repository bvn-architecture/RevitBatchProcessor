
# Revit Batch Processor

Fully automated batch processing of Revit files with your own Python or Dynamo task scripts!

![alt text](BatchRvt_Screenshot.png)

# Overview

Fully automated batch processing of Revit files with your own Python or Dynamo task scripts!

# Build & Installation Instructions

Open the solution file RevitBatchProcessor.sln in Visual Studio 2013 or later and run Build Solution (F6).

Revit addins will be automatically deployed to the Addins folder for each available Revit version [2015-2018]. e.g. %APPDATA%\Autodesk\Revit\Addins\2018

The BatchRvtGUI project is the GUI that drives the underlying engine (the BatchRvt project). Once built, run BatchRvtGUI.exe to start the Revit Batch Processor GUI.

# Features

- Batch processing of Revit files (.rvt and .rfa files) using either a specific version of Revit or a version that matches the version of Revit the file was saved in. Currently supports processing files in Revit versions 2015 through 2018. (Of course the required version of Revit must be installed!)
- Custom task scripts written in Python (specifically IronPython). These scripts of course have full access to the Revit API in addition to some batch processing-related information. They can even execute Dynamo scripts with just a small amount of python code! (Stay tuned for HOW-TO)
- Option to create a new Python task script at the click of a button that contains the minimal amount of code required for the custom task script to operate on an opened Revit file. The new task script can then easily be extended to do some useful work.
- Option for custom pre- and post-processing task scripts. Useful if the overall batch processing task requires some additional setup / tear down work to be done.
- Central file processing options (Create a new local file, Detach from central).
- Option to process files (of the same Revit version) in the same Revit session, or to process each file in its own Revit session. The latter is useful if Revit happens to crash during processing, since this won't block further processing.
- Automatic Revit dialog / message box handling. These, in addition to Revit error messages are handled and logged to the GUI console. This makes the batch processor very likely to complete its tasks without any user intervention required!
- Ability to import and export settings. This feature combined with a simple command line interface allows for batch processing tasks to be setup to run automatically on a schedule (using the Windows Task Scheduler) without the GUI.

[TODO: include some useful sample task scripts]

# Requirements

- At least one version of Revit installed. Currently supports Revit versions 2015 through 2018.
- To build from source code, Visual Studio version 2013 or later.
- If executing Dynamo scripts from the task script, Dynamo 1.3+ installed (currently supports Revit versions 2016 through 2018)

# License

This project is licensed under the terms of [The GNU General Public License v3.0](https://www.gnu.org/licenses/gpl.html)

Copyright (c) 2017  Daniel Rumery, BVN

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

# Credits

Daniel Rumery

# Known Limitations / Issues

[ TO DO ]

# Contribute

[ TO DO ]

# Manual

[ TO DO ]

# Sample Scripts

[ TO DO ]

# Release Notes

[ TO DO ]

