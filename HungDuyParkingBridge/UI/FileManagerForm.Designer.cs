namespace HungDuyParkingBridge.UI
{
    partial class FileManagerForm
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
            this.btnCompare = new Button();
            this.btnPreview = new Button();
            this.btnCleanupOld = new Button();
            this.numCleanupDays = new NumericUpDown();
            this.label1 = new Label();
            this.btnDeleteSelected = new Button();
            this.btnOpenFolder = new Button();
            this.btnRefresh = new Button();
            this.panel3 = new Panel();
            this.listViewFiles = new ListView();
            this.columnFileName = new ColumnHeader();
            this.columnSize = new ColumnHeader();
            this.columnCreated = new ColumnHeader();
            this.columnModified = new ColumnHeader();
            this.columnPath = new ColumnHeader();
            this.panel4 = new Panel();
            this.lblTotalSize = new Label();
            this.lblFileCount = new Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCleanupDays)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = Color.FromArgb(64, 64, 64);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(1500, 60);
            this.panel1.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(194, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Quản lý File nhận được";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCompare);
            this.panel2.Controls.Add(this.btnPreview);
            this.panel2.Controls.Add(this.btnCleanupOld);
            this.panel2.Controls.Add(this.numCleanupDays);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnDeleteSelected);
            this.panel2.Controls.Add(this.btnOpenFolder);
            this.panel2.Controls.Add(this.btnRefresh);
            this.panel2.Dock = DockStyle.Top;
            this.panel2.Location = new Point(0, 60);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new Padding(10);
            this.panel2.Size = new Size(1500, 60);
            this.panel2.TabIndex = 1;
            // 
            // btnCompare
            // 
            this.btnCompare.BackColor = Color.FromArgb(255, 20, 147);
            this.btnCompare.FlatStyle = FlatStyle.Flat;
            this.btnCompare.ForeColor = Color.White;
            this.btnCompare.Location = new Point(440, 15);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new Size(100, 30);
            this.btnCompare.TabIndex = 7;
            this.btnCompare.Text = "So sánh";
            this.btnCompare.UseVisualStyleBackColor = false;
            this.btnCompare.Click += new EventHandler(this.btnCompare_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = Color.FromArgb(255, 165, 0);
            this.btnPreview.FlatStyle = FlatStyle.Flat;
            this.btnPreview.ForeColor = Color.White;
            this.btnPreview.Location = new Point(330, 15);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new Size(100, 30);
            this.btnPreview.TabIndex = 6;
            this.btnPreview.Text = "Xem trước";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new EventHandler(this.btnPreview_Click);
            // 
            // btnCleanupOld
            // 
            this.btnCleanupOld.BackColor = Color.FromArgb(255, 128, 128);
            this.btnCleanupOld.FlatStyle = FlatStyle.Flat;
            this.btnCleanupOld.ForeColor = Color.White;
            this.btnCleanupOld.Location = new Point(580, 15);
            this.btnCleanupOld.Name = "btnCleanupOld";
            this.btnCleanupOld.Size = new Size(100, 30);
            this.btnCleanupOld.TabIndex = 5;
            this.btnCleanupOld.Text = "Dọn dẹp cũ";
            this.btnCleanupOld.UseVisualStyleBackColor = false;
            this.btnCleanupOld.Click += new EventHandler(this.btnCleanupOld_Click);
            // 
            // numCleanupDays
            // 
            this.numCleanupDays.Location = new Point(530, 18);
            this.numCleanupDays.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.numCleanupDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCleanupDays.Name = "numCleanupDays";
            this.numCleanupDays.Size = new Size(50, 23);
            this.numCleanupDays.TabIndex = 4;
            this.numCleanupDays.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(460, 21);
            this.label1.Name = "label1";
            this.label1.Size = new Size(64, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Cũ hơn:";
            // 
            // btnDeleteSelected
            // 
            this.btnDeleteSelected.BackColor = Color.FromArgb(255, 128, 0);
            this.btnDeleteSelected.FlatStyle = FlatStyle.Flat;
            this.btnDeleteSelected.ForeColor = Color.White;
            this.btnDeleteSelected.Location = new Point(220, 15);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Size = new Size(100, 30);
            this.btnDeleteSelected.TabIndex = 2;
            this.btnDeleteSelected.Text = "Xóa đã chọn";
            this.btnDeleteSelected.UseVisualStyleBackColor = false;
            this.btnDeleteSelected.Click += new EventHandler(this.btnDeleteSelected_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.BackColor = Color.FromArgb(128, 128, 255);
            this.btnOpenFolder.FlatStyle = FlatStyle.Flat;
            this.btnOpenFolder.ForeColor = Color.White;
            this.btnOpenFolder.Location = new Point(120, 15);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new Size(100, 30);
            this.btnOpenFolder.TabIndex = 1;
            this.btnOpenFolder.Text = "Mở thư mục";
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            this.btnOpenFolder.Click += new EventHandler(this.btnOpenFolder_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = Color.FromArgb(0, 128, 0);
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.Location = new Point(20, 15);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(100, 30);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.listViewFiles);
            this.panel3.Dock = DockStyle.Fill;
            this.panel3.Location = new Point(0, 120);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new Padding(10);
            this.panel3.Size = new Size(1000, 540); // Adjusted for 500px review panel
            this.panel3.TabIndex = 2;
            // 
            // listViewFiles
            // 
            this.listViewFiles.Columns.AddRange(new ColumnHeader[] {
            this.columnFileName,
            this.columnSize,
            this.columnCreated,
            this.columnModified,
            this.columnPath});
            this.listViewFiles.Dock = DockStyle.Fill;
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.GridLines = true;
            this.listViewFiles.Location = new Point(10, 10);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new Size(980, 520);
            this.listViewFiles.TabIndex = 0;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = View.Details;
            this.listViewFiles.Click += new EventHandler(this.listViewFiles_Click);
            this.listViewFiles.DoubleClick += new EventHandler(this.listViewFiles_DoubleClick);
            // 
            // columnFileName
            // 
            this.columnFileName.Text = "Tên file";
            this.columnFileName.Width = 300;
            // 
            // columnSize
            // 
            this.columnSize.Text = "Kích thước";
            this.columnSize.Width = 100;
            // 
            // columnCreated
            // 
            this.columnCreated.Text = "Ngày tạo";
            this.columnCreated.Width = 150;
            // 
            // columnModified
            // 
            this.columnModified.Text = "Ngày sửa";
            this.columnModified.Width = 150;
            // 
            // columnPath
            // 
            this.columnPath.Text = "Đường dẫn";
            this.columnPath.Width = 250;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblTotalSize);
            this.panel4.Controls.Add(this.lblFileCount);
            this.panel4.Dock = DockStyle.Bottom;
            this.panel4.Location = new Point(0, 660);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new Padding(10);
            this.panel4.Size = new Size(1500, 40);
            this.panel4.TabIndex = 3;
            // 
            // lblTotalSize
            // 
            this.lblTotalSize.AutoSize = true;
            this.lblTotalSize.Location = new Point(200, 15);
            this.lblTotalSize.Name = "lblTotalSize";
            this.lblTotalSize.Size = new Size(87, 15);
            this.lblTotalSize.TabIndex = 1;
            this.lblTotalSize.Text = "Dung lượng: 0 B";
            // 
            // lblFileCount
            // 
            this.lblFileCount.AutoSize = true;
            this.lblFileCount.Location = new Point(20, 15);
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new Size(85, 15);
            this.lblFileCount.TabIndex = 0;
            this.lblFileCount.Text = "Tổng số file: 0";
            // 
            // FileManagerForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1500, 700); // Increased from 1400 to 1500 for larger review panel
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new Size(1300, 600); // Increased minimum size
            this.Name = "FileManagerForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Quản lý File - Hùng Duy Parking";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCleanupDays)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label lblTitle;
        private Panel panel2;
        private Button btnRefresh;
        private Button btnOpenFolder;
        private Button btnDeleteSelected;
        private Button btnPreview;
        private Button btnCompare;
        private Panel panel3;
        private ListView listViewFiles;
        private ColumnHeader columnFileName;
        private ColumnHeader columnSize;
        private ColumnHeader columnCreated;
        private ColumnHeader columnModified;
        private ColumnHeader columnPath;
        private Panel panel4;
        private Label lblFileCount;
        private Label lblTotalSize;
        private Button btnCleanupOld;
        private NumericUpDown numCleanupDays;
        private Label label1;
    }
}