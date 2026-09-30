namespace InnEase_CRM
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lblBrand = new System.Windows.Forms.Label();
            this.lblTagline = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblUsernameHint = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPasswordHint = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();

            this.btnLogin = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            this.pnlLeft.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.SuspendLayout();

            // pnlLeft
            this.pnlLeft.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlLeft.Controls.Add(this.lblBrand);
            this.pnlLeft.Controls.Add(this.lblTagline);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(280, 400);

            // lblBrand
            this.lblBrand.AutoSize = false;
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblBrand.Location = new System.Drawing.Point(30, 200);
            this.lblBrand.Size = new System.Drawing.Size(220, 60);
            this.lblBrand.Text = "InnEase";
            this.lblBrand.Name = "lblBrand";

            // lblTagline
            this.lblTagline.AutoSize = false;
            this.lblTagline.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblTagline.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTagline.Location = new System.Drawing.Point(30, 260);
            this.lblTagline.Size = new System.Drawing.Size(220, 60);
            this.lblTagline.Text = "Hotel CRM\nManagement System";
            this.lblTagline.Name = "lblTagline";

            // pnlRight
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlRight.Controls.Add(this.lblTitle);
            this.pnlRight.Controls.Add(this.lblSubtitle);
            this.pnlRight.Controls.Add(this.lblUsernameHint);
            this.pnlRight.Controls.Add(this.txtUsername);
            this.pnlRight.Controls.Add(this.lblPasswordHint);
            this.pnlRight.Controls.Add(this.txtPassword);

            this.pnlRight.Controls.Add(this.btnLogin);
            this.pnlRight.Controls.Add(this.lblStatus);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(280, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(420, 400);

            // lblTitle
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblTitle.Location = new System.Drawing.Point(40, 40);
            this.lblTitle.Size = new System.Drawing.Size(340, 40);
            this.lblTitle.Text = "Welcome back";
            this.lblTitle.Name = "lblTitle";

            // lblSubtitle
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblSubtitle.Location = new System.Drawing.Point(40, 82);
            this.lblSubtitle.Size = new System.Drawing.Size(340, 20);
            this.lblSubtitle.Text = "Sign in to your account to continue";
            this.lblSubtitle.Name = "lblSubtitle";

            // lblUsernameHint
            this.lblUsernameHint.AutoSize = true;
            this.lblUsernameHint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUsernameHint.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblUsernameHint.Location = new System.Drawing.Point(40, 120);
            this.lblUsernameHint.Text = "Username";
            this.lblUsernameHint.Name = "lblUsernameHint";

            // txtUsername
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.txtUsername.Location = new System.Drawing.Point(40, 140);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(340, 28);
            this.txtUsername.TabIndex = 0;
            this.txtUsername.BackColor = System.Drawing.Color.White;

            // lblPasswordHint
            this.lblPasswordHint.AutoSize = true;
            this.lblPasswordHint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPasswordHint.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblPasswordHint.Location = new System.Drawing.Point(40, 185);
            this.lblPasswordHint.Text = "Password";
            this.lblPasswordHint.Name = "lblPasswordHint";

            // txtPassword
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.txtPassword.Location = new System.Drawing.Point(40, 205);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(340, 28);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.BackColor = System.Drawing.Color.White;

            // btnLogin
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(40, 278);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(340, 42);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "Sign In";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            // lblStatus
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.lblStatus.Location = new System.Drawing.Point(40, 340);
            this.lblStatus.Size = new System.Drawing.Size(340, 20);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "";

            // LoginForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(700, 400);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InnEase CRM — Login";
            this.pnlLeft.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Label lblTagline;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblUsernameHint;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPasswordHint;
        private System.Windows.Forms.TextBox txtPassword;

        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblStatus;
    }
}
