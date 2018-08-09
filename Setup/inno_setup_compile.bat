@echo off
set InnoSetupFolderPathA="C:\Program Files (x86)\Inno Setup 5"

if EXIST %InnoSetupFolderPathA% (
  set IronPythonFolderPath=%InnoSetupFolderPathA%
) else (
  echo.
  echo ERROR: could not locate an Inno Setup 5 installation folder!
  exit /b 1
)

set InnoSetupCompileOptions=

%IronPythonFolderPath%\ISCC.exe %InnoSetupCompileOptions% %*
