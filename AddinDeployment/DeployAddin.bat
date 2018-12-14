@echo off

IF [%3]==[] (
  echo ERROR : One or more arguments are missing.
  exit
)

set ProjectDir=%~dp1
set TargetDir=%~dp2
set RevitVersion=%~3
set AddinFileName=BatchRvtAddin%RevitVersion%.addin
set SourceAddinFilePath="%ProjectDir%\%AddinFileName%"
set SourceAddinFolderPath="%TargetDir%"
set TargetAddinsDir=%APPDATA%\Autodesk\Revit\Addins\%RevitVersion%
set TargetAddinFolderPath="%TargetAddinsDir%\BatchRvt"
set TargetAddinFilePath="%TargetAddinsDir%\%AddinFileName%"

echo.
echo ********************************************************************************

echo.
echo Removing existing addin...
echo   [addin file path: %TargetAddinFilePath%]
echo   [addin folder path: %TargetAddinFolderPath%]

if EXIST %TargetAddinFolderPath% (
  rmdir /S /Q %TargetAddinFolderPath%
  IF ERRORLEVEL 1 (
    echo ERROR: Could not remove the existing addin folder! Please remove it manually.
    exit
  )
)

if EXIST %TargetAddinFilePath% (
  del %TargetAddinFilePath%
  rem DEL does not set the ERRORLEVEL! (or may set it to 0)
  rem So another technique is used to detect failure.
  if EXIST %TargetAddinFilePath% (
    echo ERROR: Could not remove the existing addin file! Please remove it manually.
    exit
  )
)

echo   Done.

echo.
echo Creating new addin folder...
echo   [addin folder path: %TargetAddinFolderPath%]
mkdir %TargetAddinFolderPath%

IF ERRORLEVEL 1 (
  echo ERROR: Could not create new addin folder!
  exit
) ELSE (
  echo   Done.
)

echo.
echo Copying addin files to the addin folder...
echo   [from: %SourceAddinFolderPath%]
echo   [to: %TargetAddinFolderPath%\]

xcopy /E /Q %SourceAddinFolderPath%* %TargetAddinFolderPath%\

IF ERRORLEVEL 1 (
  echo ERROR: Could not copy all addin files to the addin folder!
  exit
)

copy /Y %SourceAddinFilePath% %TargetAddinFilePath%

IF ERRORLEVEL 1 (
  echo ERROR: Could not copy the addin file to the addin folder!
  exit
) ELSE (
  echo   Done.
)

echo.
