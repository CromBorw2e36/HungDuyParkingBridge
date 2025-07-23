namespace HungDuyParkingBridge.UI
{
    partial class FileCompareForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new Panel();
            this.lblTitle = new Label();
            this.panel2 = new Panel();
            this.btnClose = new Button();
            this.btnOpenFile2 = new Button();
            this.btnOpenFile1 = new Button();
            this.panel3 = new Panel();
            this.lblTimeDiff = new Label();
            this.lblSizeDiff = new Label();
            this.lblFile2Info = new Label();
            this.lblFile1Info = new Label();
            this.splitContainer1 = new SplitContainer();
            this.panel4 = new Panel();
            this.lblFile1Title = new Label();
            this.pictureBox1 = new PictureBox();
            this.txtFile1 = new TextBox();
            this.panel5 = new Panel();
            this.lblFile2Title = new Label();
            this.pictureBox2 = new PictureBox();
            this.txtFile2 = new TextBox();
            this.panel6 = new Panel();
            this.lblComparisonResult = new Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = Color.FromArgb(64, 64, 64);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(1200, 50);
            this.panel1.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(114, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "So sánh File";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.btnOpenFile2);
            this.panel2.Controls.Add(this.btnOpenFile1);
            this.panel2.Dock = DockStyle.Bottom;
            this.panel2.Location = new Point(0, 650);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new Padding(10);
            this.panel2.Size = new Size(1200, 50);
            this.panel2.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnClose.BackColor = Color.FromArgb(128, 128, 128);
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.ForeColor = Color.White;
            this.btnClose.Location = new Point(1090, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(100, 30);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Đóng";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            // 
            // btnOpenFile2
            // 
            this.btnOpenFile2.BackColor = Color.FromArgb(0, 128, 128);
            this.btnOpenFile2.FlatStyle = FlatStyle.Flat;
            this.btnOpenFile2.ForeColor = Color.White;
            this.btnOpenFile2.Location = new Point(130, 10);
            this.btnOpenFile2.Name = "btnOpenFile2";
            this.btnOpenFile2.Size = new Size(120, 30);
            this.btnOpenFile2.TabIndex = 1;
            this.btnOpenFile2.Text = "Mở File 2";
            this.btnOpenFile2.UseVisualStyleBackColor = false;
            this.btnOpenFile2.Click += new EventHandler(this.btnOpenFile2_Click);
            // 
            // btnOpenFile1
            // 
            this.btnOpenFile1.BackColor = Color.FromArgb(0, 128, 0);
            this.btnOpenFile1.FlatStyle = FlatStyle.Flat;
            this.btnOpenFile1.ForeColor = Color.White;
            this.btnOpenFile1.Location = new Point(10, 10);
            this.btnOpenFile1.Name = "btnOpenFile1";
            this.btnOpenFile1.Size = new Size(120, 30);
            this.btnOpenFile1.TabIndex = 0;
            this.btnOpenFile1.Text = "Mở File 1";
            this.btnOpenFile1.UseVisualStyleBackColor = false;
            this.btnOpenFile1.Click += new EventHandler(this.btnOpenFile1_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblTimeDiff);
            this.panel3.Controls.Add(this.lblSizeDiff);
            this.panel3.Controls.Add(this.lblFile2Info);
            this.panel3.Controls.Add(this.lblFile1Info);
            this.panel3.Dock = DockStyle.Top;
            this.panel3.Location = new Point(0, 50);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new Padding(10);
            this.panel3.Size = new Size(1200, 80);
            this.panel3.TabIndex = 2;
            // 
            // lblTimeDiff
            // 
            this.lblTimeDiff.AutoSize = true;
            this.lblTimeDiff.Font = new Font("Segoe UI", 9F);
            this.lblTimeDiff.Location = new Point(13, 60);
            this.lblTimeDiff.Name = "lblTimeDiff";
            this.lblTimeDiff.Size = new Size(66, 15);
            this.lblTimeDiff.TabIndex = 3;
            this.lblTimeDiff.Text = "Thời gian:";
            // 
            // lblSizeDiff
            // 
            this.lblSizeDiff.AutoSize = true;
            this.lblSizeDiff.Font = new Font("Segoe UI", 9F);
            this.lblSizeDiff.Location = new Point(13, 40);
            this.lblSizeDiff.Name = "lblSizeDiff";
            this.lblSizeDiff.Size = new Size(71, 15);
            this.lblSizeDiff.TabIndex = 2;
            this.lblSizeDiff.Text = "Chênh lệch:";
            // 
            // lblFile2Info
            // 
            this.lblFile2Info.AutoSize = true;
            this.lblFile2Info.Font = new Font("Segoe UI", 9F);
            this.lblFile2Info.Location = new Point(13, 25);
            this.lblFile2Info.Name = "lblFile2Info";
            this.lblFile2Info.Size = new Size(37, 15);
            this.lblFile2Info.TabIndex = 1;
            this.lblFile2Info.Text = "File 2:";
            // 
            // lblFile1Info
            // 
            this.lblFile1Info.AutoSize = true;
            this.lblFile1Info.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblFile1Info.Location = new Point(13, 10);
            this.lblFile1Info.Name = "lblFile1Info";
            this.lblFile1Info.Size = new Size(36, 15);
            this.lblFile1Info.TabIndex = 0;
            this.lblFile1Info.Text = "File 1:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.Location = new Point(0, 130);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel5);
            this.splitContainer1.Size = new Size(1200, 420);
            this.splitContainer1.SplitterDistance = 600;
            this.splitContainer1.TabIndex = 3;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.pictureBox1);
            this.panel4.Controls.Add(this.txtFile1);
            this.panel4.Controls.Add(this.lblFile1Title);
            this.panel4.Dock = DockStyle.Fill;
            this.panel4.Location = new Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new Padding(5);
            this.panel4.Size = new Size(600, 420);
            this.panel4.TabIndex = 0;
            // 
            // lblFile1Title
            // 
            this.lblFile1Title.BackColor = Color.FromArgb(220, 220, 255);
            this.lblFile1Title.Dock = DockStyle.Top;
            this.lblFile1Title.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblFile1Title.Location = new Point(5, 5);
            this.lblFile1Title.Name = "lblFile1Title";
            this.lblFile1Title.Padding = new Padding(5);
            this.lblFile1Title.Size = new Size(590, 30);
            this.lblFile1Title.TabIndex = 0;
            this.lblFile1Title.Text = "File 1";
            this.lblFile1Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox1.Dock = DockStyle.Fill;
            this.pictureBox1.Location = new Point(5, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(590, 380);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // txtFile1
            // 
            this.txtFile1.BackColor = Color.White;
            this.txtFile1.Dock = DockStyle.Fill;
            this.txtFile1.Font = new Font("Consolas", 9F);
            this.txtFile1.Location = new Point(5, 35);
            this.txtFile1.Multiline = true;
            this.txtFile1.Name = "txtFile1";
            this.txtFile1.ReadOnly = true;
            this.txtFile1.ScrollBars = ScrollBars.Both;
            this.txtFile1.Size = new Size(590, 380);
            this.txtFile1.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.pictureBox2);
            this.panel5.Controls.Add(this.txtFile2);
            this.panel5.Controls.Add(this.lblFile2Title);
            this.panel5.Dock = DockStyle.Fill;
            this.panel5.Location = new Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new Padding(5);
            this.panel5.Size = new Size(596, 420);
            this.panel5.TabIndex = 0;
            // 
            // lblFile2Title
            // 
            this.lblFile2Title.BackColor = Color.FromArgb(255, 220, 220);
            this.lblFile2Title.Dock = DockStyle.Top;
            this.lblFile2Title.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblFile2Title.Location = new Point(5, 5);
            this.lblFile2Title.Name = "lblFile2Title";
            this.lblFile2Title.Padding = new Padding(5);
            this.lblFile2Title.Size = new Size(586, 30);
            this.lblFile2Title.TabIndex = 0;
            this.lblFile2Title.Text = "File 2";
            this.lblFile2Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox2.Dock = DockStyle.Fill;
            this.pictureBox2.Location = new Point(5, 35);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(586, 380);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // txtFile2
            // 
            this.txtFile2.BackColor = Color.White;
            this.txtFile2.Dock = DockStyle.Fill;
            this.txtFile2.Font = new Font("Consolas", 9F);
            this.txtFile2.Location = new Point(5, 35);
            this.txtFile2.Multiline = true;
            this.txtFile2.Name = "txtFile2";
            this.txtFile2.ReadOnly = true;
            this.txtFile2.ScrollBars = ScrollBars.Both;
            this.txtFile2.Size = new Size(586, 380);
            this.txtFile2.TabIndex = 1;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.lblComparisonResult);
            this.panel6.Dock = DockStyle.Bottom;
            this.panel6.Location = new Point(0, 550);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new Padding(10);
            this.panel6.Size = new Size(1200, 100);
            this.panel6.TabIndex = 4;
            // 
            // lblComparisonResult
            // 
            this.lblComparisonResult.BackColor = Color.FromArgb(240, 240, 240);
            this.lblComparisonResult.BorderStyle = BorderStyle.FixedSingle;
            this.lblComparisonResult.Dock = DockStyle.Fill;
            this.lblComparisonResult.Font = new Font("Segoe UI", 9F);
            this.lblComparisonResult.Location = new Point(10, 10);
            this.lblComparisonResult.Name = "lblComparisonResult";
            this.lblComparisonResult.Padding = new Padding(5);
            this.lblComparisonResult.Size = new Size(1180, 80);
            this.lblComparisonResult.TabIndex = 0;
            this.lblComparisonResult.Text = "Kết quả so sánh sẽ hiển thị ở đây...";
            // 
            // FileCompareForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 700);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new Size(800, 600);
            this.Name = "FileCompareForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "So sánh File - Hùng Duy Parking";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label lblTitle;
        private Panel panel2;
        private Button btnClose;
        private Button btnOpenFile2;
        private Button btnOpenFile1;
        private Panel panel3;
        private Label lblTimeDiff;
        private Label lblSizeDiff;
        private Label lblFile2Info;
        private Label lblFile1Info;
        private SplitContainer splitContainer1;
        private Panel panel4;
        private PictureBox pictureBox1;
        private TextBox txtFile1;
        private Label lblFile1Title;
        private Panel panel5;
        private PictureBox pictureBox2;
        private TextBox txtFile2;
        private Label lblFile2Title;
        private Panel panel6;
        private Label lblComparisonResult;
    }
}