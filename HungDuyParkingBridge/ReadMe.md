Lệnh publish .exe với icon embeddedcd .\HungDuyParkingBridge\# Standard publish với embedded icondotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true# Hoặc sử dụng publish profile tùy chỉnhdotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=embeddedDi chuyển đến thư mục để lấy file .exe đã publishK:\Project\HungDuyCoLTD\HungDuyParkingBridge\HungDuyParkingBridge\bin\Release\net9.0-windows\win-x64\publishcd .\bin\Release\net9.0-windows\win-x64\publish
explorer .

Di duyển đến thư mục project từ publish
cd ../../../../../

## 🔐 Private Key Authentication System (NEW v1.0.2)

### 🛡️ **Two-Tier Security Model**

HungDuy Parking Bridge hiện có **hệ thống xác thực private key** để bảo vệ các tính năng quản trị quan trọng.

#### **🔒 Guest Mode (Default)**
Khi khởi động, ứng dụng mặc định ở **Guest Mode**:
- ✅ **File Manager**: Xem và quản lý files (read-only functions)
- ❌ **HTTP Server**: Không hiển thị thông tin server
- ❌ **WebSocket**: Tab WebSocket bị ẩn
- ❌ **Storage Folder**: Không thể truy cập thư mục lưu trữ
- ❌ **Delete Functions**: Không thể xóa files
- ❌ **Server Management**: Không thể restart/manage server
- ❌ **Administrative Tools**: Các công cụ quản trị bị vô hiệu hóa

#### **🔓 Admin Mode (After Authentication)**
Sau khi xác thực với private key:
- ✅ **Full Access**: Toàn quyền truy cập tất cả tính năng
- ✅ **HTTP Server Management**: Quản lý HTTP server
- ✅ **WebSocket Tab**: Tab WebSocket hiển thị và hoạt động
- ✅ **Storage Folder Access**: Truy cập thư mục lưu trữ
- ✅ **Delete Functions**: Xóa files và cleanup
- ✅ **Server Controls**: Restart, stop, start services
- ✅ **Administrative Tools**: Tất cả công cụ quản trị

### 🔑 **Private Key Information**

**Default Private Key:** `P@ssw0rd`
**Location:** `HDParkingConst.key` trong source code

### 🎮 **How to Use Authentication**

#### **1. Access Authentication:**Help > Private Key > Authenticationhoặc sử dụng shortcut: `Ctrl + Shift + A`

#### **2. Enter Private Key:**
- Nhập private key: `P@ssw0rd`
- Click "OK" để xác thực
- ✅ Thành công → Chuyển sang Admin Mode

#### **3. Logout:**Help > Private Key > Logouthoặc sử dụng shortcut: `Ctrl + Shift + L`

### 📊 **Authentication Status Indicators**

#### **Status Bar:**
- 🔒 **`| Guest Mode`** (Red) - Limited access
- 🔓 **`| Admin Mode`** (Green) - Full access

#### **Tray Icon Tooltip:**
- **Guest Mode**: "HungDuy Parking FileReceiver Beta - Guest Mode"
- **Admin Mode**: "HungDuy Parking FileReceiver Beta - Admin Mode"

### 🎯 **Features Controlled by Authentication**

#### **Menu Items Visibility:**🔒 Guest Mode:
├── File
│   ├── [HIDDEN] Mở thư mục
│   └── Thoát
├── View
│   ├── Trang chính
│   ├── Quản lí tập tin
│   ├── [HIDDEN] WebSocket
│   └── Ẩn đi
├── Tools
│   ├── [HIDDEN] Dọn dẹp
│   ├── [HIDDEN] Khởi động lại
│   └── Settings
└── Help
    ├── Private Key
    │   ├── 🔓 Authentication (Enabled)
    │   └── 🚪 Logout (Disabled)
    └── Giới thiệu

🔓 Admin Mode:
├── File
│   ├── [VISIBLE] Mở thư mục ✅
│   └── Thoát
├── View
│   ├── Trang chính
│   ├── Quản lí tập tin
│   ├── [VISIBLE] WebSocket ✅
│   └── Ẩn đi
├── Tools
│   ├── [VISIBLE] Dọn dẹp ✅
│   ├── [VISIBLE] Khởi động lại ✅
│   └── Settings
└── Help
    ├── Private Key
    │   ├── 🔓 Authentication (Disabled)
    │   └── 🚪 Logout (Enabled) ✅
    └── Giới thiệu
#### **Tab Visibility:**
- **🏠 Home Tab**: Always visible, content changes based on auth
- **🔌 WebSocket Tab**: Only visible in Admin Mode
- **📁 File Manager Tab**: Always visible, functions limited in Guest Mode

#### **Home Tab Content:**🔒 Guest Mode:
├── 🔐 Authentication Status (Red warning)
├── Instructions to authenticate
└── Limited information display

