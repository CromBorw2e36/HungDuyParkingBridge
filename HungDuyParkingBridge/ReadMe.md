Lệnh publish .exe với icon embedded
cd .\HungDuyParkingBridge\

# Standard publish với embedded icon
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

# Hoặc sử dụng publish profile tùy chỉnh
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=embedded

Di chuyển đến thư mục để lấy file .exe đã publish
K:\Project\HungDuyCoLTD\HungDuyParkingBridge\HungDuyParkingBridge\bin\Release\net9.0-windows\win-x64\publish
cd .\bin\Release\net9.0-windows\win-x64\publish
explorer .

Di duyển đến thư mục project từ publish
cd ../../../../../

## 🎨 Giải quyết vấn đề Icon mới

### Vấn đề thường gặp khi cập nhật icon:

1. **Build Cache**: Build cache cũ vẫn sử dụng icon cũ
2. **Resource Embedding**: Icon mới chưa được embed đúng cách
3. **File Path**: Icon file không được tìm thấy ở đúng vị trí

### ✅ Giải pháp đã cập nhật (v1.0.2):

#### 1. **Cấu hình Project mới**:
- Icon được embed như `EmbeddedResource`
- Icon được copy như `Content` 
- Cache invalidation tự động

#### 2. **Enhanced ResourceHelper**:
- Debug logging chi tiết
- Icon caching với reload capability
- Comprehensive fallback paths
- Real-time debugging

#### 3. **Refresh Commands**:

**Quick Refresh (sau khi update icon):**# Chạy script tự động
```bash
.\refresh-icon.bat
```
# Hoặc thủ công:
```bash
dotnet clean
dotnet build -c Debug
```
**Full Clean Build:**# Xóa hoàn toàn cache
```bash
rmdir /s /q bin obj
dotnet restore
dotnet build#### 4. **Debug Icon Loading**:
Khi chạy app, check console output để xem:
- Embedded resources được tìm thấy
- File system paths
- Icon loading status

### 🔍 Kiểm tra Icon:

#### Debug Mode:dotnet run
# Check console cho icon loading debug info
#### Published Version:dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

# Check embedded resources:
ildasm HungDuyParkingBridge.exe
### 🚨 Troubleshooting:

#### Icon không hiển thị:
1. **Check file tồn tại**: `logoTapDoan.ico` phải ở project root
2. **Check file format**: Icon phải là .ico format hợp lệ
3. **Rebuild clean**: Chạy `refresh-icon.bat`
4. **Check debug output**: Xem console khi chạy app

#### Icon bị lỗi:
1. **Validate icon file**: Thử mở icon bằng image viewer
2. **Check size**: Icon nên có multiple sizes (16x16, 32x32, 48x48)
3. **Try different file**: Test với icon khác

#### Published app không có icon:
1. **Check embedding**: Verify EmbeddedResource trong .csproj
2. **Force rebuild**: Clean solution rồi rebuild
3. **Check resources**: Dùng ildasm để verify

### 📋 Checklist sau khi update icon:

- [ ] File `logoTapDoan.ico` ở project root
- [ ] Chạy `refresh-icon.bat` hoặc `dotnet clean && dotnet build`
- [ ] Test debug version
- [ ] Check console output
- [ ] Test published version
- [ ] Verify tray icon
- [ ] Verify window icons

## 🌐 API Status Endpoint (NEW v1.0.2)

### Status API luôn trả về `true`:

#### **Available Endpoints:**
- `GET http://localhost:5000/api/status`
- `GET http://localhost:5000/api/health` 
- `GET http://localhost:5000/api/ping`

#### **Response Format:**{
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
#### **Testing API:**

**PowerShell Test:**# Chạy script test tự động
.\test-api-status.ps1

# Manual test
Invoke-RestMethod -Uri "http://localhost:5000/api/status" -Method GET
**cURL Commands:**# Test status endpoint
curl http://localhost:5000/api/status

# Test health endpoint  
curl http://localhost:5000/api/healthx

# Test ping endpoint
curl http://localhost:5000/api/ping
#### **Features:**
- ✅ **Always returns true**: Status luôn là `true` khi server running
- ✅ **Server info**: Tên server, version, uptime
- ✅ **CORS enabled**: Cross-origin requests supported
- ✅ **Multiple endpoints**: 3 endpoint aliases for flexibility
- ✅ **Comprehensive data**: Timestamp, uptime, available endpoints list

#### **Use Cases:**
- **Health monitoring**: Check if server is alive
- **Load balancer**: Health check endpoint
- **Service discovery**: Verify service availability
- **Debugging**: Quick server status verification

### 🧪 Testing Status API:

1. **Start Application**: Chạy app hoặc `dotnet run`
2. **Run Test Script**: `.\test-api-status.ps1`
3. **Check Response**: Verify `status: true` in response
4. **Monitor Uptime**: Track server uptime information

## 🔌 WebSocket Real-time Communication (NEW v1.0.2)

### ⚠️ **IMPORTANT: Background Service Behavior**

**WebSocket service chạy hoàn toàn trong background!**

- ✅ **Continues running khi close main window**
- ✅ **Runs in system tray independently** 
- ✅ **Only stops when explicitly exit from tray menu**
- ✅ **Restart from tray menu keeps service running**

### 🚀 Native WebSocket Implementation:

HungDuy Parking Bridge hiện hỗ trợ **WebSocket real-time communication** cho việc thông báo file upload/download và trạng thái server.

#### **🚀 WebSocket Features:**
- ✅ **Native WebSocket**: Sử dụng System.Net.WebSockets của .NET 9
- ✅ **Background Service**: Chạy độc lập trong background
- ✅ **Real-time notifications**: File upload/download events
- ✅ **Bi-directional communication**: Client ↔ Server messaging
- ✅ **Multiple clients**: Hỗ trợ nhiều client kết nối đồng thời
- ✅ **JSON messaging**: Structured message protocol
- ✅ **Auto-reconnection**: Client tự động kết nối lại
- ✅ **System tray integration**: Control via tray menu

