@echo off
echo Building Hung Duy Parking Bridge Installer...
echo.

powershell -ExecutionPolicy Bypass -File "%~dp0build-installer.ps1"

echo.
pause