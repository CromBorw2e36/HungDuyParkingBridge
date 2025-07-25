using Microsoft.Win32;
using HungDuyParkingBridge.Services;
using HungDuyParkingBridge.Utilities;
using HungDuyParkingBridge.Utils;
using System.Text;
using System.Globalization;
using System.Drawing.Drawing2D;

namespace HungDuyParkingBridge.UI
{
    public partial class MainForm : Form
    {
        NotifyIcon trayIcon;
        ContextMenuStrip trayMenu;
        private FileReceiverService _receiver = new();
        private FileCleanupService _cleanupService = new();
        private TabControl tabControl;
        private FileManagerUserControl fileManagerControl;

        public MainForm()
        {
            InitializeComponent();
            
            // Force reload icon (important after icon update)
            ResourceHelper.SetWindowIcon(this, forceReload: true);
            
            // Set up English culture support
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            
            this.Load += MainForm_Load;
            this.Shown += MainForm_Shown;
            
            SetupTabControl();
            
            // Initialize UI state
            UpdateStatus("Initializing...");
            UpdateFileCount();
            UpdateAuthenticationStatus();
            
            // Start cleanup timer
            timer1.Start();
        }

        private void SetupTabControl()
        {
            // Create TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F),
                ImageList = new ImageList()
            };

            // Add tab pages based on authentication
            AddHomeTab();
            if (HDParkingConst.IsAdminAuthenticated)
            {
                AddWebSocketTab();
            }
            AddFileManagerTab();

            // Add TabControl to the main panel
            panel2.Controls.Clear();
            panel2.Controls.Add(tabControl);
        }

        private void UpdateAuthenticationStatus()
        {
            if (HDParkingConst.IsAdminAuthenticated)
            {
                lblAuthStatus.Text = "| 🔓 Admin";
                lblAuthStatus.ForeColor = Color.Green;
                
                // Show admin menu items
                openFolderToolStripMenuItem.Visible = true;
                cleanupNowToolStripMenuItem.Visible = true;
                restartServerToolStripMenuItem.Visible = true;
                webSocketTabToolStripMenuItem.Visible = true;
                
                // Enable logout
                logoutToolStripMenuItem.Enabled = true;
                authenticationToolStripMenuItem.Enabled = false;
            }
            else
            {
                lblAuthStatus.Text = "| 🔒 Guest";
                lblAuthStatus.ForeColor = Color.Red;
                
                // Hide admin menu items
                openFolderToolStripMenuItem.Visible = false;
                cleanupNowToolStripMenuItem.Visible = false;
                restartServerToolStripMenuItem.Visible = false;
                webSocketTabToolStripMenuItem.Visible = false;
                
                // Enable authentication
                logoutToolStripMenuItem.Enabled = false;
                authenticationToolStripMenuItem.Enabled = true;
            }
        }

        private void RefreshTabsBasedOnAuth()
        {
            // Clear existing tabs
            tabControl.TabPages.Clear();
            
            // Add tabs based on authentication
            AddHomeTab();
            if (HDParkingConst.IsAdminAuthenticated)
            {
                AddWebSocketTab();
            }
            AddFileManagerTab();
            
            // Refresh the FileManagerUserControl authentication status
            if (fileManagerControl != null)
            {
                fileManagerControl.RefreshAuthenticationStatus();
            }
        }

        private void AddHomeTab()
        {
            var homeTab = new TabPage("🏠 Trang chính")
            {
                UseVisualStyleBackColor = true,
                Padding = new Padding(20)
            };

            // Create home content panel
            var homePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            if (!HDParkingConst.IsAdminAuthenticated)
            {
                // Show background image for guest mode
                AddBackgroundImageToPanel(homePanel);
            }
            else
            {
                // Show admin controls
                AddAdminControlsToPanel(homePanel);
            }

            homeTab.Controls.Add(homePanel);
            tabControl.TabPages.Add(homeTab);
        }

