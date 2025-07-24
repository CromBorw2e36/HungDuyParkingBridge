Lệnh publish .exe
```bash

cd .\HungDuyParkingBridge\

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

```

Di chuyển đến thư mục để lấy file .exe đã publish

```bash
K:\Project\HungDuyCoLTD\HungDuyParkingBridge\HungDuyParkingBridge\bin\Release\net9.0-windows\win-x64\publish
```
