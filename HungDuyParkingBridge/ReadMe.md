Lệnh publish .exe
```bash

cd .\HungDuyParkingBridge\

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

```
