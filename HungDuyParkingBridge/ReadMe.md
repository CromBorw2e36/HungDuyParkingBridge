Lệnh publish .exe với icon embedded
cd .\HungDuyParkingBridge\

# Standard publish với embedded icon
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

# Hoặc sử dụng publish profile tùy chỉnh
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=embedded

Di chuyển đến thư mục để lấy file .exe đã publish
K:\Project\HungDuyCoLTD\HungDuyParkingBridge\HungDuyParkingBridge\bin\Release\net9.0-windows\win-x64\publish
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
.\refresh-icon.bat

# Hoặc thủ công:
dotnet clean
dotnet build -c Debug
**Full Clean Build:**# Xóa hoàn toàn cache
rmdir /s /q bin obj
dotnet restore
dotnet build
#### 4. **Debug Icon Loading**:
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
