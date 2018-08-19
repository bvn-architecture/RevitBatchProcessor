@echo off

IF [%1]==[] (
  echo ERROR : One or more arguments are missing.
  exit /b
)

set RevitVersion=%~1
set AddinFileName=BatchRvtAddin%RevitVersion%.addin
set TargetAddinsDir=%APPDATA%\Autodesk\Revit\Addins\%RevitVersion%
set TargetAddinFolderPath=%TargetAddinsDir%\BatchRvt
set TargetAddinFilePath=%TargetAddinsDir%\%AddinFileName%

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
    exit /b
  )
)

if EXIST %TargetAddinFilePath% (
  del %TargetAddinFilePath%
  rem DEL does not set the ERRORLEVEL! (or may set it to 0)
  rem So another technique is used to detect failure.
  if EXIST %TargetAddinFilePath% (
    echo ERROR: Could not remove the existing addin file! Please remove it manually.
    exit /b
  )
)

echo   Done.
echo.
