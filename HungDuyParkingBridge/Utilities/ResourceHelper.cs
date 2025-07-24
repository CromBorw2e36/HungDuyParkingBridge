using System.Reflection;

namespace HungDuyParkingBridge.Utilities
{
    public static class ResourceHelper
    {
        private static Icon? _cachedIcon = null;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Clears the cached icon to force reload (useful when icon is updated)
        /// </summary>
        public static void ClearIconCache()
        {
            lock (_lockObject)
            {
                _cachedIcon?.Dispose();
                _cachedIcon = null;
                System.Diagnostics.Debug.WriteLine("Icon cache cleared");
            }
        }

        /// <summary>
        /// Gets the embedded icon from assembly resources
        /// </summary>
        /// <returns>Icon if found, otherwise null</returns>
        public static Icon? GetEmbeddedIcon()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                
                // List all embedded resource names for debugging
                var resourceNames = assembly.GetManifestResourceNames();
                System.Diagnostics.Debug.WriteLine("=== EMBEDDED RESOURCES DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Total resources found: {resourceNames.Length}");
                foreach (var name in resourceNames)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {name}");
                }
                System.Diagnostics.Debug.WriteLine("===============================");

                // Try specific resource names for your new icon
                var possibleNames = new[]
                {
                    "HungDuyParkingBridge.logoTapDoan.ico",
                    "logoTapDoan.ico",
                    assembly.GetName().Name + ".logoTapDoan.ico"
                };

                foreach (var resourceName in possibleNames)
                {
                    System.Diagnostics.Debug.WriteLine($"Trying resource: {resourceName}");
                    using var stream = assembly.GetManifestResourceStream(resourceName);
                    if (stream != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ SUCCESS: Found icon resource: {resourceName}");
                        return new Icon(stream);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ FAILED: Resource not found: {resourceName}");
                    }
                }

                // Try finding any resource with ico extension
                var iconResource = resourceNames.FirstOrDefault(name => 
                    name.EndsWith(".ico", StringComparison.OrdinalIgnoreCase));
                
                if (!string.IsNullOrEmpty(iconResource))
                {
                    System.Diagnostics.Debug.WriteLine($"Trying fallback icon: {iconResource}");
                    using var stream = assembly.GetManifestResourceStream(iconResource);
                    if (stream != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ SUCCESS: Found icon resource by extension: {iconResource}");
                        return new Icon(stream);
                    }
                }

                System.Diagnostics.Debug.WriteLine("❌ No embedded icon found");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR loading embedded icon: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets icon from file system
        /// </summary>
        /// <returns>Icon if found, otherwise null</returns>
        public static Icon? GetFileSystemIcon()
        {
            try
            {
                var possiblePaths = new[]
                {
                    Path.Combine(Application.StartupPath, "logoTapDoan.ico"),
                    Path.Combine(AppContext.BaseDirectory, "logoTapDoan.ico"),
                    Path.Combine(Environment.CurrentDirectory, "logoTapDoan.ico"),
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "logoTapDoan.ico")
                };

                System.Diagnostics.Debug.WriteLine("=== FILE SYSTEM ICON DEBUG ===");
                foreach (var iconPath in possiblePaths)
                {
                    System.Diagnostics.Debug.WriteLine($"Checking path: {iconPath}");
                    if (File.Exists(iconPath))
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ SUCCESS: Found icon file: {iconPath}");
                        return new Icon(iconPath);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ FAILED: File not found: {iconPath}");
                    }
                }
                System.Diagnostics.Debug.WriteLine("===============================");

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR loading file system icon: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the application icon with comprehensive fallback and caching
        /// </summary>
        /// <returns>Application icon or system default</returns>
        public static Icon GetApplicationIcon()
        {
            lock (_lockObject)
            {
                // Return cached icon if available
                if (_cachedIcon != null)
                {
                    return _cachedIcon;
                }

                System.Diagnostics.Debug.WriteLine("🔍 Loading application icon...");

                // Try embedded resource first (works in published apps)
                var embeddedIcon = GetEmbeddedIcon();
                if (embeddedIcon != null)
                {
                    _cachedIcon = embeddedIcon;
                    System.Diagnostics.Debug.WriteLine("✅ Using embedded icon");
                    return _cachedIcon;
                }

                // Try file system (works in debug mode)
                var fileIcon = GetFileSystemIcon();
                if (fileIcon != null)
                {
                    _cachedIcon = fileIcon;
                    System.Diagnostics.Debug.WriteLine("✅ Using file system icon");
                    return _cachedIcon;
                }

                // Use system default as fallback
                System.Diagnostics.Debug.WriteLine("⚠️ Using system default icon as fallback");
                return SystemIcons.Application;
            }
        }

        /// <summary>
        /// Forces reload of the application icon (useful after icon update)
        /// </summary>
        /// <returns>The reloaded application icon</returns>
        public static Icon ReloadApplicationIcon()
        {
            ClearIconCache();
            return GetApplicationIcon();
        }

        /// <summary>
        /// Sets the window icon for a form using the custom application icon
        /// </summary>
        /// <param name="form">The form to set the icon for</param>
        /// <param name="forceReload">Force reload the icon from disk/resources</param>
        public static void SetWindowIcon(Form form, bool forceReload = false)
        {
            try
            {
                if (form == null) return;

                var icon = forceReload ? ReloadApplicationIcon() : GetApplicationIcon();
                if (icon != null && icon != SystemIcons.Application)
                {
                    form.Icon = icon;
                    System.Diagnostics.Debug.WriteLine($"✅ Set window icon for form: {form.Name}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Using default icon for form: {form.Name}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error setting window icon for form {form?.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the window icon for all open forms in the application
        /// </summary>
        /// <param name="forceReload">Force reload the icon from disk/resources</param>
        public static void SetWindowIconForAllForms(bool forceReload = false)
        {
            try
            {
                var icon = forceReload ? ReloadApplicationIcon() : GetApplicationIcon();
                if (icon == null || icon == SystemIcons.Application) return;

                foreach (Form form in Application.OpenForms)
                {
                    if (form.Icon == null || form.Icon == SystemIcons.Application)
                    {
                        form.Icon = icon;
                        System.Diagnostics.Debug.WriteLine($"✅ Set window icon for form: {form.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error setting window icons for all forms: {ex.Message}");
            }
        }

        /// <summary>
        /// Debug method to test icon loading
        /// </summary>
        public static void DebugIconLoading()
        {
            System.Diagnostics.Debug.WriteLine("🔍 === ICON LOADING DEBUG SESSION ===");
            
            // Test embedded resources
            System.Diagnostics.Debug.WriteLine("1. Testing embedded resources...");
            var embeddedIcon = GetEmbeddedIcon();
            System.Diagnostics.Debug.WriteLine($"   Result: {(embeddedIcon != null ? "SUCCESS" : "FAILED")}");
            
            // Test file system
            System.Diagnostics.Debug.WriteLine("2. Testing file system...");
            var fileIcon = GetFileSystemIcon();
            System.Diagnostics.Debug.WriteLine($"   Result: {(fileIcon != null ? "SUCCESS" : "FAILED")}");
            
            // Test overall loading
            System.Diagnostics.Debug.WriteLine("3. Testing overall icon loading...");
            var appIcon = GetApplicationIcon();
            System.Diagnostics.Debug.WriteLine($"   Result: {(appIcon != SystemIcons.Application ? "SUCCESS" : "USING DEFAULT")}");
            
            System.Diagnostics.Debug.WriteLine("=== END DEBUG SESSION ===");
        }
    }
}