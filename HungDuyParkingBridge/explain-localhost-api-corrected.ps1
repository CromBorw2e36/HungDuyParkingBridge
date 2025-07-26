# 🔍 HungDuy Parking Bridge - CORRECTED Server Architecture Explanation
# This script explains the ACTUAL flow of when the HTTP server starts

Write-Host "🔍 HungDuy Parking Bridge - CORRECTED Server Architecture" -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "✅ CORRECTED FLOW - WHEN HTTP SERVER STARTS:" -ForegroundColor Yellow
Write-Host "=============================================" -ForegroundColor Yellow

Write-Host ""
Write-Host "🔄 ACTUAL APPLICATION FLOW:" -ForegroundColor Green
Write-Host "1. 🚀 App starts (HungDuyParkingBridge.exe)" -ForegroundColor White
Write-Host "2. 📋 MainForm_Load() executes" -ForegroundColor White
Write-Host "3. 🔍 Check: if (HDParkingConst.IsAdminAuthenticated)" -ForegroundColor White
Write-Host "   🔒 If FALSE (Guest): Services NOT started" -ForegroundColor Red
Write-Host "   🔓 If TRUE (Admin): await _receiver.Start() called" -ForegroundColor Green
Write-Host "4. 🌐 HTTP Server starts immediately on app launch (if admin)" -ForegroundColor White

Write-Host ""
Write-Host "📝 CRITICAL CORRECTION:" -ForegroundColor Red
Write-Host "========================" -ForegroundColor Red
Write-Host "❌ WRONG: 'Switch to Admin Mode → HTTP server starts'" -ForegroundColor Red
Write-Host "✅ RIGHT: 'App runs → HTTP server starts (if already admin)'" -ForegroundColor Green

Write-Host ""
Write-Host "🎯 TWO SCENARIOS:" -ForegroundColor Cyan
Write-Host "=================" -ForegroundColor Cyan

Write-Host ""
Write-Host "🔒 SCENARIO 1: Guest Mode Start (Default)" -ForegroundColor Yellow
Write-Host "1. App starts → IsAdminAuthenticated = FALSE" -ForegroundColor Gray
Write-Host "2. MainForm_Load → Services NOT started" -ForegroundColor Gray
Write-Host "3. Status: 'Guest mode - Services not started'" -ForegroundColor Gray
Write-Host "4. Later: Authenticate → _receiver.Start() called" -ForegroundColor Gray
Write-Host "5. HTTP server becomes available" -ForegroundColor Gray

Write-Host ""
Write-Host "🔓 SCENARIO 2: Admin Mode Start (If Pre-authenticated)" -ForegroundColor Yellow
Write-Host "1. App starts → IsAdminAuthenticated = TRUE" -ForegroundColor Gray
Write-Host "2. MainForm_Load → await _receiver.Start() immediately" -ForegroundColor Gray
Write-Host "3. Status: 'Running - HTTP and WebSocket servers started'" -ForegroundColor Gray
Write-Host "4. HTTP server available from app start" -ForegroundColor Gray

Write-Host ""
Write-Host "🔧 CODE ANALYSIS:" -ForegroundColor Cyan
Write-Host "=================" -ForegroundColor Cyan

Write-Host "private async void MainForm_Load(object sender, EventArgs e)" -ForegroundColor White
Write-Host "{" -ForegroundColor White
Write-Host "    SetupTray();" -ForegroundColor Gray
Write-Host "    AddToStartup();" -ForegroundColor Gray
Write-Host "    " -ForegroundColor Gray
Write-Host "    // Only start services if authenticated" -ForegroundColor Green
Write-Host "    if (HDParkingConst.IsAdminAuthenticated)" -ForegroundColor Yellow
Write-Host "    {" -ForegroundColor White
Write-Host "        await _receiver.Start();  // ← HTTP SERVER STARTS HERE" -ForegroundColor Red
Write-Host "        UpdateStatus('Running - HTTP and WebSocket servers started');" -ForegroundColor Gray
Write-Host "    }" -ForegroundColor White
Write-Host "    else" -ForegroundColor White
Write-Host "    {" -ForegroundColor White
Write-Host "        UpdateStatus('Guest mode - Services not started');" -ForegroundColor Gray
Write-Host "    }" -ForegroundColor White
Write-Host "}" -ForegroundColor White

Write-Host ""
Write-Host "🎮 AUTHENTICATION FLOW:" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan

