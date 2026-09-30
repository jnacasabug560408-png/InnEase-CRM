using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _httpClient;


        // =====================================================
        // TENANT API URL
        // =====================================================

        private string ApiUrl =>
            $"https://localhost:7129/api/guests";


        // =====================================================
        // DATA GRID ROW
        // =====================================================

        private class GuestRow
        {
            public int GuestId { get; set; }

            public string FullName { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            public string Phone { get; set; } = string.Empty;

            public string Address { get; set; } = string.Empty;
        }


        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public Form1()
        {
            InitializeComponent();

            _httpClient = ApiClient.Instance;

            this.Load += Form1_Load;

            dgvGuests.CellClick += dgvGuests_CellClick;

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;

            // Input validation
            txtFullName.KeyPress += txtFullName_KeyPress;
            txtPhone.KeyPress += txtPhone_KeyPress;
        }


        // =====================================================
        // FORM LOAD
        // =====================================================

        private async void Form1_Load(
            object sender,
            EventArgs e)
        {
            await LoadGuests();
        }


        // =====================================================
        // LOAD GUESTS
        // =====================================================

        private async Task LoadGuests()
        {
            try
            {
                var guests =
                    await _httpClient.GetFromJsonAsync<List<Guest>>(
                        ApiUrl);

                var rows = guests?
                    .Select(g => new GuestRow
                    {
                        GuestId =
                            g.GuestId,

                        FullName =
                            $"{g.FirstName} {g.LastName}".Trim(),

                        Email =
                            g.Email,

                        Phone =
                            g.ContactNumber,

                        Address =
                            g.Address
                    })
                    .ToList()
                    ?? new List<GuestRow>();


                dgvGuests.DataSource = rows;


                // =================================================
                // GRID COLUMN SETTINGS
                // =================================================

                if (dgvGuests.Columns.Count > 0)
                {
                    dgvGuests.Columns["GuestId"].HeaderText =
                        "Guest ID";

                    dgvGuests.Columns["FullName"].HeaderText =
                        "Full Name";

                    dgvGuests.Columns["Email"].HeaderText =
                        "Email";

                    dgvGuests.Columns["Phone"].HeaderText =
                        "Phone";

                    dgvGuests.Columns["Address"].HeaderText =
                        "Address";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load guests.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // =====================================================
        // ADD GUEST
        // =====================================================

        private async void btnAdd_Click(
            object sender,
            EventArgs e)
        {
            if (!ValidateGuestInformation())
                return;


            try
            {
                // -------------------------------------------------
                // GET NAME
                // -------------------------------------------------

                var nameParts =
                    NormalizeFullName(
                        txtFullName.Text)
                    .Split(
                     ' ',
                    StringSplitOptions.RemoveEmptyEntries);

                var firstName =
                    string.Join(
                        " ",
                        nameParts.Take(nameParts.Length - 1));

                var lastName =
                    nameParts.Last();

                // -------------------------------------------------
                // CHECK DUPLICATE IN CURRENT GRID
                // -------------------------------------------------

                if (GuestNameAlreadyExists(
                    firstName,
                    lastName))
                {
                    MessageBox.Show(
                        $"A guest with the name '{firstName} {lastName}' already exists.\n\n" +
                        "Please enter a different guest name.",
                        "Duplicate Guest",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtFullName.Focus();

                    return;
                }


                // -------------------------------------------------
                // CREATE GUEST
                // -------------------------------------------------

                var guest = new Guest
                {
                    FirstName =
                        firstName,

                    LastName =
                        lastName,

                    ContactNumber =
                        txtPhone.Text.Trim(),

                    Email =
                        txtEmail.Text.Trim(),

                    Address =
                        txtAddress.Text.Trim(),

                    IsActive = true
                };


                // -------------------------------------------------
                // SEND TO API
                // -------------------------------------------------

                var response =
                    await _httpClient.PostAsJsonAsync(
                        ApiUrl,
                        guest);


                // -------------------------------------------------
                // SUCCESS
                // -------------------------------------------------

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Guest added successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadGuests();

                    return;
                }


                // -------------------------------------------------
                // DUPLICATE FROM API
                // -------------------------------------------------

                if (response.StatusCode ==
                    System.Net.HttpStatusCode.Conflict)
                {
                    var error =
                        await response.Content
                            .ReadAsStringAsync();

                    MessageBox.Show(
                        "This guest already exists.\n\n" +
                        "The same first name and surname cannot be added twice.",
                        "Duplicate Guest",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtFullName.Focus();

                    return;
                }


                // -------------------------------------------------
                // OTHER ERROR
                // -------------------------------------------------

                MessageBox.Show(
                    "Failed to add guest.\n\n" +
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
        // SELECT GUEST FROM GRID
        // =====================================================

        private void dgvGuests_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;


            var row =
                dgvGuests.Rows[e.RowIndex];


            txtFullName.Text =
                row.Cells["FullName"]
                    .Value?
                    .ToString()
                ?? string.Empty;


            txtEmail.Text =
                row.Cells["Email"]
                    .Value?
                    .ToString()
                ?? string.Empty;


            txtPhone.Text =
                row.Cells["Phone"]
                    .Value?
                    .ToString()
                ?? string.Empty;


            txtAddress.Text =
                row.Cells["Address"]
                    .Value?
                    .ToString()
                ?? string.Empty;
        }


        // =====================================================
        // UPDATE GUEST
        // =====================================================

        private async void btnUpdate_Click(
            object sender,
            EventArgs e)
        {
            if (dgvGuests.CurrentRow == null)
            {
                MessageBox.Show(
                    "Please select a guest first.",
                    "Update Guest",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            if (!ValidateGuestInformation())
                return;


            try
            {
                // -------------------------------------------------
                // GET CURRENT GUEST ID
                // -------------------------------------------------

                int guestId =
                    Convert.ToInt32(
                        dgvGuests.CurrentRow
                            .Cells["GuestId"]
                            .Value);


                // -------------------------------------------------
                // GET NAME
                // -------------------------------------------------

                var nameParts =
                    NormalizeFullName(
                        txtFullName.Text)
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries);

                var firstName =
                    string.Join(
                        " ",
                        nameParts.Take(nameParts.Length - 1));

                var lastName =
                    nameParts.Last();


                // -------------------------------------------------
                // CHECK DUPLICATE IN CURRENT GRID
                //
                // Ignore the SAME selected guest.
                // -------------------------------------------------

                if (GuestNameAlreadyExists(
                    firstName,
                    lastName,
                    guestId))
                {
                    MessageBox.Show(
                        $"A guest with the name '{firstName} {lastName}' already exists.\n\n" +
                        "Please enter a different guest name.",
                        "Duplicate Guest",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtFullName.Focus();

                    return;
                }


                // -------------------------------------------------
                // CREATE UPDATED GUEST
                // -------------------------------------------------

                var guest = new Guest
                {
                    GuestId =
                        guestId,

                    FirstName =
                        firstName,

                    LastName =
                        lastName,

                    ContactNumber =
                        txtPhone.Text.Trim(),

                    Email =
                        txtEmail.Text.Trim(),

                    Address =
                        txtAddress.Text.Trim(),

                    IsActive = true
                };


                // -------------------------------------------------
                // SEND UPDATE TO API
                // -------------------------------------------------

                var response =
                    await _httpClient.PutAsJsonAsync(
                        $"{ApiUrl}/{guestId}",
                        guest);


                // -------------------------------------------------
                // SUCCESS
                // -------------------------------------------------

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Guest updated successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    dgvGuests.ClearSelection();

                    await LoadGuests();

                    return;
                }


                // -------------------------------------------------
                // DUPLICATE FROM API
                // -------------------------------------------------

                if (response.StatusCode ==
                    System.Net.HttpStatusCode.Conflict)
                {
                    MessageBox.Show(
                        "This guest name already exists.\n\n" +
                        "The same first name and surname cannot be used for another guest.",
                        "Duplicate Guest",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtFullName.Focus();

                    return;
                }


                // -------------------------------------------------
                // OTHER ERROR
                // -------------------------------------------------

                MessageBox.Show(
                    "Failed to update guest.\n\n" +
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
        // DELETE GUEST
        // =====================================================

        private async void btnDelete_Click(
            object sender,
            EventArgs e)
        {
            if (dgvGuests.CurrentRow == null)
            {
                MessageBox.Show(
                    "Please select a guest first.",
                    "Delete Guest",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            try
            {
                int guestId =
                    Convert.ToInt32(
                        dgvGuests.CurrentRow
                            .Cells["GuestId"]
                            .Value);


                string guestName =
                    dgvGuests.CurrentRow
                        .Cells["FullName"]
                        .Value?
                        .ToString()
                        ?? "this guest";


                var confirm =
                    MessageBox.Show(
                        $"Are you sure you want to delete {guestName}?\n\n" +
                        "The guest will be moved to the archive.",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);


                if (confirm != DialogResult.Yes)
                    return;


                // -------------------------------------------------
                // DELETE / ARCHIVE
                // -------------------------------------------------

                var response =
                    await _httpClient.DeleteAsync(
                        $"{ApiUrl}/{guestId}");


                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Guest deleted and archived successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadGuests();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to delete guest.\n\n" +
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
        // CHECK DUPLICATE GUEST NAME
        // =====================================================
        //
        // This checks the guests currently displayed
        // in the DataGridView.
        //
        // Example:
        //
        // Juan Dela Cruz
        // juan dela cruz
        // Juan  Dela   Cruz
        //
        // are treated as the same name.
        //
        // =====================================================

        private bool GuestNameAlreadyExists(
            string firstName,
            string lastName,
            int? ignoreGuestId = null)
        {
            string newName =
                NormalizeNameForComparison(
                    $"{firstName} {lastName}");


            foreach (DataGridViewRow row
                in dgvGuests.Rows)
            {
                if (row.IsNewRow)
                    continue;


                int existingGuestId =
                    Convert.ToInt32(
                        row.Cells["GuestId"]
                            .Value);


                // Ignore the guest currently being updated
                if (ignoreGuestId.HasValue &&
                    existingGuestId ==
                    ignoreGuestId.Value)
                {
                    continue;
                }


                string existingName =
                    row.Cells["FullName"]
                        .Value?
                        .ToString()
                    ?? string.Empty;


                string normalizedExistingName =
                    NormalizeNameForComparison(
                        existingName);


                if (normalizedExistingName ==
                    newName)
                {
                    return true;
                }
            }


            return false;
        }


        // =====================================================
        // NORMALIZE FULL NAME
        // =====================================================
        //
        // Removes unnecessary spaces.
        //
        // "  Juan   Dela   Cruz  "
        //
        // becomes:
        //
        // "Juan Dela Cruz"
        //
        // =====================================================

        private string NormalizeFullName(
            string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;


            return string.Join(
                " ",
                name
                    .Trim()
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries));
        }


        // =====================================================
        // NORMALIZE NAME FOR COMPARISON
        // =====================================================
        //
        // Case-insensitive comparison.
        //
        // Juan Dela Cruz
        // JUAN DELA CRUZ
        // juan dela cruz
        //
        // are treated as the same.
        //
        // =====================================================

        private string NormalizeNameForComparison(
            string name)
        {
            return NormalizeFullName(name)
                .ToLowerInvariant();
        }


        // =====================================================
        // VALIDATE ALL GUEST INFORMATION
        // =====================================================

        private bool ValidateGuestInformation()
        {
            // -------------------------------------------------
            // FULL NAME
            // -------------------------------------------------

            string fullName =
                txtFullName.Text.Trim();


            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show(
                    "Please enter the guest's full name.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtFullName.Focus();

                return false;
            }


            // Only letters and spaces
            if (!Regex.IsMatch(
                fullName,
                @"^[A-Za-zÀ-ÿ\s]+$"))
            {
                MessageBox.Show(
                    "Full name should contain letters and spaces only.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtFullName.Focus();

                return false;
            }


            // Require first name AND surname
            string[] nameParts =
                fullName.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);


            if (nameParts.Length < 2)
            {
                MessageBox.Show(
                    "Please enter both the first name and surname.\n\n" +
                    "Example: Jillian Nacasabug",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtFullName.Focus();

                return false;
            }


            // -------------------------------------------------
            // EMAIL
            // -------------------------------------------------

            string email =
                txtEmail.Text.Trim();


            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show(
                    "Please enter the guest's email address.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtEmail.Focus();

                return false;
            }


            if (!IsValidEmail(email))
            {
                MessageBox.Show(
                    "Please enter a valid email address.\n\n" +
                    "Example: name@gmail.com",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtEmail.Focus();

                return false;
            }


            // -------------------------------------------------
            // PHONE
            // -------------------------------------------------

            string phone =
                txtPhone.Text.Trim();


            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show(
                    "Please enter the guest's phone number.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPhone.Focus();

                return false;
            }


            if (!phone.All(char.IsDigit))
            {
                MessageBox.Show(
                    "Phone number should contain numbers only.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPhone.Focus();

                return false;
            }


            if (phone.Length < 10 ||
                phone.Length > 15)
            {
                MessageBox.Show(
                    "Please enter a valid phone number.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPhone.Focus();

                return false;
            }


            // -------------------------------------------------
            // ADDRESS
            // -------------------------------------------------

            string address =
                txtAddress.Text.Trim();


            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show(
                    "Please enter the guest's complete address.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtAddress.Focus();

                return false;
            }


            // Require at least 10 characters
            if (address.Length < 10)
            {
                MessageBox.Show(
                    "Please enter a specific and complete address.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtAddress.Focus();

                return false;
            }


            // Address must contain letters
            if (!Regex.IsMatch(
                address,
                @"[A-Za-zÀ-ÿ]"))
            {
                MessageBox.Show(
                    "Please enter a valid address containing letters.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtAddress.Focus();

                return false;
            }


            // Require at least two separate parts
            string[] addressParts =
                address.Split(
                    new char[] { ',', ' ' },
                    StringSplitOptions.RemoveEmptyEntries);


            if (addressParts.Length < 2)
            {
                MessageBox.Show(
                    "Please enter a specific and complete address.\n\n" +
                    "Example: Purok 5, Barangay Tacunan, Davao City",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtAddress.Focus();

                return false;
            }


            // Address should contain at least letters
            if (!Regex.IsMatch(
                address,
                @"[A-Za-zÀ-ÿ]"))
            {
                MessageBox.Show(
                    "Please enter a valid address.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtAddress.Focus();

                return false;
            }


            return true;
        }


        // =====================================================
        // EMAIL VALIDATION
        // =====================================================

        private bool IsValidEmail(
            string email)
        {
            try
            {
                var match =
                    Regex.Match(
                        email,
                        @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                return match.Success;
            }
            catch
            {
                return false;
            }
        }


        // =====================================================
        // FULL NAME - LETTERS ONLY
        // =====================================================

        private void txtFullName_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;


            if (char.IsLetter(e.KeyChar) ||
                char.IsWhiteSpace(e.KeyChar))
            {
                return;
            }


            e.Handled = true;

            System.Media.SystemSounds.Beep.Play();
        }


        // =====================================================
        // PHONE - NUMBERS ONLY
        // =====================================================

        private void txtPhone_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;


            if (char.IsDigit(e.KeyChar))
                return;


            e.Handled = true;

            System.Media.SystemSounds.Beep.Play();
        }


        // =====================================================
        // CLEAR INPUT FIELDS
        // =====================================================

        private void ClearFields()
        {
            txtFullName.Clear();

            txtEmail.Clear();

            txtPhone.Clear();

            txtAddress.Clear();

            dgvGuests.ClearSelection();

            txtFullName.Focus();
        }
    }
}

