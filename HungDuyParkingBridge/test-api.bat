@echo off
echo ========================================
echo Hung Duy Parking Bridge - API Status Test
echo ========================================

cd /d "%~dp0"

echo 🚀 Starting application...
start "" /min "bin\Debug\net9.0-windows\HungDuyParkingBridge.exe"

echo ⏳ Waiting for server to start...
timeout /t 5 /nobreak >nul

echo 🧪 Testing API Status Endpoints...
echo.

echo Testing /api/status:
curl -s "http://localhost:5000/api/status" || echo ❌ Failed to connect

echo.
echo Testing /api/health:
curl -s "http://localhost:5000/api/health" || echo ❌ Failed to connect

echo.
echo Testing /api/ping:
curl -s "http://localhost:5000/api/ping" || echo ❌ Failed to connect

echo.
echo ========================================
echo 💡 API Endpoints Available:
echo    GET http://localhost:5000/api/status
echo    GET http://localhost:5000/api/health  
echo    GET http://localhost:5000/api/ping
echo.
echo 📋 Expected Response: status: true
echo ========================================

pause