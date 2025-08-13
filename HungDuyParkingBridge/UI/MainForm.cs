using Microsoft.Win32;
using HungDuyParkingBridge.Services;
using HungDuyParkingBridge.Utilities;
using HungDuyParkingBridge.Utils;
using System.Text;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Security.Principal;
using System.Management;

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
                
                // Try the LogicalName first (for single file publish)
                using (var stream = assembly.GetManifestResourceStream("background-home-page.png"))
                {
                    if (stream != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Successfully loaded background from embedded resource: background-home-page.png");
                        return Image.FromStream(stream);
                    }
                }

                // Try different possible resource names as fallback
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
                Text = "✅ Admin access granted - All features available | Servers always running",
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
        private async void AuthenticationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var authDialog = new PrivateKeyDialog())
            {
                if (authDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UpdateAuthenticationStatus();
                    RefreshTabsBasedOnAuth();
                    
                    // Server is already running - just update status message
                    UpdateStatus("Running - HTTP and WebSocket servers available");
                    
                    MessageBox.Show("🎉 Welcome to Admin Mode!\n\nAll administrative features are now available.\n\nNote: HTTP and WebSocket servers are always running.", 
                        "Authentication Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void LogoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout from admin mode?\n\nNote: HTTP and WebSocket servers will continue running.", 
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Only update authentication status - DO NOT stop servers
                HDParkingConst.SetAdminAccess(false);
                UpdateAuthenticationStatus();
                RefreshTabsBasedOnAuth();
                
                // Servers continue running - just update status message
                UpdateStatus("Running - HTTP and WebSocket servers available (Guest mode)");
                
                MessageBox.Show("🔒 Logged out successfully.\n\nSwitched to Guest Mode.\n\nNote: HTTP and WebSocket servers continue running.", 
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
                // Allow restart for anyone since servers are always running
                try
                {
                    await _receiver.Stop();
                    await Task.Delay(1000);
                    await _receiver.Start();
                    
                    var authStatus = HDParkingConst.IsAdminAuthenticated ? "Admin mode" : "Guest mode";
                    UpdateStatus($"Running - HTTP and WebSocket servers restarted ({authStatus})");
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
                // Always stop services before exiting (regardless of auth status)
                try
                {
                    await _receiver.Stop();
                }
                catch (Exception ex)
                {
                    // Log error but continue with exit
                    System.Diagnostics.Debug.WriteLine($"Error stopping services: {ex.Message}");
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
        
        /// <summary>
        /// Checks if this application is set to run at startup via registry entries.
        /// If no registry startup entry exists, creates one in HKCU.
        /// This only runs when launched from shortcuts, not background services.
        /// </summary>
        private void EnsureStartup()
        {
            try
            {
                // Check if this is a startup launch or normal user launch
                if (IsLaunchedFromStartup())
                {
                    // Skip startup registration if launched during Windows startup
                    System.Diagnostics.Debug.WriteLine("Application launched during Windows startup - skipping startup registration");
                    return;
                }
                
                // First check if already in HKLM (set by installer)
                bool inHKLM = false;
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
                    {
                        if (key != null)
                        {
                            string value = key.GetValue("HungDuyParkingBridge") as string;
                            inHKLM = !string.IsNullOrEmpty(value) && value.Contains(Path.GetFileName(Application.ExecutablePath));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue checking HKCU
                    System.Diagnostics.Debug.WriteLine($"Error checking HKLM startup registry: {ex.Message}");
                }
                
                // If not in HKLM, check if already in HKCU
                if (!inHKLM)
                {
                    bool inHKCU = false;
                    try
                    {
                        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
                        {
                            if (key != null)
                            {
                                string value = key.GetValue("HungDuyParkingBridge") as string;
                                inHKCU = !string.IsNullOrEmpty(value) && value.Contains(Path.GetFileName(Application.ExecutablePath));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue
                        System.Diagnostics.Debug.WriteLine($"Error checking HKCU startup registry: {ex.Message}");
                    }
                    
                    // If not in HKLM and not in HKCU, add to HKCU
                    if (!inHKCU)
                    {
                        try
                        {
                            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                            {
                                if (key != null)
                                {
                                    key.SetValue("HungDuyParkingBridge", Application.ExecutablePath);
                                    System.Diagnostics.Debug.WriteLine("Added application to HKCU startup registry");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Failed to open HKCU registry key for writing");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log error but don't crash
                            System.Diagnostics.Debug.WriteLine($"Error adding to HKCU startup registry: {ex.Message}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Application already in HKCU startup registry");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Application already in HKLM startup registry (set by installer)");
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected errors but don't crash the application
                System.Diagnostics.Debug.WriteLine($"Unexpected error in EnsureStartup: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Determines if the application was launched during Windows startup or by a user
        /// </summary>
        private bool IsLaunchedFromStartup()
        {
            try
            {
                // Check for common signs of being launched from startup:
                
                // 1. Process start time is close to system boot time
                TimeSpan runningTime = DateTime.Now - Process.GetCurrentProcess().StartTime;
                bool startedShortlyAfterBoot = runningTime.TotalMinutes < 5;
                
                // 2. Check command line for specific arguments that might indicate startup launch
                string commandLine = Environment.CommandLine.ToLowerInvariant();
                bool hasStartupArgs = commandLine.Contains("/auto") || 
                                      commandLine.Contains("-auto") || 
                                      commandLine.Contains("/background") || 
                                      commandLine.Contains("-background");
                
                // 3. Check if explorer.exe has started recently (indicative of Windows startup)
                bool explorerStartedRecently = false;
                try
                {
                    Process[] explorerProcesses = Process.GetProcessesByName("explorer");
                    if (explorerProcesses.Length > 0)
                    {
                        TimeSpan explorerRunningTime = DateTime.Now - explorerProcesses[0].StartTime;
                        explorerStartedRecently = explorerRunningTime.TotalMinutes < 5;
                    }
                }
                catch
                {
                    // Ignore errors checking explorer process
                }
                
                // Consider it a startup launch if any of these are true
                return startedShortlyAfterBoot || hasStartupArgs || explorerStartedRecently;
            }
            catch (Exception ex)
            {
                // If there's an error, log it and assume it's not a startup launch
                System.Diagnostics.Debug.WriteLine($"Error in IsLaunchedFromStartup: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Determines if the application was launched from a desktop or start menu shortcut
        /// </summary>
        private bool IsLaunchedFromShortcut()
        {
            try
            {
                // Get command line arguments
                string[] args = Environment.GetCommandLineArgs();
                
                // Check for shell integration command line arguments that indicate shortcut launch
                if (args.Length > 1)
                {
                    foreach (string arg in args)
                    {
                        // Common shell integration argument patterns
                        if (arg.Contains("/explorer") || 
                            arg.Contains("-shortcut") || 
                            arg.Contains("/desktop") || 
                            arg.Contains("-startmenu"))
                        {
                            return true;
                        }
                    }
                }
                
                // Check parent process - if explorer.exe launched us directly, likely from a shortcut
                try
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    int parentProcessId = 0;
                    
                    // Try to get parent process ID through WMI
                    using (var searcher = new System.Management.ManagementObjectSearcher(
                        $"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {currentProcess.Id}"))
                    {
                        foreach (var obj in searcher.Get())
                        {
                            parentProcessId = Convert.ToInt32(obj["ParentProcessId"]);
                            break;
                        }
                    }
                    
                    if (parentProcessId > 0)
                    {
                        try
                        {
                            Process parentProcess = Process.GetProcessById(parentProcessId);
                            if (parentProcess.ProcessName.ToLowerInvariant() == "explorer")
                            {
                                return true;
                            }
                        }
                        catch
                        {
                            // Parent process may have exited
                        }
                    }
                }
                catch
                {
                    // If we can't determine parent process, ignore this check
                }
                
                // Additional heuristic: if not running elevated and not a startup launch, 
                // it's more likely to be a shortcut launch
                if (!IsRunningElevated() && !IsLaunchedFromStartup())
                {
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                // If there's an error, log it and assume not launched from shortcut
                System.Diagnostics.Debug.WriteLine($"Error in IsLaunchedFromShortcut: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Checks if the application is running with elevated (administrator) privileges
        /// </summary>
        private bool IsRunningElevated()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                // If we can't determine elevation status, assume not elevated
                return false;
            }
        }

        private void AddToStartup()
        {
            try
            {
                // Don't add to startup registry here if installer is managing startup
                // This application will be started by the installer's registry entry if user chose that option
                
                // Keeping this method but not executing any code to avoid creating duplicate registry entries
                // The InnoSetup installer now handles this task with proper cleanup of old entries
                
                // Original code (commented out):
                // string appName = "HungDuyParkingFileReceiver_beta";
                // string exePath = Application.ExecutablePath;
                // RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                // if (key != null)
                // {
                //     key.SetValue(appName, exePath);
                // }
                
                // Just log that startup entries are managed by the installer
                System.Diagnostics.Debug.WriteLine("Startup entry management is handled by the installer");
            }
            catch (Exception ex)
            {
                // Still report errors if they happen
                MessageBox.Show("Cannot manage Windows startup:\n" + ex.Message, 
                    "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private async void MainForm_Load(object sender, EventArgs e)
        {
            SetupTray();
            
            // Only ensure startup when launched from a shortcut (not during startup or from other processes)
            if (IsLaunchedFromShortcut())
            {
                EnsureStartup();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Not launched from shortcut - skipping startup registry check");
            }
            
            // ALWAYS start HTTP and WebSocket servers regardless of authentication
            // Authentication only controls UI access, not service availability
            try
            {
                // Extract port numbers for checking
                int httpPort = NetworkHelper.ExtractPortFromUri(HDParkingConst.portHttp);
                int wsPort = NetworkHelper.ExtractPortFromUri(HDParkingConst.portWebSocket);
                
                // Default to 5000 and 5001 if extraction fails
                if (httpPort <= 0) httpPort = 5000;
                if (wsPort <= 0) wsPort = 5001;
                
                // Check if any required ports are in use before starting servers
                var portsInUse = NetworkHelper.GetPortsInUse(httpPort, wsPort);
                if (portsInUse.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Cannot start the server because the following ports are already in use:");
                    
                    foreach (var port in portsInUse)
                    {
                        sb.AppendLine($"• Port {port}");
                    }
                    
                    sb.AppendLine("\nThe application will now close.");
                    
                    UpdateStatus($"Error: Required ports {string.Join(", ", portsInUse)} are in use");
                    MessageBox.Show(sb.ToString(), "Port Conflict Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    // Close the application
                    Application.Exit();
                    Environment.Exit(1);
                    return;
                }
                
                await _receiver.Start();
                UpdateStatus("Running - HTTP and WebSocket servers started");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error starting servers: {ex.Message}");
                
                // Show error message for other types of server start failures
                MessageBox.Show($"Failed to start servers: {ex.Message}\n\nThe application will continue in limited mode.", 
                    "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            // Always update file count since servers are always running
            UpdateFileCount();
            
            // Only run cleanup if admin authenticated (cleanup is admin-only feature)
            if (HDParkingConst.IsAdminAuthenticated && _cleanupService.IsEnabled)
            {
                _cleanupService.CleanupOldFiles();
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
                string savePath = HDParkingConst.pathSaveFile;
                if (Directory.Exists(savePath))
                {
                    int fileCount = Directory.GetFiles(savePath, "*", SearchOption.AllDirectories).Length;
                    
                    // Show file count but indicate access level
                    if (HDParkingConst.IsAdminAuthenticated)
                    {
                        lblFileCount.Text = $"Files: {fileCount} (Admin access)";
                    }
                    else
                    {
                        lblFileCount.Text = $"Files: {fileCount} (Guest access)";
                    }
                }
                else
                {
                    lblFileCount.Text = "Files: 0 (Folder not found)";
                }
            }
            catch (Exception ex)
            {
                lblFileCount.Text = $"Files: Error ({ex.Message})";
            }
        }

        // WebSocket test methods - work regardless of authentication since servers are always running
        private async Task SendTestMessage(string message)
        {
            try
            {
                await _receiver.BroadcastMessage($"Test: {message}");
                var authNote = HDParkingConst.IsAdminAuthenticated ? " (Admin mode)" : " (Guest mode)";
                MessageBox.Show($"Test message sent via WebSocket!{authNote}", "Success", 
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
            try
            {
                string testFileName = $"test-file-{DateTime.Now:yyyyMMdd-HHmmss}.txt";
                await _receiver.NotifyFileUploaded(testFileName, 1024);
                var authNote = HDParkingConst.IsAdminAuthenticated ? " (Admin mode)" : " (Guest mode)";
                MessageBox.Show($"Test file notification sent: {testFileName}{authNote}", "Success", 
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
            // Allow server restart regardless of authentication since servers are always running
            try
            {
                var result = MessageBox.Show("Are you sure you want to restart the HTTP and WebSocket servers?\n\nThis will temporarily interrupt service for all users.", 
                    "Confirm Server Restart", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    UpdateStatus("Restarting servers...");
                    await _receiver.Stop();
                    
                    // Small delay before restart
                    await Task.Delay(1000);
                    
                    await _receiver.Start();
                    
                    var authStatus = HDParkingConst.IsAdminAuthenticated ? "Admin mode" : "Guest mode";
                    UpdateStatus($"Running - HTTP and WebSocket servers restarted ({authStatus})");
                    
                    MessageBox.Show("✅ Servers restarted successfully!\n\nHTTP and WebSocket services are now available.", 
                        "Restart Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error restarting servers: {ex.Message}");
                MessageBox.Show($"Error restarting servers: {ex.Message}", "Restart Error", 
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