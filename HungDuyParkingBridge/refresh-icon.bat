@echo off
echo ========================================
echo Hung Duy Parking Bridge - Icon Refresh
echo ========================================

cd /d "%~dp0"

echo 🔄 Clearing build cache...
dotnet clean

echo 🧹 Cleaning bin and obj folders...
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"

echo 📦 Restoring packages...
dotnet restore

echo 🔨 Building with new icon...
dotnet build -c Debug

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ Build successful! Testing icon...
    echo.
    
    echo 🎨 Starting application to test icon...
    start "" "bin\Debug\net9.0-windows\win-x64\HungDuyParkingBridge.exe"
    
    echo.
    echo 📋 Check the console output for icon loading details.
    echo 💡 If icon still doesn't show, try:
    echo    1. Ensure logoTapDoan.ico is in the project root
    echo    2. Check the icon file is not corrupted
    echo    3. Try a different icon file format
    echo.
) else (
    echo ❌ Build failed! Check errors above.
)

pause