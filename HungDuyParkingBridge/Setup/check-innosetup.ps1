# Check if InnoSetup is installed and provide download instructions if needed

# Common paths where InnoSetup might be installed
$innoSetupPaths = @(
    "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
    "C:\Program Files\Inno Setup 6\ISCC.exe",
    "C:\Program Files (x86)\Inno Setup 5\ISCC.exe",
    "C:\Program Files\Inno Setup 5\ISCC.exe"
)

$innoSetupFound = $false
$innoSetupPath = ""

foreach ($path in $innoSetupPaths) {
    if (Test-Path $path) {
        $innoSetupFound = $true
        $innoSetupPath = $path
        break
    }
}

if ($innoSetupFound) {
    Write-Host "? InnoSetup found at: $innoSetupPath" -ForegroundColor Green
    Write-Host "  You can now run Build-Installer.bat to create the installer" -ForegroundColor Cyan
} else {
    Write-Host "? InnoSetup was not found on your system" -ForegroundColor Red
    Write-Host "  To create the installer, you need to install InnoSetup:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  1. Download InnoSetup from: https://jrsoftware.org/isdl.php" -ForegroundColor White
    Write-Host "  2. Install InnoSetup with default options" -ForegroundColor White
    Write-Host "  3. After installation, run Build-Installer.bat to create the installer" -ForegroundColor White
    Write-Host ""
    
    # Ask if the user wants to download InnoSetup now
    $response = Read-Host "Would you like to open the InnoSetup download page now? (y/n)"
    if ($response -eq "y" -or $response -eq "Y") {
        Start-Process "https://jrsoftware.org/isdl.php"
    }
}

# Provide information about the installer creation process
Write-Host ""
Write-Host "===== INSTALLER CREATION PROCESS =====" -ForegroundColor Cyan
Write-Host "When you run Build-Installer.bat, the following steps will be performed:" -ForegroundColor White
Write-Host "1. Build the HungDuyParkingBridge project in Release mode" -ForegroundColor White
Write-Host "2. Publish as a self-contained single file executable" -ForegroundColor White
Write-Host "3. Create the installer using InnoSetup" -ForegroundColor White
Write-Host "4. The installer will be available at: .\Output\HungDuyParkingBridge_Setup.exe" -ForegroundColor White
Write-Host ""
Write-Host "Note: The installer will include everything needed to run the application," -ForegroundColor Yellow
Write-Host "      including the .NET runtime, so users don't need to install it separately." -ForegroundColor Yellow
Write-Host ""

# Check if ports 5000 and 5001 are in use
$port5000InUse = Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue
$port5001InUse = Get-NetTCPConnection -LocalPort 5001 -ErrorAction SilentlyContinue

if ($port5000InUse -or $port5001InUse) {
    Write-Host "WARNING: The following ports required by the application are currently in use:" -ForegroundColor Red
    if ($port5000InUse) {
        Write-Host "- Port 5000 (HTTP Server)" -ForegroundColor Red
    }
    if ($port5001InUse) {
        Write-Host "- Port 5001 (WebSocket Server)" -ForegroundColor Red
    }
    Write-Host "This may cause issues when testing the application after installation." -ForegroundColor Red
    Write-Host "Consider closing applications that might be using these ports before testing." -ForegroundColor Red
}

# Wait for user to read the information
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")