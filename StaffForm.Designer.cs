namespace InnEase_CRM
{
    partial class StaffForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();

            lblStaffId = new Label();
            txtStaffId = new TextBox();

            lblFullName = new Label();
            txtFullName = new TextBox();

            lblRole = new Label();
            cmbRole = new ComboBox();

            lblEmail = new Label();
            txtEmail = new TextBox();

            lblUsername = new Label();
            txtUsername = new TextBox();

            lblIsActive = new Label();
            chkIsActive = new CheckBox();

            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();

            dgvStaff = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)dgvStaff).BeginInit();
            SuspendLayout();

            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font(
                "Segoe UI",
                14F,
                FontStyle.Bold);

            lblTitle.Location = new Point(30, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 25);
            lblTitle.Text = "InnEase CRM - Staff Accounts";

            // 
            // lblStaffId
            // 
            lblStaffId.AutoSize = true;
            lblStaffId.Location = new Point(30, 73);
            lblStaffId.Name = "lblStaffId";
            lblStaffId.Size = new Size(60, 15);
            lblStaffId.Text = "Staff ID:";

            // 
            // txtStaffId
            // 
            txtStaffId.Location = new Point(150, 70);
            txtStaffId.Name = "txtStaffId";
            txtStaffId.Size = new Size(90, 25);
            txtStaffId.ReadOnly = true;
            txtStaffId.BackColor = Color.WhiteSmoke;
            txtStaffId.ForeColor = Color.Black;

            // 
            // lblFullName
            // 
            lblFullName.AutoSize = true;
            lblFullName.Location = new Point(350, 73);
            lblFullName.Name = "lblFullName";
            lblFullName.Size = new Size(65, 15);
            lblFullName.Text = "Full Name:";

            // 
            // txtFullName
            // 
            txtFullName.Location = new Point(450, 70);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(220, 25);

            // 
            // lblRole
            // 
            lblRole.AutoSize = true;
            lblRole.Location = new Point(30, 113);
            lblRole.Name = "lblRole";
            lblRole.Size = new Size(35, 15);
            lblRole.Text = "Role:";

            // 
            // cmbRole
            // 
            cmbRole.Location = new Point(150, 110);
            cmbRole.Name = "cmbRole";
            cmbRole.Size = new Size(180, 25);
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbRole.Items.AddRange(
                new object[]
                {
                    "SuperAdmin",
                    "Admin",
                    "Manager",
                    "Staff"
                });

            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(350, 113);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(40, 15);
            lblEmail.Text = "Email:";

            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(450, 110);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(220, 25);

            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(30, 153);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(65, 15);
            lblUsername.Text = "Username:";

            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(150, 150);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(180, 25);

            // 
            // lblIsActive
            // 
            lblIsActive.AutoSize = true;
            lblIsActive.Location = new Point(350, 153);
            lblIsActive.Name = "lblIsActive";
            lblIsActive.Size = new Size(45, 15);
            lblIsActive.Text = "Active:";

            // 
            // chkIsActive
            // 
            chkIsActive.Location = new Point(450, 150);
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Size = new Size(80, 25);
            chkIsActive.Text = "Yes";
            chkIsActive.Checked = true;

            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(30, 200);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(95, 32);
            btnAdd.Text = "ADD";
            btnAdd.UseVisualStyleBackColor = true;

            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(135, 200);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(95, 32);
            btnUpdate.Text = "UPDATE";
            btnUpdate.UseVisualStyleBackColor = true;

            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(240, 200);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(95, 32);
            btnDelete.Text = "DELETE";
            btnDelete.UseVisualStyleBackColor = true;


            // 
            // dgvStaff
            // 
            dgvStaff.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            dgvStaff.Location = new Point(30, 250);

            dgvStaff.Name = "dgvStaff";

            dgvStaff.Size = new Size(
                1200,
                500);

            dgvStaff.AllowUserToAddRows = false;
            dgvStaff.AllowUserToDeleteRows = false;
            dgvStaff.ReadOnly = true;

            dgvStaff.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvStaff.MultiSelect = false;

            dgvStaff.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvStaff.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // 
            // StaffForm
            // 
            AutoScaleDimensions =
                new SizeF(7F, 15F);

            AutoScaleMode =
                AutoScaleMode.Font;

            ClientSize =
                new Size(1280, 800);

            Controls.Add(lblTitle);

            Controls.Add(lblStaffId);
            Controls.Add(txtStaffId);

            Controls.Add(lblFullName);
            Controls.Add(txtFullName);

            Controls.Add(lblRole);
            Controls.Add(cmbRole);

            Controls.Add(lblEmail);
            Controls.Add(txtEmail);

            Controls.Add(lblUsername);
            Controls.Add(txtUsername);

            Controls.Add(lblIsActive);
            Controls.Add(chkIsActive);

            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
           

            Controls.Add(dgvStaff);

            Name = "StaffForm";
            Text = "InnEase CRM - Staff Accounts";

            ((System.ComponentModel.ISupportInitialize)dgvStaff).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitle;

        private Label lblStaffId;
        private TextBox txtStaffId;

        private Label lblFullName;
        private TextBox txtFullName;

        private Label lblRole;
        private ComboBox cmbRole;

        private Label lblEmail;
        private TextBox txtEmail;

        private Label lblUsername;
        private TextBox txtUsername;

        private Label lblIsActive;
        private CheckBox chkIsActive;

        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private Button btnRefresh;

        private DataGridView dgvStaff;
    }
}
