using HungDuyParkingBridge.Services;
using System.Diagnostics;
using System.Text;
using System.Globalization;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace HungDuyParkingBridge.UI
{
    public partial class FileManagerForm : Form
    {
        private readonly string _savePath = @"C:\HungDuyParkingReceivedFiles";
        private FileCleanupService _cleanupService = new();
        private Panel fileReviewPanel;
        private Label lblSelectedFileInfo;
        private PictureBox pictureBoxPreview;
        private TextBox txtQuickPreview;

        public FileManagerForm()
        {
            // Set up Vietnamese culture and encoding support
            Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            InitializeComponent();
            this.Load += FileManagerForm_Load;
        }

        private void FileManagerForm_Load(object sender, EventArgs e)
        {
            RefreshFileList();
            UpdateStats();
            SetupContextMenu();
            SetupFileReviewPanel();
            
            // Enable multi-select for comparison
            listViewFiles.MultiSelect = true;
            
            // Add selection changed event for immediate updates
            listViewFiles.SelectedIndexChanged += listViewFiles_SelectedIndexChanged;
            
            // Make the form start maximized for better use of space
            this.WindowState = FormWindowState.Maximized;
        }

        private void SetupFileReviewPanel()
        {
            // Create right-side review panel - INCREASED SIZE
            fileReviewPanel = new Panel
            {
                Width = 500, // Increased from 400 to 500
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            // Add a splitter to make the panel resizable
            var splitter = new Splitter
            {
                Dock = DockStyle.Right,
                Width = 4,
                BackColor = Color.Gray,
                Cursor = Cursors.SizeWE
            };

            // TOP: File review area (image/text preview) - INCREASED HEIGHT
            var previewPanel = new Panel
            {
                Height = 400, // Increased from 300 to 400
                Dock = DockStyle.Top,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8) // Increased padding for better appearance
            };

            // Preview controls - OPTIMIZED FOR LARGER IMAGES
            pictureBoxPreview = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Zoom, // Changed back to Zoom for better fitting
                Visible = false,
                BackColor = Color.White
            };

            txtQuickPreview = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 10F), // Increased font size
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Text = "Chọn file để xem preview..."
            };

            previewPanel.Controls.Add(pictureBoxPreview);
            previewPanel.Controls.Add(txtQuickPreview);

            // BOTTOM: Detailed file info area
            var infoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            lblSelectedFileInfo = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F),
                Text = "Chọn một file để xem thông tin chi tiết...",
                TextAlign = ContentAlignment.TopLeft,
                AutoSize = false,
                BackColor = Color.Transparent
            };

            infoPanel.Controls.Add(lblSelectedFileInfo);

            // Add a splitter between top preview and bottom info
            var horizontalSplitter = new Splitter
            {
                Dock = DockStyle.Top,
                Height = 4,
                BackColor = Color.LightGray,
                Cursor = Cursors.SizeNS
            };

            // Add controls to the review panel in correct order
            fileReviewPanel.Controls.Add(infoPanel);        // Bottom (added first for dock order)
            fileReviewPanel.Controls.Add(horizontalSplitter); // Middle splitter
            fileReviewPanel.Controls.Add(previewPanel);      // Top

            // Add splitter and panel to form
            this.Controls.Add(splitter);
            this.Controls.Add(fileReviewPanel);
        }

        private void SetupContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            
            var previewItem = new ToolStripMenuItem("Xem trước");
            previewItem.Click += (s, e) => ShowFilePreview();
            contextMenu.Items.Add(previewItem);
            
            var openItem = new ToolStripMenuItem("Mở file");
            openItem.Click += (s, e) => OpenSelectedFile();
            contextMenu.Items.Add(openItem);
            
            var openFolderItem = new ToolStripMenuItem("Mở thư mục chứa");
            openFolderItem.Click += (s, e) => OpenFileLocation();
            contextMenu.Items.Add(openFolderItem);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            var compareItem = new ToolStripMenuItem("So sánh với...");
            compareItem.Click += (s, e) => btnCompare_Click(s, e);
            contextMenu.Items.Add(compareItem);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            var deleteItem = new ToolStripMenuItem("Xóa");
            deleteItem.Click += (s, e) => btnDeleteSelected_Click(s, e);
            contextMenu.Items.Add(deleteItem);
            
            listViewFiles.ContextMenuStrip = contextMenu;
        }

        private void RefreshFileList()
        {
            listViewFiles.Items.Clear();
            
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
                return;
            }

            try
            {
                var files = Directory.GetFiles(_savePath, "*", SearchOption.AllDirectories)
                    .Where(f => !Path.GetFileName(f).Equals("metadata.json", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(f => new FileInfo(f).CreationTime);

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    var item = new ListViewItem(Path.GetFileName(file));
                    
                    item.SubItems.Add(FormatFileSize(fileInfo.Length));
                    item.SubItems.Add(fileInfo.CreationTime.ToString("dd/MM/yyyy HH:mm"));
                    item.SubItems.Add(fileInfo.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
                    item.SubItems.Add(Path.GetDirectoryName(file)?.Replace(_savePath, "") ?? "");
                    item.Tag = file; // Store full path
                    
                    // Add file type icon/indicator
                    item.ImageIndex = GetFileTypeImageIndex(fileInfo.Extension);
                    
                    listViewFiles.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách file: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetFileTypeImageIndex(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".txt" or ".log" => 0,
                ".jpg" or ".png" or ".gif" or ".bmp" => 1,
                ".pdf" => 2,
                ".zip" or ".rar" or ".7z" => 3,
                ".doc" or ".docx" => 4,
                ".xls" or ".xlsx" => 5,
                _ => 6
            };
        }

        private void UpdateStats()
        {
            try
            {
                int fileCount = _cleanupService.GetFileCount();
                long totalSize = _cleanupService.GetTotalSize();
                
                lblFileCount.Text = $"Tổng số file: {fileCount}";
                lblTotalSize.Text = $"Dung lượng: {FormatFileSize(totalSize)}";
            }
            catch
            {
                lblFileCount.Text = "Tổng số file: 0";
                lblTotalSize.Text = "Dung lượng: 0 B";
            }
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

        // ENHANCED: Single-click event to show file review in side panel
        private void listViewFiles_Click(object sender, EventArgs e)
        {
            ShowFileReview();
        }

        // ENHANCED: Selection changed event for immediate review update
        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowFileReview();
        }

        // NEW: Show file review in right panel - top area for preview, bottom area for detailed info
        private void ShowFileReview()
        {
            if (listViewFiles.SelectedItems.Count == 0)
            {
                lblSelectedFileInfo.Text = "Chọn một file để xem thông tin chi tiết...";
                txtQuickPreview.Text = "Chọn file để xem preview...";
                pictureBoxPreview.Visible = false;
                txtQuickPreview.Visible = true;
                return;
            }

            if (listViewFiles.SelectedItems.Count > 1)
            {
                lblSelectedFileInfo.Text = $"📋 Đã chọn {listViewFiles.SelectedItems.Count} file\n\n" +
                                          $"💡 Nhấn 'So sánh' để so sánh 2 file\n" +
                                          $"💡 Chọn 1 file để xem chi tiết";
                txtQuickPreview.Text = "Đã chọn nhiều file.\nChọn 1 file để xem preview hoặc 2 file để so sánh.";
                pictureBoxPreview.Visible = false;
                txtQuickPreview.Visible = true;
                return;
            }

            try
            {
                string filePath = listViewFiles.SelectedItems[0].Tag?.ToString() ?? "";
                if (!File.Exists(filePath))
                    return;

                var fileInfo = new FileInfo(filePath);
                
                // Update bottom info panel with detailed file information
                lblSelectedFileInfo.Text = $"📂 TÊN FILE\n{fileInfo.Name}\n\n" +
                                          $"📏 KÍCH THƯỚC\n{FormatFileSize(fileInfo.Length)}\n\n" +
                                          $"📅 NGÀY TẠO\n{fileInfo.CreationTime:dd/MM/yyyy HH:mm:ss}\n\n" +
                                          $"✏️ NGÀY SỬA\n{fileInfo.LastWriteTime:dd/MM/yyyy HH:mm:ss}\n\n" +
                                          $"🏷️ LOẠI FILE\n{fileInfo.Extension.ToUpperInvariant()}\n\n" +
                                          $"📍 ĐƯỜNG DẪN\n{fileInfo.DirectoryName}";

                string extension = fileInfo.Extension.ToLowerInvariant();
                
                // Show preview in top panel
                if (IsImageFile(extension))
                {
                    LoadImageReview(filePath);
                }
                else if (IsTextFile(extension))
                {
                    LoadTextReview(filePath);
                }
                else
                {
                    ShowFileTypePreview(fileInfo);
                }
            }
            catch (Exception ex)
            {
                lblSelectedFileInfo.Text = $"❌ LỖIX\n{ex.Message}";
                txtQuickPreview.Text = "Không thể tải preview";
                pictureBoxPreview.Visible = false;
                txtQuickPreview.Visible = true;
            }
        }

        private void LoadImageReview(string filePath)
        {
            try
            {
                using var originalImage = Image.FromFile(filePath);
                
                // Get the actual available size for the larger preview panel
                var availableSize = new Size(
                    fileReviewPanel.Width - 40,  // Account for padding and borders (increased)
                    400 - 20                     // Preview panel height minus padding (increased)
                );
                
                // Use high-quality scaling with better size handling
                var scaledImage = ScaleImageToFitBetter(originalImage, availableSize);
                
                if (pictureBoxPreview.Image != null)
                    pictureBoxPreview.Image.Dispose();
                    
                pictureBoxPreview.Image = scaledImage;
                pictureBoxPreview.Visible = true;
                txtQuickPreview.Visible = false;
            }
            catch (Exception ex)
            {
                txtQuickPreview.Text = $"❌ Không thể tải ảnh:\n{ex.Message}";
                pictureBoxPreview.Visible = false;
                txtQuickPreview.Visible = true;
            }
        }

        private void LoadTextReview(string filePath)
        {
            try
            {
                using var reader = new StreamReader(filePath, Encoding.UTF8);
                char[] buffer = new char[1200]; // Optimized for preview panel size
                int charsRead = reader.Read(buffer, 0, buffer.Length);
                
                string preview = new string(buffer, 0, charsRead);
                if (charsRead == 1200)
                {
                    preview += "\n\n[...Xem thêm trong 'Xem trước'...]";
                }
                
                txtQuickPreview.Text = preview;
                pictureBoxPreview.Visible = false;
                txtQuickPreview.Visible = true;
            }
            catch (Exception ex)
            {
                txtQuickPreview.Text = $"❌ Không thể đọc file:\n{ex.Message}";
                pictureBoxPreview.Visible = false;
                txtQuickPreview.Visible = true;
            }
        }

        private void ShowFileTypePreview(FileInfo fileInfo)
        {
            txtQuickPreview.Text = $"📋 {fileInfo.Extension.ToUpperInvariant()} FILE\n\n" +
                                  $"Tên: {fileInfo.Name}\n" +
                                  $"Kích thước: {FormatFileSize(fileInfo.Length)}\n\n" +
                                  $"💡 Không có preview cho loại file này.\n" +
                                  $"💡 Nhấn 'Xem trước' để xem chi tiết.\n" +
                                  $"💡 Double-click để mở file.";
            pictureBoxPreview.Visible = false;
            txtQuickPreview.Visible = true;
        }

        private void ShowFileTypeInfo(FileInfo fileInfo)
        {
            var header = $"📋 File Information - {fileInfo.Extension.ToUpperInvariant()}\n";
            header += "".PadRight(60, '─') + "\n\n";
            
            txtQuickPreview.Text = header +
                                  $"📂 Tên file: {fileInfo.Name}\n" +
                                  $"📏 Kích thước: {FormatFileSize(fileInfo.Length)}\n" +
                                  $"📅 Ngày tạo: {fileInfo.CreationTime:dd/MM/yyyy HH:mm:ss}\n" +
                                  $"✏️ Ngày sửa: {fileInfo.LastWriteTime:dd/MM/yyyy HH:mm:ss}\n" +
                                  $"📍 Đường dẫn: {fileInfo.FullName}\n" +
                                  $"🏷️ Loại file: {fileInfo.Extension.ToUpperInvariant()}\n\n" +
                                  $"💡 Thao tác:\n" +
                                  $"   • Nhấn 'Xem trước' để xem chi tiết\n" +
                                  $"   • Double-click để mở file\n" +
                                  $"   • Right-click để xem menu";
            pictureBoxPreview.Visible = false;
            txtQuickPreview.Visible = true;
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
            var scaled = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppPArgb);
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

        private Image ScaleImageToFit(Image original, Size maxSize)
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

        private Image ScaleImageToFitBetter(Image original, Size maxSize)
        {
            // Calculate scaling ratio while maintaining aspect ratio
            double ratioX = (double)maxSize.Width / original.Width;
            double ratioY = (double)maxSize.Height / original.Height;
            double ratio = Math.Min(ratioX, ratioY);
            
            // Allow scaling up for small images, but limit to reasonable size
            if (ratio > 3.0) // Don't scale beyond 3x for very small images
                ratio = 3.0;
            else if (ratio < 0.1) // Don't scale below 10% for very large images
                ratio = 0.1;
            
            int newWidth = (int)(original.Width * ratio);
            int newHeight = (int)(original.Height * ratio);
            
            // Ensure minimum readable size for very small images
            if (newWidth < 100 && newHeight < 100)
            {
                double minRatio = Math.Max(100.0 / original.Width, 100.0 / original.Height);
                if (minRatio <= 3.0) // Only apply if not too extreme
                {
                    ratio = minRatio;
                    newWidth = (int)(original.Width * ratio);
                    newHeight = (int)(original.Height * ratio);
                }
            }
            
            // Create high-quality scaled image
            var scaled = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppPArgb);
            scaled.SetResolution(original.HorizontalResolution, original.VerticalResolution);
            
            using (var graphics = Graphics.FromImage(scaled))
            {
                // Use highest quality settings
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                
                // Clear the canvas with transparent background
                graphics.Clear(Color.Transparent);
                
                // Draw the scaled image with high quality
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

        // NEW: Compare button functionality
        private void btnCompare_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count != 2)
            {
                MessageBox.Show("Vui lòng chọn đúng 2 file để so sánh!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string file1Path = listViewFiles.SelectedItems[0].Tag?.ToString() ?? "";
                string file2Path = listViewFiles.SelectedItems[1].Tag?.ToString() ?? "";
                
                if (File.Exists(file1Path) && File.Exists(file2Path))
                {
                    var compareForm = new FileCompareForm(file1Path, file2Path);
                    compareForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể so sánh file: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshFileList();
            UpdateStats();
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(_savePath))
                {
                    Process.Start("explorer.exe", _savePath);
                }
                else
                {
                    MessageBox.Show("Thư mục không tồn tại!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở thư mục: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn file để xóa!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa {listViewFiles.SelectedItems.Count} file đã chọn?", 
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                int deletedCount = 0;
                foreach (ListViewItem item in listViewFiles.SelectedItems)
                {
                    try
                    {
                        string filePath = item.Tag?.ToString() ?? "";
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            deletedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi xóa file {item.Text}: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (deletedCount > 0)
                {
                    MessageBox.Show($"Đã xóa thành công {deletedCount} file!", "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshFileList();
                    UpdateStats();
                }
            }
        }

        private void btnCleanupOld_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa tất cả file cũ hơn {numCleanupDays.Value} ngày?", 
                "Xác nhận dọn dẹp", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _cleanupService.IsEnabled = true;
                _cleanupService.DeleteAfterDays = (int)numCleanupDays.Value;
                _cleanupService.ForceCleanup();
                
                MessageBox.Show("Đã hoàn thành dọn dẹp file cũ!", "Thành công", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                RefreshFileList();
                UpdateStats();
            }
        }

        private void listViewFiles_DoubleClick(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                string filePath = listViewFiles.SelectedItems[0].Tag?.ToString() ?? "";
                if (File.Exists(filePath))
                {
                    var previewForm = new FilePreviewForm(filePath);
                    var result = previewForm.ShowDialog();
                    
                    if (result == DialogResult.OK || 
                        MessageBox.Show("Bạn có muốn mở file này không?", "Mở file", 
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        OpenSelectedFile();
                    }
                }
            }
        }

        private void ShowFilePreview()
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                try
                {
                    string filePath = listViewFiles.SelectedItems[0].Tag?.ToString() ?? "";
                    if (File.Exists(filePath))
                    {
                        var previewForm = new FilePreviewForm(filePath);
                        previewForm.Show();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể xem trước file: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OpenSelectedFile()
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                try
                {
                    string filePath = listViewFiles.SelectedItems[0].Tag?.ToString() ?? "";
                    if (File.Exists(filePath))
                    {
                        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở file: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OpenFileLocation()
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                try
                {
                    string filePath = listViewFiles.SelectedItems[0].Tag?.ToString() ?? "";
                    if (File.Exists(filePath))
                    {
                        Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở vị trí file: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                ShowFilePreview();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn file để xem trước!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}