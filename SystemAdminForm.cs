using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class SystemAdminForm : Form
    {
        private readonly HttpClient _httpClient;
        private List<Staff> _staffList = new List<Staff>();

        private const string ApiBase = "https://localhost:7129/api";

        public SystemAdminForm()
        {
            InitializeComponent();
            _httpClient = ApiClient.Instance;

            btnSave.Click += btnSave_Click;
            Load += SystemAdminForm_Load;

            // Wire up grid events
            btnAddTenant.Text    = "Add Staff";
            btnEditTenant.Text   = "Edit Staff";
            btnDeleteTenant.Text = "Delete Staff";
            btnActivateTenant.Text  = "Activate";
            btnSuspendTenant.Text   = "Deactivate";
            btnRefreshSubscriptions.Text = "Refresh Staff";

            btnAddTenant.Click    += btnAddStaff_Click;
            btnEditTenant.Click   += btnEditStaff_Click;
            btnDeleteTenant.Click += btnDeleteStaff_Click;
            btnActivateTenant.Click  += btnActivateStaff_Click;
            btnSuspendTenant.Click   += btnDeactivateStaff_Click;
            dgvTenants.CellClick     += dgvStaff_CellClick;
            btnRefreshSubscriptions.Click += async (s, e) => await LoadStaff();
        }

        private async void SystemAdminForm_Load(object sender, EventArgs e)
        {
            await LoadStaff();
        }

        // =========================================================
        // SYSTEM SETTINGS
        // =========================================================

        private void btnSave_Click(object sender, EventArgs e)
        {
            lblStatus.Text = $"Settings saved at {DateTime.Now:hh:mm tt}";
        }

        // =========================================================
        // STAFF MANAGEMENT  (replaces old Tenant Management)
        // =========================================================

        private async Task LoadStaff()
        {
            try
            {
                _staffList = await _httpClient.GetFromJsonAsync<List<Staff>>($"{ApiBase}/staff")
                    ?? new List<Staff>();

                dgvTenants.DataSource = null;
                dgvTenants.DataSource = _staffList;

                lblTenantStatus.Text = $"Loaded {_staffList.Count} staff member(s)";

                // Also show summary in the subscriptions grid
                dgvSubscriptions.DataSource = null;
                dgvSubscriptions.DataSource = _staffList;
                lblSubscriptionStatus.Text = $"{_staffList.Count} staff total";
            }
            catch (Exception ex)
            {
                lblTenantStatus.Text = "Failed to load staff";
                MessageBox.Show($"Unable to load staff.\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvStaff_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var staff = dgvTenants.Rows[e.RowIndex].DataBoundItem as Staff;
            if (staff != null)
            {
                btnActivateTenant.Enabled  = !staff.IsActive;
                btnSuspendTenant.Enabled   = staff.IsActive;
            }
        }

        private async void btnAddStaff_Click(object sender, EventArgs e)
        {
            using var dialog = new StaffDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync($"{ApiBase}/staff", dialog.Staff);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Staff member added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadStaff();
                    }
                    else
                    {
                        var err = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to add staff.\n\n{err}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnEditStaff_Click(object sender, EventArgs e)
        {
            if (dgvTenants.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a staff member to edit.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selected = dgvTenants.SelectedRows[0].DataBoundItem as Staff;
            if (selected == null) return;

            using var dialog = new StaffDialog(selected);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"{ApiBase}/staff/{selected.StaffId}", dialog.Staff);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Staff member updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadStaff();
                    }
                    else
                    {
                        var err = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to update.\n\n{err}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnDeleteStaff_Click(object sender, EventArgs e)
        {
            if (dgvTenants.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a staff member to delete.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selected = dgvTenants.SelectedRows[0].DataBoundItem as Staff;
            if (selected == null) return;

            if (MessageBox.Show($"Delete '{selected.FullName}'?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBase}/staff/{selected.StaffId}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Staff member deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadStaff();
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to delete.\n\n{err}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnActivateStaff_Click(object sender, EventArgs e)
        {
            var selected = dgvTenants.SelectedRows.Count > 0
                ? dgvTenants.SelectedRows[0].DataBoundItem as Staff : null;
            if (selected == null) return;
            selected.IsActive = true;
            await _httpClient.PutAsJsonAsync($"{ApiBase}/staff/{selected.StaffId}", selected);
            await LoadStaff();
        }

        private async void btnDeactivateStaff_Click(object sender, EventArgs e)
        {
            var selected = dgvTenants.SelectedRows.Count > 0
                ? dgvTenants.SelectedRows[0].DataBoundItem as Staff : null;
            if (selected == null) return;
            selected.IsActive = false;
            await _httpClient.PutAsJsonAsync($"{ApiBase}/staff/{selected.StaffId}", selected);
            await LoadStaff();
        }
    }

    // =========================================================
    // STAFF DIALOG  (Add / Edit)
    // =========================================================

    public class StaffDialog : Form
    {
        private TextBox txtFullName;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtEmail;
        private ComboBox cmbRole;
        private Button btnOK;
        private Button btnCancel;

        public Staff Staff { get; private set; }

        public StaffDialog(Staff existing = null)
        {
            Staff = existing != null
                ? new Staff
                {
                    StaffId      = existing.StaffId,
                    TenantId     = existing.TenantId,
                    FullName     = existing.FullName,
                    Username     = existing.Username,
                    PasswordHash = existing.PasswordHash,
                    Email        = existing.Email,
                    Role         = existing.Role,
                    IsActive     = existing.IsActive,
                    CreatedAt    = existing.CreatedAt
                }
                : new Staff { TenantId = 1, IsActive = true, CreatedAt = DateTime.UtcNow };

            Build();
            Populate();
        }

        private void Build()
        {
            this.Text             = Staff.StaffId == 0 ? "Add Staff Member" : "Edit Staff Member";
            this.FormBorderStyle  = FormBorderStyle.FixedDialog;
            this.StartPosition    = FormStartPosition.CenterParent;
            this.ClientSize       = new System.Drawing.Size(400, 310);
            this.MinimizeBox      = false;
            this.MaximizeBox      = false;

            int y = 20, lw = 110, fw = 240, fh = 26, gap = 10;

            var mkLbl = (string t, int yy) => new Label { Text = t, Location = new System.Drawing.Point(20, yy), Size = new System.Drawing.Size(lw, 20), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var mkTxt = (int yy) => new TextBox { Location = new System.Drawing.Point(140, yy), Size = new System.Drawing.Size(fw, fh) };

            var lName = mkLbl("Full Name:", y);       txtFullName = mkTxt(y); y += fh + gap;
            var lUser = mkLbl("Username:", y);         txtUsername = mkTxt(y); y += fh + gap;
            var lPass = mkLbl("Password:", y);         txtPassword = mkTxt(y); txtPassword.PasswordChar = '●'; y += fh + gap;
            var lMail = mkLbl("Email:", y);            txtEmail    = mkTxt(y); y += fh + gap;
            var lRole = mkLbl("Role:", y);
            cmbRole = new ComboBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(fw, fh), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new object[] { "master", "admin", "manager", "staff" });
            y += fh + gap + 10;

            btnOK     = new Button { Text = "Save",   DialogResult = DialogResult.OK,     Location = new System.Drawing.Point(20,  y), Size = new System.Drawing.Size(90, 32) };
            btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(120, y), Size = new System.Drawing.Size(90, 32) };

            this.Controls.AddRange(new Control[] { lName, txtFullName, lUser, txtUsername, lPass, txtPassword, lMail, txtEmail, lRole, cmbRole, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void Populate()
        {
            txtFullName.Text  = Staff.FullName;
            txtUsername.Text  = Staff.Username;
            txtPassword.Text  = Staff.PasswordHash;
            txtEmail.Text     = Staff.Email;
            cmbRole.SelectedItem = Staff.Role;
            if (cmbRole.SelectedIndex < 0) cmbRole.SelectedIndex = 3; // default staff
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    MessageBox.Show("Full name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; return;
                }
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    MessageBox.Show("Username is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; return;
                }

                Staff.FullName     = txtFullName.Text.Trim();
                Staff.Username     = txtUsername.Text.Trim();
                Staff.PasswordHash = txtPassword.Text.Trim();
                Staff.Email        = txtEmail.Text.Trim();
                Staff.Role         = cmbRole.SelectedItem?.ToString() ?? "staff";
            }
            base.OnFormClosing(e);
        }
    }
}
