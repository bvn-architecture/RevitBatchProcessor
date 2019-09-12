@echo off
set InnoSetupFolderPathA="C:\Program Files (x86)\Inno Setup 6"
set InnoSetupFolderPathB="C:\Program Files (x86)\Inno Setup 5"

if EXIST %InnoSetupFolderPathA% (
  set InnoSetupFolderPath=%InnoSetupFolderPathA%
) else (
  if EXIST %InnoSetupFolderPathB% (
    set InnoSetupFolderPath=%InnoSetupFolderPathB%
  ) else (
    echo.
    echo ERROR: could not locate an Inno Setup 5 or 6 installation folder!
    exit /b 1
  )
)

set InnoSetupCompileOptions=

%InnoSetupFolderPath%\ISCC.exe %InnoSetupCompileOptions% %*
