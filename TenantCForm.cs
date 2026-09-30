using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class TenantCForm : Form
    {
        private readonly HttpClient _httpClient;
        private List<Reservation> _reservations = new List<Reservation>();
        private List<Guest> _guests = new List<Guest>();
        private List<GuestFeedback> _feedback = new List<GuestFeedback>();
        private List<InnEaseCRM.domain.entities.Action> _actions = new List<InnEaseCRM.domain.entities.Action>();
        private List<Branch> _branches = new List<Branch>();

        private string ReservationsUrl =>
            $"https://localhost:7129/api/reservations";
        private string GuestsUrl =>
            $"https://localhost:7129/api/guests";
        private string FeedbackUrl =>
            $"https://localhost:7129/api/guestfeedback";
        private string ActionsUrl =>
            $"https://localhost:7129/api/actions";
        private string BranchesUrl =>
            $"https://localhost:7129/api/branches";

        public TenantCForm()
        {
            InitializeComponent();
            _httpClient = ApiClient.Instance;
            BuildTenantCInterface();
            Load += TenantCForm_Load;
            btnRefresh.Click += btnRefresh_Click;
            cmbBranchFilter.SelectedIndexChanged += cmbBranchFilter_SelectedIndexChanged;
        }

        private async void TenantCForm_Load(object sender, EventArgs e)
        {
            await LoadBranches();
            await LoadTenantCData();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadTenantCData();
        }

        private async void cmbBranchFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            await LoadTenantCData();
        }

        private async Task LoadBranches()
        {
            try
            {
                _branches = await _httpClient.GetFromJsonAsync<List<Branch>>(BranchesUrl)
                    ?? new List<Branch>();

                cmbBranchFilter.Items.Clear();
                cmbBranchFilter.Items.Add("All Branches");
                
                foreach (var branch in _branches)
                {
                    cmbBranchFilter.Items.Add($"{branch.Name} ({branch.Code})");
                }

                cmbBranchFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to load branches.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task LoadTenantCData()
        {
            try
            {
                var reservationsTask = _httpClient.GetFromJsonAsync<List<Reservation>>(ReservationsUrl);
                var guestsTask = _httpClient.GetFromJsonAsync<List<Guest>>(GuestsUrl);
                var feedbackTask = _httpClient.GetFromJsonAsync<List<GuestFeedback>>(FeedbackUrl);
                var actionsTask = _httpClient.GetFromJsonAsync<List<InnEaseCRM.domain.entities.Action>>(ActionsUrl);

                await Task.WhenAll(reservationsTask, guestsTask, feedbackTask, actionsTask);

                _reservations = reservationsTask.Result ?? new List<Reservation>();
                _guests = guestsTask.Result ?? new List<Guest>();
                _feedback = feedbackTask.Result ?? new List<GuestFeedback>();
                _actions = actionsTask.Result ?? new List<InnEaseCRM.domain.entities.Action>();

                // Apply branch filter
                ApplyBranchFilter();

                UpdateTenantCDashboard();
                UpdateActionsBoard();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to load Tenant C data.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ApplyBranchFilter()
        {
            if (cmbBranchFilter.SelectedIndex <= 0) // "All Branches" selected
                return;

            string selectedBranch = cmbBranchFilter.SelectedItem.ToString();
            string branchCode = selectedBranch.Contains("(") 
                ? selectedBranch.Split('(')[1].Trim(')')
                : selectedBranch;

            var selectedBranchData = _branches.FirstOrDefault(b => b.Code == branchCode);
            if (selectedBranchData != null)
            {
                _reservations = _reservations.Where(r => r.BranchId == selectedBranchData.BranchId).ToList();
                _guests = _guests.Where(g => g.BranchId == selectedBranchData.BranchId).ToList();
                _feedback = _feedback.Where(f => f.BranchId == selectedBranchData.BranchId).ToList();
                _actions = _actions.Where(a => a.BranchId == selectedBranchData.BranchId).ToList();
            }
        }

        private void BuildTenantCInterface()
        {
            this.Text = "Tenant C - Multi-Branch Business Intelligence";
            this.BackColor = Color.FromArgb(244, 245, 252);
            this.Font = new Font("Segoe UI", 9F);

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(244, 245, 252)
            };

            // Header
            var headerLabel = new Label
            {
                Text = "Tenant C - Multi-Branch Business Intelligence",
                Font = new Font("Segoe UI Semibold", 18F),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var subLabel = new Label
            {
                Text = "Coral Group of Hotels - Full BI dashboard with multi-branch support",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(112, 118, 142),
                Location = new Point(20, 55),
                AutoSize = true
            };

            // Branch Filter
            var branchFilterLabel = new Label
            {
                Text = "Branch Filter:",
                Location = new Point(600, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(30, 34, 58)
            };

            cmbBranchFilter = new ComboBox
            {
                Location = new Point(700, 17),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnRefresh = new Button
            {
                Text = "Refresh Data",
                Location = new Point(920, 17),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(146, 30, 98),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // KPI Cards Section
            var kpiSection = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(1100, 180),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var kpiTitle = new Label
            {
                Text = "Key Performance Indicators",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(15, 15),
                AutoSize = true
            };

            kpiTotalReservations = CreateKpiLabel("Total Reservations", "0", Color.FromArgb(30, 136, 229), 15, 50);
            kpiActiveGuests = CreateKpiLabel("Active Guests", "0", Color.FromArgb(34, 160, 90), 280, 50);
            kpiOccupancyRate = CreateKpiLabel("Occupancy Rate", "0%", Color.FromArgb(226, 164, 52), 545, 50);
            kpiRevenue = CreateKpiLabel("Total Revenue", "₱0", Color.FromArgb(146, 30, 98), 810, 50);

            kpiSection.Controls.Add(kpiTitle);
            kpiSection.Controls.Add(kpiTotalReservations);
            kpiSection.Controls.Add(kpiActiveGuests);
            kpiSection.Controls.Add(kpiOccupancyRate);
            kpiSection.Controls.Add(kpiRevenue);

            // Analytics Charts Section
            var analyticsSection = new Panel
            {
                Location = new Point(20, 300),
                Size = new Size(1100, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var analyticsTitle = new Label
            {
                Text = "Analytics Overview",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(15, 15),
                AutoSize = true
            };

            // Simple chart representation
            var chartPanel = new Panel
            {
                Location = new Point(15, 50),
                Size = new Size(1070, 130),
                BackColor = Color.FromArgb(248, 250, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var chartLabel = new Label
            {
                Text = "Occupancy Trends and Feedback Analytics",
                Location = new Point(10, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(112, 118, 142)
            };

            var placeholderLabel = new Label
            {
                Text = "Charts will display occupancy trends, guest reservation statuses, and feedback analytics for the selected branch.",
                Location = new Point(10, 40),
                Size = new Size(1050, 80),
                TextAlign = ContentAlignment.TopLeft,
                ForeColor = Color.FromArgb(30, 34, 58)
            };

            chartPanel.Controls.Add(chartLabel);
            chartPanel.Controls.Add(placeholderLabel);

            analyticsSection.Controls.Add(analyticsTitle);
            analyticsSection.Controls.Add(chartPanel);

            // Actions Board Section
            var actionsSection = new Panel
            {
                Location = new Point(20, 520),
                Size = new Size(1100, 250),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var actionsTitle = new Label
            {
                Text = "Operational Task List",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(15, 15),
                AutoSize = true
            };

            var actionsSubLabel = new Label
            {
                Text = "Log, assign, and update actions for feedback follow-ups, room maintenance, and operational tasks",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(112, 118, 142),
                Location = new Point(15, 40),
                AutoSize = true
            };

            dgvActions = new DataGridView
            {
                Location = new Point(15, 70),
                Size = new Size(1070, 160),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            actionsSection.Controls.Add(actionsTitle);
            actionsSection.Controls.Add(actionsSubLabel);
            actionsSection.Controls.Add(dgvActions);

            mainPanel.Controls.Add(headerLabel);
            mainPanel.Controls.Add(subLabel);
            mainPanel.Controls.Add(branchFilterLabel);
            mainPanel.Controls.Add(cmbBranchFilter);
            mainPanel.Controls.Add(btnRefresh);
            mainPanel.Controls.Add(kpiSection);
            mainPanel.Controls.Add(analyticsSection);
            mainPanel.Controls.Add(actionsSection);

            this.Controls.Add(mainPanel);
        }

        private Panel CreateKpiLabel(string title, string value, Color color, int x, int y)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(250, 100),
                BackColor = Color.FromArgb(248, 250, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(112, 118, 142),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(10, 35),
                AutoSize = true
            };

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(valueLabel);

            return panel;
        }

        private void UpdateTenantCDashboard()
        {
            int totalReservations = _reservations.Count;
            int activeGuests = _guests.Count;
            
            // Calculate occupancy rate
            int totalRooms = 150; // Assuming 150 rooms across all branches
            int occupiedRooms = _reservations.Count(r => 
                string.Equals(r.Status, "CheckedIn", StringComparison.OrdinalIgnoreCase));
            double occupancyRate = totalRooms > 0 ? (double)occupiedRooms / totalRooms * 100 : 0;

            // Calculate total revenue
            decimal totalRevenue = _reservations
                .Where(r => !string.Equals(r.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                .Sum(r => r.TotalAmount);

            // Update KPI labels
            var valueLabel = kpiTotalReservations.Controls[1] as Label;
            valueLabel.Text = totalReservations.ToString("N0");

            valueLabel = kpiActiveGuests.Controls[1] as Label;
            valueLabel.Text = activeGuests.ToString("N0");

            valueLabel = kpiOccupancyRate.Controls[1] as Label;
            valueLabel.Text = $"{occupancyRate:F1}%";

            valueLabel = kpiRevenue.Controls[1] as Label;
            valueLabel.Text = $"₱{totalRevenue:N2}";
        }

        private void UpdateActionsBoard()
        {
            var actionData = _actions
                .Select(a => new
                {
                    ActionId = a.ActionId,
                    Title = a.Title,
                    Type = a.Type,
                    Priority = a.Priority,
                    Status = a.Status,
                    DueDate = a.DueDate?.ToString("MMM dd, yyyy") ?? "No due date",
                    AssignedTo = a.AssignedTo?.ToString() ?? "Unassigned"
                })
                .ToList();

            dgvActions.DataSource = actionData;
        }

        // Control declarations
        private Button btnRefresh;
        private ComboBox cmbBranchFilter;
        private Panel kpiTotalReservations;
        private Panel kpiActiveGuests;
        private Panel kpiOccupancyRate;
        private Panel kpiRevenue;
        private DataGridView dgvActions;
    }
}