🔓 Admin Mode:
├── 🔐 Authentication Status (Green success)
├── HTTP Server URL
├── WebSocket Server URL  
├── Storage folder path
├── Auto delete settings
├── Statistics panel
└── Quick Actions panel
### 🛠️ **Service Management Based on Authentication**

#### **🔒 Guest Mode:**
- ❌ HTTP Server: **Not started**
- ❌ WebSocket Server: **Not started**  
- ❌ File Services: **Limited**
- ❌ Background Services: **Minimal**

#### **🔓 Admin Mode:**
- ✅ HTTP Server: **Running on port 5000**
- ✅ WebSocket Server: **Running on port 5001**
- ✅ File Services: **Full functionality**
- ✅ Background Services: **All active**

### 🧪 **Testing Authentication System**

**PowerShell Test Script:**.\test-private-key.ps1
**Manual Testing Steps:**
1. **Start Application**: App opens in Guest Mode 🔒
2. **Verify Restrictions**: Try accessing hidden features
3. **Authenticate**: Help > Private Key > Authentication
4. **Enter Key**: `P@ssw0rd`
5. **Verify Access**: All admin features now available 🔓
6. **Test Logout**: Help > Private Key > Logout
7. **Verify Restriction**: Back to limited access 🔒

### 🔧 **Developer Configuration**

#### **Change Private Key:**// In HDParkingConst.cs
public static readonly string key = "YourNewPrivateKey";
#### **Check Authentication Status:**if (HDParkingConst.IsAdminAuthenticated)
{
    // Admin-only code
}
#### **Set Authentication:**HDParkingConst.SetAdminAccess(true);  // Enable admin
HDParkingConst.SetAdminAccess(false); // Disable admin
### 🚨 **Security Considerations**

#### **✅ Implemented Security:**
- Private key validation
- Session-based authentication
- Feature-level access control
- Visual authentication indicators
- Secure logout functionality
- Service control based on auth status

#### **⚠️ Security Notes:**
- Private key stored in source code (suitable for internal use)
- Authentication is session-based (resets on app restart)
- No encryption of private key in current implementation
- Suitable for controlled environments

### 🎯 **Use Cases**

#### **🏢 Corporate Environment:**
- **Guest Mode**: End users can view files only
- **Admin Mode**: IT administrators have full control

#### **🔧 Development/Testing:**
- **Guest Mode**: Safe testing without affecting services
- **Admin Mode**: Full development and debugging access

#### **🏠 Home/Personal Use:**
- **Guest Mode**: Family members can browse files
- **Admin Mode**: Main user controls server and settings

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

**Quick Refresh (sau khi update icon):**# Chạy script tự động.\refresh-icon.bat# Hoặc thủ công:dotnet clean
dotnet build -c Debug**Full Clean Build:**# Xóa hoàn toàn cachermdir /s /q bin obj
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
curl http://localhost:5000/api/health

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
- 🔐 **Authentication Required**: Only available in Admin Mode

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
  "server": "HungDuyParkingBridge",
  "version": "1.0.2", 
  "connectedClients": 3,
  "timestamp": "2024-01-01T12:00:00.000Z"
}

// Pong response
{
  "type": "pong",
  "timestamp": "2024-01-01T12:00:00.000Z"
}
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
- 🔐 **Admin Only**: Tab chỉ hiển thị khi đã authenticate

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
- **Khởi động lại**: Restart both HTTP and WebSocket services (Admin only)
- **Thoát**: Stop all services and exit application

**Tray Icon Status:**
- Icon shows when service is running
- Tooltip: Shows current authentication mode
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
2. **Authenticate**: Enter private key for admin access
3. **Close Window**: Service continues in background
4. **Check Status**: `.\test-websocket.ps1`
5. **Open Test Page**: `http://localhost:5001/`
6. **Upload File**: Test thông báo real-time
7. **Full Exit**: Use tray menu "Thoát"

#### **⚠️ Important Behavior Notes:**

1. **Window close ≠ Service stop**: Closing window chỉ hide UI, service vẫn chạy
2. **Background notifications**: Tray shows balloon tip khi minimize
3. **Complete shutdown**: Chỉ "Thoát" từ tray menu mới stop service
4. **Restart safety**: Tray restart safely stops và starts lại services
5. **Multiple instances**: Tránh chạy multiple instances cùng lúc
6. **🔐 Authentication Required**: Services only start in Admin Mode

### 🎯 Use Cases:

- **📱 Mobile Apps**: Real-time file notifications
- **🖥️ Desktop Apps**: Live status updates  
- **🌐 Web Dashboard**: Real-time monitoring
- **📊 Analytics**: Live file transfer metrics
- **🔔 Notifications**: Instant upload/download alerts
- **🚀 Background Services**: Always-on file monitoring
- **⚡ Server Monitoring**: Continuous uptime tracking
- **🔐 Secure Operations**: Admin-controlled access

