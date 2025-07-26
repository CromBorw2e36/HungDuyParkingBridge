# 🔍 HungDuy Parking Bridge - Server Architecture Explanation
# This script explains why localhost:5000 is YOUR app, not remote

Write-Host "🔍 HungDuy Parking Bridge - Server Architecture Explanation" -ForegroundColor Cyan
Write-Host "=========================================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "❓ WHY localhost:5000 IS YOUR APP (NOT REMOTE):" -ForegroundColor Yellow
Write-Host "===============================================" -ForegroundColor Yellow

Write-Host ""
Write-Host "1. 🏗️  YOUR APPLICATION IS A SERVER" -ForegroundColor Green
Write-Host "   ✅ HungDuyParkingBridge.exe = HTTP Server + Desktop App"
Write-Host "   ✅ Uses .NET HttpListener to create web server"
Write-Host "   ✅ Listens on port 5000 for HTTP requests"
Write-Host "   ✅ Listens on port 5001 for WebSocket connections"

Write-Host ""
Write-Host "2. 🔌 SERVICES IN YOUR APP:" -ForegroundColor Green
Write-Host "   📁 FileReceiverService    → HTTP Server (port 5000)"
Write-Host "   🔌 WebSocketService       → WebSocket Server (port 5001)"
Write-Host "   📂 FileApiService         → API endpoints (/api/status, /api/files, etc.)"
Write-Host "   📤 FileUploadHandler      → Handle file uploads"
Write-Host "   📥 FileDownloadHandler    → Handle file downloads"

Write-Host ""
Write-Host "3. 🚦 AUTHENTICATION CONTROL:" -ForegroundColor Green
Write-Host "   🔒 Guest Mode:  Services NOT started"
Write-Host "   🔓 Admin Mode:  Services started and running"

Write-Host ""
Write-Host "4. 🌐 API ENDPOINTS (when running):" -ForegroundColor Green
Write-Host "   GET  http://localhost:5000/api/status    → Server status"
Write-Host "   GET  http://localhost:5000/api/files     → List files"
Write-Host "   POST http://localhost:5000/upload/       → Upload files"
Write-Host "   GET  http://localhost:5000/download/     → Download files"
Write-Host "   GET  ws://localhost:5001/ws              → WebSocket connection"

Write-Host ""
Write-Host "🔍 CHECKING CURRENT STATUS:" -ForegroundColor Cyan
Write-Host "============================" -ForegroundColor Cyan

# Check if ports are in use (indicating your app is running)
function Test-Port {
    param([int]$Port, [string]$Service)
    
    try {
        $connection = Test-NetConnection -ComputerName "localhost" -Port $Port -WarningAction SilentlyContinue
        if ($connection.TcpTestSucceeded) {
            Write-Host "✅ Port $Port ($Service) - ACTIVE (Your app is running)" -ForegroundColor Green
            return $true
        } else {
            Write-Host "❌ Port $Port ($Service) - INACTIVE (Your app not running)" -ForegroundColor Red
            return $false
        }
    } catch {
        Write-Host "❓ Port $Port ($Service) - Cannot test" -ForegroundColor Yellow
        return $false
    }
}

$httpActive = Test-Port -Port 5000 -Service "HTTP API"
$wsActive = Test-Port -Port 5001 -Service "WebSocket"

Write-Host ""
if ($httpActive -or $wsActive) {
    Write-Host "🎉 YOUR APPLICATION IS RUNNING!" -ForegroundColor Green
    Write-Host "   The localhost:5000 endpoints are served by YOUR app" -ForegroundColor Green
    
    if ($httpActive) {
        Write-Host ""
        Write-Host "🧪 Testing YOUR API endpoint:" -ForegroundColor Yellow
        try {
            $response = Invoke-RestMethod -Uri "http://localhost:5000/api/status" -Method GET -TimeoutSec 3
            Write-Host "✅ API Response received:" -ForegroundColor Green
            Write-Host "   Server: $($response.Data.server)" -ForegroundColor Gray
            Write-Host "   Version: $($response.Data.version)" -ForegroundColor Gray
            Write-Host "   Status: $($response.Data.status)" -ForegroundColor Gray
            Write-Host "   Message: $($response.Message)" -ForegroundColor Gray
        } catch {
            Write-Host "⚠️  API test failed: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "💤 YOUR APPLICATION IS NOT RUNNING" -ForegroundColor Yellow
    Write-Host "   Start HungDuyParkingBridge.exe and authenticate as admin" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "📖 HOW IT WORKS:" -ForegroundColor Cyan
Write-Host "=================" -ForegroundColor Cyan

Write-Host ""
Write-Host "🔄 Application Flow:" -ForegroundColor White
Write-Host "1. Start HungDuyParkingBridge.exe" -ForegroundColor Gray
Write-Host "2. App opens in Guest Mode (servers NOT running)" -ForegroundColor Gray
Write-Host "3. Enter private key to authenticate" -ForegroundColor Gray
Write-Host "4. Switch to Admin Mode → FileReceiverService.Start() called" -ForegroundColor Gray
Write-Host "5. HttpListener starts on port 5000" -ForegroundColor Gray
Write-Host "6. WebSocket service starts on port 5001" -ForegroundColor Gray
Write-Host "7. Now localhost:5000/api/status responds!" -ForegroundColor Gray

Write-Host ""
Write-Host "💡 KEY POINTS:" -ForegroundColor White
Write-Host "• localhost:5000 = YOUR app's built-in web server" -ForegroundColor Gray
Write-Host "• NOT an external/remote service" -ForegroundColor Gray
Write-Host "• Only active when authenticated as admin" -ForegroundColor Gray
Write-Host "• Desktop app + web server in one executable" -ForegroundColor Gray

Write-Host ""
Write-Host "🚀 TO TEST EVERYTHING:" -ForegroundColor Cyan
Write-Host "=======================" -ForegroundColor Cyan

Write-Host "1. Run: .\bin\Release\net9.0-windows\win-x64\publish\HungDuyParkingBridge.exe" -ForegroundColor White
Write-Host "2. Authenticate with private key (default: 012233)" -ForegroundColor White
Write-Host "3. Check Admin Mode shows server URLs" -ForegroundColor White
Write-Host "4. Test API: curl http://localhost:5000/api/status" -ForegroundColor White
Write-Host "5. Test WebSocket: Open http://localhost:5001/ in browser" -ForegroundColor White

Write-Host ""
Write-Host "✨ CONCLUSION:" -ForegroundColor Green
Write-Host "localhost:5000/api/status is YOUR application's API endpoint!" -ForegroundColor Green
Write-Host "Not remote - it's your own built-in web server! 🎯" -ForegroundColor Green