@echo off

pushd %~dp0

for %%i in (2015 2016 2017 2018 2019 2020 2021 2022 2023 2024) do (
	echo.
	echo Removing BatchRvt addin for Revit %%i
	call RemoveAddin.bat %%i
	)

echo Done.
echo.

popd
