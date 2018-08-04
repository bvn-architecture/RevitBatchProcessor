#define AppName "Revit Batch Processor"
#define AppVersion "1.3.2"

[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
PrivilegesRequired=lowest
AppId={{B5CA57EA-7BB2-4620-916C-AE98376C1EF1}
DisableDirPage=auto
DefaultDirName={localappdata}\RevitBatchProcessor
SetupLogging=True
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
DefaultGroupName=Revit Batch Processor
OutputBaseFilename=RevitBatchProcessorSetup

[Files]
Source: "..\BatchRvtGUI\bin\x64\Release\*"; DestDir: "{app}"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2015\bin\x64\Release\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2015\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2015\BatchRvtAddin2015.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2015"; Flags: ignoreversion
Source: "..\BatchRvtAddin2016\bin\x64\Release\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2016\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2016\BatchRvtAddin2016.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2016"; Flags: ignoreversion
Source: "..\BatchRvtAddin2017\bin\x64\Release\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2017\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2017\BatchRvtAddin2017.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2017"; Flags: ignoreversion
Source: "..\BatchRvtAddin2018\bin\x64\Release\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2018\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2018\BatchRvtAddin2018.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2018"; Flags: ignoreversion

[Icons]
Name: "{group}\Revit Batch Processor (GUI)"; Filename: "{app}\BatchRvtGUI.exe"; WorkingDir: "{app}"
