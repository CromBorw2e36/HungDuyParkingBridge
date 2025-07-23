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
            panel1 = new Panel();
            lblTitle = new Label();
            panel2 = new Panel();
            btnCompare = new Button();
            btnPreview = new Button();
            btnCleanupOld = new Button();
            numCleanupDays = new NumericUpDown();
            label1 = new Label();
            btnDeleteSelected = new Button();
            btnOpenFolder = new Button();
            btnRefresh = new Button();
            panel3 = new Panel();
            listViewFiles = new ListView();
            columnFileName = new ColumnHeader();
            columnSize = new ColumnHeader();
            columnCreated = new ColumnHeader();
            columnModified = new ColumnHeader();
            columnPath = new ColumnHeader();
            panel4 = new Panel();
            lblTotalSize = new Label();
            lblFileCount = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCleanupDays).BeginInit();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(64, 64, 64);
            panel1.Controls.Add(lblTitle);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1500, 60);
            panel1.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(313, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Received Files Manager";
            // 
            // panel2
            // 
            panel2.Controls.Add(btnCompare);
            panel2.Controls.Add(btnPreview);
            panel2.Controls.Add(btnCleanupOld);
            panel2.Controls.Add(numCleanupDays);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(btnDeleteSelected);
            panel2.Controls.Add(btnOpenFolder);
            panel2.Controls.Add(btnRefresh);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 60);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(10);
            panel2.Size = new Size(1500, 60);
            panel2.TabIndex = 1;
            // 
            // btnCompare
            // 
            btnCompare.BackColor = Color.FromArgb(255, 20, 147);
            btnCompare.FlatStyle = FlatStyle.Flat;
            btnCompare.ForeColor = Color.White;
            btnCompare.Location = new Point(440, 15);
            btnCompare.Name = "btnCompare";
            btnCompare.Size = new Size(100, 30);
            btnCompare.TabIndex = 7;
            btnCompare.Text = "Compare";
            btnCompare.UseVisualStyleBackColor = false;
            btnCompare.Click += btnCompare_Click;
            // 
            // btnPreview
            // 
            btnPreview.BackColor = Color.FromArgb(255, 165, 0);
            btnPreview.FlatStyle = FlatStyle.Flat;
            btnPreview.ForeColor = Color.White;
            btnPreview.Location = new Point(330, 15);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new Size(100, 30);
            btnPreview.TabIndex = 6;
            btnPreview.Text = "Preview";
            btnPreview.UseVisualStyleBackColor = false;
            btnPreview.Click += btnPreview_Click;
            // 
            // btnCleanupOld
            // 
            btnCleanupOld.BackColor = Color.FromArgb(255, 128, 128);
            btnCleanupOld.FlatStyle = FlatStyle.Flat;
            btnCleanupOld.ForeColor = Color.White;
            btnCleanupOld.Location = new Point(685, 15);
            btnCleanupOld.Name = "btnCleanupOld";
            btnCleanupOld.Size = new Size(100, 30);
            btnCleanupOld.TabIndex = 5;
            btnCleanupOld.Text = "Cleanup Old";
            btnCleanupOld.UseVisualStyleBackColor = false;
            btnCleanupOld.Click += btnCleanupOld_Click;
            // 
            // numCleanupDays
            // 
            numCleanupDays.Location = new Point(625, 18);
            numCleanupDays.Maximum = new decimal(new int[] { 365, 0, 0, 0 });
            numCleanupDays.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCleanupDays.Name = "numCleanupDays";
            numCleanupDays.Size = new Size(50, 23);
            numCleanupDays.TabIndex = 4;
            numCleanupDays.Value = new decimal(new int[] { 7, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(550, 21);
            label1.Name = "label1";
            label1.Size = new Size(67, 15);
            label1.TabIndex = 3;
            label1.Text = "Older than:";
            // 
            // btnDeleteSelected
            // 
            btnDeleteSelected.BackColor = Color.FromArgb(255, 128, 0);
            btnDeleteSelected.FlatStyle = FlatStyle.Flat;
            btnDeleteSelected.ForeColor = Color.White;
            btnDeleteSelected.Location = new Point(220, 15);
            btnDeleteSelected.Name = "btnDeleteSelected";
            btnDeleteSelected.Size = new Size(100, 30);
            btnDeleteSelected.TabIndex = 2;
            btnDeleteSelected.Text = "Delete Selected";
            btnDeleteSelected.UseVisualStyleBackColor = false;
            btnDeleteSelected.Click += btnDeleteSelected_Click;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.BackColor = Color.FromArgb(128, 128, 255);
            btnOpenFolder.FlatStyle = FlatStyle.Flat;
            btnOpenFolder.ForeColor = Color.White;
            btnOpenFolder.Location = new Point(120, 15);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(100, 30);
            btnOpenFolder.TabIndex = 1;
            btnOpenFolder.Text = "Open Folder";
            btnOpenFolder.UseVisualStyleBackColor = false;
            btnOpenFolder.Click += btnOpenFolder_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(0, 128, 0);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(20, 15);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(listViewFiles);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(0, 120);
            panel3.Name = "panel3";
            panel3.Padding = new Padding(10);
            panel3.Size = new Size(1500, 540);
            panel3.TabIndex = 2;
            // 
            // listViewFiles
            // 
            listViewFiles.Columns.AddRange(new ColumnHeader[] { columnFileName, columnSize, columnCreated, columnModified, columnPath });
            listViewFiles.Dock = DockStyle.Fill;
            listViewFiles.FullRowSelect = true;
            listViewFiles.GridLines = true;
            listViewFiles.Location = new Point(10, 10);
            listViewFiles.Name = "listViewFiles";
            listViewFiles.Size = new Size(1480, 520);
            listViewFiles.TabIndex = 0;
            listViewFiles.UseCompatibleStateImageBehavior = false;
            listViewFiles.View = View.Details;
            listViewFiles.Click += listViewFiles_Click;
            listViewFiles.DoubleClick += listViewFiles_DoubleClick;
            // 
            // columnFileName
            // 
            columnFileName.Text = "File Name";
            columnFileName.Width = 300;
            // 
            // columnSize
            // 
            columnSize.Text = "Size";
            columnSize.Width = 100;
            // 
            // columnCreated
            // 
            columnCreated.Text = "Created";
            columnCreated.Width = 150;
            // 
            // columnModified
            // 
            columnModified.Text = "Modified";
            columnModified.Width = 150;
            // 
            // columnPath
            // 
            columnPath.Text = "Path";
            columnPath.Width = 250;
            // 
            // panel4
            // 
            panel4.Controls.Add(lblTotalSize);
            panel4.Controls.Add(lblFileCount);
            panel4.Dock = DockStyle.Bottom;
            panel4.Location = new Point(0, 660);
            panel4.Name = "panel4";
            panel4.Padding = new Padding(10);
            panel4.Size = new Size(1500, 40);
            panel4.TabIndex = 3;
            // 
            // lblTotalSize
            // 
            lblTotalSize.AutoSize = true;
            lblTotalSize.Location = new Point(200, 15);
            lblTotalSize.Name = "lblTotalSize";
            lblTotalSize.Size = new Size(111, 15);
            lblTotalSize.TabIndex = 1;
            lblTotalSize.Text = "Storage: 0 B";
            // 
            // lblFileCount
            // 
            lblFileCount.AutoSize = true;
            lblFileCount.Location = new Point(20, 15);
            lblFileCount.Name = "lblFileCount";
            lblFileCount.Size = new Size(98, 15);
            lblFileCount.TabIndex = 0;
            lblFileCount.Text = "Total files: 0";
            // 
            // FileManagerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1500, 700);
            Controls.Add(panel3);
            Controls.Add(panel4);
            Controls.Add(panel2);
            Controls.Add(panel1);
            MinimumSize = new Size(1300, 600);
            Name = "FileManagerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "File Manager - Hung Duy Parking";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCleanupDays).EndInit();
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ResumeLayout(false);
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