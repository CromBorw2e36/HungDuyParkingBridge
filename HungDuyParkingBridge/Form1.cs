using Microsoft.Win32;

namespace HungDuyParkingBridge
{
    public partial class Form1 : Form
    {

        NotifyIcon trayIcon;
        ContextMenuStrip trayMenu;
        private FileReceiverService _receiver = new();

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.Shown += Form1_Shown;
        }

        private void SetupTray()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Mở", null, (s, e) => this.Show());

            trayMenu.Items.Add("Khởi động lại", null, (s, e) =>
            {
                trayIcon.Visible = false;   
                trayIcon.Dispose();         
                Application.Restart();      
                Application.Exit();         
                Environment.Exit(0);
            });

            trayMenu.Items.Add("Thoát", null, (s, e) =>
            {
                trayIcon.Visible = false;  
                trayIcon.Dispose();        
                Application.Exit();
                Environment.Exit(0);
            });

            trayIcon = new NotifyIcon
            {
                Text = "Hùng Duy Parking FileReceiver Beta",
                Icon = SystemIcons.Application,
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.DoubleClick += (s, e) => this.Show();
        }

        private void AddToStartup()
        {
            try
            {
                string appName = "HungDuyParkingFileReceiver_beta"; // Tên tuỳ ý, dùng để đặt trong Registry
                string exePath = Application.ExecutablePath;

                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (key != null)
                {
                    key.SetValue(appName, exePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thêm vào khởi động cùng Windows:\n" + ex.Message);
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
