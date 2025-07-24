Lệnh publish .exe với icon embedded
cd .\HungDuyParkingBridge\

# Standard publish với embedded icon
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

# Hoặc sử dụng publish profile tùy chỉnh
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=embedded

Di chuyển đến thư mục để lấy file .exe đã publish
K:\Project\HungDuyCoLTD\HungDuyParkingBridge\HungDuyParkingBridge\bin\Release\net9.0-windows\win-x64\publish
## Giải thích vấn đề Icon

Vấn đề icon không hiển thị khi publish là do:

1. **PublishSingleFile=true**: Khi publish thành single file, các file Content không được extract ra ngoài
2. **Resource Embedding**: Icon cần được embed như EmbeddedResource thay vì chỉ là Content
3. **Path Resolution**: Trong published app, Application.StartupPath có thể khác với thư mục chứa file gốc

## Giải pháp đã áp dụng:

1. **Dual Configuration**: Icon được cấu hình vừa là EmbeddedResource vừa là Content
2. **Smart Loading**: ResourceHelper tự động thử load từ embedded resource trước, sau đó mới thử file system
3. **Comprehensive Fallback**: Nhiều đường dẫn và phương thức loading khác nhau
4. **Debug Logging**: Ghi log để debug vấn đề trong published version

## Kiểm tra sau khi publish:
# Kiểm tra resource có được embed không
ildasm HungDuyParkingBridge.exe

# Hoặc sử dụng dotnet để list resources
dotnet-dump ps
