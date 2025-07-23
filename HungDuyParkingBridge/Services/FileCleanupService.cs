using System.Diagnostics;

namespace HungDuyParkingBridge.Services
{
    internal class FileCleanupService
    {
        private readonly string _savePath = @"C:\HungDuyParkingReceivedFiles";
        private readonly string _metadataPath;
        
        public bool IsEnabled { get; set; } = false;
        public int DeleteAfterDays { get; set; } = 7;

        public FileCleanupService()
        {
            _metadataPath = Path.Combine(_savePath, "metadata.json");
        }

        public void CleanupOldFiles()
        {
            if (!IsEnabled || !Directory.Exists(_savePath))
                return;

            try
            {
                var cutoffDate = DateTime.Now.AddDays(-DeleteAfterDays);
                var filesToDelete = new List<string>();
                
                // Get all files in the directory
                var files = Directory.GetFiles(_savePath, "*", SearchOption.AllDirectories);
                
                foreach (var file in files)
                {
                    // Skip metadata file
                    if (Path.GetFileName(file).Equals("metadata.json", StringComparison.OrdinalIgnoreCase))
                        continue;
                    
                    var fileInfo = new FileInfo(file);
                    
                    // Check if file is older than cutoff date
                    if (fileInfo.CreationTime < cutoffDate && fileInfo.LastWriteTime < cutoffDate)
                    {
                        filesToDelete.Add(file);
                    }
                }

                // Delete old files
                foreach (var file in filesToDelete)
                {
                    try
                    {
                        File.Delete(file);
                        Debug.WriteLine($"[FileCleanup] ?ã xóa file c?: {Path.GetFileName(file)}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[FileCleanup] L?i xóa file {Path.GetFileName(file)}: {ex.Message}");
                    }
                }

                // Clean up empty directories
                CleanupEmptyDirectories(_savePath);
                
                if (filesToDelete.Count > 0)
                {
                    Debug.WriteLine($"[FileCleanup] ?ã xóa {filesToDelete.Count} file c? (h?n {DeleteAfterDays} ngày)");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileCleanup] L?i trong quá trình d?n d?p: {ex.Message}");
            }
        }

        private void CleanupEmptyDirectories(string path)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    CleanupEmptyDirectories(directory);
                    
                    // If directory is empty, delete it
                    if (!Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        Directory.Delete(directory);
                        Debug.WriteLine($"[FileCleanup] ?ã xóa th? m?c tr?ng: {Path.GetFileName(directory)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileCleanup] L?i xóa th? m?c tr?ng: {ex.Message}");
            }
        }

        public void ForceCleanup()
        {
            if (IsEnabled)
            {
                CleanupOldFiles();
            }
        }

        public int GetFileCount()
        {
            try
            {
                if (!Directory.Exists(_savePath))
                    return 0;

                return Directory.GetFiles(_savePath, "*", SearchOption.AllDirectories)
                    .Where(f => !Path.GetFileName(f).Equals("metadata.json", StringComparison.OrdinalIgnoreCase))
                    .Count();
            }
            catch
            {
                return 0;
            }
        }

        public long GetTotalSize()
        {
            try
            {
                if (!Directory.Exists(_savePath))
                    return 0;

                return Directory.GetFiles(_savePath, "*", SearchOption.AllDirectories)
                    .Where(f => !Path.GetFileName(f).Equals("metadata.json", StringComparison.OrdinalIgnoreCase))
                    .Sum(f => new FileInfo(f).Length);
            }
            catch
            {
                return 0;
            }
        }
    }
}