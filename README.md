
# Revit Batch Processor

Fully automated batch processing of Revit files with your own Python or Dynamo task scripts!

## Latest version

[Installer for Revit Batch Processor v1.2.1](https://github.com/bvn-architecture/RevitBatchProcessor/releases/download/2018.03.31/RevitBatchProcessorSetup.exe)

## Use cases

This tool doesn't _do_ any of these things, but it _allows_ you to do them:

- Open all the Revit files across your Revit projects and run a health-check script against them. Keeping an eye on the health and performance of many Revit files is time-consuming. You could use this to check in on all your files daily and react to problems before they get too gnarly.
- Perform project and family audits across your Revit projects.
- Run large scale queries against many Revit files.
- Mine data from your Revit projects for analytics or machine learning projects.
- Automated milestoning of Revit projects.
- Automated housekeeping tasks (e.g. place elements on appropriate worksets)
- Batch upgrading of Revit projects and family files.
- Essentially anything you can do to one Revit file with the Revit API or a Dynamo script, you can now do to many!

## Features

- Batch processing of Revit files (.rvt and .rfa files) using either a specific version of Revit or a version that matches the version of Revit the file was saved in. Currently supports processing files in Revit versions 2015 through 2018. (Of course the required version of Revit must be installed!)
- Custom task scripts written in Python or Dynamo! Python scripts have full access to the Revit API. Dynamo scripts can of course do whatever Dynamo can do :)
- Option to create a new Python task script at the click of a button that contains the minimal amount of code required for the custom task script to operate on an opened Revit file. The new task script can then easily be extended to do some useful work.
- Option for custom pre- and post-processing task scripts. Useful if the overall batch processing task requires some additional setup / tear down work to be done.
- Central file processing options (Create a new local file, Detach from central).
- Option to process files (of the same Revit version) in the same Revit session, or to process each file in its own Revit session. The latter is useful if Revit happens to crash during processing, since this won't block further processing.
- Automatic Revit dialog / message box handling. These, in addition to Revit error messages are handled and logged to the GUI console. This makes the batch processor very likely to complete its tasks without any user intervention required!
- Ability to import and export settings. This feature combined with the simple [command-line interface](#command-line-interface) allows for batch processing tasks to be setup to run automatically on a schedule (using the Windows Task Scheduler) without the GUI.

![Screenshot of the UI](BatchRvt_Screenshot.png)

## Who's this for?

> "With great power come great responsibility"
[-- Spiderman](https://quoteinvestigator.com/2015/07/23/great-power/)

This tool enables you to do things with Revit on a very large scale. Because of this power, Python or Dynamo scripts that make modifications to Revit files (esp. workshared files) should be developed with the utmost care! You will need to be confident in your ability to write Python or Dynamo scripts that won't ruin your files en-masse. The Revit Batch Processor's 'Detach from Central' option should be used both while testing and for scripts that do not explicitly depend on working with a live workshared Central file (the 'Create New local' option).

# Build & Installation Instructions

## Installer

[Installer for Revit Batch Processor v1.2.1](https://github.com/bvn-architecture/RevitBatchProcessor/releases/download/2018.03.31/RevitBatchProcessorSetup.exe)

The Revit Batch Processor (GUI) application will appear in the Start menu after the installation.

## Build from Source code

Open the solution file RevitBatchProcessor.sln in Visual Studio 2013 or later and run Build Solution (F6).

Revit addins will be automatically deployed to the Addins folder for each available Revit version [2015-2018]. e.g. %APPDATA%\Autodesk\Revit\Addins\2018

The BatchRvtGUI project is the GUI that drives the underlying engine (the BatchRvt project). Once built, run BatchRvtGUI.exe to start the Revit Batch Processor GUI.

# Requirements

- At least one version of Revit installed. Currently supports Revit versions 2015 through 2018.
- To build from source code, Visual Studio version 2013 or later.
- If executing Dynamo scripts from the task script, Dynamo 1.3+ installed (currently supports Revit versions 2016 through 2018). NOTE: The Dynamo script MUST have been saved with the 'Automatic' Run mode.

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

# Sample Scripts

Some simple Task scripts to demonstrate how they work:

'Hello World' task script:

```python
'''Output "Hello Revit world!" to the log.'''

# This section is common to all of these scripts. 
import clr
import System

clr.AddReference("RevitAPI")
clr.AddReference("RevitAPIUI")
from Autodesk.Revit.DB import *

import revit_script_util
from revit_script_util import Output

sessionId = revit_script_util.GetSessionId()
uiapp = revit_script_util.GetUIApplication()

# NOTE: these only make sense for batch Revit file processing mode.
doc = revit_script_util.GetScriptDocument()
revitFilePath = revit_script_util.GetRevitFilePath()

# The code above is boilerplate, everything below is yours!

Output()
Output("Hello Revit world!")
```

# Command-line Interface

Revit Batch Processor can be run from the command-line (bypassing the GUI). First configure and export the required processing settings from the GUI application. Once this is done you can simply run the command line utility **BatchRvt.exe** passing the exported settings file path as an argument:

```
%LOCALAPPDATA%\RevitBatchProcessor\BatchRvt.exe --settings_file "BatchRvt.Settings.json"
```

Optionally you can also specify the location for the log file:

```
%LOCALAPPDATA%\RevitBatchProcessor\BatchRvt.exe --log_folder "C:\MyBatchTasks\Logs" --settings_file "C:\MyBatchTasks\BatchRvt.Settings.json"
```

# Manual

Detailed instructions to come!

# Contribute

Feedback and suggestions for improvement are more than welcome! Please track and submit bugs via the Github Issues page. If you're feeling particularly adventurous you may even submit your own code via a Github pull request.

<https://github.com/bvn-architecture/RevitBatchProcessor>

# Known Limitations / Issues

- Dynamo scripts will always be executed using the 'Use separate Revit session for each Revit file' option. This restriction is due to the context in which Revit Batch Processor operates with the Revit API, which prevents the active UI document from being closed or switched during the Revit session. (Note that When executing a Dynamo task script, the Revit Batch Processor opens the document in the UI and is therefore subject to this Revit API limitation. For Python task scripts, the Revit Batch Processor only opens the document in memory, so Python scripts do not suffer this restriction!)

<!---

# Release Notes

[ TO DO ]

--->
