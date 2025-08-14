using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;

namespace HungDuyParkingBridge.Utils
{
    /// <summary>
    /// Manages the application's startup registry entries and ensures the app stays enabled in Task Manager
    /// </summary>
    public static class StartupManager
    {
        private const string APP_NAME = "Hung Duy Parking Bridge";
        private const string RUN_KEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string STARTUP_APPROVED_KEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

        /// <summary>
        /// Ensures the application is registered in Windows startup and enabled in Task Manager
        /// </summary>
        public static void EnsureStartupEnabled()
        {
            //try
            //{
            //    bool inHKLM = false;
            //    bool hklmEnabled = false;

            //    // Check HKLM first (for installer-created entries)
            //    try
            //    {
            //        (inHKLM, hklmEnabled) = CheckStartupEntry(Registry.LocalMachine, APP_NAME);
                    
            //        // If in HKLM but disabled, try to enable it (if we have admin rights)
            //        if (inHKLM && !hklmEnabled && IsRunningElevated())
            //        {
            //            EnableStartupEntry(Registry.LocalMachine, APP_NAME);
            //            Debug.WriteLine("Enabled HKLM startup entry");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine($"Error checking/updating HKLM startup: {ex.Message}");
            //    }

            //    // If not in HKLM or disabled in HKLM, check/update HKCU
            //    if (!inHKLM || (inHKLM && !hklmEnabled && !IsRunningElevated()))
            //    {
            //        bool inHKCU = false;
            //        bool hkcuEnabled = false;

            //        try
            //        {
            //            (inHKCU, hkcuEnabled) = CheckStartupEntry(Registry.CurrentUser, APP_NAME);

            //            // If not in HKCU, add it
            //            if (!inHKCU)
            //            {
            //                AddStartupEntry(Registry.CurrentUser, APP_NAME, Application.ExecutablePath);
            //                EnableStartupEntry(Registry.CurrentUser, APP_NAME);
            //                Debug.WriteLine("Added and enabled HKCU startup entry");
            //            }
            //            // If in HKCU but disabled, enable it
            //            else if (!hkcuEnabled)
            //            {
            //                EnableStartupEntry(Registry.CurrentUser, APP_NAME);
            //                Debug.WriteLine("Enabled existing HKCU startup entry");
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Debug.WriteLine($"Error checking/updating HKCU startup: {ex.Message}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine($"Error in EnsureStartupEnabled: {ex.Message}");
            //}
        }

        /// <summary>
        /// Checks if an application is registered to run at startup and if it's enabled
        /// </summary>
        /// <param name="root">Registry hive (HKLM or HKCU)</param>
        /// <param name="appName">Application name in registry</param>
        /// <returns>Tuple of (exists, isEnabled)</returns>
        public static (bool exists, bool isEnabled) CheckStartupEntry(RegistryKey root, string appName)
        {
            bool exists = false;
            bool isEnabled = false;

            // Check if entry exists in Run key
            using (RegistryKey runKey = root.OpenSubKey(RUN_KEY_PATH))
            {
                if (runKey != null)
                {
                    exists = runKey.GetValue(appName) != null;
                }
            }

            if (exists)
            {
                // Check if entry is enabled in StartupApproved key
                using (RegistryKey approvedKey = root.OpenSubKey(STARTUP_APPROVED_KEY_PATH))
                {
                    if (approvedKey != null)
                    {
                        byte[] value = approvedKey.GetValue(appName) as byte[];

                        // If no value exists in StartupApproved, it's considered enabled
                        if (value == null)
                        {
                            isEnabled = true;
                        }
                        else
                        {
                            // First byte: 02 = disabled, 00 = enabled
                            isEnabled = (value.Length > 0 && value[0] == 0);
                        }
                    }
                    else
                    {
                        // If StartupApproved key doesn't exist, the entry is enabled by default
                        isEnabled = true;
                    }
                }
            }

            return (exists, isEnabled);
        }

        /// <summary>
        /// Adds an application to Windows startup
        /// </summary>
        /// <param name="root">Registry hive (HKLM or HKCU)</param>
        /// <param name="appName">Name to use in registry</param>
        /// <param name="appPath">Full path to executable</param>
        public static void AddStartupEntry(RegistryKey root, string appName, string appPath)
        {
            using (RegistryKey runKey = root.OpenSubKey(RUN_KEY_PATH, true))
            {
                if (runKey != null)
                {
                    runKey.SetValue(appName, appPath);
                }
            }
        }

        /// <summary>
        /// Enables a startup entry in the StartupApproved registry key
        /// </summary>
        /// <param name="root">Registry hive (HKLM or HKCU)</param>
        /// <param name="appName">Application name in registry</param>
        public static void EnableStartupEntry(RegistryKey root, string appName)
        {
            try
            {
                using (RegistryKey approvedKey = root.OpenSubKey(STARTUP_APPROVED_KEY_PATH, true))
                {
                    if (approvedKey != null)
                    {
                        // Create a 12-byte array of zeros (enabled state)
                        byte[] enabledValue = new byte[12];
                        approvedKey.SetValue(appName, enabledValue, RegistryValueKind.Binary);
                    }
                    else
                    {
                        // If key doesn't exist, try to create it (requires admin for HKLM)
                        using (RegistryKey newKey = root.CreateSubKey(STARTUP_APPROVED_KEY_PATH, true))
                        {
                            if (newKey != null)
                            {
                                byte[] enabledValue = new byte[12];
                                newKey.SetValue(appName, enabledValue, RegistryValueKind.Binary);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error enabling startup entry: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if the application is running with administrator privileges
        /// </summary>
        private static bool IsRunningElevated()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}