Write-Host ""
Write-Host "📱 When User Authenticates (After App Start):" -ForegroundColor Yellow
Write-Host "1. User clicks Help > Private Key > Authentication" -ForegroundColor Gray
Write-Host "2. AuthenticationToolStripMenuItem_Click() called" -ForegroundColor Gray
Write-Host "3. Private key dialog opens" -ForegroundColor Gray
Write-Host "4. If success → HDParkingConst.SetAdminAccess(true)" -ForegroundColor Gray
Write-Host "5. UpdateAuthenticationStatus() called" -ForegroundColor Gray
Write-Host "6. RefreshTabsBasedOnAuth() called" -ForegroundColor Gray
Write-Host "7. BUT: _receiver.Start() is NOT called here!" -ForegroundColor Red
Write-Host "8. Server starts only via restart button or app restart" -ForegroundColor Yellow

Write-Host ""
Write-Host "🚨 IMPORTANT FINDING:" -ForegroundColor Red
Write-Host "=====================" -ForegroundColor Red
Write-Host "The HTTP server does NOT automatically start when authenticating!" -ForegroundColor Red
Write-Host "It only starts:" -ForegroundColor Yellow
Write-Host "• At app launch (if already admin)" -ForegroundColor Gray
Write-Host "• Via 'Restart Server' button (in admin mode)" -ForegroundColor Gray
Write-Host "• Via tray menu 'Khởi động lại' (restart)" -ForegroundColor Gray

Write-Host ""
Write-Host "🔍 CHECKING CURRENT STATUS:" -ForegroundColor Cyan
Write-Host "============================" -ForegroundColor Cyan

# Check if ports are in use (indicating your app is running)
function Test-Port {
    param([int]$Port, [string]$Service)
    
    try {
        $connection = Test-NetConnection -ComputerName "localhost" -Port $Port -WarningAction SilentlyContinue
        if ($connection.TcpTestSucceeded) {
            Write-Host "✅ Port $Port ($Service) - ACTIVE" -ForegroundColor Green
            return $true
        } else {
            Write-Host "❌ Port $Port ($Service) - INACTIVE" -ForegroundColor Red
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
    Write-Host "🎉 HTTP SERVER IS RUNNING!" -ForegroundColor Green
    Write-Host "   This means either:" -ForegroundColor Yellow
    Write-Host "   • App started in Admin Mode, OR" -ForegroundColor Gray
    Write-Host "   • Server was manually started via Restart button" -ForegroundColor Gray
    
    if ($httpActive) {
        Write-Host ""
        Write-Host "🧪 Testing API endpoint:" -ForegroundColor Yellow
        try {
            $response = Invoke-RestMethod -Uri "http://localhost:5000/api/status" -Method GET -TimeoutSec 3
            Write-Host "✅ API Response:" -ForegroundColor Green
            Write-Host "   Server: $($response.Data.server)" -ForegroundColor Gray
            Write-Host "   Version: $($response.Data.version)" -ForegroundColor Gray
            Write-Host "   Status: $($response.Data.status)" -ForegroundColor Gray
        } catch {
            Write-Host "⚠️  API test failed: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "💤 HTTP SERVER IS NOT RUNNING" -ForegroundColor Yellow
    Write-Host "   This means either:" -ForegroundColor Yellow
    Write-Host "   • App is not running, OR" -ForegroundColor Gray
    Write-Host "   • App is running in Guest Mode" -ForegroundColor Gray
    Write-Host "   • App needs server restart via button" -ForegroundColor Gray
}

Write-Host ""
Write-Host "🎯 SUMMARY:" -ForegroundColor Cyan
Write-Host "============" -ForegroundColor Cyan

Write-Host ""
Write-Host "✅ CORRECTED UNDERSTANDING:" -ForegroundColor Green
Write-Host "• localhost:5000 = YOUR app's HTTP server" -ForegroundColor White
Write-Host "• Server starts at app launch IF already admin" -ForegroundColor White
Write-Host "• Authentication alone does NOT start server" -ForegroundColor White  
Write-Host "• Manual restart needed after authentication" -ForegroundColor White
Write-Host "• App = Desktop UI + HTTP Server + WebSocket Server" -ForegroundColor White

Write-Host ""
Write-Host "🚀 TO START SERVER AFTER AUTHENTICATION:" -ForegroundColor Cyan
Write-Host "1. Authenticate with private key (012233)" -ForegroundColor White
Write-Host "2. Click 'Restart Server' button, OR" -ForegroundColor White
Write-Host "3. Use tray menu 'Khởi động lại', OR" -ForegroundColor White
Write-Host "4. Restart the entire application" -ForegroundColor White

Write-Host ""
Write-Host "✨ CONCLUSION:" -ForegroundColor Green
Write-Host "You were RIGHT to correct me! 🎯" -ForegroundColor Green
Write-Host "App runs FIRST, then server starts (if conditions met)" -ForegroundColor Green