        private void AddBackgroundImageToPanel(Panel homePanel)
        {
            try
            {
                Image backgroundImage = null;

                // First try to load from embedded resources (for published single file)
                backgroundImage = LoadBackgroundFromEmbeddedResources();

                // If not found in embedded resources, try file system paths
                if (backgroundImage == null)
                {
                    string[] possiblePaths = {
                        Path.Combine(Application.StartupPath, "Publics", "Images", "background-home-page.png"),
                        Path.Combine(Application.StartupPath, "Images", "background-home-page.png"),
                        Path.Combine(Application.StartupPath, "background-home-page.png"),
                        Path.Combine(Directory.GetCurrentDirectory(), "Publics", "Images", "background-home-page.png")
                    };

                    string backgroundPath = null;
                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                        {
                            backgroundPath = path;
                            break;
                        }
                    }

                    if (backgroundPath != null)
                    {
                        backgroundImage = Image.FromFile(backgroundPath);
                    }
                }

                // If we have a background image (from either source), display it
                if (backgroundImage != null)
                {
                    var pictureBox = new PictureBox
                    {
                        Dock = DockStyle.Fill,
                        Image = backgroundImage,
                        SizeMode = PictureBoxSizeMode.Zoom, // Better for background images
                        BackColor = Color.FromArgb(245, 245, 250)
                    };
                    homePanel.Controls.Add(pictureBox);
                }
                else
                {
                    // Create and save placeholder if no background found
                    string defaultPath = Path.Combine(Application.StartupPath, "Publics", "Images", "background-home-page.png");
                    Directory.CreateDirectory(Path.GetDirectoryName(defaultPath));
                    CreatePlaceholderBackground(defaultPath);
                    
                    // Try to load the created placeholder
                    if (File.Exists(defaultPath))
                    {
                        backgroundImage = Image.FromFile(defaultPath);
                        var pictureBox = new PictureBox
                        {
                            Dock = DockStyle.Fill,
                            Image = backgroundImage,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            BackColor = Color.FromArgb(245, 245, 250)
                        };
                        homePanel.Controls.Add(pictureBox);
                    }
                    else
                    {
                        // Ultimate fallback: Just background color
                        homePanel.BackColor = Color.FromArgb(245, 245, 250);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Error loading background image: {ex.Message}");
                
                // Fallback: Just background color
                homePanel.BackColor = Color.FromArgb(245, 245, 250);
            }
        }

        private Image LoadBackgroundFromEmbeddedResources()
        {
            try
            {
                // Get the current assembly
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                
                // Try different possible resource names
                string[] possibleResourceNames = {
                    "HungDuyParkingBridge.Publics.Images.background-home-page.png",
                    "HungDuyParkingBridge.Images.background-home-page.png", 
                    "HungDuyParkingBridge.background-home-page.png"
                };

                foreach (string resourceName in possibleResourceNames)
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Successfully loaded background from embedded resource: {resourceName}");
                            return Image.FromStream(stream);
                        }
                    }
                }

                // Debug: List all available embedded resources
                System.Diagnostics.Debug.WriteLine("Available embedded resources:");
                foreach (string name in assembly.GetManifestResourceNames())
                {
                    if (name.Contains("background") || name.Contains("png") || name.Contains("image"))
                    {
                        System.Diagnostics.Debug.WriteLine($" - {name}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading background from embedded resources: {ex.Message}");
            }

            return null;
        }

