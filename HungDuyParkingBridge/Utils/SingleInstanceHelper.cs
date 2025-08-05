using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace HungDuyParkingBridge.Utils
{
    /// <summary>
    /// Helper class to ensure only one instance of the application runs at a time using named Mutex
    /// </summary>
    public static class SingleInstanceHelper
    {
        private static Mutex? _applicationMutex;
        private static readonly string _mutexName = $"Global\\{HDParkingConst.nameSoftware}_SingleInstance_{{B8F4A7C2-1D3E-4F5A-8B9C-2E6D8A4C7F9E}}";
        
        // Windows API imports for bringing window to front
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        
        private const int SW_RESTORE = 9;
        private const int SW_SHOW = 5;

        /// <summary>
        /// Checks if this is the first instance of the application and acquires the mutex
        /// </summary>
        public static bool IsFirstInstance()
        {
            try
            {
                // Use the correct Mutex constructor
                _applicationMutex = new Mutex(false, _mutexName, out bool createdNew);

                if (!createdNew)
                {
                    // Another instance exists, try to wait briefly
                    bool acquired = _applicationMutex.WaitOne(TimeSpan.FromSeconds(2), false);
                    if (!acquired)
                    {
                        _applicationMutex?.Dispose();
                        _applicationMutex = null;
                        return false;
                    }
                }

                // Set up cleanup when application exits
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
                Application.ApplicationExit += OnApplicationExit;

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return TryUserSpecificMutex();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SingleInstanceHelper.IsFirstInstance: {ex.Message}");
                return true;
            }
        }

        private static bool TryUserSpecificMutex()
        {
            try
            {
                string userSpecificMutexName = $"Local\\{HDParkingConst.nameSoftware}_User_{Environment.UserName}_{{B8F4A7C2-1D3E-4F5A-8B9C-2E6D8A4C7F9E}}";
                
                _applicationMutex = new Mutex(false, userSpecificMutexName, out bool createdNew);

                if (!createdNew)
                {
                    bool acquired = _applicationMutex.WaitOne(TimeSpan.FromSeconds(1), false);
                    if (!acquired)
                    {
                        _applicationMutex?.Dispose();
                        _applicationMutex = null;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in user-specific mutex: {ex.Message}");
                return true;
            }
        }

        /// <summary>
        /// Shows a user-friendly message when another instance is detected and attempts to bring the existing instance to front
        /// </summary>
        public static void ShowInstanceAlreadyRunningMessage()
        {
            // Try to bring existing instance to front
            BringExistingInstanceToFront();

            // Show message to user
            MessageBox.Show(
                $"{HDParkingConst.nameSoftware} đã đang chạy!\n\n" +
                "Ứng dụng có thể đang chạy trong System Tray.\n" +
                "Vui lòng kiểm tra biểu tượng trong khay hệ thống hoặc Task Manager.",
                $"{HDParkingConst.nameSoftware} - Ứng dụng đã chạy",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// Attempts to find and bring the existing application instance to the foreground
        /// </summary>
        private static void BringExistingInstanceToFront()
        {
            try
            {
                // Find the existing process by name
                string currentProcessName = Process.GetCurrentProcess().ProcessName;
                var existingProcesses = Process.GetProcessesByName(currentProcessName);

                foreach (var process in existingProcesses)
                {
                    // Skip the current process
                    if (process.Id == Process.GetCurrentProcess().Id)
                        continue;

                    // Try to bring the existing process window to front
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        // If the window is minimized, restore it
                        if (IsIconic(process.MainWindowHandle))
                        {
                            ShowWindow(process.MainWindowHandle, SW_RESTORE);
                        }
                        else
                        {
                            ShowWindow(process.MainWindowHandle, SW_SHOW);
                        }

                        // Bring to foreground
                        SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error bringing existing instance to front: {ex.Message}");
            }
        }

        /// <summary>
        /// Releases the mutex when the application exits
        /// </summary>
        public static void ReleaseMutex()
        {
            try
            {
                if (_applicationMutex != null)
                {
                    _applicationMutex.ReleaseMutex();
                    _applicationMutex.Close();
                    _applicationMutex.Dispose();
                    _applicationMutex = null;
                    Debug.WriteLine("🔓 Application mutex released successfully");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Warning: Error releasing mutex: {ex.Message}");
            }
        }

        /// <summary>
        /// Event handler for process exit
        /// </summary>
        private static void OnProcessExit(object? sender, EventArgs e)
        {
            ReleaseMutex();
        }

        /// <summary>
        /// Event handler for application exit
        /// </summary>
        private static void OnApplicationExit(object? sender, EventArgs e)
        {
            ReleaseMutex();
        }

        /// <summary>
        /// Gets the mutex name used for single instance checking (for debugging purposes)
        /// </summary>
        public static string GetMutexName()
        {
            return _mutexName;
        }

        /// <summary>
        /// Checks if the mutex is currently owned by this instance
        /// </summary>
        public static bool IsMutexOwned()
        {
            return _applicationMutex != null;
        }

        /// <summary>
        /// Advanced method to check for running instances using multiple strategies
        /// </summary>
        public static bool IsAnyInstanceRunning()
        {
            try
            {
                // Method 1: Check by mutex
                using var testMutex = new Mutex(false, _mutexName);
                bool mutexAvailable = testMutex.WaitOne(100, false);
                if (mutexAvailable)
                {
                    testMutex.ReleaseMutex();
                }

                // Method 2: Check by process name
                string currentProcessName = Process.GetCurrentProcess().ProcessName;
                var processes = Process.GetProcessesByName(currentProcessName);
                bool hasOtherProcesses = processes.Length > 1;

                return !mutexAvailable || hasOtherProcesses;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking for running instances: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets detailed information about the current single instance status for debugging
        /// </summary>
        public static string GetStatusInfo()
        {
            try
            {
                var info = new System.Text.StringBuilder();
                info.AppendLine("🔐 Single Instance Helper Status:");
                info.AppendLine($"  Mutex Name: {_mutexName}");
                info.AppendLine($"  Mutex Owned: {IsMutexOwned()}");
                info.AppendLine($"  Application: {HDParkingConst.nameSoftware}");
                info.AppendLine($"  Process Name: {Process.GetCurrentProcess().ProcessName}");
                info.AppendLine($"  Process ID: {Process.GetCurrentProcess().Id}");
                
                // Check for other processes with the same name
                var processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
                info.AppendLine($"  Total Processes: {processes.Length}");
                
                if (processes.Length > 1)
                {
                    info.AppendLine("  Other Process IDs:");
                    foreach (var proc in processes)
                    {
                        if (proc.Id != Process.GetCurrentProcess().Id)
                        {
                            info.AppendLine($"    - PID: {proc.Id} (HasMainWindow: {proc.MainWindowHandle != IntPtr.Zero})");
                        }
                    }
                }
                
                return info.ToString();
            }
            catch (Exception ex)
            {
                return $"Error getting status info: {ex.Message}";
            }
        }
    }
}