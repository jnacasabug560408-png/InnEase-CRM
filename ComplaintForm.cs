#nullable enable
#pragma warning disable CS8618
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
    public partial class ComplaintForm : Form
    {
        private readonly HttpClient _httpClient;
        private List<Complaint> _complaints = new List<Complaint>();
        private List<Guest> _guests = new List<Guest>();
        private Complaint? _selectedComplaint;

        private string ComplaintsUrl => "https://localhost:7129/api/complaints";
        private string GuestsUrl => "https://localhost:7129/api/guests";

        public ComplaintForm()
        {
            _httpClient = ApiClient.Instance;
            InitializeComponent();
            Load += ComplaintForm_Load;
        }

        private async void ComplaintForm_Load(object sender, EventArgs e)
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                await LoadGuests();
                await LoadComplaints();
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

        private async Task LoadComplaints()
        {
            _complaints = await _httpClient.GetFromJsonAsync<List<Complaint>>(ComplaintsUrl) ?? new List<Complaint>();
            BindComplaintsGrid();
        }

        private void BindComplaintsGrid()
        {
            dgvComplaints.DataSource = _complaints
                .Select(c => new
                {
                    c.ComplaintId,
                    GuestName = _guests.FirstOrDefault(g => g.GuestId == c.GuestId)?.FullName ?? "Unknown",
                    c.ComplaintCategory,
                    c.Severity,
                    c.Subject,
                    c.Status,
                    c.Priority,
                    c.StaffHandling,
                    c.EscalationLevel,
                    c.ComplaintDate
                })
                .ToList();
        }

        private void ClearForm()
        {
            cmbGuest.SelectedIndex = -1;
            txtCategory.Clear();
            txtSubject.Clear();
            txtDescription.Clear();
            cmbSeverity.SelectedIndex = 1;
            cmbStatus.SelectedIndex = 0;
            cmbPriority.SelectedIndex = 1;
            txtStaffHandling.Clear();
            txtResolutionDetails.Clear();
            cmbEscalationLevel.SelectedIndex = 0;
            _selectedComplaint = null;
            btnSave.Text = "Add Complaint";
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
                var complaint = new Complaint
                {
                    GuestId = (int)cmbGuest.SelectedValue,
                    ComplaintCategory = txtCategory.Text,
                    Severity = cmbSeverity.Text,
                    Subject = txtSubject.Text,
                    Description = txtDescription.Text,
                    Status = cmbStatus.Text,
                    Priority = cmbPriority.Text,
                    StaffHandling = txtStaffHandling.Text,
                    ResolutionDetails = txtResolutionDetails.Text,
                    EscalationLevel = cmbEscalationLevel.Text
                };

                if (_selectedComplaint != null)
                {
                    // Update existing
                    complaint.ComplaintId = _selectedComplaint.ComplaintId;
                    complaint.TenantId = _selectedComplaint.TenantId;
                    complaint.BranchId = _selectedComplaint.BranchId;
                    complaint.ComplaintDate = _selectedComplaint.ComplaintDate;
                    complaint.CreatedAt = _selectedComplaint.CreatedAt;

                    var response = await _httpClient.PutAsJsonAsync($"{ComplaintsUrl}/{complaint.ComplaintId}", complaint);
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("Complaint updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Create new
                    complaint.TenantId = TenantSession.TenantId ?? 1;
                    complaint.BranchId = TenantSession.BranchId;

                    var response = await _httpClient.PostAsJsonAsync(ComplaintsUrl, complaint);
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("Complaint created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await LoadComplaints();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving complaint: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedComplaint == null)
            {
                MessageBox.Show("Please select a complaint to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to archive this complaint?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"{ComplaintsUrl}/{_selectedComplaint.ComplaintId}");
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("Complaint archived successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadComplaints();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error archiving complaint: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvComplaints_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvComplaints.SelectedRows.Count > 0)
            {
                var selectedRow = dgvComplaints.SelectedRows[0];
                var complaintId = (int)selectedRow.Cells["ComplaintId"].Value;
                _selectedComplaint = _complaints.FirstOrDefault(c => c.ComplaintId == complaintId);

                if (_selectedComplaint != null)
                {
                    cmbGuest.SelectedValue = _selectedComplaint.GuestId;
                    txtCategory.Text = _selectedComplaint.ComplaintCategory;
                    txtSubject.Text = _selectedComplaint.Subject;
                    txtDescription.Text = _selectedComplaint.Description;
                    cmbSeverity.Text = _selectedComplaint.Severity;
                    cmbStatus.Text = _selectedComplaint.Status;
                    cmbPriority.Text = _selectedComplaint.Priority;
                    txtStaffHandling.Text = _selectedComplaint.StaffHandling;
                    txtResolutionDetails.Text = _selectedComplaint.ResolutionDetails;
                    cmbEscalationLevel.Text = _selectedComplaint.EscalationLevel;
                    btnSave.Text = "Update Complaint";
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void InitializeComponent()
        {
            this.Text = "Complaint Management";
            this.Size = new Size(1100, 650);
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
                Height = 280,
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

            var lblSeverity = new Label { Text = "Severity:", Location = new Point(570, 15), Size = new Size(80, 20) };
            cmbSeverity = new ComboBox { Location = new Point(650, 12), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSeverity.Items.AddRange(new[] { "low", "medium", "high", "critical" });
            cmbSeverity.SelectedIndex = 1;

            var lblSubject = new Label { Text = "Subject:", Location = new Point(10, 50), Size = new Size(80, 20) };
            txtSubject = new TextBox { Location = new Point(100, 47), Size = new Size(300, 25) };

            var lblStatus = new Label { Text = "Status:", Location = new Point(420, 50), Size = new Size(80, 20) };
            cmbStatus = new ComboBox { Location = new Point(500, 47), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new[] { "open", "in_progress", "resolved", "escalated", "closed" });
            cmbStatus.SelectedIndex = 0;

            var lblPriority = new Label { Text = "Priority:", Location = new Point(620, 50), Size = new Size(80, 20) };
            cmbPriority = new ComboBox { Location = new Point(700, 47), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPriority.Items.AddRange(new[] { "low", "normal", "high", "urgent" });
            cmbPriority.SelectedIndex = 1;

            var lblDescription = new Label { Text = "Description:", Location = new Point(10, 85), Size = new Size(80, 20) };
            txtDescription = new TextBox { Location = new Point(100, 82), Size = new Size(400, 60), Multiline = true, ScrollBars = ScrollBars.Vertical };

            var lblStaffHandling = new Label { Text = "Staff Handling:", Location = new Point(520, 85), Size = new Size(100, 20) };
            txtStaffHandling = new TextBox { Location = new Point(620, 82), Size = new Size(150, 25) };

            var lblResolutionDetails = new Label { Text = "Resolution Details:", Location = new Point(10, 155), Size = new Size(120, 20) };
            txtResolutionDetails = new TextBox { Location = new Point(140, 152), Size = new Size(400, 60), Multiline = true, ScrollBars = ScrollBars.Vertical };

            var lblEscalationLevel = new Label { Text = "Escalation Level:", Location = new Point(560, 155), Size = new Size(120, 20) };
            cmbEscalationLevel = new ComboBox { Location = new Point(680, 152), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbEscalationLevel.Items.AddRange(new[] { "none", "supervisor", "manager", "director" });
            cmbEscalationLevel.SelectedIndex = 0;

            // Buttons
            btnSave = new Button { Text = "Add Complaint", Location = new Point(10, 250), Size = new Size(120, 25), BackColor = Color.FromArgb(30, 136, 229), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete = new Button { Text = "Archive", Location = new Point(140, 250), Size = new Size(100, 25), BackColor = Color.FromArgb(214, 80, 110), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClear = new Button { Text = "Clear", Location = new Point(250, 250), Size = new Size(100, 25), BackColor = Color.FromArgb(112, 118, 142), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            // Data grid
            dgvComplaints = new DataGridView
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
            inputPanel.Controls.AddRange(new Control[] { lblGuest, cmbGuest, lblCategory, txtCategory, lblSeverity, cmbSeverity, lblSubject, txtSubject, lblStatus, cmbStatus, lblPriority, cmbPriority, lblDescription, txtDescription, lblStaffHandling, txtStaffHandling, lblResolutionDetails, txtResolutionDetails, lblEscalationLevel, cmbEscalationLevel, btnSave, btnDelete, btnClear });
            gridPanel.Controls.Add(dgvComplaints);
            mainPanel.Controls.AddRange(new Control[] { inputPanel, gridPanel });
            this.Controls.Add(mainPanel);

            // Event handlers
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;
            dgvComplaints.SelectionChanged += dgvComplaints_SelectionChanged;
        }

        // Control declarations
        private ComboBox cmbGuest;
        private TextBox txtCategory;
        private TextBox txtSubject;
        private TextBox txtDescription;
        private ComboBox cmbSeverity;
        private ComboBox cmbStatus;
        private ComboBox cmbPriority;
        private TextBox txtStaffHandling;
        private TextBox txtResolutionDetails;
        private ComboBox cmbEscalationLevel;
        private Button btnSave;
        private Button btnDelete;
        private Button btnClear;
        private DataGridView dgvComplaints;
    }
}