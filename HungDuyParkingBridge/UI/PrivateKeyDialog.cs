using System;
using System.Drawing;
using System.Windows.Forms;
using HungDuyParkingBridge.Utils;

namespace HungDuyParkingBridge.UI
{
    public partial class PrivateKeyDialog : Form
    {
        private TextBox txtPrivateKey;
        private Button btnOK;
        private Button btnCancel;
        private CheckBox chkShowPassword;

        public bool IsAuthenticated { get; private set; } = false;

        public PrivateKeyDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtPrivateKey = new TextBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.chkShowPassword = new CheckBox();
            this.SuspendLayout();

            // Form settings - much smaller and compact
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(280, 110);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "🔐 Private Key";
            this.BackColor = Color.White;

            // Private key textbox - smaller
            this.txtPrivateKey.Font = new Font("Segoe UI", 9F);
            this.txtPrivateKey.Location = new Point(15, 20);
            this.txtPrivateKey.Size = new Size(250, 23);
            this.txtPrivateKey.UseSystemPasswordChar = true;
            this.txtPrivateKey.PlaceholderText = "Enter key...";

            // Show password checkbox - smaller
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Font = new Font("Segoe UI", 8F);
            this.chkShowPassword.Location = new Point(15, 50);
            this.chkShowPassword.Text = "Show";
            this.chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;

            // OK button - smaller
            this.btnOK.BackColor = Color.FromArgb(0, 123, 255);
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.Font = new Font("Segoe UI", 8F);
            this.btnOK.ForeColor = Color.White;
            this.btnOK.Location = new Point(135, 75);
            this.btnOK.Size = new Size(60, 25);
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += BtnOK_Click;

            // Cancel button - smaller
            this.btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Font = new Font("Segoe UI", 8F);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.Location = new Point(205, 75);
            this.btnCancel.Size = new Size(60, 25);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += BtnCancel_Click;
            this.btnCancel.DialogResult = DialogResult.Cancel;

            // Add controls to form
            this.Controls.Add(this.txtPrivateKey);
            this.Controls.Add(this.chkShowPassword);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);

            // Set default button and cancel button
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;

            // Focus on textbox
            this.txtPrivateKey.Select();

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPrivateKey.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string inputKey = txtPrivateKey.Text.Trim();

            if (string.IsNullOrEmpty(inputKey))
            {
                MessageBox.Show("Please enter a private key.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrivateKey.Focus();
                return;
            }

            if (HDParkingConst.ValidatePrivateKey(inputKey))
            {
                IsAuthenticated = true;
                HDParkingConst.SetAdminAccess(true);
                MessageBox.Show("✅ Authentication successful!", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("❌ Invalid private key!", 
                    "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPrivateKey.Clear();
                txtPrivateKey.Focus();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}