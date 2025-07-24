using HungDuyParkingBridge.UI;
using HungDuyParkingBridge.Utilities;
using System.Reflection;

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
            // Test icon loading before starting the application
            TestIconLoading();
            
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
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