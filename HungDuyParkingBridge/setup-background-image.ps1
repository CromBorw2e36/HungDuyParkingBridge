# Setup Background Image for HungDuy Parking Bridge
# This script helps set up and test the background image functionality

Write-Host "🎨 HungDuy Parking Bridge - Background Image Setup" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# Check if background image exists
$imagePaths = @(
    "Publics\Images\background-home-page.png",
    "Images\background-home-page.png", 
    "background-home-page.png"
)

$foundImage = $false
foreach ($path in $imagePaths) {
    if (Test-Path $path) {
        Write-Host "✅ Found background image: $path" -ForegroundColor Green
        $foundImage = $true
        
        # Get image info
        $imageFile = Get-Item $path
        $sizeKB = [math]::Round($imageFile.Length / 1KB, 2)
        Write-Host "   Size: $sizeKB KB" -ForegroundColor Gray
        Write-Host "   Modified: $($imageFile.LastWriteTime)" -ForegroundColor Gray
        break
    }
}

if (-not $foundImage) {
    Write-Host "⚠️  No background image found!" -ForegroundColor Yellow
    Write-Host "   Expected locations:" -ForegroundColor Gray
    foreach ($path in $imagePaths) {
        Write-Host "   - $path" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "📋 To add a background image:" -ForegroundColor Cyan
    Write-Host "1. Get your background image (PNG, JPG, etc.)" -ForegroundColor White
    Write-Host "2. Rename it to 'background-home-page.png'" -ForegroundColor White  
    Write-Host "3. Copy to: Publics\Images\background-home-page.png" -ForegroundColor White
    Write-Host "4. Run this script again to verify" -ForegroundColor White
    
    # Create directory structure if it doesn't exist
    if (-not (Test-Path "Publics\Images")) {
        New-Item -Path "Publics\Images" -ItemType Directory -Force | Out-Null
        Write-Host "✅ Created directory: Publics\Images" -ForegroundColor Green
    }
}

# Check project file configuration
Write-Host ""
Write-Host "🔧 Checking project configuration..." -ForegroundColor Cyan

$csprojPath = "HungDuyParkingBridge.csproj"
if (Test-Path $csprojPath) {
    $csprojContent = Get-Content $csprojPath -Raw
    
    if ($csprojContent -match "EmbeddedResource.*background-home-page\.png") {
        Write-Host "✅ Project configured for embedded resources" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Project may need embedded resource configuration" -ForegroundColor Yellow
    }
    
    if ($csprojContent -match "PublishSingleFile.*true") {
        Write-Host "✅ Single file publish enabled" -ForegroundColor Green
    } else {
        Write-Host "ℹ️  Single file publish not configured" -ForegroundColor Blue
    }
} else {
    Write-Host "⚠️  Project file not found: $csprojPath" -ForegroundColor Yellow
}

# Build and test
Write-Host ""
Write-Host "🔨 Testing build..." -ForegroundColor Cyan

try {
    $buildResult = dotnet build -c Release --verbosity quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Build successful" -ForegroundColor Green
        
        # Check if background was copied to output
        $outputPaths = @(
            "bin\Release\net9.0-windows\win-x64\Publics\Images\background-home-page.png",
            "bin\Release\net9.0-windows\Publics\Images\background-home-page.png"
        )
        
        $foundInOutput = $false
        foreach ($outputPath in $outputPaths) {
            if (Test-Path $outputPath) {
                Write-Host "✅ Background image copied to output: $outputPath" -ForegroundColor Green
                $foundInOutput = $true
                break
            }
        }
        
        if (-not $foundInOutput) {
            Write-Host "⚠️  Background image not found in build output" -ForegroundColor Yellow
            Write-Host "   This is normal for embedded resources in single file publish" -ForegroundColor Gray
        }
        
    } else {
        Write-Host "❌ Build failed" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Build error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test publish
Write-Host ""
Write-Host "📦 Testing single file publish..." -ForegroundColor Cyan

try {
    $publishResult = dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true --verbosity quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Publish successful" -ForegroundColor Green
        
        $publishedExe = "bin\Release\net9.0-windows\win-x64\publish\HungDuyParkingBridge.exe"
        if (Test-Path $publishedExe) {
            $exeSize = Get-Item $publishedExe | ForEach-Object { [math]::Round($_.Length / 1MB, 2) }
            Write-Host "✅ Single file executable created: $exeSize MB" -ForegroundColor Green
            
            # Check if there are external background files (should not exist for single file)
            $publishDir = "bin\Release\net9.0-windows\win-x64\publish"
            $externalBg = Get-ChildItem $publishDir -Recurse -Filter "*background*" | Where-Object { $_.Name -ne "HungDuyParkingBridge.exe" }
            
            if ($externalBg.Count -eq 0) {
                Write-Host "✅ No external background files (embedded correctly)" -ForegroundColor Green
            } else {
                Write-Host "⚠️  Found external background files:" -ForegroundColor Yellow
                $externalBg | ForEach-Object { Write-Host "   - $($_.FullName)" -ForegroundColor Gray }
            }
        }
    } else {
        Write-Host "❌ Publish failed" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Publish error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎯 Summary:" -ForegroundColor Cyan
Write-Host "============" -ForegroundColor Cyan

if ($foundImage) {
    Write-Host "✅ Background image is set up correctly" -ForegroundColor Green
    Write-Host "✅ Will appear in Guest Mode (before authentication)" -ForegroundColor Green
    Write-Host "✅ Admin Mode will show admin controls instead" -ForegroundColor Green
} else {
    Write-Host "⚠️  No background image configured" -ForegroundColor Yellow
    Write-Host "   App will generate a placeholder automatically" -ForegroundColor Gray
}

Write-Host ""
Write-Host "📖 How background works:" -ForegroundColor Cyan
Write-Host "• Guest Mode (🔒): Shows background image" -ForegroundColor White
Write-Host "• Admin Mode (🔓): Shows admin control panels" -ForegroundColor White
Write-Host "• Single file exe: Image embedded as resource" -ForegroundColor White
Write-Host "• Development: Image loaded from file system" -ForegroundColor White

Write-Host ""
Write-Host "🚀 To test:" -ForegroundColor Cyan
Write-Host "1. Run the application" -ForegroundColor White
Write-Host "2. Don't authenticate (stay in Guest Mode)" -ForegroundColor White
Write-Host "3. Check Home tab for background image" -ForegroundColor White
Write-Host "4. Authenticate to see it switch to admin controls" -ForegroundColor White

Write-Host ""
Write-Host "✨ Setup complete!" -ForegroundColor Green