## 🎨 Background Image for Guest Mode (NEW v1.0.2)

### 📸 **Custom Background Image Setup**

HungDuy Parking Bridge hiện hỗ trợ **background image tùy chỉnh** cho Guest Mode, tạo giao diện chuyên nghiệp và thương hiệu.

#### **🎯 Background Image Locations:**

Application sẽ tự động tìm kiếm `background-home-page.png` theo thứ tự:

1. **`Publics\Images\background-home-page.png`** ⭐ (Recommended)
2. **`Images\background-home-page.png`**
3. **`background-home-page.png`** (Project root)
4. **Application startup directory + any of the above paths**

#### **📋 Image Requirements:**

- **Format**: PNG (recommended), JPG, BMP, GIF
- **Size**: Any resolution (automatically scaled to fit)
- **Quality**: High resolution recommended (1200x800+)
- **Style**: Corporate/professional appearance
- **File name**: Must be exactly `background-home-page.png`

#### **🚀 Quick Setup:**

**PowerShell Setup Script:**.\setup-background.ps1
**Manual Setup:**
1. **Get your background image** (company logo, branding, etc.)
2. **Rename to**: `background-home-page.png`
3. **Copy to**: `Publics\Images\background-home-page.png`
4. **Build**: `dotnet build`
5. **Test**: Run app in Guest Mode

#### **🎨 How It Works:**

**🔒 Guest Mode (No Authentication):**
- ✅ **Custom background image** displayed on Home tab
- ✅ **Professional appearance** with company branding
- ✅ **Clean interface** without cluttered instructions
- ✅ **Automatic fallback** to generated placeholder if image missing

**🔓 Admin Mode (After Authentication):**
- ✅ **Normal controls** displayed instead of background
- ✅ **Full functionality** with admin panels and settings

#### **🛠️ Technical Details:**

**Automatic Image Handling:**// Multiple path checking
string[] possiblePaths = {
    "Publics\\Images\\background-home-page.png",
    "Images\\background-home-page.png", 
    "background-home-page.png"
};

// High-quality scaling
SizeMode = PictureBoxSizeMode.Zoom
**Fallback Placeholder:**
- **Auto-generated** if no custom image found
- **Professional gradient** background
- **Company title** and lock icon
- **1200x800 resolution** with anti-aliasing

#### **🎯 Use Cases:**

**Corporate Branding:**
- **Company logo** as background
- **Brand colors** and styling
- **Professional appearance** for client-facing systems

**Department Customization:**
- **Department-specific** backgrounds
- **Location branding** for multiple sites
- **Custom messaging** or information

**Seasonal/Event Themes:**
- **Holiday themes** for special occasions
- **Event branding** for conferences
- **Promotional backgrounds** for campaigns

#### **📁 Project Structure:**HungDuyParkingBridge/
├── Publics/
│   └── Images/
│       └── background-home-page.png ⭐
├── Images/
│   └── background-home-page.png
├── background-home-page.png
└── setup-background.ps1
#### **🧪 Testing Background Image:**

**Test Steps:**
1. **Start Application**: Run HungDuyParkingBridge
2. **Ensure Guest Mode**: Don't authenticate (red status: "🔒 Guest Mode")
3. **Check Home Tab**: Should display your background image
4. **Test Authentication**: Login to see it switches to admin controls
5. **Test Logout**: Background should reappear in guest mode

**Debug Issues:**# Check if image exists
.\setup-background.ps1

# Test different locations
dir Publics\Images\background-home-page.png
dir Images\background-home-page.png
dir background-home-page.png

# Rebuild application
dotnet clean
dotnet build
#### **💡 Design Recommendations:**

**Professional Backgrounds:**
- **Subtle gradients** or solid colors
- **Company logo** prominently displayed
- **Minimal text** to avoid clutter
- **High contrast** with UI elements

**Image Optimization:**
- **Compressed PNG** for smaller file size
- **RGB color space** for consistent display
- **Sharp graphics** that scale well
- **Transparent elements** if needed

#### **🔧 Advanced Configuration:**

**Custom Placeholder Generation:**
The application automatically creates a professional placeholder with:
- **Gradient background** (light blue theme)
- **Company title**: "HungDuy Parking Bridge"
- **Subtitle**: "File Management System"
- **Lock icon** indicating authentication needed
- **Decorative elements** for visual appeal

**Build Integration:**<!-- In .csproj file -->
<Content Include="Publics\Images\background-home-page.png">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</Content>
### 🎨 Background Image Ready!

The background image system provides a professional, branded experience for Guest Mode while maintaining full functionality in Admin Mode.