        private void CreatePlaceholderBackground(string backgroundPath)
        {
            try
            {
                // Create a professional-looking 1200x800 background image
                using (var bitmap = new Bitmap(1200, 800))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    // Enable high-quality rendering
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    // Create a beautiful gradient background
                    using (var brush = new LinearGradientBrush(
                        new Rectangle(0, 0, 1200, 800),
                        Color.FromArgb(240, 248, 255), // Light blue
                        Color.FromArgb(230, 240, 250), // Slightly darker blue
                        LinearGradientMode.Vertical))
                    {
                        graphics.FillRectangle(brush, 0, 0, 1200, 800);
                    }

                    // Add subtle pattern or texture
                    using (var overlayBrush = new LinearGradientBrush(
                        new Rectangle(0, 0, 1200, 400),
                        Color.FromArgb(20, 255, 255, 255), // Very light overlay
                        Color.FromArgb(5, 255, 255, 255),
                        LinearGradientMode.Horizontal))
                    {
                        graphics.FillRectangle(overlayBrush, 0, 0, 1200, 400);
                    }

                    // Draw company logo/title area
                    using (var titleBrush = new SolidBrush(Color.FromArgb(64, 64, 64)))
                    using (var titleFont = new Font("Segoe UI", 36, FontStyle.Bold))
                    using (var subtitleFont = new Font("Segoe UI", 18, FontStyle.Regular))
                    {
                        var titleText = HDParkingConst.nameSoftware;
                        var subtitleText = "";
                        
                        var titleSize = graphics.MeasureString(titleText, titleFont);
                        var subtitleSize = graphics.MeasureString(subtitleText, subtitleFont);
                        
                        // Center the title
                        var titleX = (1200 - titleSize.Width) / 2;
                        var titleY = 280;
                        var subtitleX = (1200 - subtitleSize.Width) / 2;
                        var subtitleY = titleY + titleSize.Height + 10;
                        
                        graphics.DrawString(titleText, titleFont, titleBrush, titleX, titleY);
                        graphics.DrawString(subtitleText, subtitleFont, titleBrush, subtitleX, subtitleY);
                    }

                    // Draw elegant lock icon
                    using (var lockBrush = new SolidBrush(Color.FromArgb(150, 64, 64, 64)))
                    using (var lockFont = new Font("Segoe UI", 72))
                    {
                        var lockText = "🔒";
                        var lockSize = graphics.MeasureString(lockText, lockFont);
                        var lockX = (1200 - lockSize.Width) / 2;
                        var lockY = 450;
                        
                        graphics.DrawString(lockText, lockFont, lockBrush, lockX, lockY);
                    }

                    // Add decorative elements
                    using (var decorBrush = new SolidBrush(Color.FromArgb(30, 64, 64, 64)))
                    {
                        // Draw some subtle decorative circles
                        graphics.FillEllipse(decorBrush, 100, 100, 80, 80);
                        graphics.FillEllipse(decorBrush, 1020, 620, 60, 60);
                        graphics.FillEllipse(decorBrush, 50, 650, 40, 40);
                    }

                    bitmap.Save(backgroundPath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating placeholder background: {ex.Message}");
            }
        }

        private void AddGuestWelcomeMessage(Panel homePanel)
        {
            // Just set background color, no welcome text
            homePanel.BackColor = Color.FromArgb(245, 245, 250);
        }

        private void AddAdminControlsToPanel(Panel homePanel)
        {
            var currentY = 20;

            // Authentication Status Panel
            var authPanel = new GroupBox
            {
                Text = "🔐 Authentication Status",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(20, currentY),
                Size = new Size(700, 60),
                BackColor = Color.FromArgb(240, 255, 240)
            };

            var lblAuthInfo = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 25),
                Size = new Size(670, 25),
                Text = "✅ Admin access granted - All features available",
                ForeColor = Color.DarkGreen
            };

            authPanel.Controls.Add(lblAuthInfo);
            homePanel.Controls.Add(authPanel);
            currentY += 80;

