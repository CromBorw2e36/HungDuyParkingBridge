# Hung Duy Parking Bridge - Startup Implementation Details

## Overview

The startup behavior of Hung Duy Parking Bridge has been implemented with two complementary mechanisms:

1. **Installer-based startup registration** - The InnoSetup installer provides an optional checkbox for users to enable automatic startup. When selected, it adds a registry entry in HKLM.

2. **Application-based startup registration** - The application itself checks for existing startup entries when launched from a desktop or start menu shortcut. If no entry exists, it automatically adds one in HKCU.

This dual approach ensures that the application starts with Windows when either:
- The user selected the startup option during installation
- The user launched the application from a shortcut

## Implementation Details

### InnoSetup Script (`HungDuyParkingBridge.iss`)

1. **Task Definition**
   ```
   [Tasks]
   Name: "startupicon"; Description: "Start automatically when Windows starts (via Registry)"; GroupDescription: "{cm:AdditionalIcons}"
   ```

2. **Registry Entry Creation (only when task is selected)**
   ```
   [Registry]
   Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "HungDuyParkingBridge"; 
   ValueData: """{app}\{#MyAppExeName}"""; Flags: uninsdeletevalue; Tasks: startupicon
   ```

3. **Cleanup of Old Entries**
   - The script includes a `CleanupStartupEntries()` procedure that removes old registry entries
   - The script also includes a `RemoveStartupShortcuts()` procedure that removes old startup shortcuts
   - Both procedures are called during setup and uninstall

### C# Application (`MainForm.cs`)

1. **Startup Check Logic (`EnsureStartup` method)**
   - Checks if application is already registered in HKLM (by installer)
   - If not in HKLM, checks if already registered in HKCU
   - If not in either location, adds an entry to HKCU

2. **Launch Detection Logic**
   - `IsLaunchedFromShortcut()` - Detects if the application was launched from a desktop or start menu shortcut
   - `IsLaunchedFromStartup()` - Detects if the application was launched during Windows startup
   - The EnsureStartup method is only called when IsLaunchedFromShortcut() returns true

3. **Error Handling**
   - All operations include try-catch blocks to prevent crashes
   - Errors are logged via Debug.WriteLine but don't interrupt application function
   - Registry access exceptions are handled gracefully

## Registry Locations

1. **HKLM Entry (added by installer)**
   - Location: `HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
   - Name: `HungDuyParkingBridge`
   - Value: Full path to the installed executable
   - Requires admin privileges to create
   - Applies to all users on the machine

2. **HKCU Entry (added by application)**
   - Location: `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
   - Name: `HungDuyParkingBridge`
   - Value: Full path to the executable (via Application.ExecutablePath)
   - Can be created without admin privileges
   - Applies only to the current user

## Uninstallation

The uninstallation process handles cleanup of all startup entries:

1. The `uninsdeletevalue` flag in the InnoSetup script ensures the HKLM registry entry is removed
2. The `CleanupStartupEntries()` procedure in the script removes any HKCU entries
3. The `RemoveStartupShortcuts()` procedure removes any legacy startup shortcuts

## Detection Algorithm

The application uses several heuristics to determine if it was launched from a shortcut:

1. **Command line arguments** - Checking for shell integration arguments
2. **Parent process** - Using WMI to determine if launched from explorer.exe
3. **Elevation status** - Non-elevated processes are more likely from shortcuts
4. **Process timing** - Using process start times to distinguish startup launches

## Testing Recommendations

1. **Clean Installation Test**
   - Install with startup option unchecked
   - Launch from desktop shortcut
   - Verify HKCU registry entry is created

2. **Installer Option Test**
   - Install with startup option checked
   - Verify HKLM registry entry is created
   - Verify application doesn't create duplicate HKCU entry

3. **Uninstallation Test**
   - Install the application
   - Let it create registry entries
   - Uninstall
   - Verify all registry entries are removed

4. **Multiple-User Test**
   - Install with admin rights
   - Test with different user accounts
   - Verify behavior is consistent across users