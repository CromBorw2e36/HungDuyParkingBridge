using Microsoft.Win32;
using HungDuyParkingBridge.Services;
using HungDuyParkingBridge.Utilities;
using System.Text;
using System.Globalization;

namespace HungDuyParkingBridge.UI
{
    public partial class Form1 : Form
    {
        NotifyIcon trayIcon;
        ContextMenuStrip trayMenu;
        private FileReceiverService _receiver = new();
        private FileCleanupService _cleanupService = new();

        public Form1()
        {
            InitializeComponent();
            
            // Set window icon
            ResourceHelper.SetWindowIcon(this);
            
            // Set up Vietnamese culture support
            Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
            
            this.Load += Form1_Load;
            this.Shown += Form1_Shown;
            
            // Initialize UI state
            UpdateStatus("Đang khởi tạo...");
            UpdateFileCount();
            
            // Start cleanup timer
            timer1.Start();
        }

        private void SetupTray()
        {
            // Ensure proper encoding support for Vietnamese
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            trayMenu = new ContextMenuStrip();
            
            // Use Vietnamese text with proper encoding
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
                Text = "Hùng Duy Parking FileReceiver Beta",
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
                string appName = "HungDuyParkingFileReceiver_beta"; // Tên tùy ý, dùng để đặt trong Registry
                string exePath = Application.ExecutablePath;

                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (key != null)
                {
                    key.SetValue(appName, exePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thêm vào khởi động cùng Windows:\n" + ex.Message, 
                    "Lỗi khởi động", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetupTray();
            AddToStartup();
            _receiver.Start();
            UpdateStatus("Đang chạy - Server đã khởi động");
            this.Hide();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _receiver.Stop();
            e.Cancel = true;
            this.Hide();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide(); // Ẩn khi form đã hiển thị xong
        }

        // New UI event handlers
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnFileManager_Click(object sender, EventArgs e)
        {
            var fileManagerForm = new FileManagerForm();
            fileManagerForm.Show();
        }

        private void chkAutoDelete_CheckedChanged(object sender, EventArgs e)
        {
            _cleanupService.IsEnabled = chkAutoDelete.Checked;
            _cleanupService.DeleteAfterDays = (int)numDeleteAfterDays.Value;
            
            if (chkAutoDelete.Checked)
            {
                UpdateStatus($"Tự động xóa file sau {numDeleteAfterDays.Value} ngày - BẬT");
            }
            else
            {
                UpdateStatus("Tự động xóa file - TẮT");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update file count every minute
            UpdateFileCount();
            
            // Run cleanup if enabled
            if (chkAutoDelete.Checked)
            {
                _cleanupService.DeleteAfterDays = (int)numDeleteAfterDays.Value;
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
            
            lblStatus.Text = $"Trạng thái: {status}";
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
                    lblFileCount.Text = $"Số file: {fileCount}";
                }
            }
            catch
            {
                lblFileCount.Text = "Số file: 0";
            }
        }
    }
}