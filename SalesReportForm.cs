using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class SalesReportForm : Form
    {
        private readonly HttpClient _httpClient;

        private string SalesUrl =>
            $"https://localhost:7129/api/sales";

        private List<Sale> _sales = new List<Sale>();
        private Dictionary<string, decimal> _dailySales = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> _weeklySales = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> _monthlySales = new Dictionary<string, decimal>();

        public SalesReportForm()
        {
            InitializeComponent();

            _httpClient = ApiClient.Instance;

            WindowState = FormWindowState.Maximized;

            Load += SalesReportForm_Load;
            btnDaily.Click += btnDaily_Click;
            btnWeekly.Click += btnWeekly_Click;
            btnMonthly.Click += btnMonthly_Click;
            btnRefresh.Click += btnRefresh_Click;
        }

        private async void SalesReportForm_Load(object sender, EventArgs e)
        {
            await LoadSalesData();
            ShowDailyReport();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadSalesData();
            // Refresh current view
            if (btnDaily.BackColor == Color.FromArgb(146, 30, 98))
                ShowDailyReport();
            else if (btnWeekly.BackColor == Color.FromArgb(146, 30, 98))
                ShowWeeklyReport();
            else
                ShowMonthlyReport();
        }

        private async Task LoadSalesData()
        {
            try
            {
                var response = await _httpClient.GetAsync(SalesUrl);
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await ApiClient.GetErrorMessageAsync(response);
                    MessageBox.Show(
                        $"Unable to load sales data.\n\n{errorMessage}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    lblStatus.Text = "Failed to load sales data";
                    lblStatus.ForeColor = Color.Red;
                    return;
                }

                _sales = await response.Content.ReadFromJsonAsync<List<Sale>>() ?? new List<Sale>();
                ProcessSalesData();
                lblStatus.Text = $"Loaded {_sales.Count} sales records";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to load sales data.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                lblStatus.Text = "Failed to load sales data";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void ProcessSalesData()
        {
            _dailySales.Clear();
            _weeklySales.Clear();
            _monthlySales.Clear();

            foreach (var sale in _sales)
            {
                string dateKey = sale.SoldAt.ToString("yyyy-MM-dd");
                string weekKey = GetWeekKey(sale.SoldAt);
                string monthKey = sale.SoldAt.ToString("MMM yyyy", CultureInfo.InvariantCulture);

                if (!_dailySales.ContainsKey(dateKey))
                    _dailySales[dateKey] = 0m;
                _dailySales[dateKey] += sale.Amount;

                if (!_weeklySales.ContainsKey(weekKey))
                    _weeklySales[weekKey] = 0m;
                _weeklySales[weekKey] += sale.Amount;

                if (!_monthlySales.ContainsKey(monthKey))
                    _monthlySales[monthKey] = 0m;
                _monthlySales[monthKey] += sale.Amount;
            }
        }

        private string GetWeekKey(DateTime date)
        {
            // Get Monday of the week
            int diff = ((int)date.DayOfWeek + 6) % 7;
            DateTime monday = date.AddDays(-diff);
            return $"Week of {monday:MMM dd, yyyy}";
        }

        private void btnDaily_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnDaily);
            ShowDailyReport();
        }

        private void btnWeekly_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnWeekly);
            ShowWeeklyReport();
        }

        private void btnMonthly_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnMonthly);
            ShowMonthlyReport();
        }

        private void SetActiveButton(Button activeButton)
        {
            Color activeColor = Color.FromArgb(146, 30, 98);
            Color inactiveColor = Color.White;
            Color activeTextColor = Color.White;
            Color inactiveTextColor = Color.FromArgb(146, 30, 98);

            btnDaily.BackColor = btnDaily == activeButton ? activeColor : inactiveColor;
            btnDaily.ForeColor = btnDaily == activeButton ? activeTextColor : inactiveTextColor;

            btnWeekly.BackColor = btnWeekly == activeButton ? activeColor : inactiveColor;
            btnWeekly.ForeColor = btnWeekly == activeButton ? activeTextColor : inactiveTextColor;

            btnMonthly.BackColor = btnMonthly == activeButton ? activeColor : inactiveColor;
            btnMonthly.ForeColor = btnMonthly == activeButton ? activeTextColor : inactiveTextColor;
        }

        private void ShowDailyReport()
        {
            lblReportTitle.Text = "Daily Sales Report";
            BuildSalesChart(_dailySales, "Daily Sales (Last 30 Days)");
            BuildSalesTable(_dailySales, "Date");
        }

        private void ShowWeeklyReport()
        {
            lblReportTitle.Text = "Weekly Sales Report";
            BuildSalesChart(_weeklySales, "Weekly Sales (Last 12 Weeks)");
            BuildSalesTable(_weeklySales, "Week");
        }

        private void ShowMonthlyReport()
        {
            lblReportTitle.Text = "Monthly Sales Report";
            BuildSalesChart(_monthlySales, "Monthly Sales (Last 12 Months)");
            BuildSalesTable(_monthlySales, "Month");
        }

        private void BuildSalesChart(Dictionary<string, decimal> data, string title)
        {
            pnlChart.Controls.Clear();

            if (data.Count == 0)
            {
                var noDataLabel = new Label
                {
                    Text = "No sales data available for the selected period",
                    AutoSize = true,
                    Location = new Point(20, 20),
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = Color.Gray
                };
                pnlChart.Controls.Add(noDataLabel);
                return;
            }

            int chartHeight = pnlChart.ClientSize.Height - 60;
            int chartWidth = pnlChart.ClientSize.Width - 40;
            int barWidth = Math.Max(20, (chartWidth - 40) / Math.Min(data.Count, 30));
            int maxBarWidth = 60;
            barWidth = Math.Min(barWidth, maxBarWidth);

            decimal maxValue = data.Values.Max();
            if (maxValue == 0) maxValue = 1;

            int x = 20;

            foreach (var kvp in data.OrderByDescending(d => d.Key))
            {
                double barHeight = (double)(kvp.Value / maxValue) * (chartHeight - 40);
                barHeight = Math.Max(2, barHeight);

                var barPanel = new Panel
                {
                    BackColor = Color.FromArgb(30, 136, 229),
                    Location = new Point(x, (int)(chartHeight - barHeight)),
                    Size = new Size(barWidth - 4, (int)barHeight),
                    BorderStyle = BorderStyle.None
                };

                var valueLabel = new Label
                {
                    Text = "\u20B1" + kvp.Value.ToString("N0"),
                    AutoSize = true,
                    Location = new Point(x, (int)(chartHeight - barHeight) - 20),
                    Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 34, 58),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var dateLabel = new Label
                {
                    Text = kvp.Key,
                    AutoSize = true,
                    Location = new Point(x, chartHeight + 5),
                    Font = new Font("Segoe UI", 7F),
                    ForeColor = Color.FromArgb(112, 118, 142),
                    MaximumSize = new Size(barWidth, 40)
                };

                pnlChart.Controls.Add(barPanel);
                pnlChart.Controls.Add(valueLabel);
                pnlChart.Controls.Add(dateLabel);

                x += barWidth;
            }

            var titleLabel = new Label
            {
                Text = title,
                AutoSize = true,
                Location = new Point(20, 10),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 34, 58)
            };

            pnlChart.Controls.Add(titleLabel);
        }

        private void BuildSalesTable(Dictionary<string, decimal> data, string periodLabel)
        {
            var tableData = data
                .OrderByDescending(d => d.Key)
                .Select(d => new
                {
                    Period = d.Key,
                    Sales = d.Value,
                    Transactions = _sales.Count(s => GetPeriodKey(s.SoldAt, periodLabel) == d.Key)
                })
                .ToList();

            dgvSalesReport.DataSource = null;
            dgvSalesReport.DataSource = tableData;

            // Format columns
            if (dgvSalesReport.Columns.Count > 0)
            {
                dgvSalesReport.Columns[0].HeaderText = periodLabel;
                dgvSalesReport.Columns[1].HeaderText = "Total Sales";
                dgvSalesReport.Columns[2].HeaderText = "Transactions";

                dgvSalesReport.Columns[1].DefaultCellStyle.Format = "C2";
                dgvSalesReport.Columns[1].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("en-PH");
            }
        }

        private string GetPeriodKey(DateTime date, string periodLabel)
        {
            switch (periodLabel)
            {
                case "Date":
                    return date.ToString("yyyy-MM-dd");
                case "Week":
                    return GetWeekKey(date);
                case "Month":
                    return date.ToString("MMM yyyy", CultureInfo.InvariantCulture);
                default:
                    return date.ToString("yyyy-MM-dd");
            }
        }
    }
}
