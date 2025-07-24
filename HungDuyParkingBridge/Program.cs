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
                Console.WriteLine("=== Icon Loading Test ===");
                
                // List all embedded resources
                var assembly = Assembly.GetExecutingAssembly();
                var resources = assembly.GetManifestResourceNames();
                
                Console.WriteLine($"Found {resources.Length} embedded resources:");
                foreach (var resource in resources)
                {
                    Console.WriteLine($"  - {resource}");
                }
                
                // Test icon loading
                var icon = ResourceHelper.GetApplicationIcon();
                Console.WriteLine($"Icon loaded successfully: {icon != null}");
                
                if (icon != null)
                {
                    Console.WriteLine($"Icon size: {icon.Width}x{icon.Height}");
                }
                
                Console.WriteLine("=== End Test ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Icon test error: {ex.Message}");
            }
        }
    }
}