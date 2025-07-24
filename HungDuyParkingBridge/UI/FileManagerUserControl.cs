using HungDuyParkingBridge.Services;
using HungDuyParkingBridge.Utils;
using System.Diagnostics;
using System.Text;
using System.Globalization;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace HungDuyParkingBridge.UI
{
    public partial class FileManagerUserControl : UserControl
    {
        private readonly string _savePath = @"C:\HungDuyParkingReceivedFiles";
        private FileCleanupService _cleanupService = new();
        private Panel fileReviewPanel;
        private Label lblSelectedFileInfo;
        private PictureBox pictureBoxPreview;
        private TextBox txtQuickPreview;
        private ListView listViewFiles;
        private Panel panel1, panel2, panel3, panel4;
        private Button btnRefresh, btnOpenFolder, btnDeleteSelected, btnPreview, btnCompare, btnCleanupOld;
        private NumericUpDown numCleanupDays;
        private Label label1, lblFileCount, lblTotalSize;
        private ColumnHeader columnFileName, columnSize, columnCreated, columnModified, columnPath;

        public FileManagerUserControl()
        {
            InitializeComponent();
            SetupControls();
            
            this.Load += FileManagerUserControl_Load;
        }

        private void SetupControls()
        {
            // Create all controls programmatically
            CreatePanels();
            CreateButtons();
            CreateListView();
            CreateLabels();
            SetupFileReviewPanel();
            SetupContextMenu();
            
            // Update UI based on authentication status
            UpdateAuthenticationStatus();
        }

        private void UpdateAuthenticationStatus()
        {
            bool isAdmin = HDParkingConst.IsAdminAuthenticated;

            // Hide/show delete-related buttons based on authentication
            if (btnDeleteSelected != null)
            {
                btnDeleteSelected.Visible = isAdmin;
            }

            if (btnCleanupOld != null)
            {
                btnCleanupOld.Visible = isAdmin;
                label1.Visible = isAdmin;
                numCleanupDays.Visible = isAdmin;
            }

            // Update button positions when delete buttons are hidden
            if (!isAdmin)
            {
                RepositionButtonsForGuestMode();
            }
        }

        private void RepositionButtonsForGuestMode()
        {
            // Reposition buttons when delete buttons are hidden
            if (btnRefresh != null) btnRefresh.Location = new Point(20, 15);
            if (btnOpenFolder != null) btnOpenFolder.Location = new Point(130, 15);
            if (btnPreview != null) btnPreview.Location = new Point(240, 15);
            if (btnCompare != null) btnCompare.Location = new Point(350, 15);
        }

        private void CreatePanels()
        {
            // Panel 1 - Title
            panel1 = new Panel
            {
                BackColor = Color.FromArgb(64, 64, 64),
                Dock = DockStyle.Top,
                Height = 60
            };

            var lblTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Text = "Received Files Manager"
            };
            panel1.Controls.Add(lblTitle);

            // Panel 2 - Buttons
            panel2 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10)
            };

            // Panel 3 - File list
            panel3 = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Panel 4 - Status
            panel4 = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(10)
            };

            this.Controls.AddRange(new Control[] { panel3, panel4, panel2, panel1 });
        }

        private void CreateButtons()
        {
            btnRefresh = new Button
            {
                BackColor = Color.FromArgb(0, 128, 0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(100, 30),
                Text = "Refresh",
                UseVisualStyleBackColor = false
            };
            btnRefresh.Click += btnRefresh_Click;

            btnOpenFolder = new Button
            {
                BackColor = Color.FromArgb(128, 128, 255),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(130, 15),
                Size = new Size(100, 30),
                Text = "Open Folder",
                UseVisualStyleBackColor = false
            };
            btnOpenFolder.Click += btnOpenFolder_Click;

            btnDeleteSelected = new Button
            {
                BackColor = Color.FromArgb(255, 128, 0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(240, 15),
                Size = new Size(100, 30),
                Text = "Delete",
                UseVisualStyleBackColor = false
            };
            btnDeleteSelected.Click += btnDeleteSelected_Click;

            btnPreview = new Button
            {
                BackColor = Color.FromArgb(255, 165, 0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(350, 15),
                Size = new Size(100, 30),
                Text = "Preview",
                UseVisualStyleBackColor = false
            };
            btnPreview.Click += btnPreview_Click;

            btnCompare = new Button
            {
                BackColor = Color.FromArgb(255, 20, 147),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(460, 15),
                Size = new Size(100, 30),
                Text = "Compare",
                UseVisualStyleBackColor = false
            };
            btnCompare.Click += btnCompare_Click;

            label1 = new Label
            {
                AutoSize = true,
                Location = new Point(570, 21),
                Text = "Older than:"
            };

            numCleanupDays = new NumericUpDown
            {
                Location = new Point(645, 18),
                Maximum = 365,
                Minimum = 1,
                Size = new Size(50, 23),
                Value = 7
            };

            btnCleanupOld = new Button
            {
                BackColor = Color.FromArgb(255, 128, 128),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(705, 15),
                Size = new Size(100, 30),
                Text = "Cleanup Old",
                UseVisualStyleBackColor = false
            };
            btnCleanupOld.Click += btnCleanupOld_Click;

            panel2.Controls.AddRange(new Control[] { 
                btnRefresh, btnOpenFolder, btnDeleteSelected, btnPreview, btnCompare,
                label1, numCleanupDays, btnCleanupOld 
            });
        }

        private void CreateListView()
        {
            columnFileName = new ColumnHeader { Text = "File Name", Width = 300 };
            columnSize = new ColumnHeader { Text = "Size", Width = 100 };
            columnCreated = new ColumnHeader { Text = "Created", Width = 150 };
            columnModified = new ColumnHeader { Text = "Modified", Width = 150 };
            columnPath = new ColumnHeader { Text = "Path", Width = 250 };

            listViewFiles = new ListView
            {
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = true,
                UseCompatibleStateImageBehavior = false,
                View = View.Details
            };

            listViewFiles.Columns.AddRange(new ColumnHeader[] {
                columnFileName, columnSize, columnCreated, columnModified, columnPath
            });

            listViewFiles.Click += listViewFiles_Click;
            listViewFiles.DoubleClick += listViewFiles_DoubleClick;
            listViewFiles.SelectedIndexChanged += listViewFiles_SelectedIndexChanged;

            panel3.Controls.Add(listViewFiles);
        }

        private void CreateLabels()
        {
            lblFileCount = new Label
            {
                AutoSize = true,
                Location = new Point(20, 15),
                Text = "Total files: 0"
            };

            lblTotalSize = new Label
            {
                AutoSize = true,
                Location = new Point(200, 15),
                Text = "Storage: 0 B"
            };

            panel4.Controls.AddRange(new Control[] { lblFileCount, lblTotalSize });
        }

        private void FileManagerUserControl_Load(object sender, EventArgs e)
        {
            RefreshFileList();
            UpdateStats();
            UpdateAuthenticationStatus(); // Ensure auth status is updated on load
        }

        // Public method to update auth status from parent form
        public void RefreshAuthenticationStatus()
        {
            UpdateAuthenticationStatus();
            SetupContextMenu(); // Refresh context menu to update delete options
        }

        // Include all the methods from FileManagerForm here
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
                Text = "Select a file to view preview..."
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
                Text = "Select a file to view detailed information...",
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
            
            var previewItem = new ToolStripMenuItem("Preview");
            previewItem.Click += (s, e) => ShowFilePreview();
            contextMenu.Items.Add(previewItem);
            
            var openItem = new ToolStripMenuItem("Open File");
            openItem.Click += (s, e) => OpenSelectedFile();
            contextMenu.Items.Add(openItem);
            
            var openFolderItem = new ToolStripMenuItem("Open File Location");
            openFolderItem.Click += (s, e) => OpenFileLocation();
            contextMenu.Items.Add(openFolderItem);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            var compareItem = new ToolStripMenuItem("Compare with...");
            compareItem.Click += (s, e) => btnCompare_Click(s, e);
            contextMenu.Items.Add(compareItem);
            
            // Only add delete option if user is authenticated
            if (HDParkingConst.IsAdminAuthenticated)
            {
                contextMenu.Items.Add(new ToolStripSeparator());
                
                var deleteItem = new ToolStripMenuItem("Delete");
                deleteItem.Click += (s, e) => btnDeleteSelected_Click(s, e);
                contextMenu.Items.Add(deleteItem);
            }
            
            listViewFiles.ContextMenuStrip = contextMenu;
        }

        // Include all the other methods from FileManagerForm...
        // (RefreshFileList, UpdateStats, event handlers, etc.
        // For brevity, I'll include key methods:

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
                    
                    listViewFiles.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file list: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStats()
        {
            try
            {
                int fileCount = _cleanupService.GetFileCount();
                long totalSize = _cleanupService.GetTotalSize();
                
                lblFileCount.Text = $"Total files: {fileCount}";
                lblTotalSize.Text = $"Storage: {FormatFileSize(totalSize)}";
            }
            catch
            {
                lblFileCount.Text = "Total files: 0";
                lblTotalSize.Text = "Storage: 0 B";
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

        // Event handlers
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshFileList();
            UpdateStats();
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if (!HDParkingConst.IsAdminAuthenticated)
            {
                MessageBox.Show("❌ Admin authentication required to open folder", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (Directory.Exists(_savePath))
                {
                    Process.Start("explorer.exe", _savePath);
                }
                else
                {
                    MessageBox.Show("Folder does not exist!", "Notice", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open folder: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (!HDParkingConst.IsAdminAuthenticated)
            {
                MessageBox.Show("❌ Admin authentication required to delete files", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (listViewFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select files to delete!", "Notice", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {listViewFiles.SelectedItems.Count} selected file(s)?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                        MessageBox.Show($"Error deleting file {item.Text}: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (deletedCount > 0)
                {
                    MessageBox.Show($"Successfully deleted {deletedCount} file(s)!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshFileList();
                    UpdateStats();
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
                MessageBox.Show("Please select a file to preview!", "Notice", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count != 2)
            {
                MessageBox.Show("Please select exactly 2 files to compare!", "Notice", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string file1Path = listViewFiles.SelectedItems[0].Tag?.ToString() ?? "";
                string file2Path = listViewFiles.SelectedItems[1].Tag?.ToString() ?? "";
                
                if (File.Exists(file1Path) && File.Exists(file2Path))
                {
                    // Open in new tab instead of new window
                    OpenFileCompareTab(file1Path, file2Path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot compare files: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCleanupOld_Click(object sender, EventArgs e)
        {
            if (!HDParkingConst.IsAdminAuthenticated)
            {
                MessageBox.Show("❌ Admin authentication required to cleanup files", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete all files older than {numCleanupDays.Value} days?", 
                "Confirm Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _cleanupService.IsEnabled = true;
                _cleanupService.DeleteAfterDays = (int)numCleanupDays.Value;
                _cleanupService.ForceCleanup();
                
                MessageBox.Show("Old files cleanup completed!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                RefreshFileList();
                UpdateStats();
            }
        }

        private void listViewFiles_Click(object sender, EventArgs e)
        {
            ShowFileReview();
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowFileReview();
        }

        private void listViewFiles_DoubleClick(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                string filePath = listViewFiles.SelectedItems[0].Tag?.ToString() ?? "";
                if (File.Exists(filePath))
                {
                    // Open in new tab instead of new window
                    OpenFilePreviewTab(filePath);
                }
            }
        }

        // Methods to open in tabs instead of new windows
        private void OpenFilePreviewTab(string filePath)
        {
            // Find the parent MainForm and add a new tab
            var mainForm = this.FindForm() as MainForm;
            if (mainForm != null)
            {
                // mainForm.AddFilePreviewTab(filePath);
                // For now, just show in dialog until tab system is fully implemented
                var previewForm = new FilePreviewForm(filePath);
                previewForm.ShowDialog();
            }
        }

        private void OpenFileCompareTab(string file1Path, string file2Path)
        {
            // Find the parent MainForm and add a new tab
            var mainForm = this.FindForm() as MainForm;
            if (mainForm != null)
            {
                // mainForm.AddFileCompareTab(file1Path, file2Path);
                // For now, just show in dialog until tab system is fully implemented
                var compareForm = new FileCompareForm(file1Path, file2Path);
                compareForm.ShowDialog();
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
                        OpenFilePreviewTab(filePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cannot preview file: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowFileReview()
        {
            if (listViewFiles.SelectedItems.Count == 0)
            {
                lblSelectedFileInfo.Text = "Select a file to view detailed information...";
                txtQuickPreview.Text = "Select a file to view preview...";
                pictureBoxPreview.Visible = false;
                txtQuickPreview.Visible = true;
                return;
            }

            if (listViewFiles.SelectedItems.Count > 1)
            {
                lblSelectedFileInfo.Text = $"Selected {listViewFiles.SelectedItems.Count} files\n\n" +
                                          $"💡 Click 'Compare' to compare 2 files\n" +
                                          $"💡 Select 1 file to view details";
                txtQuickPreview.Text = "Multiple files selected.\nSelect 1 file to view preview or 2 files to compare.";
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
                lblSelectedFileInfo.Text = $"FILE NAME\n{fileInfo.Name}\n\n" +
                                          $"SIZE\n{FormatFileSize(fileInfo.Length)}\n\n" +
                                          $"CREATED\n{fileInfo.CreationTime:dd/MM/yyyy HH:mm:ss}\n\n" +
                                          $"MODIFIED\n{fileInfo.LastWriteTime:dd/MM/yyyy HH:mm:ss}\n\n" +
                                          $"TYPE\n{fileInfo.Extension.ToUpperInvariant()}\n\n" +
                                          $"PATH\n{fileInfo.DirectoryName}";

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
                lblSelectedFileInfo.Text = $"❌ ERROR\n{ex.Message}";
                txtQuickPreview.Text = "Cannot load preview";
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
                txtQuickPreview.Text = $"❌ Cannot load image:\n{ex.Message}";
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
                    preview += "\n\n[...See more in 'Preview'...]";
                }
                
                txtQuickPreview.Text = preview;
                pictureBoxPreview.Visible = false;
                txtQuickPreview.Visible = true;
            }
            catch (Exception ex)
            {
                txtQuickPreview.Text = $"❌ Cannot read file:\n{ex.Message}";
                pictureBoxPreview.Visible = false;
                txtQuickPreview.Visible = true;
            }
        }

        private void ShowFileTypePreview(FileInfo fileInfo)
        {
            txtQuickPreview.Text = $"📋 {fileInfo.Extension.ToUpperInvariant()} FILE\n\n" +
                                  $"Name: {fileInfo.Name}\n" +
                                  $"Size: {FormatFileSize(fileInfo.Length)}\n\n" +
                                  $"💡 No preview available for this file type.\n" +
                                  $"💡 Click 'Preview' for detailed view.\n" +
                                  $"💡 Double-click to open file.";
            pictureBoxPreview.Visible = false;
            txtQuickPreview.Visible = true;
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
                    MessageBox.Show($"❌ Cannot open file: {ex.Message}", "Error", 
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
                    MessageBox.Show($"❌ Cannot open file location: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}