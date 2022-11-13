@echo off

pushd %~dp0

echo.
echo Removing BatchRvt addin for Revit 2015
call RemoveAddin.bat 2015

echo.
echo Removing BatchRvt addin for Revit 2016
call RemoveAddin.bat 2016

echo.
echo Removing BatchRvt addin for Revit 2017
call RemoveAddin.bat 2017

echo.
echo Removing BatchRvt addin for Revit 2018
call RemoveAddin.bat 2018

echo.
echo Removing BatchRvt addin for Revit 2019
call RemoveAddin.bat 2019

echo.
echo Removing BatchRvt addin for Revit 2020
call RemoveAddin.bat 2020

echo.
echo Removing BatchRvt addin for Revit 2021
call RemoveAddin.bat 2021

echo.
echo Removing BatchRvt addin for Revit 2022
call RemoveAddin.bat 2022

echo.
echo Removing BatchRvt addin for Revit 2023
call RemoveAddin.bat 2023

echo Done.
echo.

popd
