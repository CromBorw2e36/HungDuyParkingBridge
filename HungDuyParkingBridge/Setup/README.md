# Hung Duy Parking Bridge - Installation Guide

## System Requirements
- Windows 10/11 (64-bit)
- .NET Runtime (included in installer)
- 100MB of free disk space for the application
- Ports 5000 and 5001 must be available (not used by other applications)

## Installation Options

### Option 1: Using the Installer (Recommended)
1. Download the installer `HungDuyParkingBridge_Setup.exe`
2. Right-click the installer and select "Run as administrator"
3. Follow the installation wizard instructions
4. Choose whether to start the application automatically with Windows (via Registry)
5. Click "Finish" to complete the installation

### Option 2: Manual Installation
1. Download the single-file executable `HungDuyParkingBridge.exe`
2. Create a folder to store the application (e.g., `C:\Program Files\Hung Duy Parking Bridge`)
3. Copy the executable to this folder
4. Create the directory `C:\HungDuyParkingReceivedFiles` for file storage
5. Set permissions on this directory to allow file writing:
   ```
   icacls "C:\HungDuyParkingReceivedFiles" /grant Users:(OI)(CI)F /T
   ```
6. Create a desktop shortcut (optional)
   - The application will automatically add itself to startup when launched from this shortcut

## Post-Installation

### First Run
1. The application will start in Guest mode
2. For Admin mode, enter the private key (default: 012233)
3. The application runs in the system tray (look for the icon in the taskbar)

### Application Data
- Received files are stored in: `C:\HungDuyParkingReceivedFiles`
- Settings are stored in the Registry under `HKCU\SOFTWARE\Hung Duy Co., LTD\Hung Duy Parking Bridge`
- Startup configuration:
  - If installed via installer with startup option: `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
  - If launched from shortcut without installer startup option: `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`

### Using the Application
- Double-click the tray icon to open the main window
- Right-click the tray icon for quick actions (restart, exit)
- When launched from a desktop or start menu shortcut, the application will ensure it's set to run at startup

## Uninstallation
1. Open Control Panel > Programs > Uninstall a program
2. Select "Hung Duy Parking Bridge" and click Uninstall
3. Follow the uninstallation wizard
4. The uninstaller will automatically:
   - Stop running instances of the application
   - Remove all registry entries including the startup registry key
   - Delete program files
5. Note: The received files directory (`C:\HungDuyParkingReceivedFiles`) is not deleted automatically to prevent data loss

## Troubleshooting

### Application Won't Start
- Check if ports 5000 and 5001 are available:
  ```
  netstat -ano | findstr :5000
  netstat -ano | findstr :5001
  ```
- If these ports are in use, close the applications using them or configure Hung Duy Parking Bridge to use different ports

### Missing Icon or Resources
- Reinstall the application
- Ensure you have administrator rights during installation

### Startup Issues
- Check the Windows Registry Editor to verify if the startup entry exists:
  1. Press Win+R, type `regedit` and press Enter
  2. Check in `HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run` for installer-created entry
  3. Check in `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run` for application-created entry
  4. Verify the entry value contains the correct path to the application
- If needed, launch the application from a desktop shortcut to automatically create the startup entry

### Other Issues
- Check the Windows Event Viewer for application errors
- Contact support at support@hungduy.com

## Building the Installer
1. Install Inno Setup: https://jrsoftware.org/isdl.php
2. Open PowerShell as Administrator
3. Navigate to the Setup directory
4. Run `.\build-installer.ps1`
5. The installer will be created in the Output directory