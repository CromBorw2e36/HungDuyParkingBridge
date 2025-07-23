namespace HungDuyParkingBridge.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openFolderToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            homeTabToolStripMenuItem = new ToolStripMenuItem();
            fileManagerTabToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            minimizeToTrayToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            cleanupNowToolStripMenuItem = new ToolStripMenuItem();
            restartServerToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            lblFileCount = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            lblLastUpdate = new ToolStripStatusLabel();
            panel1 = new Panel();
            btnMinimize = new Button();
            lblTitle = new Label();
            panel2 = new Panel();
            timer1 = new System.Windows.Forms.Timer(components);
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1200, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFolderToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // openFolderToolStripMenuItem
            // 
            openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            openFolderToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openFolderToolStripMenuItem.Size = new Size(195, 22);
            openFolderToolStripMenuItem.Text = "&Open Folder";
            openFolderToolStripMenuItem.Click += (s, e) => OpenSaveFolder();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(192, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitToolStripMenuItem.Size = new Size(195, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { homeTabToolStripMenuItem, fileManagerTabToolStripMenuItem, toolStripSeparator2, minimizeToTrayToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "&View";
            // 
            // homeTabToolStripMenuItem
            // 
            homeTabToolStripMenuItem.Name = "homeTabToolStripMenuItem";
            homeTabToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D1;
            homeTabToolStripMenuItem.Size = new Size(220, 22);
            homeTabToolStripMenuItem.Text = "🏠 &Home";
            homeTabToolStripMenuItem.Click += (s, e) => tabControl.SelectedIndex = 0;
            // 
            // fileManagerTabToolStripMenuItem
            // 
            fileManagerTabToolStripMenuItem.Name = "fileManagerTabToolStripMenuItem";
            fileManagerTabToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D2;
            fileManagerTabToolStripMenuItem.Size = new Size(220, 22);
            fileManagerTabToolStripMenuItem.Text = "📁 &File Manager";
            fileManagerTabToolStripMenuItem.Click += (s, e) => tabControl.SelectedIndex = 1;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(217, 6);
            // 
            // minimizeToTrayToolStripMenuItem
            // 
            minimizeToTrayToolStripMenuItem.Name = "minimizeToTrayToolStripMenuItem";
            minimizeToTrayToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.M;
            minimizeToTrayToolStripMenuItem.Size = new Size(220, 22);
            minimizeToTrayToolStripMenuItem.Text = "&Minimize to Tray";
            minimizeToTrayToolStripMenuItem.Click += btnMinimize_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cleanupNowToolStripMenuItem, restartServerToolStripMenuItem, toolStripSeparator3, settingsToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "&Tools";
            // 
            // cleanupNowToolStripMenuItem
            // 
            cleanupNowToolStripMenuItem.Name = "cleanupNowToolStripMenuItem";
            cleanupNowToolStripMenuItem.Size = new Size(180, 22);
            cleanupNowToolStripMenuItem.Text = "🗑️ &Cleanup Now";
            cleanupNowToolStripMenuItem.Click += (s, e) => CleanupNow();
            // 
            // restartServerToolStripMenuItem
            // 
            restartServerToolStripMenuItem.Name = "restartServerToolStripMenuItem";
            restartServerToolStripMenuItem.Size = new Size(180, 22);
            restartServerToolStripMenuItem.Text = "🔄 &Restart Server";
            restartServerToolStripMenuItem.Click += (s, e) => RestartServer();
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(177, 6);
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(180, 22);
            settingsToolStripMenuItem.Text = "⚙️ &Settings";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(152, 22);
            aboutToolStripMenuItem.Text = "&About...";
            aboutToolStripMenuItem.Click += helpToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus, toolStripStatusLabel1, lblFileCount, lblLastUpdate });
            statusStrip1.Location = new Point(0, 728);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1200, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(63, 17);
            lblStatus.Text = "Status:";
            lblStatus.Spring = true;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblFileCount
            // 
            lblFileCount.Name = "lblFileCount";
            lblFileCount.Size = new Size(51, 17);
            lblFileCount.Text = "Files: 0";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(10, 17);
            toolStripStatusLabel1.Text = "|";
            // 
            // lblLastUpdate
            // 
            lblLastUpdate.Name = "lblLastUpdate";
            lblLastUpdate.Size = new Size(100, 17);
            lblLastUpdate.Text = "Updated: None";
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(64, 64, 64);
            panel1.Controls.Add(btnMinimize);
            panel1.Controls.Add(lblTitle);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 24);
            panel1.Name = "panel1";
            panel1.Size = new Size(1200, 60);
            panel1.TabIndex = 2;
            // 
            // btnMinimize
            // 
            btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimize.BackColor = Color.FromArgb(255, 128, 0);
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.ForeColor = Color.White;
            btnMinimize.Location = new Point(1090, 15);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(100, 30);
            btnMinimize.TabIndex = 1;
            btnMinimize.Text = "Minimize";
            btnMinimize.UseVisualStyleBackColor = false;
            btnMinimize.Click += btnMinimize_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(288, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Hung Duy Parking FileReceiver";
            // 
            // panel2
            // 
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 84);
            panel2.Name = "panel2";
            panel2.Size = new Size(1200, 644);
            panel2.TabIndex = 3;
            // 
            // timer1
            // 
            timer1.Interval = 60000;
            timer1.Tick += timer1_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 750);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(1000, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Hung Duy Parking FileReceiver";
            FormClosing += MainForm_FormClosing;
            Resize += MainForm_Resize;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openFolderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem homeTabToolStripMenuItem;
        private ToolStripMenuItem fileManagerTabToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem minimizeToTrayToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem cleanupNowToolStripMenuItem;
        private ToolStripMenuItem restartServerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblFileCount;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel lblLastUpdate;
        private Panel panel1;
        private Button btnMinimize;
        private Label lblTitle;
        private Panel panel2;
        private System.Windows.Forms.Timer timer1;
    }
}