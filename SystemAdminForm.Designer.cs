namespace InnEase_CRM
{
    partial class SystemAdminForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // Main container
            tabControl = new TabControl();
            
            // System Settings Tab
            tabSystemSettings = new TabPage("System Settings");
            lblTitle = new Label();
            lblCompanyName = new Label();
            txtCompanyName = new TextBox();
            lblTimezone = new Label();
            cmbTimezone = new ComboBox();
            lblCurrency = new Label();
            cmbCurrency = new ComboBox();
            lblMaintenanceMode = new Label();
            chkMaintenanceMode = new CheckBox();
            btnSave = new Button();
            lblStatus = new Label();

            // Tenant Management Tab
            tabTenantManagement = new TabPage("Tenant Management");
            lblTenantTitle = new Label();
            dgvTenants = new DataGridView();
            btnAddTenant = new Button();
            btnEditTenant = new Button();
            btnDeleteTenant = new Button();
            btnActivateTenant = new Button();
            btnSuspendTenant = new Button();
            lblTenantStatus = new Label();

            // Subscription Management Tab
            tabSubscriptionManagement = new TabPage("Subscription Management");
            lblSubscriptionTitle = new Label();
            dgvSubscriptions = new DataGridView();
            btnRefreshSubscriptions = new Button();
            lblSubscriptionStatus = new Label();

            ((System.ComponentModel.ISupportInitialize)dgvTenants).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvSubscriptions).BeginInit();
            SuspendLayout();

            // =====================================================
            // TAB CONTROL
            // =====================================================
            tabControl.Location = new Point(12, 12);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1160, 680);
            tabControl.TabIndex = 0;

            // =====================================================
            // SYSTEM SETTINGS TAB
            // =====================================================
            tabSystemSettings.BackColor = Color.White;
            tabSystemSettings.Location = new Point(4, 22);
            tabSystemSettings.Name = "tabSystemSettings";
            tabSystemSettings.Padding = new Padding(20);
            tabSystemSettings.Size = new Size(1152, 654);
            tabSystemSettings.TabIndex = 0;
            tabSystemSettings.Text = "System Settings";

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(300, 30);
            lblTitle.Text = "System Administration";

            lblCompanyName.AutoSize = true;
            lblCompanyName.Location = new Point(20, 80);
            lblCompanyName.Name = "lblCompanyName";
            lblCompanyName.Size = new Size(100, 20);
            lblCompanyName.Text = "Company Name:";

            txtCompanyName.Location = new Point(140, 77);
            txtCompanyName.Name = "txtCompanyName";
            txtCompanyName.Size = new Size(300, 25);
            txtCompanyName.Text = "InnEase Hotel";

            lblTimezone.AutoSize = true;
            lblTimezone.Location = new Point(20, 120);
            lblTimezone.Name = "lblTimezone";
            lblTimezone.Size = new Size(70, 20);
            lblTimezone.Text = "Timezone:";

            cmbTimezone.Location = new Point(140, 117);
            cmbTimezone.Name = "cmbTimezone";
            cmbTimezone.Size = new Size(300, 25);
            cmbTimezone.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTimezone.Items.AddRange(new object[] { "UTC+8 (Philippines)", "UTC+0 (GMT)", "UTC-5 (Eastern US)" });

            lblCurrency.AutoSize = true;
            lblCurrency.Location = new Point(20, 160);
            lblCurrency.Name = "lblCurrency";
            lblCurrency.Size = new Size(60, 20);
            lblCurrency.Text = "Currency:";

            cmbCurrency.Location = new Point(140, 157);
            cmbCurrency.Name = "cmbCurrency";
            cmbCurrency.Size = new Size(300, 25);
            cmbCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCurrency.Items.AddRange(new object[] { "PHP", "USD", "EUR" });

            lblMaintenanceMode.AutoSize = true;
            lblMaintenanceMode.Location = new Point(20, 200);
            lblMaintenanceMode.Name = "lblMaintenanceMode";
            lblMaintenanceMode.Size = new Size(120, 20);
            lblMaintenanceMode.Text = "Maintenance Mode:";

            chkMaintenanceMode.Location = new Point(140, 197);
            chkMaintenanceMode.Name = "chkMaintenanceMode";
            chkMaintenanceMode.Size = new Size(200, 25);
            chkMaintenanceMode.Text = "Enabled";

            btnSave.Location = new Point(20, 250);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(150, 35);
            btnSave.Text = "SAVE SETTINGS";
            btnSave.UseVisualStyleBackColor = true;

            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Green;
            lblStatus.Location = new Point(180, 258);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 20);
            lblStatus.Text = "";

            tabSystemSettings.Controls.Add(lblTitle);
            tabSystemSettings.Controls.Add(lblCompanyName);
            tabSystemSettings.Controls.Add(txtCompanyName);
            tabSystemSettings.Controls.Add(lblTimezone);
            tabSystemSettings.Controls.Add(cmbTimezone);
            tabSystemSettings.Controls.Add(lblCurrency);
            tabSystemSettings.Controls.Add(cmbCurrency);
            tabSystemSettings.Controls.Add(lblMaintenanceMode);
            tabSystemSettings.Controls.Add(chkMaintenanceMode);
            tabSystemSettings.Controls.Add(btnSave);
            tabSystemSettings.Controls.Add(lblStatus);

            // =====================================================
            // TENANT MANAGEMENT TAB
            // =====================================================
            tabTenantManagement.BackColor = Color.White;
            tabTenantManagement.Location = new Point(4, 22);
            tabTenantManagement.Name = "tabTenantManagement";
            tabTenantManagement.Padding = new Padding(20);
            tabTenantManagement.Size = new Size(1152, 654);
            tabTenantManagement.TabIndex = 1;
            tabTenantManagement.Text = "Tenant Management";

            lblTenantTitle.AutoSize = true;
            lblTenantTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTenantTitle.Location = new Point(20, 20);
            lblTenantTitle.Name = "lblTenantTitle";
            lblTenantTitle.Size = new Size(250, 30);
            lblTenantTitle.Text = "Tenant Management";

            dgvTenants.AllowUserToAddRows = false;
            dgvTenants.AllowUserToDeleteRows = false;
            dgvTenants.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTenants.BackgroundColor = Color.White;
            dgvTenants.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTenants.Location = new Point(20, 70);
            dgvTenants.Name = "dgvTenants";
            dgvTenants.ReadOnly = true;
            dgvTenants.RowHeadersVisible = false;
            dgvTenants.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTenants.Size = new Size(1110, 400);
            dgvTenants.TabIndex = 0;

            btnAddTenant.Location = new Point(20, 490);
            btnAddTenant.Name = "btnAddTenant";
            btnAddTenant.Size = new Size(120, 35);
            btnAddTenant.Text = "Add Tenant";
            btnAddTenant.UseVisualStyleBackColor = true;

            btnEditTenant.Location = new Point(150, 490);
            btnEditTenant.Name = "btnEditTenant";
            btnEditTenant.Size = new Size(120, 35);
            btnEditTenant.Text = "Edit Tenant";
            btnEditTenant.UseVisualStyleBackColor = true;

            btnDeleteTenant.Location = new Point(280, 490);
            btnDeleteTenant.Name = "btnDeleteTenant";
            btnDeleteTenant.Size = new Size(120, 35);
            btnDeleteTenant.Text = "Delete Tenant";
            btnDeleteTenant.UseVisualStyleBackColor = true;

            btnActivateTenant.Location = new Point(410, 490);
            btnActivateTenant.Name = "btnActivateTenant";
            btnActivateTenant.Size = new Size(120, 35);
            btnActivateTenant.Text = "Activate";
            btnActivateTenant.UseVisualStyleBackColor = true;

            btnSuspendTenant.Location = new Point(540, 490);
            btnSuspendTenant.Name = "btnSuspendTenant";
            btnSuspendTenant.Size = new Size(120, 35);
            btnSuspendTenant.Text = "Suspend";
            btnSuspendTenant.UseVisualStyleBackColor = true;

            lblTenantStatus.AutoSize = true;
            lblTenantStatus.ForeColor = Color.Green;
            lblTenantStatus.Location = new Point(670, 498);
            lblTenantStatus.Name = "lblTenantStatus";
            lblTenantStatus.Size = new Size(0, 20);
            lblTenantStatus.Text = "";

            tabTenantManagement.Controls.Add(lblTenantTitle);
            tabTenantManagement.Controls.Add(dgvTenants);
            tabTenantManagement.Controls.Add(btnAddTenant);
            tabTenantManagement.Controls.Add(btnEditTenant);
            tabTenantManagement.Controls.Add(btnDeleteTenant);
            tabTenantManagement.Controls.Add(btnActivateTenant);
            tabTenantManagement.Controls.Add(btnSuspendTenant);
            tabTenantManagement.Controls.Add(lblTenantStatus);

            // =====================================================
            // SUBSCRIPTION MANAGEMENT TAB
            // =====================================================
            tabSubscriptionManagement.BackColor = Color.White;
            tabSubscriptionManagement.Location = new Point(4, 22);
            tabSubscriptionManagement.Name = "tabSubscriptionManagement";
            tabSubscriptionManagement.Padding = new Padding(20);
            tabSubscriptionManagement.Size = new Size(1152, 654);
            tabSubscriptionManagement.TabIndex = 2;
            tabSubscriptionManagement.Text = "Subscription Management";

            lblSubscriptionTitle.AutoSize = true;
            lblSubscriptionTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblSubscriptionTitle.Location = new Point(20, 20);
            lblSubscriptionTitle.Name = "lblSubscriptionTitle";
            lblSubscriptionTitle.Size = new Size(300, 30);
            lblSubscriptionTitle.Text = "Subscription Plan Management";

            dgvSubscriptions.AllowUserToAddRows = false;
            dgvSubscriptions.AllowUserToDeleteRows = false;
            dgvSubscriptions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSubscriptions.BackgroundColor = Color.White;
            dgvSubscriptions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSubscriptions.Location = new Point(20, 70);
            dgvSubscriptions.Name = "dgvSubscriptions";
            dgvSubscriptions.ReadOnly = true;
            dgvSubscriptions.RowHeadersVisible = false;
            dgvSubscriptions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSubscriptions.Size = new Size(1110, 450);
            dgvSubscriptions.TabIndex = 0;

            btnRefreshSubscriptions.Location = new Point(20, 540);
            btnRefreshSubscriptions.Name = "btnRefreshSubscriptions";
            btnRefreshSubscriptions.Size = new Size(150, 35);
            btnRefreshSubscriptions.Text = "Refresh Data";
            btnRefreshSubscriptions.UseVisualStyleBackColor = true;

            lblSubscriptionStatus.AutoSize = true;
            lblSubscriptionStatus.ForeColor = Color.Green;
            lblSubscriptionStatus.Location = new Point(180, 548);
            lblSubscriptionStatus.Name = "lblSubscriptionStatus";
            lblSubscriptionStatus.Size = new Size(0, 20);
            lblSubscriptionStatus.Text = "";

            tabSubscriptionManagement.Controls.Add(lblSubscriptionTitle);
            tabSubscriptionManagement.Controls.Add(dgvSubscriptions);
            tabSubscriptionManagement.Controls.Add(btnRefreshSubscriptions);
            tabSubscriptionManagement.Controls.Add(lblSubscriptionStatus);

            // =====================================================
            // MAIN FORM
            // =====================================================
            tabControl.Controls.Add(tabSystemSettings);
            tabControl.Controls.Add(tabTenantManagement);
            tabControl.Controls.Add(tabSubscriptionManagement);

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 704);
            Controls.Add(tabControl);
            Font = new Font("Segoe UI", 9F);
            Name = "SystemAdminForm";
            Text = "InnEase CRM - Master Administration";
            WindowState = FormWindowState.Maximized;

            ((System.ComponentModel.ISupportInitialize)dgvTenants).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvSubscriptions).EndInit();
            ResumeLayout(false);
        }

        // System Settings
        private TabControl tabControl;
        private TabPage tabSystemSettings;
        private Label lblTitle;
        private Label lblCompanyName;
        private TextBox txtCompanyName;
        private Label lblTimezone;
        private ComboBox cmbTimezone;
        private Label lblCurrency;
        private ComboBox cmbCurrency;
        private Label lblMaintenanceMode;
        private CheckBox chkMaintenanceMode;
        private Button btnSave;
        private Label lblStatus;

        // Tenant Management
        private TabPage tabTenantManagement;
        private Label lblTenantTitle;
        private DataGridView dgvTenants;
        private Button btnAddTenant;
        private Button btnEditTenant;
        private Button btnDeleteTenant;
        private Button btnActivateTenant;
        private Button btnSuspendTenant;
        private Label lblTenantStatus;

        // Subscription Management
        private TabPage tabSubscriptionManagement;
        private Label lblSubscriptionTitle;
        private DataGridView dgvSubscriptions;
        private Button btnRefreshSubscriptions;
        private Label lblSubscriptionStatus;
    }
}
