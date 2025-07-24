using HungDuyParkingBridge.Utilities;
using System.Text;
using System.Globalization;
using System.Drawing.Imaging;

namespace HungDuyParkingBridge.UI
{
    public partial class FilePreviewForm : Form
    {
        private string _filePath;
        private FileInfo _fileInfo;

        public FilePreviewForm(string filePath)
        {
            _filePath = filePath;
            _fileInfo = new FileInfo(filePath);
            
            // Set window icon
            ResourceHelper.SetWindowIcon(this);
            
            // Set up English culture and encoding support
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            
            // Ensure proper encoding support
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            InitializeComponent();
            LoadFilePreview();
        }

        private void LoadFilePreview()
        {
            try
            {
                lblFileName.Text = $"File name: {_fileInfo.Name}";
                lblFileSize.Text = $"Size: {FormatFileSize(_fileInfo.Length)}";
                lblCreatedDate.Text = $"Created: {_fileInfo.CreationTime:dd/MM/yyyy HH:mm:ss}";
                lblModifiedDate.Text = $"Modified: {_fileInfo.LastWriteTime:dd/MM/yyyy HH:mm:ss}";
                lblFilePath.Text = $"Path: {_fileInfo.FullName}";
                
                string extension = _fileInfo.Extension.ToLowerInvariant();
                
                // Load preview based on file type
                switch (extension)
                {
                    case ".txt":
                    case ".log":
                    case ".csv":
                    case ".xml":
                    case ".json":
                    case ".config":
                        LoadTextPreview();
                        break;
                        
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".gif":
                    case ".bmp":
                    case ".tiff":
                        LoadImagePreview();
                        break;
                        
                    case ".pdf":
                        LoadPdfInfo();
                        break;
                        
                    case ".zip":
                    case ".rar":
                    case ".7z":
                        LoadArchiveInfo();
                        break;
                        
                    case ".doc":
                    case ".docx":
                    case ".xls":
                    case ".xlsx":
                    case ".ppt":
                    case ".pptx":
                        LoadOfficeFileInfo();
                        break;
                        
                    default:
                        LoadDefaultInfo();
                        break;
                }
            }
            catch (Exception ex)
            {
                txtPreview.Text = $"Error loading preview: {ex.Message}";
            }
        }

