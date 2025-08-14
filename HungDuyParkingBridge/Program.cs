using HungDuyParkingBridge.UI;
using HungDuyParkingBridge.Utilities;
using HungDuyParkingBridge.Utils;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Text;

namespace HungDuyParkingBridge
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Check if another instance is already running using the improved SingleInstanceHelper
            if (!SingleInstanceHelper.IsFirstInstance())
            {
                // Another instance is running, show message and attempt to bring it to front
                SingleInstanceHelper.ShowInstanceAlreadyRunningMessage();
                return;
            }

            // Check if required ports are already in use
            if (!CheckRequiredPorts())
            {
                return; // Exit application if ports are not available
            }

            // Test icon loading before starting the application
            TestIconLoading();
            
            // Ensure startup registry entries exist and are enabled
            //try
            //{
            //    StartupManager.EnsureStartupEnabled();
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Error ensuring startup on application launch: {ex.Message}");
            //    // Continue even if there's an error with startup registration
            //}
            
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            try
            {
                Application.Run(new MainForm());
            }
            finally
            {
                // Ensure mutex is properly released when application exits
                SingleInstanceHelper.ReleaseMutex();
            }
        }

        private static bool CheckRequiredPorts()
        {
            try
            {
                // Extract port numbers from the URIs
                int httpPort = NetworkHelper.ExtractPortFromUri(HDParkingConst.portHttp);
                int wsPort = NetworkHelper.ExtractPortFromUri(HDParkingConst.portWebSocket);
                
                // Default to 5000 and 5001 if extraction fails
                if (httpPort <= 0) httpPort = 5000;
                if (wsPort <= 0) wsPort = 5001;
                
                // Check if both ports are in use
                var portsInUse = NetworkHelper.GetPortsInUse(httpPort, wsPort);
                
                if (portsInUse.Count > 0)
                {
                    // Build a meaningful error message with the specific ports
                    var sb = new StringBuilder();
                    sb.AppendLine("Cannot start the application because the following ports are already in use:");
                    
                    foreach (var port in portsInUse)
                    {
                        sb.AppendLine($"• Port {port}");
                    }
                    
                    sb.AppendLine("\nPossible solutions:");
                    sb.AppendLine("1. Close other applications that might be using these ports");
                    sb.AppendLine("2. Check if another instance of this application is already running");
                    
                    MessageBox.Show(sb.ToString(), 
                        "Port Conflict Error", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                    
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                // Log error and continue anyway, let the application handle it later
                System.Diagnostics.Debug.WriteLine($"Error checking ports: {ex.Message}");
                return true;
            }
        }

        private static void TestIconLoading()
        {
            try
            {
                Console.WriteLine("🔍 === ICON LOADING TEST ===");
                
                // Run detailed debug
                ResourceHelper.DebugIconLoading();
                
                // List all embedded resources
                var assembly = Assembly.GetExecutingAssembly();
                var resources = assembly.GetManifestResourceNames();
                
                Console.WriteLine($"📦 Found {resources.Length} embedded resources:");
                foreach (var resource in resources)
                {
                    Console.WriteLine($"  - {resource}");
                    if (resource.EndsWith(".ico"))
                    {
                        Console.WriteLine($"    ⭐ ICO FILE FOUND: {resource}");
                    }
                }
                
                // Test icon loading
                var icon = ResourceHelper.GetApplicationIcon();
                Console.WriteLine($"🎨 Icon loaded: {(icon != null && icon != SystemIcons.Application ? "✅ SUCCESS" : "❌ FAILED (using default)")}" );
                
                if (icon != null && icon != SystemIcons.Application)
                {
                    Console.WriteLine($"📏 Icon size: {icon.Width}x{icon.Height}");
                }
                
                // Test file system paths
                Console.WriteLine("\n📁 Checking file system paths:");
                var paths = new[]
                {
                    Path.Combine(Application.StartupPath, "logoTapDoan.ico"),
                    Path.Combine(AppContext.BaseDirectory, "logoTapDoan.ico"),
                    Path.Combine(Environment.CurrentDirectory, "logoTapDoan.ico")
                };
                
                foreach (var path in paths)
                {
                    var exists = File.Exists(path);
                    Console.WriteLine($"  {(exists ? "✅" : "❌")} {path}");
                }
                
                Console.WriteLine("=== END TEST ===\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Icon test error: {ex.Message}");
            }
        }
    }
}