#### **📡 Available Endpoints:**

**WebSocket Connection:**ws://localhost:5001/ws
**HTTP Status APIs:**
- `GET http://localhost:5001/status` - WebSocket server status
- `GET http://localhost:5001/` - Built-in test page
- `POST http://localhost:5001/test` - Trigger test notification

#### **💬 Message Protocol:**

**Client → Server Messages:**// Ping server
{ "type": "ping" }

// Send message to all clients  
{ "type": "message", "content": "Hello everyone!" }

// Request server status
{ "type": "status" }
**Server → Client Messages:**// Welcome message
{
  "type": "connected",
  "message": "Connected to HungDuy Parking Bridge WebSocket",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "server": "HungDuyParkingBridge v1.0.2"
}

// File notification
{
  "type": "fileNotification", 
  "fileName": "document.pdf",
  "action": "uploaded",
  "fileSize": 1024,
  "timestamp": "2024-01-01T12:00:00.000Z"
}

// Broadcast message
{
  "type": "message",
  "sender": "user", 
  "content": "Hello everyone!",
  "timestamp": "2024-01-01T12:00:00.000Z"
}

// Server status
{
  "type": "status",
#### **🧪 Testing WebSocket:**

**1. PowerShell Test Script:**.\test-websocket.ps1
**2. Built-in Test Page:**# Open browser to:
http://localhost:5001/
**3. Local HTML Test:**# Open file in browser:
.\websocket-test.html
**4. JavaScript Client Example:**const ws = new WebSocket('ws://localhost:5001/ws');

ws.onopen = () => {
    console.log('Connected to WebSocket');
    // Send ping
    ws.send(JSON.stringify({ type: 'ping' }));
};

ws.onmessage = (event) => {
    const data = JSON.parse(event.data);
    console.log('Received:', data);
    
    if (data.type === 'fileNotification') {
        console.log(`File ${data.action}: ${data.fileName}`);
    }
};

// Send message to all clients
ws.send(JSON.stringify({ 
    type: 'message', 
    content: 'Hello from JavaScript!' 
}));
#### **🔗 Integration với File Operations:**

WebSocket tự động gửi thông báo khi:
- ✅ **File Upload**: Client upload file qua HTTP API
- ✅ **File Download**: Client download file
- ✅ **System Events**: Server start/stop, client connect/disconnect

#### **📊 WebSocket Status Dashboard:**

Trong MainForm có tab **"🔌 WebSocket"** với:
- ✅ **Connection status**: Trạng thái WebSocket server
- ✅ **Test controls**: Send test messages và notifications
- ✅ **Client count**: Số lượng client đang kết nối
- ✅ **Real-time logging**: Debug WebSocket events
- ✅ **Background service info**: Cảnh báo service chạy background

#### **🛠️ Development Usage:**

**Frontend Integration:**// Kết nối WebSocket cho real-time UI updates
const ws = new WebSocket('ws://localhost:5001/ws');

ws.onmessage = (event) => {
    const data = JSON.parse(event.data);
    
    if (data.type === 'fileNotification') {
        // Update file list in UI
        updateFileList();
        showNotification(`File ${data.action}: ${data.fileName}`);
    }
};
**Monitoring Integration:**# Check WebSocket status via HTTP
curl http://localhost:5001/status

# Trigger test notification
curl -X POST http://localhost:5001/test
#### **🎮 System Tray Control:**

**Available Actions:**
- **Mở cửa sổ**: Show main window
- **Khởi động lại**: Restart both HTTP and WebSocket services
- **Thoát**: Stop all services and exit application

**Tray Icon Status:**
- Icon shows when service is running
- Tooltip: "Hung Duy Parking FileReceiver Beta - WebSocket Running"
- Balloon notifications when minimized to tray

#### **🚨 Troubleshooting WebSocket:**

**Connection Issues:**
1. **Check ports**: Ensure 5001 không bị block
2. **Firewall**: Allow WebSocket connections
3. **Browser**: Modern browsers support WebSocket
4. **CORS**: WebSocket server has CORS enabled

**Service Management:**
1. **Background running**: Service continues when window closed
2. **Full stop**: Only via "Thoát" in tray menu
3. **Restart**: Use tray menu "Khởi động lại"
4. **Status check**: Use `.\test-websocket.ps1`

**Testing Steps:**
1. **Start Application**: Run HungDuyParkingBridge
2. **Close Window**: Service continues in background
3. **Check Status**: `.\test-websocket.ps1`
4. **Open Test Page**: `http://localhost:5001/`
5. **Upload File**: Test thông báo real-time
6. **Full Exit**: Use tray menu "Thoát"

#### **⚠️ Important Behavior Notes:**

1. **Window close ≠ Service stop**: Closing window chỉ hide UI, service vẫn chạy
2. **Background notifications**: Tray shows balloon tip khi minimize
3. **Complete shutdown**: Chỉ "Thoát" từ tray menu mới stop service
4. **Restart safety**: Tray restart safely stops và starts lại services
5. **Multiple instances**: Tránh chạy multiple instances cùng lúc

### 🎯 Use Cases:

- **📱 Mobile Apps**: Real-time file notifications
- **🖥️ Desktop Apps**: Live status updates  
- **🌐 Web Dashboard**: Real-time monitoring
- **📊 Analytics**: Live file transfer metrics
- **🔔 Notifications**: Instant upload/download alerts
- **🚀 Background Services**: Always-on file monitoring
- **⚡ Server Monitoring**: Continuous uptime tracking
