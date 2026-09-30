using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class ReservationForm : Form
    {
        private readonly HttpClient _httpClient;

        // =====================================================
        // SELECTED RESERVATION ID
        // =====================================================
        // ReservationId is NOT shown in a textbox.
        // It is only stored here when a row is selected.
        // =====================================================

        private int _selectedReservationId = 0;


        // =====================================================
        // TENANT API URL
        // =====================================================

        private string ApiUrl =>
            $"https://localhost:7129/api/reservations";


        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public ReservationForm()
        {
            InitializeComponent();

            _httpClient = ApiClient.Instance;

            this.Load += ReservationForm_Load;

            dgvReservations.CellClick += dgvReservations_CellClick;

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;

            cmbStatus.SelectedIndex = 0;
        }


        // =====================================================
        // FORM LOAD
        // =====================================================

        private async void ReservationForm_Load(
            object sender,
            EventArgs e)
        {
            await LoadReservations();
        }


        // =====================================================
        // LOAD RESERVATIONS
        // =====================================================

        private async Task LoadReservations()
        {
            try
            {
                var reservations =
                    await _httpClient.GetFromJsonAsync<List<Reservation>>(
                        ApiUrl);

                dgvReservations.DataSource =
                    reservations ??
                    new List<Reservation>();


                // =================================================
                // DO NOT HIDE ReservationId
                // =================================================
                // ReservationId will remain visible in the grid.
                // =================================================
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load reservations.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // =====================================================
        // ADD RESERVATION
        // =====================================================

        private async void btnAdd_Click(
            object sender,
            EventArgs e)
        {
            if (!ValidateReservationInformation())
                return;


            try
            {
                int guestId =
                    int.Parse(
                        txtGuestId.Text.Trim());


                // -------------------------------------------------
                // CREATE RESERVATION
                // -------------------------------------------------

                var reservation = new Reservation
                {
                    GuestId =
                        guestId,

                    RoomNumber =
                        txtRoomNumber.Text.Trim(),

                    CheckInDate =
                        dtpCheckIn.Value,

                    CheckOutDate =
                        dtpCheckOut.Value,

                    Status =
                        cmbStatus.SelectedItem?.ToString()
                        ?? "Pending"
                };


                // -------------------------------------------------
                // SEND TO API
                // -------------------------------------------------

                var response =
                    await _httpClient.PostAsJsonAsync(
                        ApiUrl,
                        reservation);


                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Reservation added successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadReservations();

                    return;
                }


                // -------------------------------------------------
                // API ERROR
                // -------------------------------------------------

                MessageBox.Show(
                    "Failed to add reservation.\n\n" +
                    await response.Content
                        .ReadAsStringAsync(),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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


        // =====================================================
        // SELECT RESERVATION FROM GRID
        // =====================================================

        private void dgvReservations_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;


            var row =
                dgvReservations.Rows[e.RowIndex];


            // =================================================
            // GET RESERVATION ID FROM GRID
            // =================================================
            // ReservationId stays visible in the grid,
            // but it is NOT copied to a textbox.
            // =================================================

            if (row.Cells["ReservationId"].Value != null)
            {
                int.TryParse(
                    row.Cells["ReservationId"]
                        .Value
                        .ToString(),
                    out _selectedReservationId);
            }


            // -------------------------------------------------
            // GUEST ID
            // -------------------------------------------------

            txtGuestId.Text =
                row.Cells["GuestId"]
                    .Value?
                    .ToString()
                ?? string.Empty;


            // -------------------------------------------------
            // ROOM NUMBER
            // -------------------------------------------------

            txtRoomNumber.Text =
                row.Cells["RoomNumber"]
                    .Value?
                    .ToString()
                ?? string.Empty;


            // -------------------------------------------------
            // CHECK-IN DATE
            // -------------------------------------------------

            if (DateTime.TryParse(
                row.Cells["CheckInDate"]
                    .Value?
                    .ToString(),
                out var checkIn))
            {
                dtpCheckIn.Value =
                    checkIn;
            }


            // -------------------------------------------------
            // CHECK-OUT DATE
            // -------------------------------------------------

            if (DateTime.TryParse(
                row.Cells["CheckOutDate"]
                    .Value?
                    .ToString(),
                out var checkOut))
            {
                dtpCheckOut.Value =
                    checkOut;
            }


            // -------------------------------------------------
            // STATUS
            // -------------------------------------------------

            var status =
                row.Cells["Status"]
                    .Value?
                    .ToString();


            if (!string.IsNullOrEmpty(status) &&
                cmbStatus.Items.Contains(status))
            {
                cmbStatus.SelectedItem =
                    status;
            }
        }


        // =====================================================
        // UPDATE RESERVATION
        // =====================================================

        private async void btnUpdate_Click(
            object sender,
            EventArgs e)
        {
            if (!ValidateReservationInformation())
                return;


            try
            {
                // -------------------------------------------------
                // CHECK SELECTED RESERVATION
                // -------------------------------------------------

                if (_selectedReservationId <= 0)
                {
                    MessageBox.Show(
                        "Please select a reservation first.",
                        "Update Reservation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }


                int guestId =
                    int.Parse(
                        txtGuestId.Text.Trim());


                // -------------------------------------------------
                // CREATE UPDATED RESERVATION
                // -------------------------------------------------

                var reservation = new Reservation
                {
                    ReservationId =
                        _selectedReservationId,

                    GuestId =
                        guestId,

                    RoomNumber =
                        txtRoomNumber.Text.Trim(),

                    CheckInDate =
                        dtpCheckIn.Value,

                    CheckOutDate =
                        dtpCheckOut.Value,

                    Status =
                        cmbStatus.SelectedItem?.ToString()
                        ?? "Pending"
                };


                // -------------------------------------------------
                // SEND UPDATE
                // -------------------------------------------------

                var response =
                    await _httpClient.PutAsJsonAsync(
                        $"{ApiUrl}/{_selectedReservationId}",
                        reservation);


                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Reservation updated successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadReservations();

                    return;
                }


                MessageBox.Show(
                    "Failed to update reservation.\n\n" +
                    await response.Content
                        .ReadAsStringAsync(),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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


        // =====================================================
        // DELETE RESERVATION
        // =====================================================

        private async void btnDelete_Click(
            object sender,
            EventArgs e)
        {
            // -------------------------------------------------
            // CHECK SELECTED RESERVATION
            // -------------------------------------------------

            if (_selectedReservationId <= 0)
            {
                MessageBox.Show(
                    "Please select a reservation first.",
                    "Delete Reservation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            try
            {
                var confirm =
                    MessageBox.Show(
                        "Are you sure you want to delete this reservation?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);


                if (confirm != DialogResult.Yes)
                    return;


                var response =
                    await _httpClient.DeleteAsync(
                        $"{ApiUrl}/{_selectedReservationId}");


                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Reservation deleted successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadReservations();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to delete reservation.\n\n" +
                        await response.Content
                            .ReadAsStringAsync(),
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


        // =====================================================
        // VALIDATE RESERVATION
        // =====================================================

        private bool ValidateReservationInformation()
        {
            // -------------------------------------------------
            // GUEST ID
            // -------------------------------------------------

            if (!int.TryParse(
                txtGuestId.Text.Trim(),
                out int guestId))
            {
                MessageBox.Show(
                    "Please enter a valid Guest ID.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtGuestId.Focus();

                return false;
            }


            if (guestId <= 0)
            {
                MessageBox.Show(
                    "Guest ID must be greater than zero.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtGuestId.Focus();

                return false;
            }


            // -------------------------------------------------
            // ROOM NUMBER
            // -------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                txtRoomNumber.Text))
            {
                MessageBox.Show(
                    "Please enter a room number.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtRoomNumber.Focus();

                return false;
            }


            // -------------------------------------------------
            // CHECK-IN / CHECK-OUT
            // -------------------------------------------------

            if (dtpCheckOut.Value <=
                dtpCheckIn.Value)
            {
                MessageBox.Show(
                    "Check-out date must be after check-in date.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                dtpCheckOut.Focus();

                return false;
            }


            // -------------------------------------------------
            // STATUS
            // -------------------------------------------------

            if (cmbStatus.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Please select a reservation status.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbStatus.Focus();

                return false;
            }


            return true;
        }


        // =====================================================
        // CLEAR FIELDS
        // =====================================================

        private void ClearFields()
        {
            // Reset selected ReservationId
            _selectedReservationId = 0;


            txtGuestId.Clear();

            txtRoomNumber.Clear();

            dtpCheckIn.Value =
                DateTime.Today;

            dtpCheckOut.Value =
                DateTime.Today.AddDays(1);

            cmbStatus.SelectedIndex = 0;

            dgvReservations.ClearSelection();

            txtGuestId.Focus();
        }
    }
}



