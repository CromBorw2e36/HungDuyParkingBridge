using System.Reflection;

namespace HungDuyParkingBridge.Utilities
{
    public static class ResourceHelper
    {
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
                System.Diagnostics.Debug.WriteLine("Available embedded resources:");
                foreach (var name in resourceNames)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {name}");
                }

                // Try specific resource names
                var possibleNames = new[]
                {
                    "HungDuyParkingBridge.logoTapDoan.ico",
                    "logoTapDoan.ico",
                    assembly.GetName().Name + ".logoTapDoan.ico"
                };

                foreach (var resourceName in possibleNames)
                {
                    using var stream = assembly.GetManifestResourceStream(resourceName);
                    if (stream != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Found icon resource: {resourceName}");
                        return new Icon(stream);
                    }
                }

                // Try finding any resource with ico extension
                var iconResource = resourceNames.FirstOrDefault(name => 
                    name.EndsWith(".ico", StringComparison.OrdinalIgnoreCase));
                
                if (!string.IsNullOrEmpty(iconResource))
                {
                    using var stream = assembly.GetManifestResourceStream(iconResource);
                    if (stream != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Found icon resource by extension: {iconResource}");
                        return new Icon(stream);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading embedded icon: {ex.Message}");
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
                    Path.Combine(Environment.CurrentDirectory, "logoTapDoan.ico")
                };

                foreach (var iconPath in possiblePaths)
                {
                    if (File.Exists(iconPath))
                    {
                        System.Diagnostics.Debug.WriteLine($"Found icon file: {iconPath}");
                        return new Icon(iconPath);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading file system icon: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the application icon with comprehensive fallback
        /// </summary>
        /// <returns>Application icon or system default</returns>
        public static Icon GetApplicationIcon()
        {
            // Try embedded resource first (works in published apps)
            var embeddedIcon = GetEmbeddedIcon();
            if (embeddedIcon != null)
                return embeddedIcon;

            // Try file system (works in debug mode)
            var fileIcon = GetFileSystemIcon();
            if (fileIcon != null)
                return fileIcon;

            // Use system default as fallback
            System.Diagnostics.Debug.WriteLine("Using system default icon as fallback");
            return SystemIcons.Application;
        }

        /// <summary>
        /// Sets the window icon for a form using the custom application icon
        /// </summary>
        /// <param name="form">The form to set the icon for</param>
        public static void SetWindowIcon(Form form)
        {
            try
            {
                if (form == null) return;

                var icon = GetApplicationIcon();
                if (icon != null)
                {
                    form.Icon = icon;
                    System.Diagnostics.Debug.WriteLine($"Set window icon for form: {form.Name}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting window icon for form {form?.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the window icon for all open forms in the application
        /// </summary>
        public static void SetWindowIconForAllForms()
        {
            try
            {
                var icon = GetApplicationIcon();
                if (icon == null) return;

                foreach (Form form in Application.OpenForms)
                {
                    if (form.Icon == null || form.Icon == SystemIcons.Application)
                    {
                        form.Icon = icon;
                        System.Diagnostics.Debug.WriteLine($"Set window icon for form: {form.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting window icons for all forms: {ex.Message}");
            }
        }
    }
}