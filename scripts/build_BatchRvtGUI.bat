
@echo off

if [%1]==[] exit /b 1

call build_Project.bat "..\RevitBatchProcessor.sln" %1 %2
