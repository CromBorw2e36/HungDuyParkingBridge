using System.Text;
using System.Globalization;

namespace HungDuyParkingBridge.UI
{
    public partial class FileCompareForm : Form
    {
        private string _file1Path;
        private string _file2Path;
        private FileInfo _file1Info;
        private FileInfo _file2Info;

        public FileCompareForm(string file1Path, string file2Path)
        {
            _file1Path = file1Path;
            _file2Path = file2Path;
            _file1Info = new FileInfo(file1Path);
            _file2Info = new FileInfo(file2Path);
            
            // Set up Vietnamese culture and encoding support
            Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
            
            InitializeComponent();
            LoadFileComparison();
        }

        private void LoadFileComparison()
        {
            try
            {
                // Load file information
                LoadFileInfo();
                
                // Compare files based on type
                string ext1 = _file1Info.Extension.ToLowerInvariant();
                string ext2 = _file2Info.Extension.ToLowerInvariant();
                
                if (ext1 == ext2 && IsTextFile(ext1))
                {
                    CompareTextFiles();
                }
                else if (ext1 == ext2 && IsImageFile(ext1))
                {
                    CompareImageFiles();
                }
                else
                {
                    ShowBasicComparison();
                }
            }
            catch (Exception ex)
            {
                txtFile1.Text = $"Error comparing files: {ex.Message}";
                txtFile2.Text = $"Error comparing files: {ex.Message}";
            }
        }

        private void LoadFileInfo()
        {
            lblFile1Info.Text = $"Tập tin 1: {_file1Info.Name} ({FormatFileSize(_file1Info.Length)}) - {_file1Info.LastWriteTime:dd/MM/yyyy HH:mm}";
            lblFile2Info.Text = $"Tập tin 2: {_file2Info.Name} ({FormatFileSize(_file2Info.Length)}) - {_file2Info.LastWriteTime:dd/MM/yyyy HH:mm}";
            
            // Show file size difference
            long sizeDiff = _file2Info.Length - _file1Info.Length;
            string sizeDiffText = sizeDiff == 0 ? "Same size" : 
                                 sizeDiff > 0 ? $"File 2 is larger by {FormatFileSize(sizeDiff)}" :
                                 $"File 1 is larger by {FormatFileSize(-sizeDiff)}";
            
            lblSizeDiff.Text = $"Difference: {sizeDiffText}";
            
            // Show time difference
            var timeDiff = _file2Info.LastWriteTime - _file1Info.LastWriteTime;
            string timeDiffText = timeDiff.TotalSeconds == 0 ? "Same modification time" :
                                 timeDiff.TotalSeconds > 0 ? $"File 2 is newer by {FormatTimeDiff(timeDiff)}" :
                                 $"File 1 is newer by {FormatTimeDiff(-timeDiff)}";
            
            lblTimeDiff.Text = $"Time: {timeDiffText}";
        }

        private void CompareTextFiles()
        {
            try
            {
                // Read both files with proper encoding detection
                string content1 = ReadFileWithEncoding(_file1Path);
                string content2 = ReadFileWithEncoding(_file2Path);
                
                // Show content side by side
                txtFile1.Text = content1;
                txtFile2.Text = content2;
                
                // Highlight differences
                HighlightTextDifferences(content1, content2);
                
                // Show summary
                var lines1 = content1.Split('\n');
                var lines2 = content2.Split('\n');
                
                lblComparisonResult.Text = $"File 1: {lines1.Length} lines, {content1.Length} characters\n" +
                                          $"File 2: {lines2.Length} lines, {content2.Length} characters\n" +
                                          $"Identical: {(content1 == content2 ? "Yes" : "No")}";
            }
            catch (Exception ex)
            {
                txtFile1.Text = $"Error reading text file: {ex.Message}";
                txtFile2.Text = $"Error reading text file: {ex.Message}";
            }
        }

        private string ReadFileWithEncoding(string filePath)
        {
            // Try multiple encodings to detect the correct one
            var encodings = new[] { Encoding.UTF8, Encoding.Unicode, Encoding.GetEncoding("windows-1252"), Encoding.ASCII };
            
            foreach (var encoding in encodings)
            {
                try
                {
                    using var reader = new StreamReader(filePath, encoding, true);
                    string content = reader.ReadToEnd();
                    
                    // Check if the content looks valid (no replacement characters)
                    if (!content.Contains('\uFFFD') && !string.IsNullOrWhiteSpace(content))
                    {
                        return content;
                    }
                }
                catch
                {
                    continue;
                }
            }
            
            // Fallback to UTF8
            using var fallbackReader = new StreamReader(filePath, Encoding.UTF8);
            return fallbackReader.ReadToEnd();
        }

