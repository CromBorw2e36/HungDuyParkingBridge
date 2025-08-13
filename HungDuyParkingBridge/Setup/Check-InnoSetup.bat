@echo off
echo Checking for InnoSetup installation...
echo.

powershell -ExecutionPolicy Bypass -File "%~dp0check-innosetup.ps1"