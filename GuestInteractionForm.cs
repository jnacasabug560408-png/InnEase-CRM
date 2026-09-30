using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class GuestInteractionForm : Form
    {
        private readonly HttpClient _httpClient;

        private const string ApiUrl =
            "https://localhost:7129/api/interactions";

        public GuestInteractionForm()
        {
            InitializeComponent();

            _httpClient = ApiClient.Instance;

            this.Load += GuestInteractionForm_Load;

            dgvInteractions.CellClick += dgvInteractions_CellClick;

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;

            cmbInteractionType.SelectedIndex = 0;
        }

        private async void GuestInteractionForm_Load(object sender, EventArgs e)
        {
            await LoadInteractions();
        }

        private async Task LoadInteractions()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await ApiClient.GetErrorMessageAsync(response);
                    MessageBox.Show(
                        $"Unable to load interactions.\n\n{errorMessage}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                var items = await response.Content.ReadFromJsonAsync<List<GuestInteraction>>() ?? new List<GuestInteraction>();
                dgvInteractions.DataSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load interactions.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtGuestId.Text, out int guestId))
                {
                    MessageBox.Show("Please enter a valid Guest ID.");
                    return;
                }

                var interaction = new GuestInteraction
                {
                    GuestId = guestId,
                    InteractionType =
                        cmbInteractionType.SelectedItem?.ToString()
                        ?? "CheckIn",
                    Notes = txtNotes.Text,
                    HandledBy = txtHandledBy.Text,
                    InteractionDate = dtpInteractionDate.Value
                };

                var response =
                    await _httpClient.PostAsJsonAsync(
                        ApiUrl,
                        interaction);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Interaction added successfully!",
                        "Success");

                    ClearFields();
                    await LoadInteractions();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to add interaction.\n\n" +
                        await response.Content.ReadAsStringAsync(),
                        "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void dgvInteractions_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = dgvInteractions.Rows[e.RowIndex];

            txtInteractionId.Text =
                row.Cells["GuestInteractionId"].Value?.ToString();

            txtGuestId.Text =
                row.Cells["GuestId"].Value?.ToString();

            txtHandledBy.Text =
                row.Cells["HandledBy"].Value?.ToString();

            txtNotes.Text =
                row.Cells["Notes"].Value?.ToString();

            var type =
                row.Cells["InteractionType"].Value?.ToString();

            if (!string.IsNullOrEmpty(type) &&
                cmbInteractionType.Items.Contains(type))
            {
                cmbInteractionType.SelectedItem = type;
            }

            if (DateTime.TryParse(
                row.Cells["InteractionDate"].Value?.ToString(),
                out var date))
            {
                dtpInteractionDate.Value = date;
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(
                    txtInteractionId.Text,
                    out int interactionId))
                {
                    MessageBox.Show(
                        "Please select an interaction first.");
                    return;
                }

                if (!int.TryParse(
                    txtGuestId.Text,
                    out int guestId))
                {
                    MessageBox.Show(
                        "Please enter a valid Guest ID.");
                    return;
                }

                var interaction = new GuestInteraction
                {
                    GuestInteractionId = interactionId,
                    GuestId = guestId,
                    InteractionType =
                        cmbInteractionType.SelectedItem?.ToString()
                        ?? "CheckIn",
                    Notes = txtNotes.Text,
                    HandledBy = txtHandledBy.Text,
                    InteractionDate = dtpInteractionDate.Value
                };

                var response =
                    await _httpClient.PutAsJsonAsync(
                        $"{ApiUrl}/{interactionId}",
                        interaction);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Interaction updated successfully!",
                        "Success");

                    ClearFields();
                    await LoadInteractions();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to update interaction.\n\n" +
                        await response.Content.ReadAsStringAsync(),
                        "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(
                    txtInteractionId.Text,
                    out int interactionId))
                {
                    MessageBox.Show(
                        "Please select an interaction first.");
                    return;
                }

                var confirm = MessageBox.Show(
                    "Are you sure you want to delete this interaction?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                var response =
                    await _httpClient.DeleteAsync(
                        $"{ApiUrl}/{interactionId}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Interaction deleted successfully!",
                        "Success");

                    ClearFields();
                    await LoadInteractions();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to delete interaction.",
                        "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ClearFields()
        {
            txtInteractionId.Clear();
            txtGuestId.Clear();
            txtHandledBy.Clear();
            txtNotes.Clear();

            cmbInteractionType.SelectedIndex = 0;

            dtpInteractionDate.Value = DateTime.Today;

            txtGuestId.Focus();
        }
    }
}



