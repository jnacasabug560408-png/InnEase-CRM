#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class InquiryForm : Form
    {
        private readonly HttpClient _httpClient;
        private List<Inquiry> _inquiries = new List<Inquiry>();
        private List<Guest> _guests = new List<Guest>();
        private Inquiry? _selectedInquiry;

        private string InquiriesUrl => "https://localhost:7129/api/inquiries";
        private string GuestsUrl => "https://localhost:7129/api/guests";

        public InquiryForm()
        {
            _httpClient = ApiClient.Instance;
            InitializeComponent();
            Load += InquiryForm_Load;
        }

        private async void InquiryForm_Load(object sender, EventArgs e)
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                await LoadGuests();
                await LoadInquiries();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadGuests()
        {
            _guests = await _httpClient.GetFromJsonAsync<List<Guest>>(GuestsUrl) ?? new List<Guest>();
            cmbGuest.DataSource = _guests;
            cmbGuest.DisplayMember = "FullName";
            cmbGuest.ValueMember = "GuestId";
        }

        private async Task LoadInquiries()
        {
            _inquiries = await _httpClient.GetFromJsonAsync<List<Inquiry>>(InquiriesUrl) ?? new List<Inquiry>();
            BindInquiriesGrid();
        }

        private void BindInquiriesGrid()
        {
            dgvInquiries.DataSource = _inquiries
                .Select(i => new
                {
                    i.InquiryId,
                    GuestName = _guests.FirstOrDefault(g => g.GuestId == i.GuestId)?.FullName ?? "Unknown",
                    i.InquiryCategory,
                    i.ContactMethod,
                    i.Subject,
                    i.Status,
                    i.Priority,
                    i.InquiryDate,
                    i.AssignedTo
                })
                .ToList();
        }

        private void ClearForm()
        {
            cmbGuest.SelectedIndex = -1;
            txtCategory.Clear();
            txtContactMethod.Clear();
            txtSubject.Clear();
            txtDescription.Clear();
            cmbStatus.SelectedIndex = 0;
            cmbPriority.SelectedIndex = 1;
            txtAssignedTo.Clear();
            txtResolutionNotes.Clear();
            _selectedInquiry = null;
            btnSave.Text = "Add Inquiry";
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbGuest.SelectedValue == null)
            {
                MessageBox.Show("Please select a guest.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                MessageBox.Show("Subject is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Description is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var inquiry = new Inquiry
                {
                    GuestId = (int)cmbGuest.SelectedValue,
                    InquiryCategory = txtCategory.Text,
                    ContactMethod = txtContactMethod.Text,
                    Subject = txtSubject.Text,
                    Description = txtDescription.Text,
                    Status = cmbStatus.Text,
                    Priority = cmbPriority.Text,
                    AssignedTo = txtAssignedTo.Text,
                    ResolutionNotes = txtResolutionNotes.Text
                };

                if (_selectedInquiry != null)
                {
                    // Update existing
                    inquiry.InquiryId = _selectedInquiry.InquiryId;
                    inquiry.TenantId = _selectedInquiry.TenantId;
                    inquiry.BranchId = _selectedInquiry.BranchId;
                    inquiry.InquiryDate = _selectedInquiry.InquiryDate;
                    inquiry.CreatedAt = _selectedInquiry.CreatedAt;

                    var response = await _httpClient.PutAsJsonAsync($"{InquiriesUrl}/{inquiry.InquiryId}", inquiry);
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("Inquiry updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Create new
                    inquiry.TenantId = TenantSession.TenantId ?? 1;
                    inquiry.BranchId = TenantSession.BranchId;

                    var response = await _httpClient.PostAsJsonAsync(InquiriesUrl, inquiry);
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("Inquiry created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await LoadInquiries();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving inquiry: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedInquiry == null)
            {
                MessageBox.Show("Please select an inquiry to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to archive this inquiry?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"{InquiriesUrl}/{_selectedInquiry.InquiryId}");
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("Inquiry archived successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadInquiries();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error archiving inquiry: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvInquiries_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvInquiries.SelectedRows.Count > 0)
            {
                var selectedRow = dgvInquiries.SelectedRows[0];
                var inquiryId = (int)selectedRow.Cells["InquiryId"].Value;
                _selectedInquiry = _inquiries.FirstOrDefault(i => i.InquiryId == inquiryId);

                if (_selectedInquiry != null)
                {
                    cmbGuest.SelectedValue = _selectedInquiry.GuestId;
                    txtCategory.Text = _selectedInquiry.InquiryCategory;
                    txtContactMethod.Text = _selectedInquiry.ContactMethod;
                    txtSubject.Text = _selectedInquiry.Subject;
                    txtDescription.Text = _selectedInquiry.Description;
                    cmbStatus.Text = _selectedInquiry.Status;
                    cmbPriority.Text = _selectedInquiry.Priority;
                    txtAssignedTo.Text = _selectedInquiry.AssignedTo;
                    txtResolutionNotes.Text = _selectedInquiry.ResolutionNotes;
                    btnSave.Text = "Update Inquiry";
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void InitializeComponent()
        {
            this.Text = "Inquiry Management";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // Main layout
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Input panel
            var inputPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Grid panel
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };

            // Input fields
            var lblGuest = new Label { Text = "Guest:", Location = new Point(10, 15), Size = new Size(80, 20) };
            cmbGuest = new ComboBox { Location = new Point(100, 12), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            var lblCategory = new Label { Text = "Category:", Location = new Point(320, 15), Size = new Size(80, 20) };
            txtCategory = new TextBox { Location = new Point(400, 12), Size = new Size(150, 25) };

            var lblContactMethod = new Label { Text = "Contact Method:", Location = new Point(570, 15), Size = new Size(100, 20) };
            txtContactMethod = new TextBox { Location = new Point(680, 12), Size = new Size(150, 25) };

            var lblSubject = new Label { Text = "Subject:", Location = new Point(10, 50), Size = new Size(80, 20) };
            txtSubject = new TextBox { Location = new Point(100, 47), Size = new Size(300, 25) };

            var lblStatus = new Label { Text = "Status:", Location = new Point(420, 50), Size = new Size(80, 20) };
            cmbStatus = new ComboBox { Location = new Point(500, 47), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new[] { "open", "in_progress", "resolved", "closed" });
            cmbStatus.SelectedIndex = 0;

            var lblPriority = new Label { Text = "Priority:", Location = new Point(620, 50), Size = new Size(80, 20) };
            cmbPriority = new ComboBox { Location = new Point(700, 47), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPriority.Items.AddRange(new[] { "low", "normal", "high", "urgent" });
            cmbPriority.SelectedIndex = 1;

            var lblDescription = new Label { Text = "Description:", Location = new Point(10, 85), Size = new Size(80, 20) };
            txtDescription = new TextBox { Location = new Point(100, 82), Size = new Size(600, 60), Multiline = true, ScrollBars = ScrollBars.Vertical };

            var lblAssignedTo = new Label { Text = "Assigned To:", Location = new Point(10, 155), Size = new Size(80, 20) };
            txtAssignedTo = new TextBox { Location = new Point(100, 152), Size = new Size(200, 25) };

            var lblResolutionNotes = new Label { Text = "Resolution Notes:", Location = new Point(320, 155), Size = new Size(120, 20) };
            txtResolutionNotes = new TextBox { Location = new Point(450, 152), Size = new Size(300, 60), Multiline = true, ScrollBars = ScrollBars.Vertical };

            // Buttons
            btnSave = new Button { Text = "Add Inquiry", Location = new Point(10, 220), Size = new Size(100, 25), BackColor = Color.FromArgb(30, 136, 229), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete = new Button { Text = "Archive", Location = new Point(120, 220), Size = new Size(100, 25), BackColor = Color.FromArgb(214, 80, 110), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClear = new Button { Text = "Clear", Location = new Point(230, 220), Size = new Size(100, 25), BackColor = Color.FromArgb(112, 118, 142), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            // Data grid
            dgvInquiries = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White
            };

            // Add controls
            inputPanel.Controls.AddRange(new Control[] { lblGuest, cmbGuest, lblCategory, txtCategory, lblContactMethod, txtContactMethod, lblSubject, txtSubject, lblStatus, cmbStatus, lblPriority, cmbPriority, lblDescription, txtDescription, lblAssignedTo, txtAssignedTo, lblResolutionNotes, txtResolutionNotes, btnSave, btnDelete, btnClear });
            gridPanel.Controls.Add(dgvInquiries);
            mainPanel.Controls.AddRange(new Control[] { inputPanel, gridPanel });
            this.Controls.Add(mainPanel);

            // Event handlers
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;
            dgvInquiries.SelectionChanged += dgvInquiries_SelectionChanged;
        }

        // Control declarations
        private ComboBox cmbGuest;
        private TextBox txtCategory;
        private TextBox txtContactMethod;
        private TextBox txtSubject;
        private ComboBox cmbStatus;
        private ComboBox cmbPriority;
        private TextBox txtDescription;
        private TextBox txtAssignedTo;
        private TextBox txtResolutionNotes;
        private Button btnSave;
        private Button btnDelete;
        private Button btnClear;
        private DataGridView dgvInquiries;
    }
}