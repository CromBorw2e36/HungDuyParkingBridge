# HungDuyParkingBridge Installer Build Script
# This script builds the application and creates an installer using InnoSetup

# Configuration
$projectDir = Split-Path -Parent $PSScriptRoot
$publishDir = "$projectDir\bin\Release\net9.0-windows\win-x64\publish"
$setupDir = "$projectDir\Setup"
$outputDir = "$setupDir\Output"
$innoSetupPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" # Adjust if your InnoSetup is installed elsewhere

# Step 1: Create necessary directories
Write-Host "Creating necessary directories..." -ForegroundColor Cyan
if (-not (Test-Path $setupDir\Output)) {
    New-Item -ItemType Directory -Path $setupDir\Output -Force | Out-Null
}

# Step 2: Create necessary installer image resources
Write-Host "Creating installer resources..." -ForegroundColor Cyan
if (-not (Test-Path "$projectDir\Publics\Images")) {
    New-Item -ItemType Directory -Path "$projectDir\Publics\Images" -Force | Out-Null
}

# Create installer background image if it doesn't exist
if (-not (Test-Path "$projectDir\Publics\Images\installer_background.bmp")) {
    # Generate installer background with correct dimensions for InnoSetup (493×312 pixels)
    $bgFile = "$projectDir\Publics\Images\installer_background.bmp"
    Write-Host "Creating placeholder installer background image at $bgFile" -ForegroundColor Yellow
    try {
        Add-Type -AssemblyName System.Drawing
        $bitmap = New-Object System.Drawing.Bitmap 493, 312
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        $graphics.FillRectangle(
            (New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(245, 245, 250))), 
            0, 0, 493, 312)
        
        # Add gradient effect
        $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
            (New-Object System.Drawing.Point(0, 0)),
            (New-Object System.Drawing.Point(0, 312)),
            [System.Drawing.Color]::FromArgb(230, 240, 250),
            [System.Drawing.Color]::FromArgb(210, 230, 250))
        $graphics.FillRectangle($brush, 0, 0, 493, 312)
        
        # Add app name and version
        $fontTitle = New-Object System.Drawing.Font("Arial", 16, [System.Drawing.FontStyle]::Bold)
        $fontVersion = New-Object System.Drawing.Font("Arial", 10)
        $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(60, 60, 60))
        $graphics.DrawString("Hung Duy Parking Bridge", $fontTitle, $textBrush, 130, 120)
        $graphics.DrawString("Version 1.0.2", $fontVersion, $textBrush, 180, 160)
        
        # Save as BMP format required by InnoSetup
        $bitmap.Save($bgFile, [System.Drawing.Imaging.ImageFormat]::Bmp)
        $graphics.Dispose()
        $bitmap.Dispose()
        
        Write-Host "  ? Background image created successfully" -ForegroundColor Green
    }
    catch {
        Write-Host "  ? Error creating background image: $_" -ForegroundColor Red
        Write-Host "    You will need to provide your own background image at: $bgFile" -ForegroundColor Yellow
    }
}

# Create installer small image if it doesn't exist
if (-not (Test-Path "$projectDir\Publics\Images\installer_small.bmp")) {
    # Generate installer small image with correct dimensions for InnoSetup (55×55 pixels)
    $smallFile = "$projectDir\Publics\Images\installer_small.bmp"
    Write-Host "Creating placeholder installer small image at $smallFile" -ForegroundColor Yellow
    try {
        Add-Type -AssemblyName System.Drawing
        $bitmap = New-Object System.Drawing.Bitmap 55, 55
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        
        # Create a nice gradient background
        $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
            (New-Object System.Drawing.Point(0, 0)),
            (New-Object System.Drawing.Point(55, 55)),
            [System.Drawing.Color]::FromArgb(0, 120, 215),
            [System.Drawing.Color]::FromArgb(0, 90, 170))
        $graphics.FillRectangle($brush, 0, 0, 55, 55)
        
        # Add simple "HD" text as logo placeholder
        $font = New-Object System.Drawing.Font("Arial", 20, [System.Drawing.FontStyle]::Bold)
        $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
        $graphics.DrawString("HD", $font, $textBrush, 8, 12)
        
        # Save as BMP format required by InnoSetup
        $bitmap.Save($smallFile, [System.Drawing.Imaging.ImageFormat]::Bmp)
        $graphics.Dispose()
        $bitmap.Dispose()
        
        Write-Host "  ? Small image created successfully" -ForegroundColor Green
    }
    catch {
        Write-Host "  ? Error creating small image: $_" -ForegroundColor Red
        Write-Host "    You will need to provide your own small image at: $smallFile" -ForegroundColor Yellow
    }
}

