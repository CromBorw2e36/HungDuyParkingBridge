using Microsoft.Win32;
using HungDuyParkingBridge.Services;
using System.Text;
using System.Globalization;

namespace HungDuyParkingBridge.UI
{
    public partial class Form1 : Form
    {
        NotifyIcon trayIcon;
        ContextMenuStrip trayMenu;
        private FileReceiverService _receiver = new();

        public Form1()
        {
            InitializeComponent();
            
            // Set up Vietnamese culture support
            Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
            
            this.Load += Form1_Load;
            this.Shown += Form1_Shown;
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
            try
            {
                // Try to load the icon from the application directory
                string iconPath = Path.Combine(Application.StartupPath, "logoTapDoan.ico");
                if (File.Exists(iconPath))
                {
                    return new Icon(iconPath);
                }

                // Try to load from embedded resources
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(name => name.EndsWith("logoTapDoan.ico"));
                
                if (!string.IsNullOrEmpty(resourceName))
                {
                    using var stream = assembly.GetManifestResourceStream(resourceName);
                    if (stream != null)
                    {
                        return new Icon(stream);
                    }
                }

                // Fallback to system icon if custom icon is not found
                return SystemIcons.Application;
            }
            catch (Exception ex)
            {
                // Log error and use default icon
                System.Diagnostics.Debug.WriteLine($"Lỗi tải biểu tượng tùy chỉnh: {ex.Message}");
                return SystemIcons.Application;
            }
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
            this.Hide(); // Ẩn khi form đã hiển thị xong
        }
    }
}