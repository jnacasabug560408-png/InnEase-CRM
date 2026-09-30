using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class GuestFeedbackForm : Form
    {
        private readonly HttpClient _httpClient;

        // Stores the selected Feedback ID internally.
        // It is NOT shown in a textbox.
        private int _selectedFeedbackId = 0;

        private string ApiUrl =>
            $"https://localhost:7129/api/guestfeedback";

        public GuestFeedbackForm()
        {
            InitializeComponent();

            _httpClient = ApiClient.Instance;

            // Open the form maximized
            this.WindowState = FormWindowState.Maximized;

            this.Load += GuestFeedbackForm_Load;
            dgvFeedback.CellClick += dgvFeedback_CellClick;

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;

            cmbRequestType.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
        }

        private async void GuestFeedbackForm_Load(object sender, EventArgs e)
        {
            await LoadFeedback();
        }

        private async Task LoadFeedback()
        {
            try
            {
                var items =
                    await _httpClient.GetFromJsonAsync<List<GuestFeedback>>(ApiUrl);

                dgvFeedback.DataSource = items;

                dgvFeedback.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load feedback.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtGuestId.Text, out int guestId) ||
                    guestId <= 0)
                {
                    MessageBox.Show(
                        "Please enter a valid Guest ID.",
                        "Invalid Guest ID",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMessage.Text))
                {
                    MessageBox.Show(
                        "Please enter a message.",
                        "Missing Message",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var feedback = new GuestFeedback
                {
                    GuestId = guestId,
                    RequestType =
                        cmbRequestType.SelectedItem?.ToString() ?? "Feedback",
                    Message = txtMessage.Text.Trim(),
                    Status =
                        cmbStatus.SelectedItem?.ToString() ?? "Open"
                };

                var response =
                    await _httpClient.PostAsJsonAsync(ApiUrl, feedback);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Feedback added successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearInputFields();
                    await LoadFeedback();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to add feedback.\n\n" +
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

        private void dgvFeedback_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = dgvFeedback.Rows[e.RowIndex];

            // Get Feedback ID from the grid
            if (row.Cells["GuestFeedbackId"].Value != null)
            {
                _selectedFeedbackId =
                    Convert.ToInt32(
                        row.Cells["GuestFeedbackId"].Value);
            }

            txtGuestId.Text =
                row.Cells["GuestId"].Value?.ToString();

            txtMessage.Text =
                row.Cells["Message"].Value?.ToString();

            var requestType =
                row.Cells["RequestType"].Value?.ToString();

            if (!string.IsNullOrEmpty(requestType) &&
                cmbRequestType.Items.Contains(requestType))
            {
                cmbRequestType.SelectedItem = requestType;
            }

            var status =
                row.Cells["Status"].Value?.ToString();

            if (!string.IsNullOrEmpty(status) &&
                cmbStatus.Items.Contains(status))
            {
                cmbStatus.SelectedItem = status;
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFeedbackId <= 0)
                {
                    MessageBox.Show(
                        "Please select a feedback record first.",
                        "No Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtGuestId.Text, out int guestId) ||
                    guestId <= 0)
                {
                    MessageBox.Show(
                        "Please enter a valid Guest ID.",
                        "Invalid Guest ID",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMessage.Text))
                {
                    MessageBox.Show(
                        "Please enter a message.",
                        "Missing Message",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var feedback = new GuestFeedback
                {
                    GuestFeedbackId = _selectedFeedbackId,
                    GuestId = guestId,
                    RequestType =
                        cmbRequestType.SelectedItem?.ToString() ?? "Feedback",
                    Message = txtMessage.Text.Trim(),
                    Status =
                        cmbStatus.SelectedItem?.ToString() ?? "Open"
                };

                var response =
                    await _httpClient.PutAsJsonAsync(
                        $"{ApiUrl}/{_selectedFeedbackId}",
                        feedback);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Feedback updated successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearInputFields();
                    await LoadFeedback();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to update feedback.\n\n" +
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

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFeedbackId <= 0)
                {
                    MessageBox.Show(
                        "Please select a feedback record first.",
                        "No Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    "Are you sure you want to delete this feedback record?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                var response =
                    await _httpClient.DeleteAsync(
                        $"{ApiUrl}/{_selectedFeedbackId}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Feedback deleted successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearInputFields();
                    await LoadFeedback();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to delete feedback.\n\n" +
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

        // Clears the fields automatically after Add, Update, or Delete.
        // There is NO Clear button.
        private void ClearInputFields()
        {
            _selectedFeedbackId = 0;

            txtGuestId.Clear();
            txtMessage.Clear();

            cmbRequestType.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;

            dgvFeedback.ClearSelection();
            txtGuestId.Focus();
        }
    }
}