            // Server URLs
            var lblServerUrl = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, currentY),
                Text = "HTTP Server:"
            };

            var txtServerUrl = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, currentY - 3),
                ReadOnly = true,
                Size = new Size(300, 25),
                Text = HDParkingConst.portHttp
            };

            currentY += 40;

            var lblWebSocketUrl = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, currentY),
                Text = "WebSocket Server:"
            };

            var txtWebSocketUrl = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, currentY - 3),
                ReadOnly = true,
                Size = new Size(300, 25),
                Text = HDParkingConst.portWebSocket,
            };

            currentY += 40;

            // Files saved location
            var lblFilesSaved = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, currentY),
                Text = "Storage folder:"
            };

            var txtFilesSaved = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, currentY - 3),
                ReadOnly = true,
                Size = new Size(500, 25),
                Text = HDParkingConst.pathSaveFile,
            };

            currentY += 50;

            // Auto delete settings
            var lblAutoDelete = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, currentY),
                Text = "Auto delete after:"
            };

            var numDeleteAfterDays = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, currentY - 3),
                Maximum = 365,
                Minimum = 1,
                Size = new Size(80, 25),
                Value = 7
            };

            var chkAutoDelete = new CheckBox
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(240, currentY),
                Text = "days (On/Off)",
                UseVisualStyleBackColor = true
            };

            chkAutoDelete.CheckedChanged += (s, e) => chkAutoDelete_CheckedChanged(s, e, chkAutoDelete, numDeleteAfterDays);

            currentY += 50;

            // Statistics panel
            var statsPanel = new GroupBox
            {
                Text = "📊 Statistics",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(20, currentY),
                Size = new Size(400, 120)
            };

            var lblTotalFiles = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 25),
                Text = "Total files: 0"
            };

            var lblTotalSize = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 50),
                Text = "Storage used: 0 B"
            };

            var lblLastUpdate = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 75),
                Text = "Last update: None"
            };

            statsPanel.Controls.AddRange(new Control[] { lblTotalFiles, lblTotalSize, lblLastUpdate });

            // Quick actions panel
            var actionsPanel = new GroupBox
            {
                Text = "⚡ Quick Actions",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(450, currentY),
                Size = new Size(300, 120)
            };

            var btnOpenSaveFolder = new Button
            {
                BackColor = Color.FromArgb(0, 123, 255),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(15, 25),
                Size = new Size(120, 35),
                Text = "📁 Open Folder",
                UseVisualStyleBackColor = false
            };

            var btnCleanupNow = new Button
            {
                BackColor = Color.FromArgb(220, 53, 69),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(150, 25),
                Size = new Size(120, 35),
                Text = "🗑️ Cleanup Now",
                UseVisualStyleBackColor = false
            };

            var btnRestartServer = new Button
            {
                BackColor = Color.FromArgb(255, 193, 7),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Black,
                Location = new Point(15, 70),
                Size = new Size(120, 35),
                Text = "🔄 Restart Server",
                UseVisualStyleBackColor = false
            };

            btnOpenSaveFolder.Click += (s, e) => OpenSaveFolder();
            btnCleanupNow.Click += (s, e) => CleanupNow();
            btnRestartServer.Click += (s, e) => RestartServer();

            actionsPanel.Controls.AddRange(new Control[] { btnOpenSaveFolder, btnCleanupNow, btnRestartServer });

            // Add all admin controls to home panel
            homePanel.Controls.AddRange(new Control[] {
                lblServerUrl, txtServerUrl,
                lblWebSocketUrl, txtWebSocketUrl,
                lblFilesSaved, txtFilesSaved,
                lblAutoDelete, numDeleteAfterDays, chkAutoDelete,
                statsPanel, actionsPanel
            });
        }

        private void AddWebSocketTab()
        {
            var webSocketTab = new TabPage("🔌 WebSocket")
            {
                UseVisualStyleBackColor = true,
                Padding = new Padding(20)
            };

            var webSocketPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // WebSocket status
            var lblWebSocketStatus = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(20, 20),
                Text = "🔌 WebSocket Real-time Communication"
            };

            var lblConnectionInfo = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 60),
                Size = new Size(600, 100),
                Text = "WebSocket Server: ws://localhost:5001/ws\n" +
                       "Status: Running in background\n" +
                       "Real-time notifications for file uploads, downloads, and server events\n" +
                       "⚠️ WebSocket continues running even when window is closed"
            };

            // Test panel
            var testPanel = new GroupBox
            {
                Text = "🧪 Test WebSocket",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(20, 180),
                Size = new Size(400, 150)
            };

            var txtTestMessage = new TextBox
            {
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 30),
                Size = new Size(250, 25),
                Text = "Test message from server"
            };

            var btnSendTestMessage = new Button
            {
                BackColor = Color.FromArgb(40, 167, 69),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(280, 28),
                Size = new Size(100, 30),
                Text = "📡 Send Test",
                UseVisualStyleBackColor = false
            };

            btnSendTestMessage.Click += async (s, e) => await SendTestMessage(txtTestMessage.Text);

            var btnSendTestNotification = new Button
            {
                BackColor = Color.FromArgb(0, 123, 255),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(15, 70),
                Size = new Size(150, 35),
                Text = "📁 Test File Upload",
                UseVisualStyleBackColor = false
            };

            btnSendTestNotification.Click += async (s, e) => await SendTestFileNotification();

            var btnOpenTestPage = new Button
            {
                BackColor = Color.FromArgb(108, 117, 125),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(180, 70),
                Size = new Size(150, 35),
                Text = "🌐 Open Test Page",
                UseVisualStyleBackColor = false
            };

            btnOpenTestPage.Click += (s, e) => OpenWebSocketTestPage();

            testPanel.Controls.AddRange(new Control[] { txtTestMessage, btnSendTestMessage, btnSendTestNotification, btnOpenTestPage });

            // Info panel
            var infoPanel = new GroupBox
            {
                Text = "ℹ️ Background Service Info",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(450, 180),
                Size = new Size(300, 200)
            };

            var lblEvents = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 30),
                Size = new Size(270, 150),
                Text = "✅ WebSocket runs in background\n" +
                       "✅ Continues when window closed\n" +
                       "✅ Real-time file notifications\n" +
                       "✅ Multi-client support\n\n" +
                       "Available Events:\n" +
                       "• FileNotification\n" +
                       "• ServerStatus\n" +
                       "• UserConnected/Disconnected\n" +
                       "• Broadcast messages\n\n" +
                       "To fully stop: Exit from tray menu"
            };

            infoPanel.Controls.Add(lblEvents);

            webSocketPanel.Controls.AddRange(new Control[] {
                lblWebSocketStatus, lblConnectionInfo, testPanel, infoPanel
            });

            webSocketTab.Controls.Add(webSocketPanel);
            tabControl.TabPages.Add(webSocketTab);
        }

        private void AddFileManagerTab()
        {
            var fileManagerTab = new TabPage("📁 File Manager")
            {
                UseVisualStyleBackColor = true
            };

            // Create a user control wrapper for FileManagerForm content
            fileManagerControl = new FileManagerUserControl();
            fileManagerControl.Dock = DockStyle.Fill;

            fileManagerTab.Controls.Add(fileManagerControl);
            tabControl.TabPages.Add(fileManagerTab);
        }

        // Authentication event handlers
        private void AuthenticationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var authDialog = new PrivateKeyDialog())
            {
                if (authDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UpdateAuthenticationStatus();
                    RefreshTabsBasedOnAuth();
                    MessageBox.Show("🎉 Welcome to Admin Mode!\n\nAll administrative features are now available.", 
                        "Authentication Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void LogoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout from admin mode?\n\nAdministrative features will be hidden.", 
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                HDParkingConst.SetAdminAccess(false);
                UpdateAuthenticationStatus();
                RefreshTabsBasedOnAuth();
                MessageBox.Show("🔒 Logged out successfully.\n\nSwitched to Guest Mode.", 
                    "Logout Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SetupTray()
        {
            // Ensure proper encoding support for English
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            trayMenu = new ContextMenuStrip();
            
            // Use English text
            var openItem = new ToolStripMenuItem("Mở cửa sổ");
            openItem.Click += (s, e) => this.Show();
            trayMenu.Items.Add(openItem);

            var restartItem = new ToolStripMenuItem("Khởi động lại");
            restartItem.Click += async (s, e) =>
            {
                if (!HDParkingConst.IsAdminAuthenticated)
                {
                    trayIcon.ShowBalloonTip(3000, "Access Denied", "Admin authentication required", ToolTipIcon.Warning);
                    return;
                }

                try
                {
                    await _receiver.Stop();
                    await Task.Delay(1000);
                    await _receiver.Start();
                    trayIcon.ShowBalloonTip(3000, HDParkingConst.nameSoftware, "Server restarted successfully", ToolTipIcon.Info);
                }
                catch (Exception ex)
                {
                    trayIcon.ShowBalloonTip(5000, "Error", $"Restart failed: {ex.Message}", ToolTipIcon.Error);
                }
            };
            trayMenu.Items.Add(restartItem);

            var exitItem = new ToolStripMenuItem("Thoát");
            exitItem.Click += async (s, e) =>
            {
                // Properly stop services before exiting
                if (HDParkingConst.IsAdminAuthenticated)
                {
                    await _receiver.Stop();
                }
                trayIcon.Visible = false;  
                trayIcon.Dispose();        
                Application.Exit();
                Environment.Exit(0);
            };
            trayMenu.Items.Add(exitItem);

            // Load the custom icon
            Icon customIcon = GetApplicationIcon();

            trayIcon = new NotifyIcon
            {
                Text = HDParkingConst.nameSoftware,
                Icon = customIcon,
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.DoubleClick += (s, e) => this.Show();
        }

        private Icon GetApplicationIcon()
        {
            return ResourceHelper.GetApplicationIcon();
        }

        private void AddToStartup()
        {
            try
            {
                string appName = "HungDuyParkingFileReceiver_beta";
                string exePath = Application.ExecutablePath;

                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (key != null)
                {
                    key.SetValue(appName, exePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot add to Windows startup:\n" + ex.Message, 
                    "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            SetupTray();
            AddToStartup();
            
            // Only start services if authenticated
            if (HDParkingConst.IsAdminAuthenticated)
            {
                await _receiver.Start();
                UpdateStatus("Running - HTTP and WebSocket servers started");
            }
            else
            {
                UpdateStatus("Guest mode - Services not started");
            }
            
            this.Hide();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // DON'T stop the receiver service - keep it running in background
            // Only hide the form to system tray
            e.Cancel = true;
            this.Hide();
            
            // Show notification that service is still running
            trayIcon.ShowBalloonTip(3000, 
                "Thông báo",
                "Ứng dụng đang chạy nền!", 
                ToolTipIcon.Info);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        // Event handlers
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void chkAutoDelete_CheckedChanged(object sender, EventArgs e, CheckBox chkAutoDelete, NumericUpDown numDeleteAfterDays)
        {
            if (!HDParkingConst.IsAdminAuthenticated) return;

            _cleanupService.IsEnabled = chkAutoDelete.Checked;
            _cleanupService.DeleteAfterDays = (int)numDeleteAfterDays.Value;
            
            if (chkAutoDelete.Checked)
            {
                UpdateStatus($"Auto delete files after {numDeleteAfterDays.Value} days - ON");
            }
            else
            {
                UpdateStatus("Auto delete files - OFF");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update file count every minute
            if (HDParkingConst.IsAdminAuthenticated)
            {
                UpdateFileCount();
                
                // Run cleanup if enabled
                if (_cleanupService.IsEnabled)
                {
                    _cleanupService.CleanupOldFiles();
                }
            }
        }

        private void UpdateStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateStatus(status));
                return;
            }
            
            lblStatus.Text = $"Status: {status}";
        }

        private void UpdateFileCount()
        {
            if (InvokeRequired)
            {
                Invoke(UpdateFileCount);
                return;
            }

            try
            {
                if (!HDParkingConst.IsAdminAuthenticated)
                {
                    lblFileCount.Text = "Files: Access denied";
                    return;
                }

                string savePath =HDParkingConst.pathSaveFile;
                if (Directory.Exists(savePath))
                {
                    int fileCount = Directory.GetFiles(savePath, "*", SearchOption.AllDirectories).Length;
                    lblFileCount.Text = $"Files: {fileCount}";
                }
            }
            catch
            {
                lblFileCount.Text = "Files: 0";
            }
        }

        // WebSocket test methods - only work if authenticated
        private async Task SendTestMessage(string message)
        {
            if (!HDParkingConst.IsAdminAuthenticated)
            {
                MessageBox.Show("❌ Admin authentication required", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await _receiver.BroadcastMessage($"Test: {message}");
                MessageBox.Show("Test message sent via WebSocket!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending test message: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SendTestFileNotification()
        {
            if (!HDParkingConst.IsAdminAuthenticated)
            {
                MessageBox.Show("❌ Admin authentication required", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string testFileName = $"test-file-{DateTime.Now:yyyyMMdd-HHmmss}.txt";
                await _receiver.NotifyFileUploaded(testFileName, 1024);
                MessageBox.Show($"Test file notification sent: {testFileName}", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending test notification: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenWebSocketTestPage()
        {
            if (!HDParkingConst.IsAdminAuthenticated)
            {
                MessageBox.Show("❌ Admin authentication required", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string testPageUrl = "http://localhost:5001/";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = testPageUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open test page: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Quick action methods - require authentication
        private void OpenSaveFolder()
        {
            if (!HDParkingConst.IsAdminAuthenticated)
            {
                MessageBox.Show("❌ Admin authentication required", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string savePath = HDParkingConst.pathSaveFile;
                if (Directory.Exists(savePath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", savePath);
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

        private void CleanupNow()
        {
            if (!HDParkingConst.IsAdminAuthenticated)
            {
                MessageBox.Show("❌ Admin authentication required", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var result = MessageBox.Show("Are you sure you want to cleanup old files now?", 
                    "Confirm Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _cleanupService.ForceCleanup();
                    MessageBox.Show("Old files cleanup completed!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateFileCount();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during cleanup: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RestartServer()
        {
            if (!HDParkingConst.IsAdminAuthenticated)
            {
                MessageBox.Show("❌ Admin authentication required", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var result = MessageBox.Show("Are you sure you want to restart the server?", 
                    "Confirm Restart", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await _receiver.Stop();
                    UpdateStatus("Restarting server...");
                    
                    // Small delay before restart
                    await Task.Delay(1000);
                    
                    await _receiver.Start();
                    UpdateStatus("Servers restarted successfully");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restarting server: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Menu event handlers
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // File menu actions
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // View menu actions
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string authInfo = HDParkingConst.IsAdminAuthenticated 
                ? "\n\n🔓 Current Mode: Admin - All features available"
                : "\n\n🔒 Current Mode: Guest - Use Help > Private Key > Authentication for admin access";

            MessageBox.Show("Hung Duy Parking FileReceiver\nVersion: 1.0.2\n\nSupport contact: support@hungduy.com\n\nFeatures:\n- HTTP File Upload/Download\n- WebSocket Real-time Notifications\n- File Management\n- Auto Cleanup\n- Private Key Authentication" + authInfo, 
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}