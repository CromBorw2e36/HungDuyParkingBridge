#!/usr/bin/env pwsh

# Icon Test Script for HungDuyParkingBridge
Write-Host "🔍 Testing Icon Configuration..." -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

# Check if icon file exists
$iconPath = ".\logoTapDoan.ico"
if (Test-Path $iconPath) {
    Write-Host "✅ Icon file found: $iconPath" -ForegroundColor Green
    $iconInfo = Get-Item $iconPath
    Write-Host "📏 File size: $([math]::Round($iconInfo.Length / 1KB, 2)) KB" -ForegroundColor Yellow
} else {
    Write-Host "❌ Icon file NOT found: $iconPath" -ForegroundColor Red
    Write-Host "💡 Please ensure logoTapDoan.ico is in the project root" -ForegroundColor Yellow
}

Write-Host ""

# Clean and rebuild
Write-Host "🧹 Cleaning previous build..." -ForegroundColor Cyan
dotnet clean -q

Write-Host "🔨 Building with icon..." -ForegroundColor Cyan
$buildResult = dotnet build -c Debug

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful!" -ForegroundColor Green
    
    # Check if executable exists
    $exePath = ".\bin\Debug\net9.0-windows\win-x64\HungDuyParkingBridge.exe"
    if (Test-Path $exePath) {
        Write-Host "✅ Executable created: $exePath" -ForegroundColor Green
        
        # Get file version info
        $fileInfo = Get-Item $exePath
        Write-Host "📦 Executable size: $([math]::Round($fileInfo.Length / 1MB, 2)) MB" -ForegroundColor Yellow
        
        # Try to get icon info
        try {
            $versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($exePath)
            Write-Host "🏷️  Product: $($versionInfo.ProductName)" -ForegroundColor Yellow
            Write-Host "🏷️  Version: $($versionInfo.FileVersion)" -ForegroundColor Yellow
        } catch {
            Write-Host "⚠️  Could not read version info" -ForegroundColor Yellow
        }
        
        Write-Host ""
        Write-Host "🚀 Starting application for icon test..." -ForegroundColor Cyan
        Write-Host "   Check the console output for icon loading debug info" -ForegroundColor Yellow
        Write-Host "   Check window title bars and system tray for your icon" -ForegroundColor Yellow
        
        # Start the application
        Start-Process $exePath
        
    } else {
        Write-Host "❌ Executable not found after build" -ForegroundColor Red
    }
} else {
    Write-Host "❌ Build failed!" -ForegroundColor Red
}

Write-Host ""
Write-Host "💡 If icon still doesn't appear:" -ForegroundColor Cyan
Write-Host "   1. Check console output when app starts" -ForegroundColor White
Write-Host "   2. Verify logoTapDoan.ico is a valid icon file" -ForegroundColor White
Write-Host "   3. Try restarting Visual Studio if using IDE" -ForegroundColor White
Write-Host "   4. Run: dotnet clean && dotnet restore && dotnet build" -ForegroundColor White