        private void LoadTextPreview()
        {
            try
            {
                // Try multiple encodings to detect the correct one
                string content = null;
                var encodings = new[] { Encoding.UTF8, Encoding.Unicode, Encoding.GetEncoding("windows-1252"), Encoding.ASCII };
                
                foreach (var encoding in encodings)
                {
                    try
                    {
                        using var reader = new StreamReader(_filePath, encoding, true);
                        content = reader.ReadToEnd();
                        
                        // Check if the content looks valid (no replacement characters)
                        if (!content.Contains('\uFFFD') && !string.IsNullOrWhiteSpace(content))
                        {
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                
                if (content == null)
                {
                    // Fallback to UTF8
                    using var reader = new StreamReader(_filePath, Encoding.UTF8);
                    content = reader.ReadToEnd();
                }
                
                // Limit preview length
                if (content.Length > 1000)
                {
                    content = content.Substring(0, 1000) + "\n\n[...More content - Double-click to view full...]";
                }
                
                txtPreview.Text = content;
                txtPreview.ReadOnly = true;
            }
            catch (Exception ex)
            {
                txtPreview.Text = $"Cannot read text file: {ex.Message}";
            }
        }

        private void LoadImagePreview()
        {
            try
            {
                using var img = Image.FromFile(_filePath);
                
                // Scale image to fit preview
                var maxSize = new Size(400, 300);
                var scaledSize = ScaleImageSize(img.Size, maxSize);
                
                var scaledImage = new Bitmap(scaledSize.Width, scaledSize.Height);
                using (var graphics = Graphics.FromImage(scaledImage))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(img, 0, 0, scaledSize.Width, scaledSize.Height);
                }
                
                pictureBoxPreview.Image = scaledImage;
                pictureBoxPreview.Visible = true;
                txtPreview.Visible = false;
                
                txtPreview.Text = $"Image {img.Width}x{img.Height} pixels\n" +
                                 $"Format: {img.RawFormat}\n" +
                                 $"Double-click to view original size";
            }
            catch (Exception ex)
            {
                txtPreview.Text = $"Cannot load image: {ex.Message}";
                pictureBoxPreview.Visible = false;
                txtPreview.Visible = true;
            }
        }

        private void LoadPdfInfo()
        {
            txtPreview.Text = $"PDF File: {_fileInfo.Name}\n" +
                             $"Size: {FormatFileSize(_fileInfo.Length)}\n\n" +
                             $"Double-click to open with default application";
        }

        private void LoadArchiveInfo()
        {
            try
            {
                var info = $"Archive file: {_fileInfo.Name}\n" +
                          $"Type: {_fileInfo.Extension.ToUpperInvariant()}\n" +
                          $"Size: {FormatFileSize(_fileInfo.Length)}\n\n";
                
                // Try to get archive content info using SharpCompress
                try
                {
                    using var archive = SharpCompress.Archives.ArchiveFactory.Open(_filePath);
                    var fileCount = archive.Entries.Count(e => !e.IsDirectory);
                    var folderCount = archive.Entries.Count(e => e.IsDirectory);
                    
                    info += $"Contains: {fileCount} files, {folderCount} folders\n\n";
                    info += "Contents:\n";
                    
                    var entries = archive.Entries.Take(10).ToList();
                    foreach (var entry in entries)
                    {
                        info += $"- {entry.Key} ({FormatFileSize(entry.Size)})\n";
                    }
                    
                    if (archive.Entries.Count() > 10)
                    {
                        info += $"... and {archive.Entries.Count() - 10} more files";
                    }
                }
                catch
                {
                    info += "Cannot read archive contents";
                }
                
                txtPreview.Text = info;
            }
            catch (Exception ex)
            {
                txtPreview.Text = $"Error reading archive file: {ex.Message}";
            }
        }

        private void LoadOfficeFileInfo()
        {
            txtPreview.Text = $"Office File: {_fileInfo.Name}\n" +
                             $"Type: {_fileInfo.Extension.ToUpperInvariant()}\n" +
                             $"Size: {FormatFileSize(_fileInfo.Length)}\n\n" +
                             $"Double-click to open with Microsoft Office";
        }

        private void LoadDefaultInfo()
        {
            txtPreview.Text = $"File: {_fileInfo.Name}\n" +
                             $"Type: {_fileInfo.Extension.ToUpperInvariant()}\n" +
                             $"Size: {FormatFileSize(_fileInfo.Length)}\n\n" +
                             $"No preview available for this file type.\n" +
                             $"Double-click to open with default application.";
        }

        private Size ScaleImageSize(Size original, Size maxSize)
        {
            double ratioX = (double)maxSize.Width / original.Width;
            double ratioY = (double)maxSize.Height / original.Height;
            double ratio = Math.Min(ratioX, ratioY);
            
            return new Size(
                (int)(original.Width * ratio),
                (int)(original.Height * ratio)
            );
        }

        private string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            
            return $"{number:n1} {suffixes[counter]}";
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(_filePath) 
                { 
                    UseShellExecute = true 
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open file: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{_filePath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open folder: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadImageReview(string filePath)
        {
            try
            {
                using var originalImage = Image.FromFile(filePath);
                var scaledImage = ScaleImageToFitHighQuality(originalImage, new Size(400, 300)); // Increased from 280x200
                
                if (pictureBoxPreview.Image != null)
                    pictureBoxPreview.Image.Dispose();
                    
                pictureBoxPreview.Image = scaledImage;
                pictureBoxPreview.Visible = true;
                txtPreview.Visible = false;
            }
            catch (Exception ex)
            {
                txtPreview.Text = $"Cannot load image: {ex.Message}";
                pictureBoxPreview.Visible = false;
                txtPreview.Visible = true;
            }
        }

        private Image ScaleImageToFitHighQuality(Image original, Size maxSize)
        {
            // Calculate scaling ratio while maintaining aspect ratio
            double ratioX = (double)maxSize.Width / original.Width;
            double ratioY = (double)maxSize.Height / original.Height;
            double ratio = Math.Min(ratioX, ratioY);
            
            // Don't scale up small images - keep original size if smaller than max
            if (ratio > 1.0)
                ratio = 1.0;
            
            int newWidth = (int)(original.Width * ratio);
            int newHeight = (int)(original.Height * ratio);
            
            // Create high-quality scaled image
            var scaled = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            scaled.SetResolution(original.HorizontalResolution, original.VerticalResolution);
            
            using (var graphics = Graphics.FromImage(scaled))
            {
                // Use highest quality settings
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                
                // Clear the canvas with transparent background
                graphics.Clear(Color.Transparent);
                
                // Draw the scaled image with high quality
                graphics.DrawImage(original, 0, 0, newWidth, newHeight);
            }
            
            return scaled;
        }
    }
}