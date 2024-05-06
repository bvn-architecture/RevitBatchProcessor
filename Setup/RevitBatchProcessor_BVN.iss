#define AppName "Revit Batch Processor_BVN"
#define AppVersion "1.10.0"

[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
PrivilegesRequired=admin
AppId={{B5CA57EA-7BB2-4620-916C-AE98376C1EF1}
DisableDirPage=auto
DefaultDirName="C:\Program Files\BVN\RevitBatchProcessor"
SetupLogging=True
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
DefaultGroupName=Revit Batch Processor
OutputBaseFilename=RevitBatchProcessorSetup_BVN_v{#AppVersion}

[Files]
Source: "..\BatchRvtGUI\bin\x64\Release\*"; DestDir: "{app}"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2015\bin\x64\Release\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2015\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2015\BatchRvtAddin2015.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2015"; Flags: ignoreversion
Source: "..\BatchRvtAddin2016\bin\x64\Release\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2016\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2016\BatchRvtAddin2016.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2016"; Flags: ignoreversion
Source: "..\BatchRvtAddin2017\bin\x64\Release\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2017\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2017\BatchRvtAddin2017.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2017"; Flags: ignoreversion
Source: "..\BatchRvtAddin2018\bin\x64\Release\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2018\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2018\BatchRvtAddin2018.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2018"; Flags: ignoreversion
Source: "..\BatchRvtAddin2019\bin\x64\Release\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2019\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2019\BatchRvtAddin2019.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2019"; Flags: ignoreversion
Source: "..\BatchRvtAddin2020\bin\x64\Release\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2020\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2020\BatchRvtAddin2020.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2020"; Flags: ignoreversion
Source: "..\BatchRvtAddin2021\bin\x64\Release\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2021\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2021\BatchRvtAddin2021.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2021"; Flags: ignoreversion
Source: "..\BatchRvtAddin2022\bin\x64\Release\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2022\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2022\BatchRvtAddin2022.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2022"; Flags: ignoreversion
Source: "..\BatchRvtAddin2023\bin\x64\Release\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2023\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2023\BatchRvtAddin2023.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2023"; Flags: ignoreversion
Source: "..\BatchRvtAddin2024\bin\x64\Release\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2024\BatchRvt"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\BatchRvtAddin2024\BatchRvtAddin2024.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2024"; Flags: ignoreversion
[Icons]
Name: "{group}\Revit Batch Processor (GUI)"; Filename: "{app}\BatchRvtGUI.exe"; WorkingDir: "{app}"
