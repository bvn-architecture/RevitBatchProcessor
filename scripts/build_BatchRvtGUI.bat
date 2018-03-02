
@echo off

if [%1]==[] exit /b 1

call build_Project.bat "..\BatchRvtGUI\BatchRvtGUI.csproj" %1 %2
