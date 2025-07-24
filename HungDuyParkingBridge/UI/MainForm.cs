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

            // Server URL
            var lblServerUrl = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 30),
                Text = "Server URL:"
            };

            var txtServerUrl = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, 27),
                ReadOnly = true,
                Size = new Size(300, 25),
                Text = "http://localhost:5000"
            };

            // Files saved location
            var lblFilesSaved = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 70),
                Text = "Storage folder:"
            };

            var txtFilesSaved = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, 67),
                ReadOnly = true,
                Size = new Size(500, 25),
                Text = "C:\\HungDuyParkingReceivedFiles"
            };

            // Auto delete settings
            var lblAutoDelete = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 110),
                Text = "Auto delete after:"
            };

            var numDeleteAfterDays = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(150, 107),
                Maximum = 365,
                Minimum = 1,
                Size = new Size(80, 25),
                Value = 7
            };

            var chkAutoDelete = new CheckBox
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(240, 109),
                Text = "days (On/Off)",
                UseVisualStyleBackColor = true
            };

            chkAutoDelete.CheckedChanged += (s, e) => chkAutoDelete_CheckedChanged(s, e, chkAutoDelete, numDeleteAfterDays);

            // Statistics panel
            var statsPanel = new GroupBox
            {
                Text = "📊 Statistics",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(20, 150),
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
                Location = new Point(450, 150),
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
                lblFilesSaved, txtFilesSaved,
                lblAutoDelete, numDeleteAfterDays, chkAutoDelete,
                statsPanel, actionsPanel
            });

            homeTab.Controls.Add(homePanel);
            tabControl.TabPages.Add(homeTab);
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
            restartItem.Click += (s, e) =>
            {
                trayIcon.Visible = false;   
                trayIcon.Dispose();         
                Application.Restart();      
                Application.Exit();         
                Environment.Exit(0);
            };
            trayMenu.Items.Add(restartItem);

            var exitItem = new ToolStripMenuItem("Thoát");
            exitItem.Click += (s, e) =>
            {
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
                Text = "Hung Duy Parking FileReceiver Beta",
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupTray();
            AddToStartup();
            _receiver.Start();
            UpdateStatus("Running - Server started");
            this.Hide();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _receiver.Stop();
            e.Cancel = true;
            this.Hide();
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
                    _receiver.Stop();
                    UpdateStatus("Restarting server...");
                    
                    // Small delay before restart
                    await Task.Delay(1000);
                    
                    _receiver.Start();
                    UpdateStatus("Server restarted successfully");
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
            MessageBox.Show("Hung Duy Parking FileReceiver\nVersion: 1.0\n\nSupport contact: support@hungduy.com", 
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}