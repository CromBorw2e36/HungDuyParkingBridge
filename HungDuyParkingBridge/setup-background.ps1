# Background Image Setup Script for HungDuy Parking Bridge
# This script helps set up the background image for guest mode

Write-Host "🎨 HungDuy Parking Bridge - Background Image Setup" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan

$projectRoot = $PSScriptRoot
$imageDirectories = @(
    "Publics\Images",
    "Images",
    "."
)

$backgroundImageName = "background-home-page.png"

Write-Host "`n📁 Checking for background image directories..." -ForegroundColor Blue

foreach ($dir in $imageDirectories) {
    $fullPath = Join-Path $projectRoot $dir
    if (!(Test-Path $fullPath)) {
        Write-Host "📂 Creating directory: $dir" -ForegroundColor Yellow
        New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
        Write-Host "✅ Created: $fullPath" -ForegroundColor Green
    } else {
        Write-Host "✅ Directory exists: $dir" -ForegroundColor Green
    }
    
    $imagePath = Join-Path $fullPath $backgroundImageName
    if (Test-Path $imagePath) {
        Write-Host "🖼️ Background image found: $imagePath" -ForegroundColor Green
        $imageInfo = Get-Item $imagePath
        Write-Host "   Size: $([math]::Round($imageInfo.Length/1KB, 2)) KB" -ForegroundColor Gray
        Write-Host "   Modified: $($imageInfo.LastWriteTime)" -ForegroundColor Gray
    } else {
        Write-Host "❌ Background image not found: $imagePath" -ForegroundColor Red
    }
}

Write-Host "`n🎯 Background Image Usage:" -ForegroundColor Yellow
Write-Host "The application will look for 'background-home-page.png' in these locations (in order):" -ForegroundColor White
Write-Host "1. $projectRoot\Publics\Images\background-home-page.png" -ForegroundColor White
Write-Host "2. $projectRoot\Images\background-home-page.png" -ForegroundColor White
Write-Host "3. $projectRoot\background-home-page.png" -ForegroundColor White
Write-Host "4. Application startup directory + any of the above paths" -ForegroundColor White

Write-Host "`n📋 Recommended Setup:" -ForegroundColor Yellow
Write-Host "1. Place your background image in: Publics\Images\background-home-page.png" -ForegroundColor White
Write-Host "2. Image should be in PNG format" -ForegroundColor White
Write-Host "3. Recommended resolution: 1200x800 or higher" -ForegroundColor White
Write-Host "4. The image will be scaled to fit the window" -ForegroundColor White

Write-Host "`n🎨 Image Requirements:" -ForegroundColor Yellow
Write-Host "• Format: PNG (recommended) or other image formats supported by .NET" -ForegroundColor White
Write-Host "• Size: Any size (will be automatically scaled)" -ForegroundColor White
Write-Host "• Quality: High resolution recommended for better appearance" -ForegroundColor White
Write-Host "• Style: Corporate/professional look recommended" -ForegroundColor White

Write-Host "`n⚙️ How it works:" -ForegroundColor Yellow
Write-Host "• When user is in Guest Mode (not authenticated)" -ForegroundColor White
Write-Host "• The background image will be displayed on the Home tab" -ForegroundColor White
Write-Host "• If no image is found, a placeholder will be generated automatically" -ForegroundColor White
Write-Host "• When user authenticates (Admin Mode), normal controls are shown instead" -ForegroundColor White

$hasAnyBackgroundImage = $false
foreach ($dir in $imageDirectories) {
    $imagePath = Join-Path (Join-Path $projectRoot $dir) $backgroundImageName
    if (Test-Path $imagePath) {
        $hasAnyBackgroundImage = $true
        break
    }
}

if ($hasAnyBackgroundImage) {
    Write-Host "`n✅ Setup Status: READY" -ForegroundColor Green
    Write-Host "Background image found and will be used in guest mode." -ForegroundColor Green
} else {
    Write-Host "`n⚠️ Setup Status: NO CUSTOM IMAGE" -ForegroundColor Yellow
    Write-Host "No background image found. Application will generate a placeholder." -ForegroundColor Yellow
    Write-Host "To add a custom background:" -ForegroundColor White
    Write-Host "1. Get your background image (PNG format recommended)" -ForegroundColor White
    Write-Host "2. Rename it to 'background-home-page.png'" -ForegroundColor White
    Write-Host "3. Copy it to: $projectRoot\Publics\Images\" -ForegroundColor White
    Write-Host "4. Rebuild the application" -ForegroundColor White
}

Write-Host "`n🔧 Build and Test:" -ForegroundColor Yellow
Write-Host "1. Run: dotnet build" -ForegroundColor White
Write-Host "2. Start the application" -ForegroundColor White
Write-Host "3. Ensure you're in Guest Mode (not authenticated)" -ForegroundColor White
Write-Host "4. Check the Home tab for your background image" -ForegroundColor White

Write-Host "`n🎨 Background Image Setup Complete!" -ForegroundColor Cyan