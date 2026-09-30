using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class CustomerConsentForm : Form
    {
        private readonly HttpClient _httpClient;

        // Stores the selected Consent ID internally
        private int _selectedConsentId = 0;

        private string ApiUrl =>
            $"https://localhost:7129/api/consents";

        public CustomerConsentForm()
        {
            InitializeComponent();

            _httpClient = ApiClient.Instance;

            // Open form maximized
            this.WindowState = FormWindowState.Maximized;

            this.Load += CustomerConsentForm_Load;

            dgvConsents.CellClick += dgvConsents_CellClick;

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;

            cmbConsentType.SelectedIndex = 0;
            chkIsGiven.Checked = true;
        }

        private async void CustomerConsentForm_Load(
            object sender,
            EventArgs e)
        {
            await LoadConsents();
        }

        private async Task LoadConsents()
        {
            try
            {
                var items =
                    await _httpClient.GetFromJsonAsync<List<CustomerConsent>>(ApiUrl);

                dgvConsents.DataSource = items;

                dgvConsents.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load consent records.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void btnAdd_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (!int.TryParse(
                    txtGuestId.Text,
                    out int guestId) ||
                    guestId <= 0)
                {
                    MessageBox.Show(
                        "Please enter a valid Guest ID.",
                        "Invalid Guest ID",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                var consent = new CustomerConsent
                {
                    GuestId = guestId,

                    ConsentType =
                        cmbConsentType.SelectedItem?.ToString()
                        ?? "Marketing",

                    IsGiven = chkIsGiven.Checked,

                    ConsentDate = dtpConsentDate.Value,

                    Notes = txtNotes.Text.Trim()
                };

                var response =
                    await _httpClient.PostAsJsonAsync(
                        ApiUrl,
                        consent);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Consent record added successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadConsents();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to add consent record.\n\n" +
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

        private void dgvConsents_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = dgvConsents.Rows[e.RowIndex];

            // Store Consent ID internally
            if (row.Cells["CustomerConsentId"].Value != null)
            {
                _selectedConsentId =
                    Convert.ToInt32(
                        row.Cells["CustomerConsentId"].Value);
            }

            txtGuestId.Text =
                row.Cells["GuestId"].Value?.ToString();

            txtNotes.Text =
                row.Cells["Notes"].Value?.ToString();

            var consentType =
                row.Cells["ConsentType"].Value?.ToString();

            if (!string.IsNullOrEmpty(consentType) &&
                cmbConsentType.Items.Contains(consentType))
            {
                cmbConsentType.SelectedItem = consentType;
            }

            if (bool.TryParse(
                row.Cells["IsGiven"].Value?.ToString(),
                out bool isGiven))
            {
                chkIsGiven.Checked = isGiven;
            }

            if (DateTime.TryParse(
                row.Cells["ConsentDate"].Value?.ToString(),
                out DateTime consentDate))
            {
                dtpConsentDate.Value = consentDate;
            }
        }

        private async void btnUpdate_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (_selectedConsentId <= 0)
                {
                    MessageBox.Show(
                        "Please select a consent record first.",
                        "No Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (!int.TryParse(
                    txtGuestId.Text,
                    out int guestId) ||
                    guestId <= 0)
                {
                    MessageBox.Show(
                        "Please enter a valid Guest ID.",
                        "Invalid Guest ID",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                var consent = new CustomerConsent
                {
                    CustomerConsentId =
                        _selectedConsentId,

                    GuestId = guestId,

                    ConsentType =
                        cmbConsentType.SelectedItem?.ToString()
                        ?? "Marketing",

                    IsGiven = chkIsGiven.Checked,

                    ConsentDate = dtpConsentDate.Value,

                    Notes = txtNotes.Text.Trim()
                };

                var response =
                    await _httpClient.PutAsJsonAsync(
                        $"{ApiUrl}/{_selectedConsentId}",
                        consent);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Consent record updated successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadConsents();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to update consent record.\n\n" +
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

        private async void btnDelete_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (_selectedConsentId <= 0)
                {
                    MessageBox.Show(
                        "Please select a consent record first.",
                        "No Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                var confirm = MessageBox.Show(
                    "Are you sure you want to delete this consent record?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                var response =
                    await _httpClient.DeleteAsync(
                        $"{ApiUrl}/{_selectedConsentId}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Consent record deleted successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadConsents();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to delete consent record.\n\n" +
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

        // Automatically clears the fields
        // There is NO Clear button
        private void ClearFields()
        {
            _selectedConsentId = 0;

            txtGuestId.Clear();

            cmbConsentType.SelectedIndex = 0;

            chkIsGiven.Checked = true;

            dtpConsentDate.Value = DateTime.Today;

            txtNotes.Clear();

            dgvConsents.ClearSelection();

            txtGuestId.Focus();
        }
    }
}

