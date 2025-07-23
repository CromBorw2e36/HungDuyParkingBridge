namespace HungDuyParkingBridge.UI
{
    partial class FilePreviewForm
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
            this.btnOpenFolder = new Button();
            this.btnOpenFile = new Button();
            this.panel3 = new Panel();
            this.lblFilePath = new Label();
            this.lblModifiedDate = new Label();
            this.lblCreatedDate = new Label();
            this.lblFileSize = new Label();
            this.lblFileName = new Label();
            this.panel4 = new Panel();
            this.pictureBoxPreview = new PictureBox();
            this.txtPreview = new TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = Color.FromArgb(64, 64, 64);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(800, 50);
            this.panel1.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(120, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "File Preview";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.btnOpenFolder);
            this.panel2.Controls.Add(this.btnOpenFile);
            this.panel2.Dock = DockStyle.Bottom;
            this.panel2.Location = new Point(0, 550);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new Padding(10);
            this.panel2.Size = new Size(800, 50);
            this.panel2.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnClose.BackColor = Color.FromArgb(128, 128, 128);
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.ForeColor = Color.White;
            this.btnClose.Location = new Point(690, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(100, 30);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.BackColor = Color.FromArgb(128, 128, 255);
            this.btnOpenFolder.FlatStyle = FlatStyle.Flat;
            this.btnOpenFolder.ForeColor = Color.White;
            this.btnOpenFolder.Location = new Point(130, 10);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new Size(120, 30);
            this.btnOpenFolder.TabIndex = 1;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            this.btnOpenFolder.Click += new EventHandler(this.btnOpenFolder_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.BackColor = Color.FromArgb(0, 128, 0);
            this.btnOpenFile.FlatStyle = FlatStyle.Flat;
            this.btnOpenFile.ForeColor = Color.White;
            this.btnOpenFile.Location = new Point(10, 10);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new Size(120, 30);
            this.btnOpenFile.TabIndex = 0;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = false;
            this.btnOpenFile.Click += new EventHandler(this.btnOpenFile_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblFilePath);
            this.panel3.Controls.Add(this.lblModifiedDate);
            this.panel3.Controls.Add(this.lblCreatedDate);
            this.panel3.Controls.Add(this.lblFileSize);
            this.panel3.Controls.Add(this.lblFileName);
            this.panel3.Dock = DockStyle.Top;
            this.panel3.Location = new Point(0, 50);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new Padding(10);
            this.panel3.Size = new Size(800, 120);
            this.panel3.TabIndex = 2;
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoSize = true;
            this.lblFilePath.Font = new Font("Segoe UI", 9F);
            this.lblFilePath.Location = new Point(13, 90);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new Size(72, 15);
            this.lblFilePath.TabIndex = 4;
            this.lblFilePath.Text = "Path:";
            // 
            // lblModifiedDate
            // 
            this.lblModifiedDate.AutoSize = true;
            this.lblModifiedDate.Font = new Font("Segoe UI", 9F);
            this.lblModifiedDate.Location = new Point(13, 70);
            this.lblModifiedDate.Name = "lblModifiedDate";
            this.lblModifiedDate.Size = new Size(62, 15);
            this.lblModifiedDate.TabIndex = 3;
            this.lblModifiedDate.Text = "Modified:";
            // 
            // lblCreatedDate
            // 
            this.lblCreatedDate.AutoSize = true;
            this.lblCreatedDate.Font = new Font("Segoe UI", 9F);
            this.lblCreatedDate.Location = new Point(13, 50);
            this.lblCreatedDate.Name = "lblCreatedDate";
            this.lblCreatedDate.Size = new Size(61, 15);
            this.lblCreatedDate.TabIndex = 2;
            this.lblCreatedDate.Text = "Created:";
            // 
            // lblFileSize
            // 
            this.lblFileSize.AutoSize = true;
            this.lblFileSize.Font = new Font("Segoe UI", 9F);
            this.lblFileSize.Location = new Point(13, 30);
            this.lblFileSize.Name = "lblFileSize";
            this.lblFileSize.Size = new Size(70, 15);
            this.lblFileSize.TabIndex = 1;
            this.lblFileSize.Text = "Size:";
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblFileName.Location = new Point(13, 10);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new Size(62, 19);
            this.lblFileName.TabIndex = 0;
            this.lblFileName.Text = "File name:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.pictureBoxPreview);
            this.panel4.Controls.Add(this.txtPreview);
            this.panel4.Dock = DockStyle.Fill;
            this.panel4.Location = new Point(0, 170);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new Padding(10);
            this.panel4.Size = new Size(800, 380);
            this.panel4.TabIndex = 3;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBoxPreview.Location = new Point(10, 10);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new Size(400, 300);
            this.pictureBoxPreview.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBoxPreview.TabIndex = 1;
            this.pictureBoxPreview.TabStop = false;
            this.pictureBoxPreview.Visible = false;
            // 
            // txtPreview
            // 
            this.txtPreview.BackColor = Color.White;
            this.txtPreview.Dock = DockStyle.Fill;
            this.txtPreview.Font = new Font("Consolas", 9F);
            this.txtPreview.Location = new Point(10, 10);
            this.txtPreview.Multiline = true;
            this.txtPreview.Name = "txtPreview";
            this.txtPreview.ReadOnly = true;
            this.txtPreview.ScrollBars = ScrollBars.Both;
            this.txtPreview.Size = new Size(780, 360);
            this.txtPreview.TabIndex = 0;
            this.txtPreview.Text = "";
            // 
            // FilePreviewForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 600);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new Size(600, 400);
            this.Name = "FilePreviewForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "File Preview - Hung Duy Parking";
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label lblTitle;
        private Panel panel2;
        private Button btnClose;
        private Button btnOpenFolder;
        private Button btnOpenFile;
        private Panel panel3;
        private Label lblFilePath;
        private Label lblModifiedDate;
        private Label lblCreatedDate;
        private Label lblFileSize;
        private Label lblFileName;
        private Panel panel4;
        private PictureBox pictureBoxPreview;
        private TextBox txtPreview;
    }
}