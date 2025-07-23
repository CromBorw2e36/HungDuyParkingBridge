namespace HungDuyParkingBridge.UI
{
    partial class Form1
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
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            lblFileCount = new ToolStripStatusLabel();
            panel1 = new Panel();
            btnFileManager = new Button();
            btnMinimize = new Button();
            lblTitle = new Label();
            panel2 = new Panel();
            chkAutoDelete = new CheckBox();
            numDeleteAfterDays = new NumericUpDown();
            lblAutoDelete = new Label();
            txtFilesSaved = new TextBox();
            lblFilesSaved = new Label();
            txtServerUrl = new TextBox();
            lblServerUrl = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            statusStrip1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDeleteAfterDays).BeginInit();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus, lblFileCount });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(63, 17);
            lblStatus.Text = "Tráº¡ng thÃ¡i:";
            // 
            // lblFileCount
            // 
            lblFileCount.Name = "lblFileCount";
            lblFileCount.Size = new Size(51, 17);
            lblFileCount.Text = "Sá»‘ file: 0";
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(64, 64, 64);
            panel1.Controls.Add(btnFileManager);
            panel1.Controls.Add(btnMinimize);
            panel1.Controls.Add(lblTitle);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 60);
            panel1.TabIndex = 1;
            // 
            // btnFileManager
            // 
            btnFileManager.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFileManager.BackColor = Color.FromArgb(128, 128, 255);
            btnFileManager.FlatStyle = FlatStyle.Flat;
            btnFileManager.ForeColor = Color.White;
            btnFileManager.Location = new Point(580, 15);
            btnFileManager.Name = "btnFileManager";
            btnFileManager.Size = new Size(100, 30);
            btnFileManager.TabIndex = 2;
            btnFileManager.Text = "Quáº£n lÃ½ File";
            btnFileManager.UseVisualStyleBackColor = false;
            btnFileManager.Click += btnFileManager_Click;
            // 
            // btnMinimize
            // 
            btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimize.BackColor = Color.FromArgb(255, 128, 0);
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.ForeColor = Color.White;
            btnMinimize.Location = new Point(690, 15);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(100, 30);
            btnMinimize.TabIndex = 1;
            btnMinimize.Text = "áº¨n xuá»‘ng Tray";
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
            lblTitle.Text = "HÃ¹ng Duy Parking FileReceiver";
            // 
            // panel2
            // 
            panel2.Controls.Add(chkAutoDelete);
            panel2.Controls.Add(numDeleteAfterDays);
            panel2.Controls.Add(lblAutoDelete);
            panel2.Controls.Add(txtFilesSaved);
            panel2.Controls.Add(lblFilesSaved);
            panel2.Controls.Add(txtServerUrl);
            panel2.Controls.Add(lblServerUrl);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 60);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(20);
            panel2.Size = new Size(800, 368);
            panel2.TabIndex = 2;
            // 
            // chkAutoDelete
            // 
            chkAutoDelete.AutoSize = true;
            chkAutoDelete.Font = new Font("Segoe UI", 10F);
            chkAutoDelete.Location = new Point(240, 112);
            chkAutoDelete.Name = "chkAutoDelete";
            chkAutoDelete.Size = new Size(114, 23);
            chkAutoDelete.TabIndex = 6;
            chkAutoDelete.Text = "ngÃ y (Báº­t/Táº¯t)";
            chkAutoDelete.UseVisualStyleBackColor = true;
            chkAutoDelete.CheckedChanged += chkAutoDelete_CheckedChanged;
            // 
            // numDeleteAfterDays
            // 
            numDeleteAfterDays.Font = new Font("Segoe UI", 10F);
            numDeleteAfterDays.Location = new Point(150, 110);
            numDeleteAfterDays.Maximum = new decimal(new int[] { 365, 0, 0, 0 });
            numDeleteAfterDays.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDeleteAfterDays.Name = "numDeleteAfterDays";
            numDeleteAfterDays.Size = new Size(80, 25);
            numDeleteAfterDays.TabIndex = 5;
            numDeleteAfterDays.Value = new decimal(new int[] { 7, 0, 0, 0 });
            // 
            // lblAutoDelete
            // 
            lblAutoDelete.AutoSize = true;
            lblAutoDelete.Font = new Font("Segoe UI", 10F);
            lblAutoDelete.Location = new Point(23, 113);
            lblAutoDelete.Name = "lblAutoDelete";
            lblAutoDelete.Size = new Size(113, 19);
            lblAutoDelete.TabIndex = 4;
            lblAutoDelete.Text = "Tá»± Ä‘á»™ng xÃ³a sau:";
            // 
            // txtFilesSaved
            // 
            txtFilesSaved.Font = new Font("Segoe UI", 10F);
            txtFilesSaved.Location = new Point(150, 70);
            txtFilesSaved.Name = "txtFilesSaved";
            txtFilesSaved.ReadOnly = true;
            txtFilesSaved.Size = new Size(500, 25);
            txtFilesSaved.TabIndex = 3;
            txtFilesSaved.Text = "C:\\HungDuyParkingReceivedFiles";
            // 
            // lblFilesSaved
            // 
            lblFilesSaved.AutoSize = true;
            lblFilesSaved.Font = new Font("Segoe UI", 10F);
            lblFilesSaved.Location = new Point(23, 73);
            lblFilesSaved.Name = "lblFilesSaved";
            lblFilesSaved.Size = new Size(110, 19);
            lblFilesSaved.TabIndex = 2;
            lblFilesSaved.Text = "ThÆ° má»¥c lÆ°u trá»¯:";
            // 
            // txtServerUrl
            // 
            txtServerUrl.Font = new Font("Segoe UI", 10F);
            txtServerUrl.Location = new Point(150, 30);
            txtServerUrl.Name = "txtServerUrl";
            txtServerUrl.ReadOnly = true;
            txtServerUrl.Size = new Size(300, 25);
            txtServerUrl.TabIndex = 1;
            txtServerUrl.Text = "http://localhost:5000";
            // 
            // lblServerUrl
            // 
            lblServerUrl.AutoSize = true;
            lblServerUrl.Font = new Font("Segoe UI", 10F);
            lblServerUrl.Location = new Point(23, 33);
            lblServerUrl.Name = "lblServerUrl";
            lblServerUrl.Size = new Size(79, 19);
            lblServerUrl.TabIndex = 0;
            lblServerUrl.Text = "Server URL:";
            // 
            // timer1
            // 
            timer1.Interval = 60000;
            timer1.Tick += timer1_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(statusStrip1);
            MinimumSize = new Size(600, 400);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HÃ¹ng Duy Parking FileReceiver";
            FormClosing += Form1_FormClosing;
            Resize += Form1_Resize;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numDeleteAfterDays).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblFileCount;
        private Panel panel1;
        private Button btnFileManager;
        private Button btnMinimize;
        private Label lblTitle;
        private Panel panel2;
        private TextBox txtServerUrl;
        private Label lblServerUrl;
        private Label lblFilesSaved;
        private TextBox txtFilesSaved;
        private Label lblAutoDelete;
        private NumericUpDown numDeleteAfterDays;
        private CheckBox chkAutoDelete;
        private System.Windows.Forms.Timer timer1;
    }
}