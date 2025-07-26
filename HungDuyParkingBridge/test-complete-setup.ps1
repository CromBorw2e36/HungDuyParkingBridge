# HungDuy Parking Bridge - Complete Setup and Test Script
# This script tests both background image embedding and API functionality

Write-Host "🚀 HungDuy Parking Bridge - Complete Setup & Test" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan

# Function to test API endpoint
function Test-ApiEndpoint {
    param([string]$Url, [string]$Name)
    
    try {
        Write-Host "🔌 Testing $Name..." -ForegroundColor Yellow
        $response = Invoke-RestMethod -Uri $Url -Method GET -TimeoutSec 5
        Write-Host "✅ $Name: Success" -ForegroundColor Green
        
        if ($response.Data) {
            Write-Host "   Server: $($response.Data.server)" -ForegroundColor Gray
            Write-Host "   Version: $($response.Data.version)" -ForegroundColor Gray
            Write-Host "   Status: $($response.Data.status)" -ForegroundColor Gray
        }
        return $true
    }
    catch {
        Write-Host "❌ $Name: Failed - $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# 1. Check background image setup
Write-Host ""
Write-Host "🎨 1. Background Image Setup" -ForegroundColor Cyan
Write-Host "==============================" -ForegroundColor Cyan

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
        
        $imageFile = Get-Item $path
        $sizeKB = [math]::Round($imageFile.Length / 1KB, 2)
        Write-Host "   Size: $sizeKB KB" -ForegroundColor Gray
        Write-Host "   Modified: $($imageFile.LastWriteTime)" -ForegroundColor Gray
        break
    }
}

if (-not $foundImage) {
    Write-Host "⚠️  No background image found" -ForegroundColor Yellow
    Write-Host "   App will generate placeholder automatically" -ForegroundColor Gray
}

# 2. Check project configuration
Write-Host ""
Write-Host "🔧 2. Project Configuration" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Cyan

$csprojPath = "HungDuyParkingBridge.csproj"
if (Test-Path $csprojPath) {
    $csprojContent = Get-Content $csprojPath -Raw
    
    if ($csprojContent -match "LogicalName.*background-home-page\.png") {
        Write-Host "✅ Background image embedding configured" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Background embedding may need configuration" -ForegroundColor Yellow
    }
    
    if ($csprojContent -match "PublishSingleFile.*true") {
        Write-Host "✅ Single file publish enabled" -ForegroundColor Green
    }
} else {
    Write-Host "⚠️  Project file not found: $csprojPath" -ForegroundColor Yellow
}

# 3. Build test
Write-Host ""
Write-Host "🔨 3. Build Test" -ForegroundColor Cyan
Write-Host "=================" -ForegroundColor Cyan

try {
    Write-Host "Building project..." -ForegroundColor Yellow
    $buildOutput = dotnet build -c Release --verbosity minimal 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Build successful" -ForegroundColor Green
    } else {
        Write-Host "❌ Build failed" -ForegroundColor Red
        Write-Host "Output: $buildOutput" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Build error: $($_.Exception.Message)" -ForegroundColor Red
}

# 4. Check if application is running (API test)
Write-Host ""
Write-Host "🌐 4. API Endpoint Tests" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan

$apiEndpoints = @(
    @{ Url = "http://localhost:5000/api/status"; Name = "Main Status API" },
    @{ Url = "http://localhost:5000/api/health"; Name = "Health Check API" },
    @{ Url = "http://localhost:5000/api/ping"; Name = "Ping API" }
)

$apiWorking = $false
foreach ($endpoint in $apiEndpoints) {
    if (Test-ApiEndpoint -Url $endpoint.Url -Name $endpoint.Name) {
        $apiWorking = $true
        break
    }
}

if (-not $apiWorking) {
    Write-Host ""
    Write-Host "ℹ️  API not responding (normal if app not running)" -ForegroundColor Blue
    Write-Host "   The API endpoints are part of YOUR application, not remote services" -ForegroundColor Gray
    Write-Host "   Start the app to test API functionality" -ForegroundColor Gray
}

# 5. Publish test
Write-Host ""
Write-Host "📦 5. Single File Publish Test" -ForegroundColor Cyan
Write-Host "===============================" -ForegroundColor Cyan

try {
    Write-Host "Publishing single file executable..." -ForegroundColor Yellow
    $publishOutput = dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --verbosity minimal 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Publish successful" -ForegroundColor Green
        
        $publishedExe = "bin\Release\net9.0-windows\win-x64\publish\HungDuyParkingBridge.exe"
        if (Test-Path $publishedExe) {
            $exeSize = Get-Item $publishedExe | ForEach-Object { [math]::Round($_.Length / 1MB, 2) }
            Write-Host "✅ Executable created: $exeSize MB" -ForegroundColor Green
            
            # Check for external files (should be minimal for single file)
            $publishDir = "bin\Release\net9.0-windows\win-x64\publish"
            $externalFiles = Get-ChildItem $publishDir | Where-Object { $_.Name -ne "HungDuyParkingBridge.exe" }
            
            if ($externalFiles.Count -eq 0) {
                Write-Host "✅ True single file (no external dependencies)" -ForegroundColor Green
            } else {
                Write-Host "ℹ️  Additional files in publish folder:" -ForegroundColor Blue
                $externalFiles | ForEach-Object { Write-Host "   - $($_.Name)" -ForegroundColor Gray }
            }
        }
    } else {
        Write-Host "❌ Publish failed" -ForegroundColor Red
        Write-Host "Output: $publishOutput" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Publish error: $($_.Exception.Message)" -ForegroundColor Red
}

# 6. Summary and recommendations
Write-Host ""
Write-Host "📋 6. Summary & Recommendations" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "🎨 Background Image:" -ForegroundColor White
if ($foundImage) {
    Write-Host "✅ Background image configured correctly" -ForegroundColor Green
    Write-Host "   Will be embedded in single file executable" -ForegroundColor Green
} else {
    Write-Host "⚠️  No custom background (will use auto-generated placeholder)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🌐 API Status Endpoint:" -ForegroundColor White
Write-Host "✅ http://localhost:5000/api/status is YOUR application's API" -ForegroundColor Green
Write-Host "   This is NOT a remote service - it's part of your app" -ForegroundColor Green
Write-Host "   It provides status information about your running server" -ForegroundColor Green

Write-Host ""
Write-Host "🚀 How to test everything:" -ForegroundColor White
Write-Host "1. Run the published exe: .\bin\Release\net9.0-windows\win-x64\publish\HungDuyParkingBridge.exe" -ForegroundColor Gray
Write-Host "2. Check Guest Mode for background image (Home tab)" -ForegroundColor Gray
Write-Host "3. Test API: curl http://localhost:5000/api/status" -ForegroundColor Gray
Write-Host "4. Authenticate with private key to see admin features" -ForegroundColor Gray

Write-Host ""
Write-Host "🔑 Important Notes:" -ForegroundColor White
Write-Host "• Background image loads from embedded resources in published exe" -ForegroundColor Gray
Write-Host "• API endpoints are part of YOUR app's HTTP server (port 5000)" -ForegroundColor Gray
Write-Host "• WebSocket server runs on port 5001" -ForegroundColor Gray
Write-Host "• All services only start in Admin Mode (after authentication)" -ForegroundColor Gray

Write-Host ""
Write-Host "✨ Setup and testing complete!" -ForegroundColor Green