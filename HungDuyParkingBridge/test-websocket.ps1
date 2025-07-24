# WebSocket API Test Script for HungDuy Parking Bridge
# Tests native WebSocket endpoints and functionality

Write-Host "🔌 HungDuy Parking Bridge - WebSocket API Test" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

$httpUrl = "http://localhost:5000"
$wsUrl = "http://localhost:5001"
$errors = @()

function Test-Endpoint {
    param(
        [string]$Url,
        [string]$Description,
        [string]$Method = "GET"
    )
    
    Write-Host "`n📡 Testing: $Description" -ForegroundColor Yellow
    Write-Host "URL: $Url" -ForegroundColor Gray
    
    try {
        if ($Method -eq "GET") {
            $response = Invoke-RestMethod -Uri $Url -Method GET -TimeoutSec 10
        } else {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -TimeoutSec 10
        }
        
        Write-Host "✅ Success!" -ForegroundColor Green
        Write-Host "Response:" -ForegroundColor White
        $response | ConvertTo-Json -Depth 3 | Write-Host
        
        return $true
    }
    catch {
        Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
        $script:errors += "$Description - $($_.Exception.Message)"
        return $false
    }
}

function Test-HttpServer {
    Write-Host "`n🌐 Testing HTTP server..." -ForegroundColor Blue
    
    # Test HTTP API endpoints
    $success1 = Test-Endpoint "$httpUrl/api/status" "HTTP API Status"
    $success2 = Test-Endpoint "$httpUrl/api/health" "HTTP API Health"
    $success3 = Test-Endpoint "$httpUrl/api/ping" "HTTP API Ping"
    
    return $success1 -and $success2 -and $success3
}

function Test-WebSocketServer {
    Write-Host "`n🔌 Testing WebSocket server..." -ForegroundColor Blue
    
    # Test WebSocket HTTP endpoints
    $success1 = Test-Endpoint "$wsUrl/status" "WebSocket Status API"
    $success2 = Test-Endpoint "$wsUrl/" "WebSocket Test Page"
    
    return $success1 -and $success2
}

function Test-WebSocketNotification {
    Write-Host "`n📢 Testing WebSocket notification..." -ForegroundColor Blue
    
    try {
        $response = Invoke-RestMethod -Uri "$wsUrl/test" -Method POST -TimeoutSec 10
        Write-Host "✅ WebSocket test notification sent successfully!" -ForegroundColor Green
        Write-Host "Response:" -ForegroundColor White
        $response | ConvertTo-Json -Depth 2 | Write-Host
        return $true
    }
    catch {
        Write-Host "❌ WebSocket notification failed: $($_.Exception.Message)" -ForegroundColor Red
        $script:errors += "WebSocket Notification - $($_.Exception.Message)"
        return $false
    }
}

function Test-PortAvailability {
    Write-Host "`n🔍 Testing port availability..." -ForegroundColor Blue
    
    $ports = @(5000, 5001)
    $allPortsOk = $true
    
    foreach ($port in $ports) {
        try {
            $tcpClient = New-Object System.Net.Sockets.TcpClient
            $tcpClient.Connect("localhost", $port)
            Write-Host "✅ Port $port is open and responding" -ForegroundColor Green
            $tcpClient.Close()
        }
        catch {
            Write-Host "❌ Port $port is not responding" -ForegroundColor Red
            $script:errors += "Port $port - Not responding"
            $allPortsOk = $false
        }
    }
    
    return $allPortsOk
}

function Show-Results {
    Write-Host "`n" + "="*50 -ForegroundColor Cyan
    Write-Host "📊 TEST RESULTS SUMMARY" -ForegroundColor Cyan
    Write-Host "="*50 -ForegroundColor Cyan
    
    if ($errors.Count -eq 0) {
        Write-Host "✅ All tests passed successfully!" -ForegroundColor Green
        Write-Host "`n🎉 WebSocket functionality is working correctly!" -ForegroundColor Green
        Write-Host "`n📋 Available services:" -ForegroundColor Yellow
        Write-Host "   • HTTP Server: http://localhost:5000" -ForegroundColor White
        Write-Host "   • WebSocket Server: ws://localhost:5001/ws" -ForegroundColor White
        Write-Host "   • Status API: http://localhost:5001/status" -ForegroundColor White
        Write-Host "   • Test Page: http://localhost:5001/" -ForegroundColor White
        Write-Host "   • Local Test: .\websocket-test.html" -ForegroundColor White
    } else {
        Write-Host "❌ Some tests failed:" -ForegroundColor Red
        foreach ($error in $errors) {
            Write-Host "   • $error" -ForegroundColor Red
        }
        
        Write-Host "`n💡 Troubleshooting:" -ForegroundColor Yellow
        Write-Host "   1. Make sure HungDuyParkingBridge application is running" -ForegroundColor White
        Write-Host "   2. Check if ports 5000 and 5001 are not blocked by firewall" -ForegroundColor White
        Write-Host "   3. Verify no other applications are using these ports" -ForegroundColor White
        Write-Host "   4. Try restarting the application" -ForegroundColor White
        Write-Host "   5. Check Windows Defender or antivirus settings" -ForegroundColor White
    }
}

function Open-TestPage {
    $testPagePath = Join-Path $PSScriptRoot "websocket-test.html"
    if (Test-Path $testPagePath) {
        Write-Host "`n🌐 Opening WebSocket test page..." -ForegroundColor Blue
        try {
            Start-Process $testPagePath
            Write-Host "✅ Test page opened in default browser" -ForegroundColor Green
        }
        catch {
            Write-Host "❌ Could not open test page: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "❌ Test page not found at: $testPagePath" -ForegroundColor Red
        Write-Host "💡 You can also access the test page at: http://localhost:5001/" -ForegroundColor Yellow
    }
}

function Show-WebSocketDemo {
    Write-Host "`n🎯 WebSocket Connection Demo:" -ForegroundColor Blue
    Write-Host "JavaScript code to connect to WebSocket:" -ForegroundColor Gray
    Write-Host @"
const ws = new WebSocket('ws://localhost:5001/ws');
ws.onopen = () => console.log('Connected!');
ws.onmessage = (event) => {
    const data = JSON.parse(event.data);
    console.log('Received:', data);
};
ws.send(JSON.stringify({ type: 'ping' }));
"@ -ForegroundColor White
}

# Main execution
Write-Host "`n⏳ Starting WebSocket API tests..." -ForegroundColor Blue

# Run tests
$portTest = Test-PortAvailability
$httpTest = Test-HttpServer
$wsTest = Test-WebSocketServer
$notificationTest = Test-WebSocketNotification

# Show results
Show-Results

# Show demo code
Show-WebSocketDemo

# Ask user if they want to open the test page
Write-Host "`n" -ForegroundColor White
$openPage = Read-Host "Do you want to open the WebSocket test page? (y/n)"
if ($openPage -eq 'y' -or $openPage -eq 'Y') {
    Open-TestPage
}

Write-Host "`n🏁 WebSocket API test completed!" -ForegroundColor Cyan
Write-Host "💡 For real-time testing, use the HTML test page or connect via JavaScript" -ForegroundColor Yellow