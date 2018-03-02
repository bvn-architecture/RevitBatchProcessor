
@echo off

if [%1]==[] exit /b 1

set TARGET=%1
set ACTIONS=%2
set BUILD_CONFIG=%3

call ipy64.bat msbuild.py amd64 %TARGET% %ACTIONS% %BUILD_CONFIG% "x64"