        private void CompareImageFiles()
        {
            try
            {
                using var img1 = Image.FromFile(_file1Path);
                using var img2 = Image.FromFile(_file2Path);
                
                // Scale images to fit preview - INCREASED SIZE
                var maxSize = new Size(400, 300); // Increased from 300x200
                var scaledImg1 = ScaleImage(img1, maxSize);
                var scaledImg2 = ScaleImage(img2, maxSize);
                
                pictureBox1.Image = scaledImg1;
                pictureBox2.Image = scaledImg2;
                
                pictureBox1.Visible = true;
                pictureBox2.Visible = true;
                txtFile1.Visible = false;
                txtFile2.Visible = false;
                
                // Show image comparison info
                lblComparisonResult.Text = $"Image 1: {img1.Width}x{img1.Height} pixels, {img1.RawFormat}\n" +
                                          $"Image 2: {img2.Width}x{img2.Height} pixels, {img2.RawFormat}\n" +
                                          $"Same dimensions: {(img1.Size == img2.Size ? "Yes" : "No")}\n" +
                                          $"Same format: {(img1.RawFormat.Equals(img2.RawFormat) ? "Yes" : "No")}";
            }
            catch (Exception ex)
            {
                lblComparisonResult.Text = $"Error comparing images: {ex.Message}";
                pictureBox1.Visible = false;
                pictureBox2.Visible = false;
                txtFile1.Visible = true;
                txtFile2.Visible = true;
            }
        }

        private void ShowBasicComparison()
        {
            txtFile1.Text = $"Tên: {_file1Info.Name}\n" +
                           $"Loại: {_file1Info.Extension.ToUpperInvariant()}\n" +
                           $"Kích thước: {FormatFileSize(_file1Info.Length)}\n" +
                           $"Ngày tạo: {_file1Info.CreationTime:dd/MM/yyyy HH:mm:ss}\n" +
                           $"Chỉnh sửa: {_file1Info.LastWriteTime:dd/MM/yyyy HH:mm:ss}";
                           
            txtFile2.Text = $"Tên: {_file2Info.Name}\n" +
                           $"Loại: {_file2Info.Extension.ToUpperInvariant()}\n" +
                           $"Kích thước: {FormatFileSize(_file2Info.Length)}\n" +
                           $"Ngày tạo: {_file2Info.CreationTime:dd/MM/yyyy HH:mm:ss}\n" +
                           $"Chỉnh sửa: {_file2Info.LastWriteTime:dd/MM/yyyy HH:mm:ss}";
            
            bool sameType = _file1Info.Extension.Equals(_file2Info.Extension, StringComparison.OrdinalIgnoreCase);
            bool sameSize = _file1Info.Length == _file2Info.Length;
            bool sameContent = sameSize && File.ReadAllBytes(_file1Path).SequenceEqual(File.ReadAllBytes(_file2Path));
            
            lblComparisonResult.Text = $"Same file type: {(sameType ? "Yes" : "No")}\n" +
                                      $"Same size: {(sameSize ? "Yes" : "No")}\n" +
                                      $"Same content: {(sameContent ? "Yes" : "No")}";
        }

        private void HighlightTextDifferences(string content1, string content2)
        {
            // Simple line-by-line comparison
            var lines1 = content1.Split('\n');
            var lines2 = content2.Split('\n');
            
            int differentLines = 0;
            for (int i = 0; i < Math.Max(lines1.Length, lines2.Length); i++)
            {
                string line1 = i < lines1.Length ? lines1[i] : "";
                string line2 = i < lines2.Length ? lines2[i] : "";
                
                if (line1 != line2)
                {
                    differentLines++;
                }
            }
            
            if (differentLines > 0)
            {
                lblComparisonResult.Text += $"\nDifferent lines: {differentLines}";
            }
        }

        private Image ScaleImage(Image original, Size maxSize)
        {
            double ratioX = (double)maxSize.Width / original.Width;
            double ratioY = (double)maxSize.Height / original.Height;
            double ratio = Math.Min(ratioX, ratioY);
            
            int newWidth = (int)(original.Width * ratio);
            int newHeight = (int)(original.Height * ratio);
            
            var scaled = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(scaled))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(original, 0, 0, newWidth, newHeight);
            }
            
            return scaled;
        }

        private bool IsTextFile(string extension)
        {
            return extension switch
            {
                ".txt" or ".log" or ".csv" or ".xml" or ".json" or ".config" or ".cs" or ".html" or ".css" or ".js" => true,
                _ => false
            };
        }

        private bool IsImageFile(string extension)
        {
            return extension switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".tiff" => true,
                _ => false
            };
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

        private string FormatTimeDiff(TimeSpan diff)
        {
            if (diff.TotalDays >= 1)
                return $"{(int)diff.TotalDays} days";
            if (diff.TotalHours >= 1)
                return $"{(int)diff.TotalHours} hours";
            if (diff.TotalMinutes >= 1)
                return $"{(int)diff.TotalMinutes} minutes";
            return $"{(int)diff.TotalSeconds} seconds";
        }

        private void btnOpenFile1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(_file1Path) 
                { 
                    UseShellExecute = true 
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open file 1: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenFile2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(_file2Path) 
                { 
                    UseShellExecute = true 
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open file 2: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}