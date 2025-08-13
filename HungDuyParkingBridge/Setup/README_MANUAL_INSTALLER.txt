To manually create the installer:

1. Download and install Inno Setup from https://jrsoftware.org/isdl.php
2. Open Inno Setup Compiler
3. Open the script file: D:\HongHungProject\HungDuyParkingBridge\HungDuyParkingBridge\Setup\HungDuyParkingBridge.iss
4. Click Build > Compile

The installer will be created in: D:\HongHungProject\HungDuyParkingBridge\HungDuyParkingBridge\Setup\Output\HungDuyParkingBridge_Setup.exe

If you encounter compilation errors:

1. Ensure all referenced image files exist:
   - Create folder: D:\HongHungProject\HungDuyParkingBridge\HungDuyParkingBridge\Publics\Images\
   - The folder should contain:
     * installer_background.bmp
     * installer_small.bmp

2. Check that logoTapDoan.ico exists in:
   - D:\HongHungProject\HungDuyParkingBridge\HungDuyParkingBridge\logoTapDoan.ico

3. If you're getting "Unknown identifier" errors, make sure all variables are properly declared
   in the [Code] section of the .iss file.

4. For "Internal error: An attempt was made to expand the 'app' constant before it was initialized":
   - This happens when you use {app} in the InitializeSetup function
   - Solution: Use ExpandConstant('{autopf}\{#MyAppName}') instead of {app} in InitializeSetup

5. Startup Entries Management:
   - The installer now uses ONLY registry-based startup (no shortcuts in Startup folder)
   - Registry entries are added to HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run only if the user selects the startup option
   - The application will automatically add a HKCU registry entry if launched from a desktop/start menu shortcut and no startup entry exists
   - Old registry entries and startup shortcuts from previous versions are automatically cleaned up
   - Uninstaller properly removes all startup entries

6. For other errors, check the InnoSetup documentation at:
   https://jrsoftware.org/ishelp/
