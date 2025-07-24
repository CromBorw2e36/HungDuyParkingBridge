#!/usr/bin/env pwsh

# API Status Test Script for HungDuyParkingBridge
Write-Host "🔍 Testing API Status Endpoints..." -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan

$baseUrl = "http://localhost:5000"
$endpoints = @(
    "/api/status",
    "/api/health", 
    "/api/ping"
)

# Function to test an endpoint
function Test-Endpoint {
    param(
        [string]$Url,
        [string]$EndpointName
    )
    
    try {
        Write-Host "🌐 Testing: $EndpointName" -ForegroundColor Yellow
        
        $response = Invoke-RestMethod -Uri $Url -Method GET -TimeoutSec 10
        
        if ($response.Success -eq $true -and $response.Data.status -eq $true) {
            Write-Host "✅ SUCCESS: $EndpointName" -ForegroundColor Green
            Write-Host "   Server: $($response.Data.server)" -ForegroundColor White
            Write-Host "   Version: $($response.Data.version)" -ForegroundColor White
            Write-Host "   Uptime: $($response.Data.uptime)" -ForegroundColor White
            Write-Host "   Status: $($response.Data.status)" -ForegroundColor White
            return $true
        }
        else {
            Write-Host "❌ FAILED: $EndpointName - Invalid response" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "❌ FAILED: $EndpointName - $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Check if server is running
Write-Host "📡 Checking if server is running on $baseUrl..." -ForegroundColor Cyan

$serverRunning = $false
foreach ($endpoint in $endpoints) {
    $url = "$baseUrl$endpoint"
    if (Test-Endpoint -Url $url -EndpointName $endpoint) {
        $serverRunning = $true
        break
    }
}

if ($serverRunning) {
    Write-Host ""
    Write-Host "🎉 Server is running and status endpoints are working!" -ForegroundColor Green
    Write-Host ""
    
    # Test all endpoints
    Write-Host "🧪 Testing all status endpoints:" -ForegroundColor Cyan
    foreach ($endpoint in $endpoints) {
        $url = "$baseUrl$endpoint"
        Test-Endpoint -Url $url -EndpointName $endpoint
        Write-Host ""
    }
    
    # Show example curl commands
    Write-Host "💡 Example API calls:" -ForegroundColor Cyan
    Write-Host "   curl http://localhost:5000/api/status" -ForegroundColor White
    Write-Host "   curl http://localhost:5000/api/health" -ForegroundColor White
    Write-Host "   curl http://localhost:5000/api/ping" -ForegroundColor White
    
} else {
    Write-Host ""
    Write-Host "❌ Server is not running or not responding!" -ForegroundColor Red
    Write-Host "💡 Please start the application first:" -ForegroundColor Yellow
    Write-Host "   dotnet run" -ForegroundColor White
    Write-Host "   or run HungDuyParkingBridge.exe" -ForegroundColor White
}

Write-Host ""
Write-Host "📋 Expected Response Format:" -ForegroundColor Cyan
Write-Host @"
{
  "Success": true,
  "Message": "Server is running",
  "Data": {
    "status": true,
    "server": "HungDuyParkingBridge",
    "version": "1.0.2",
    "timestamp": "2024-01-01T12:00:00.000Z",
    "uptime": "0d 1h 30m 45s",
    "endpoints": [
      "GET /api/status",
      "GET /api/health",
      "GET /api/ping",
      "GET /api/files",
      "POST /api/files",
      "POST /upload/",
      "GET /download/{filename}"
    ]
  }
}
"@ -ForegroundColor White