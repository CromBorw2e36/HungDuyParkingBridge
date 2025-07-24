using Microsoft.Win32;
using HungDuyParkingBridge.Services;
using HungDuyParkingBridge.Utilities;
using System.Text;
using System.Globalization;

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

            // Add tab pages
            AddHomeTab();
            AddWebSocketTab();
            AddFileManagerTab();

            // Add TabControl to the main panel
            panel2.Controls.Clear();
            panel2.Controls.Add(tabControl);
        }

        private void AddHomeTab()
        {
            var homeTab = new TabPage("🏠 Home")
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

            // Server URLs
            var lblServerUrl = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 30),
                Text = "HTTP Server:"
            };

            var txtServerUrl = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, 27),
                ReadOnly = true,
                Size = new Size(300, 25),
                Text = "http://localhost:5000"
            };

            var lblWebSocketUrl = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 60),
                Text = "WebSocket Server:"
            };

            var txtWebSocketUrl = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, 57),
                ReadOnly = true,
                Size = new Size(300, 25),
                Text = "ws://localhost:5001/ws"
            };

            // Files saved location
            var lblFilesSaved = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 100),
                Text = "Storage folder:"
            };

            var txtFilesSaved = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, 97),
                ReadOnly = true,
                Size = new Size(500, 25),
                Text = "C:\\HungDuyParkingReceivedFiles"
            };

            // Auto delete settings
            var lblAutoDelete = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 140),
                Text = "Auto delete after:"
            };

            var numDeleteAfterDays = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, 137),
                Maximum = 365,
                Minimum = 1,
                Size = new Size(80, 25),
                Value = 7
            };

            var chkAutoDelete = new CheckBox
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(240, 139),
                Text = "days (On/Off)",
                UseVisualStyleBackColor = true
            };

            chkAutoDelete.CheckedChanged += (s, e) => chkAutoDelete_CheckedChanged(s, e, chkAutoDelete, numDeleteAfterDays);

            // Statistics panel
            var statsPanel = new GroupBox
            {
                Text = "📊 Statistics",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(20, 180),
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
                Location = new Point(450, 180),
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

            // Add all controls to home panel
            homePanel.Controls.AddRange(new Control[] {
                lblServerUrl, txtServerUrl,
                lblWebSocketUrl, txtWebSocketUrl,
                lblFilesSaved, txtFilesSaved,
                lblAutoDelete, numDeleteAfterDays, chkAutoDelete,
                statsPanel, actionsPanel
            });

            homeTab.Controls.Add(homePanel);
            tabControl.TabPages.Add(homeTab);
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
                try
                {
                    await _receiver.Stop();
                    await Task.Delay(1000);
                    await _receiver.Start();
                    trayIcon.ShowBalloonTip(3000, "HungDuy Parking", "Server restarted successfully", ToolTipIcon.Info);
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
                await _receiver.Stop();
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
                Text = "Hung Duy Parking FileReceiver Beta - WebSocket Running",
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
            await _receiver.Start();
            UpdateStatus("Running - HTTP and WebSocket servers started");
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
                "HungDuy Parking Bridge", 
                "Application minimized to tray. WebSocket service continues running in background.", 
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
            UpdateFileCount();
            
            // Run cleanup if enabled
            if (_cleanupService.IsEnabled)
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
                string savePath = @"C:\HungDuyParkingReceivedFiles";
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

        // WebSocket test methods
        private async Task SendTestMessage(string message)
        {
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

        // Quick action methods
        private void OpenSaveFolder()
        {
            try
            {
                string savePath = @"C:\HungDuyParkingReceivedFiles";
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
            MessageBox.Show("Hung Duy Parking FileReceiver\nVersion: 1.0.2\n\nSupport contact: support@hungduy.com\n\nFeatures:\n- HTTP File Upload/Download\n- WebSocket Real-time Notifications\n- File Management\n- Auto Cleanup\n\n⚠️ WebSocket service runs in background even when window is closed", 
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}