# Step 3: Build and publish the application
Write-Host "Building and publishing the application..." -ForegroundColor Cyan
$dotnetPath = "dotnet"

# Check if ports 5000 and 5001 are in use and warn user
$port5000InUse = Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue
$port5001InUse = Get-NetTCPConnection -LocalPort 5001 -ErrorAction SilentlyContinue

if ($port5000InUse -or $port5001InUse) {
    Write-Host "WARNING: Ports 5000 and/or 5001 are in use. This may affect the application's functionality." -ForegroundColor Yellow
    Write-Host "Consider closing applications that might be using these ports before testing the installer." -ForegroundColor Yellow
}

# Build and publish
$buildResult = & $dotnetPath publish "$projectDir\HungDuyParkingBridge.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed with exit code $LASTEXITCODE" -ForegroundColor Red
    Write-Host $buildResult
    exit 1
}

# Step 4: Run InnoSetup compiler if it exists
Write-Host "Creating installer using InnoSetup..." -ForegroundColor Cyan
if (Test-Path $innoSetupPath) {
    # Check for common issues in the InnoSetup script
    $scriptPath = "$setupDir\HungDuyParkingBridge.iss"
    $scriptContent = Get-Content $scriptPath -Raw
    
    # Create a backup of the script
    Copy-Item $scriptPath "$scriptPath.bak" -Force
    
    # Check for required image files
    $bgImagePath = "$projectDir\Publics\Images\installer_background.bmp"
    $smallImagePath = "$projectDir\Publics\Images\installer_small.bmp"
    
    if (-not (Test-Path $bgImagePath)) {
        Write-Host "Warning: The installer background image is missing. InnoSetup compilation might fail." -ForegroundColor Yellow
        Write-Host "Path not found: $bgImagePath" -ForegroundColor Yellow
    }
    if (-not (Test-Path $smallImagePath)) {
        Write-Host "Warning: The installer small image is missing. InnoSetup compilation might fail." -ForegroundColor Yellow
        Write-Host "Path not found: $smallImagePath" -ForegroundColor Yellow
    }
    
    # Check for the logo icon
    $iconPath = "$projectDir\logoTapDoan.ico"
    if (-not (Test-Path $iconPath)) {
        Write-Host "Warning: The application icon is missing. InnoSetup compilation might fail." -ForegroundColor Yellow
        Write-Host "Path not found: $iconPath" -ForegroundColor Yellow
        Write-Host "Creating a placeholder icon..." -ForegroundColor Yellow
        
        try {
            Add-Type -AssemblyName System.Drawing
            $bitmap = New-Object System.Drawing.Bitmap 32, 32
            $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
            $graphics.FillRectangle(
                (New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(0, 120, 215))), 
                0, 0, 32, 32)
            
            # Add "HD" text
            $font = New-Object System.Drawing.Font("Arial", 14, [System.Drawing.FontStyle]::Bold)
            $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
            $graphics.DrawString("HD", $font, $textBrush, 3, 5)
            
            # Save as icon
            $memoryStream = New-Object System.IO.MemoryStream
            $bitmap.Save($memoryStream, [System.Drawing.Imaging.ImageFormat]::Png)
            $bytes = $memoryStream.ToArray()
            [System.IO.File]::WriteAllBytes($iconPath, $bytes)
            
            Write-Host "  ? Placeholder icon created successfully" -ForegroundColor Green
        }
        catch {
            Write-Host "  ? Error creating placeholder icon: $_" -ForegroundColor Red
        }
    }
    
    # Run InnoSetup compiler
    Write-Host "Running InnoSetup compiler..." -ForegroundColor Cyan
    $innoResult = & $innoSetupPath "$setupDir\HungDuyParkingBridge.iss" /Q
    if ($LASTEXITCODE -ne 0) {
        Write-Host "InnoSetup compilation failed with exit code $LASTEXITCODE" -ForegroundColor Red
        
        # Attempt to capture more detailed error information from InnoSetup
        Write-Host "Attempting to compile with detailed error output..." -ForegroundColor Yellow
        $detailedResult = & $innoSetupPath "$setupDir\HungDuyParkingBridge.iss" 2>&1
        Write-Host "InnoSetup compiler output:" -ForegroundColor Yellow
        Write-Host $detailedResult
        
        # Suggest potential fixes
        Write-Host "`nPossible solutions to common InnoSetup errors:" -ForegroundColor Cyan
        Write-Host "1. Make sure all referenced files exist (icons, images, etc.)" -ForegroundColor White
        Write-Host "2. Check for syntax errors in the .iss script" -ForegroundColor White
        Write-Host "3. Verify that the paths in the script are correct" -ForegroundColor White
        Write-Host "4. Ensure InnoSetup has appropriate permissions" -ForegroundColor White
        Write-Host "5. Make sure you don't use {app} constant in InitializeSetup function" -ForegroundColor White
        
        exit 1
    }
    
    # Check if installer was created successfully
    $installerPath = "$setupDir\Output\HungDuyParkingBridge_Setup.exe"
    if (Test-Path $installerPath) {
        Write-Host "Installer created successfully at: $installerPath" -ForegroundColor Green
        Write-Host "Installer size: $([math]::Round((Get-Item $installerPath).Length / 1MB, 2)) MB" -ForegroundColor Green
    } else {
        Write-Host "Installer was not created at the expected location: $installerPath" -ForegroundColor Red
    }
} else {
    Write-Host "InnoSetup not found at $innoSetupPath" -ForegroundColor Yellow
    Write-Host "Please download and install InnoSetup from https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
    Write-Host "Or update the script with your InnoSetup installation path" -ForegroundColor Yellow
    
    # Check for alternative InnoSetup locations
    $altLocations = @(
        "C:\Program Files\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles}\Inno Setup 6\ISCC.exe",
        "C:\Program Files (x86)\Inno Setup 5\ISCC.exe",
        "C:\Program Files\Inno Setup 5\ISCC.exe"
    )
    
    foreach ($location in $altLocations) {
        if (Test-Path $location) {
            Write-Host "Found InnoSetup at: $location" -ForegroundColor Green
            Write-Host "Update the 'innoSetupPath' variable in this script to use this location" -ForegroundColor Yellow
            break
        }
    }
    
    # Create a README about manual installer creation
    $readmePath = "$setupDir\README_MANUAL_INSTALLER.txt"
    @"
To manually create the installer:

1. Download and install Inno Setup from https://jrsoftware.org/isdl.php
2. Open Inno Setup Compiler
3. Open the script file: $setupDir\HungDuyParkingBridge.iss
4. Click Build > Compile

The installer will be created in: $setupDir\Output\HungDuyParkingBridge_Setup.exe

If you encounter compilation errors:

1. Ensure all referenced image files exist:
   - Create folder: $projectDir\Publics\Images\
   - The folder should contain:
     * installer_background.bmp
     * installer_small.bmp

2. Check that logoTapDoan.ico exists in:
   - $projectDir\logoTapDoan.ico

3. If you're getting "Unknown identifier" errors, make sure all variables are properly declared
   in the [Code] section of the .iss file.

4. For "Internal error: An attempt was made to expand the 'app' constant before it was initialized":
   - This happens when you use {app} in the InitializeSetup function
   - Solution: Use ExpandConstant('{autopf}\{#MyAppName}') instead of {app} in InitializeSetup

5. For other errors, check the InnoSetup documentation at:
   https://jrsoftware.org/ishelp/
"@ | Out-File -FilePath $readmePath -Encoding utf8

    Write-Host "Created instructions for manual installer creation at: $readmePath" -ForegroundColor Cyan
}

# Step 5: Provide final instructions
Write-Host "`nBuild process completed." -ForegroundColor Green
Write-Host "To install the application:" -ForegroundColor Cyan
Write-Host "1. Run the installer: $setupDir\Output\HungDuyParkingBridge_Setup.exe" -ForegroundColor White
Write-Host "2. Follow the installation wizard" -ForegroundColor White
Write-Host "3. The application will start automatically after installation" -ForegroundColor White
Write-Host "`nNotes:" -ForegroundColor Cyan
Write-Host "- The application uses ports 5000 and 5001 for its HTTP and WebSocket servers" -ForegroundColor White
Write-Host "- Files will be stored in C:\HungDuyParkingReceivedFiles" -ForegroundColor White
Write-Host "- Default private key for admin access: 012233" -ForegroundColor White