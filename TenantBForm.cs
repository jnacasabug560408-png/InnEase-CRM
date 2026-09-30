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
    public partial class TenantBForm : Form
    {
        private readonly HttpClient _httpClient;
        private List<Reservation> _reservations = new List<Reservation>();
        private List<Guest> _guests = new List<Guest>();
        private List<GuestFeedback> _feedback = new List<GuestFeedback>();
        private List<InnEaseCRM.domain.entities.Action> _actions = new List<InnEaseCRM.domain.entities.Action>();
        private List<Promotion> _promotions = new List<Promotion>();

        private string ReservationsUrl => $"https://localhost:7129/api/reservations";
        private string GuestsUrl => $"https://localhost:7129/api/guests";
        private string FeedbackUrl => $"https://localhost:7129/api/guestfeedback";
        private string ActionsUrl => $"https://localhost:7129/api/actions";
        private string PromotionsUrl => $"https://localhost:7129/api/promotions";
        private string BiKpiUrl => $"https://localhost:7129/api/businessintelligence/kpi";
        private string BiGraphUrl => $"https://localhost:7129/api/businessintelligence/graph";
        private string BiReportsUrl => $"https://localhost:7129/api/businessintelligence/reports";
        private string RetentionUrl => $"https://localhost:7129/api/customer-retention";

        public TenantBForm()
        {
            _httpClient = ApiClient.Instance;
            BuildTenantBInterface();
            Load += TenantBForm_Load;
            btnRefresh.Click += btnRefresh_Click;
        }

        private async void btnGenerateOffers_Click(object sender, EventArgs e)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Dictionary<string, object>>($"{RetentionUrl}/offers");
                if (response != null)
                {
                    MessageBox.Show($"Generated {response.GetValueOrDefault("TotalOffers", 0)} retention offers for {response.GetValueOrDefault("InactiveGuests", 0)} inactive guests.", "Retention Offers", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating retention offers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAnalyzeChurn_Click(object sender, EventArgs e)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Dictionary<string, object>>($"{RetentionUrl}/churn-analysis");
                if (response != null)
                {
                    var highRisk = response.GetValueOrDefault("HighRiskGuests", 0);
                    var mediumRisk = response.GetValueOrDefault("MediumRiskGuests", 0);
                    var lowRisk = response.GetValueOrDefault("LowRiskGuests", 0);
                    MessageBox.Show($"Churn Risk Analysis:\nHigh Risk: {highRisk}\nMedium Risk: {mediumRisk}\nLow Risk: {lowRisk}", "Churn Analysis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error analyzing churn risk: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void TenantBForm_Load(object sender, EventArgs e)
        {
            await LoadTenantBData();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadTenantBData();
        }

        private async Task LoadTenantBData()
        {
            try
            {
                var reservationsTask = _httpClient.GetFromJsonAsync<List<Reservation>>(ReservationsUrl);
                var guestsTask = _httpClient.GetFromJsonAsync<List<Guest>>(GuestsUrl);
                var feedbackTask = _httpClient.GetFromJsonAsync<List<GuestFeedback>>(FeedbackUrl);
                var actionsTask = _httpClient.GetFromJsonAsync<List<InnEaseCRM.domain.entities.Action>>(ActionsUrl);
                var promotionsTask = _httpClient.GetFromJsonAsync<List<Promotion>>(PromotionsUrl);
                
                // Try to load BI data, but continue if it fails
                Dictionary<string, decimal> kpiMetrics = new Dictionary<string, decimal>();
                Dictionary<string, object> loyaltyStatus = new Dictionary<string, object>();
                
                try
                {
                    var kpiTask = _httpClient.GetFromJsonAsync<Dictionary<string, decimal>>(BiKpiUrl);
                    var retentionTask = _httpClient.GetFromJsonAsync<Dictionary<string, object>>($"{RetentionUrl}/loyalty-status");
                    
                    await Task.WhenAll(kpiTask, retentionTask);
                    
                    kpiMetrics = kpiTask.Result ?? new Dictionary<string, decimal>();
                    loyaltyStatus = retentionTask.Result ?? new Dictionary<string, object>();
                }
                catch
                {
                    // BI service might not be available, continue with basic data
                }

                await Task.WhenAll(reservationsTask, guestsTask, feedbackTask, actionsTask, promotionsTask);

                _reservations = reservationsTask.Result ?? new List<Reservation>();
                _guests = guestsTask.Result ?? new List<Guest>();
                _feedback = feedbackTask.Result ?? new List<GuestFeedback>();
                _actions = actionsTask.Result ?? new List<InnEaseCRM.domain.entities.Action>();
                _promotions = promotionsTask.Result ?? new List<Promotion>();

                UpdateTenantBDashboard(kpiMetrics, loyaltyStatus);
                UpdateActionsBoard();
                UpdatePromotionsBoard();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to load Tenant B data.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BuildTenantBInterface()
        {
            this.Text = "Tenant B - Business Intelligence & Actions";
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
                Text = "Tenant B - Business Intelligence & Actions",
                Font = new Font("Segoe UI Semibold", 18F),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var subLabel = new Label
            {
                Text = "Beach Resort - High-level dashboard with essential performance metrics and action logging",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(112, 118, 142),
                Location = new Point(20, 55),
                AutoSize = true
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

            kpiTotalReservations = CreateClickableKpiCard("Total Reservations", "0", Color.FromArgb(30, 136, 229), 15, 50, "reservations");
            kpiActiveGuests = CreateClickableKpiCard("Active Guests", "0", Color.FromArgb(34, 160, 90), 280, 50, "guests");
            kpiOccupancyRate = CreateClickableKpiCard("Occupancy Rate", "0%", Color.FromArgb(226, 164, 52), 545, 50, null);
            kpiFeedbackScore = CreateClickableKpiCard("Feedback Score", "0", Color.FromArgb(214, 80, 110), 810, 50, "feedback");

            kpiSection.Controls.Add(kpiTitle);
            kpiSection.Controls.Add(kpiTotalReservations);
            kpiSection.Controls.Add(kpiActiveGuests);
            kpiSection.Controls.Add(kpiOccupancyRate);
            kpiSection.Controls.Add(kpiFeedbackScore);

            // Analytics Section
            var analyticsSection = new Panel
            {
                Location = new Point(20, 300),
                Size = new Size(1100, 180),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var analyticsTitle = new Label
            {
                Text = "Business Intelligence Overview",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(15, 15),
                AutoSize = true
            };

            // Simple chart representation
            var chartPanel = new Panel
            {
                Location = new Point(15, 50),
                Size = new Size(1070, 110),
                BackColor = Color.FromArgb(248, 250, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var chartLabel = new Label
            {
                Text = "Performance Metrics and Analytics",
                Location = new Point(10, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(112, 118, 142)
            };

            var placeholderLabel = new Label
            {
                Text = "Essential performance metrics: reservation trends, guest activity, and feedback satisfaction scores.",
                Location = new Point(10, 40),
                Size = new Size(1050, 60),
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
                Location = new Point(20, 500),
                Size = new Size(1100, 250),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var actionsTitle = new Label
            {
                Text = "Action Logging Board",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(15, 15),
                AutoSize = true
            };

            var actionsSubLabel = new Label
            {
                Text = "Update guest issue resolution statuses (Open, In Progress, Resolved)",
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

            // Promotions Section
            var promotionsSection = new Panel
            {
                Location = new Point(20, 770),
                Size = new Size(1100, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var promotionsTitle = new Label
            {
                Text = "Promotions & Loyalty",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(15, 15),
                AutoSize = true
            };

            var promotionsSubLabel = new Label
            {
                Text = "Customer retention campaigns and loyalty reward discounts",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(112, 118, 142),
                Location = new Point(15, 40),
                AutoSize = true
            };

            var btnGenerateOffers = new Button
            {
                Text = "Generate Retention Offers",
                Location = new Point(850, 15),
                Size = new Size(150, 25),
                BackColor = Color.FromArgb(146, 30, 98),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var btnAnalyzeChurn = new Button
            {
                Text = "Analyze Churn Risk",
                Location = new Point(1010, 15),
                Size = new Size(120, 25),
                BackColor = Color.FromArgb(38, 150, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            dgvPromotions = new DataGridView
            {
                Location = new Point(15, 70),
                Size = new Size(1070, 110),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            promotionsSection.Controls.Add(promotionsTitle);
            promotionsSection.Controls.Add(promotionsSubLabel);
            promotionsSection.Controls.Add(btnGenerateOffers);
            promotionsSection.Controls.Add(btnAnalyzeChurn);
            promotionsSection.Controls.Add(dgvPromotions);

            btnGenerateOffers.Click += btnGenerateOffers_Click;
            btnAnalyzeChurn.Click += btnAnalyzeChurn_Click;

            mainPanel.Controls.Add(headerLabel);
            mainPanel.Controls.Add(subLabel);
            mainPanel.Controls.Add(btnRefresh);
            mainPanel.Controls.Add(kpiSection);
            mainPanel.Controls.Add(analyticsSection);
            mainPanel.Controls.Add(actionsSection);
            mainPanel.Controls.Add(promotionsSection);

            this.Controls.Add(mainPanel);
        }

        private Panel CreateClickableKpiCard(string title, string value, Color color, int x, int y, string navigationTarget)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(250, 100),
                BackColor = Color.FromArgb(248, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand
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

            // Hover effect
            panel.MouseEnter += (s, e) =>
            {
                panel.BackColor = Color.FromArgb(244, 245, 252);
            };

            panel.MouseLeave += (s, e) =>
            {
                panel.BackColor = Color.FromArgb(248, 250, 255);
            };

            // Click navigation
            if (!string.IsNullOrEmpty(navigationTarget))
            {
                panel.Click += (s, e) =>
                {
                    NavigateToKpi(navigationTarget);
                };
            }

            return panel;
        }

        private void NavigateToKpi(string target)
        {
            // This would trigger navigation to the appropriate form
            // For now, show a message indicating the navigation
            MessageBox.Show(
                $"Navigate to {target} (This would open the corresponding form)",
                "KPI Navigation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void UpdateTenantBDashboard(Dictionary<string, decimal> kpiMetrics, Dictionary<string, object> loyaltyStatus)
        {
            int totalReservations = _reservations.Count;
            int activeGuests = _guests.Count;
            
            // Calculate occupancy rate
            int totalRooms = 100; // Assuming 100 rooms for Tenant B
            int occupiedRooms = _reservations.Count(r => 
                string.Equals(r.Status, "CheckedIn", StringComparison.OrdinalIgnoreCase));
            double occupancyRate = totalRooms > 0 ? (double)occupiedRooms / totalRooms * 100 : 0;

            // Calculate feedback score
            double feedbackScore = _feedback.Any() 
                ? _feedback.Average(f => f.Rating) 
                : 0;

            // Use BI service metrics if available
            if (kpiMetrics.ContainsKey("TotalRevenue"))
            {
                var revenue = kpiMetrics["TotalRevenue"];
                // Could display revenue in a separate KPI card
            }

            // Update KPI labels
            var valueLabel = kpiTotalReservations.Controls[1] as Label;
            valueLabel.Text = totalReservations.ToString("N0");

            valueLabel = kpiActiveGuests.Controls[1] as Label;
            valueLabel.Text = activeGuests.ToString("N0");

            valueLabel = kpiOccupancyRate.Controls[1] as Label;
            valueLabel.Text = $"{occupancyRate:F1}%";

            valueLabel = kpiFeedbackScore.Controls[1] as Label;
            valueLabel.Text = $"{feedbackScore:F1}/5";

            // Update loyalty information if available
            if (loyaltyStatus.ContainsKey("LoyaltyRate"))
            {
                var loyaltyRate = loyaltyStatus["LoyaltyRate"];
                // Could display loyalty rate in dashboard
            }
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

        private void UpdatePromotionsBoard()
        {
            var promotionData = _promotions
                .Select(p => new
                {
                    PromotionId = p.PromotionId,
                    PromoCode = p.PromoCode,
                    Description = p.Description,
                    DiscountType = p.DiscountType,
                    DiscountValue = p.DiscountValue,
                    StartDate = p.StartDate.ToString("MMM dd, yyyy"),
                    EndDate = p.EndDate.ToString("MMM dd, yyyy"),
                    Status = p.ApprovalStatus,
                    IsActive = p.IsActive ? "Active" : "Inactive"
                })
                .ToList();

            dgvPromotions.DataSource = promotionData;
        }

        // Control declarations
        private Button btnRefresh;
        private Button btnGenerateOffers;
        private Button btnAnalyzeChurn;
        private Panel kpiTotalReservations;
        private Panel kpiActiveGuests;
        private Panel kpiOccupancyRate;
        private Panel kpiFeedbackScore;
        private DataGridView dgvActions;
        private DataGridView dgvPromotions;
    }
}