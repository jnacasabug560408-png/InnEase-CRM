using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class StaffForm : Form
    {
        private readonly HttpClient _httpClient;

        private const string ApiUrl =
            "https://localhost:7129/api/staff";

        public StaffForm()
        {
            InitializeComponent();

            _httpClient = ApiClient.Instance;

            // Open form maximized
            this.WindowState = FormWindowState.Maximized;

            this.Load += StaffForm_Load;

            dgvStaff.CellClick += dgvStaff_CellClick;

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
         
            cmbRole.SelectedIndex = 3;
            chkIsActive.Checked = true;
        }

        // =========================
        // LOAD STAFF
        // =========================
        private async void StaffForm_Load(object sender, EventArgs e)
        {
            await LoadStaff();
        }

        private async Task LoadStaff()
        {
            try
            {
                var staff = await _httpClient
                    .GetFromJsonAsync<List<Staff>>(ApiUrl)
                    ?? new List<Staff>();

                dgvStaff.DataSource = staff;

                dgvStaff.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load staff accounts.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // ADD
        // =========================
        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    MessageBox.Show(
                        "Please enter the staff's full name.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    MessageBox.Show(
                        "Please enter a username.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show(
                        "Please enter an email.");
                    return;
                }

                var staff = new Staff
                {
                    FullName = txtFullName.Text.Trim(),
                    Role = cmbRole.SelectedItem?.ToString() ?? "Staff",
                    Email = txtEmail.Text.Trim(),
                    Username = txtUsername.Text.Trim(),
                    IsActive = chkIsActive.Checked
                };

                var response = await _httpClient.PostAsJsonAsync(
                    ApiUrl,
                    staff);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Staff account added successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    await LoadStaff();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to add staff account.\n\n" +
                        await response.Content.ReadAsStringAsync(),
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // SELECT ROW
        // =========================
        private void dgvStaff_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = dgvStaff.Rows[e.RowIndex];

            txtStaffId.Text =
                row.Cells["StaffId"].Value?.ToString();

            txtFullName.Text =
                row.Cells["FullName"].Value?.ToString();

            txtEmail.Text =
                row.Cells["Email"].Value?.ToString();

            txtUsername.Text =
                row.Cells["Username"].Value?.ToString();

            var role =
                row.Cells["Role"].Value?.ToString();

            if (!string.IsNullOrEmpty(role) &&
                cmbRole.Items.Contains(role))
            {
                cmbRole.SelectedItem = role;
            }

            if (row.Cells["IsActive"].Value != null)
            {
                chkIsActive.Checked =
                    Convert.ToBoolean(
                        row.Cells["IsActive"].Value);
            }
        }

        // =========================
        // UPDATE
        // =========================
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(
                    txtStaffId.Text,
                    out int staffId))
                {
                    MessageBox.Show(
                        "Please select a staff account first.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    MessageBox.Show(
                        "Please enter the staff's full name.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    MessageBox.Show(
                        "Please enter a username.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show(
                        "Please enter an email.");
                    return;
                }

                var staff = new Staff
                {
                    StaffId = staffId,
                    FullName = txtFullName.Text.Trim(),
                    Role = cmbRole.SelectedItem?.ToString() ?? "Staff",
                    Email = txtEmail.Text.Trim(),
                    Username = txtUsername.Text.Trim(),
                    IsActive = chkIsActive.Checked
                };

                var response = await _httpClient.PutAsJsonAsync(
                    $"{ApiUrl}/{staffId}",
                    staff);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Staff account updated successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    await LoadStaff();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to update staff account.\n\n" +
                        await response.Content.ReadAsStringAsync(),
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // DELETE
        // =========================
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(
                    txtStaffId.Text,
                    out int staffId))
                {
                    MessageBox.Show(
                        "Please select a staff account first.");
                    return;
                }

                var confirm = MessageBox.Show(
                    "Are you sure you want to delete this staff account?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                var response = await _httpClient.DeleteAsync(
                    $"{ApiUrl}/{staffId}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Staff account deleted successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    await LoadStaff();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to delete staff account.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // CLEAR
        // =========================
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtStaffId.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtUsername.Clear();

            cmbRole.SelectedIndex = 3;
            chkIsActive.Checked = true;

            txtFullName.Focus();
        }

        // =========================
        // REFRESH
        // =========================
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadStaff();
        }
    }
}

