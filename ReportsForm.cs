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
    public partial class ReportsForm : Form
    {
        private readonly HttpClient _httpClient;

        // =========================================================
        // TENANT-AWARE API URLS
        // =========================================================

        private string GuestsUrl =>
            $"https://localhost:7129/api/guests";

        private string ReservationsUrl =>
            $"https://localhost:7129/api/reservations";

        private string FeedbackUrl =>
            $"https://localhost:7129/api/guestfeedback";

        private string InteractionsUrl =>
            $"https://localhost:7129/api/interactions";

        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public ReportsForm()
        {
            InitializeComponent();

            _httpClient = ApiClient.Instance;

            WindowState = FormWindowState.Maximized;

            Load += ReportsForm_Load;
            btnRefresh.Click += btnRefresh_Click;
        }

        // =========================================================
        // FORM LOAD
        // =========================================================

        private async void ReportsForm_Load(object sender, EventArgs e)
        {
            await LoadAllReports();
        }

        // =========================================================
        // REFRESH
        // =========================================================

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadAllReports();
        }

        // =========================================================
        // LOAD ALL REPORTS
        // =========================================================

        private async Task LoadAllReports()
        {
            try
            {
                                var guests =
                    await _httpClient.GetFromJsonAsync<List<Guest>>(GuestsUrl)
                    ?? new List<Guest>();

                var reservations =
                    await _httpClient.GetFromJsonAsync<List<Reservation>>(ReservationsUrl)
                    ?? new List<Reservation>();

                var feedback =
                    await _httpClient.GetFromJsonAsync<List<GuestFeedback>>(FeedbackUrl)
                    ?? new List<GuestFeedback>();

                var interactions =
                    await _httpClient.GetFromJsonAsync<List<GuestInteraction>>(InteractionsUrl)
                    ?? new List<GuestInteraction>();

                // =================================================
                // TAB 1: CUSTOMER REPORTS
                // =================================================

                var reportRows = guests
                    .Select(g => new
                    {
                        GuestId = g.GuestId,
                        FullName = $"{g.FirstName} {g.LastName}".Trim(),
                        Email = g.Email,

                        TotalReservations =
                            reservations.Count(r => r.GuestId == g.GuestId),

                        TotalFeedback =
                            feedback.Count(f => f.GuestId == g.GuestId)
                    })
                    .ToList();

                dgvReports.DataSource = reportRows;

                // =================================================
                // TAB 2: RELATIONSHIPS
                // =================================================

                var relationshipRows = interactions
                    .OrderByDescending(i => i.InteractionDate)
                    .Select(i => new
                    {
                        i.GuestInteractionId,
                        i.GuestId,
                        i.InteractionType,
                        i.HandledBy,
                        i.InteractionDate,
                        i.Notes
                    })
                    .ToList();

                dgvRelationships.DataSource = relationshipRows;

                // =================================================
                // TAB 3: RETENTION
                // =================================================

                var retentionRows = guests
                    .Select(g => new
                    {
                        GuestId = g.GuestId,
                        FullName = $"{g.FirstName} {g.LastName}".Trim(),

                        TotalReservations =
                            reservations.Count(
                                r => r.GuestId == g.GuestId)
                    })
                    .Where(x => x.TotalReservations > 1)
                    .OrderByDescending(x => x.TotalReservations)
                    .ToList();

                dgvRetention.DataSource = retentionRows;

                // =================================================
                // TAB 4: STRATEGIC KPIs
                // =================================================

                BuildStrategicKpis(
                    guests,
                    reservations,
                    feedback,
                    interactions,
                    retentionRows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to load reports for Company {1}.\n\n" +
                    ex.Message,
                    "Reports Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // STRATEGIC KPIs
        // =========================================================

        private void BuildStrategicKpis(
            List<Guest> guests,
            List<Reservation> reservations,
            List<GuestFeedback> feedback,
            List<GuestInteraction> interactions,
            int repeatGuestCount)
        {
            pnlKpiContainer.Controls.Clear();
            pnlChartContainer.Controls.Clear();

            int totalGuests = guests.Count;
            int totalReservations = reservations.Count;
            int totalFeedback = feedback.Count;
            int totalInteractions = interactions.Count;

            // =====================================================
            // RETENTION RATE
            // =====================================================

            double retentionRate =
                totalGuests == 0
                    ? 0
                    : (double)repeatGuestCount /
                      totalGuests * 100;

            // =====================================================
            // CANCELLATION RATE
            // =====================================================

            int cancelledCount =
                reservations.Count(r =>
                    string.Equals(
                        r.Status,
                        "Cancelled",
                        StringComparison.OrdinalIgnoreCase));

            double cancellationRate =
                totalReservations == 0
                    ? 0
                    : (double)cancelledCount /
                      totalReservations * 100;

            // =====================================================
            // FEEDBACK RESOLUTION
            // =====================================================

            int resolvedCount =
                feedback.Count(f =>
                    string.Equals(
                        f.Status,
                        "Resolved",
                        StringComparison.OrdinalIgnoreCase));

            double resolutionRate =
                totalFeedback == 0
                    ? 0
                    : (double)resolvedCount /
                      totalFeedback * 100;

            // =====================================================
            // COMPLAINT RATIO
            // =====================================================

            int complaintCount =
                feedback.Count(f =>
                    string.Equals(
                        f.RequestType,
                        "Complaint",
                        StringComparison.OrdinalIgnoreCase));

            double complaintRatio =
                totalFeedback == 0
                    ? 0
                    : (double)complaintCount /
                      totalFeedback * 100;

            // =====================================================
            // AVERAGE LENGTH OF STAY
            // =====================================================

            double avgLengthOfStay = 0;

            if (reservations.Count > 0)
            {
                avgLengthOfStay =
                    reservations.Average(
                        r => (r.CheckOutDate -
                              r.CheckInDate).TotalDays);
            }

            // =====================================================
            // MOST COMMON INTERACTION
            // =====================================================

            string topInteractionType =
                interactions
                    .GroupBy(i => i.InteractionType)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault()
                ?? "N/A";

            // =====================================================
            // KPI LIST
            // =====================================================

            var kpis =
                new List<(string Title,
                          string Value,
                          string Interpretation,
                          Color BarColor,
                          double BarPercent)>
            {
                (
                    "Guest Retention Rate",
                    $"{retentionRate:F1}%",
                    retentionRate >= 30
                        ? "Repeat guests are being recorded in the customer base."
                        : "Repeat bookings are limited. Consider customer retention activities.",
                    retentionRate >= 30
                        ? Color.SeaGreen
                        : Color.IndianRed,
                    retentionRate
                ),

                (
                    "Reservation Cancellation Rate",
                    $"{cancellationRate:F1}%",
                    cancellationRate <= 15
                        ? "Cancelled reservations represent a smaller portion of bookings."
                        : "Review cancellation patterns and guest communication.",
                    cancellationRate <= 15
                        ? Color.SeaGreen
                        : Color.IndianRed,
                    cancellationRate
                ),

                (
                    "Feedback Resolution Rate",
                    $"{resolutionRate:F1}%",
                    resolutionRate >= 70
                        ? "Most recorded feedback has a resolved status."
                        : "Review unresolved feedback and follow-up procedures.",
                    resolutionRate >= 70
                        ? Color.SeaGreen
                        : Color.IndianRed,
                    resolutionRate
                ),

                (
                    "Complaint Ratio",
                    $"{complaintRatio:F1}%",
                    complaintRatio <= 20
                        ? "Complaints represent a smaller portion of recorded feedback."
                        : "Review recurring complaint categories and service issues.",
                    complaintRatio <= 20
                        ? Color.SeaGreen
                        : Color.IndianRed,
                    complaintRatio
                ),

                (
                    "Average Length of Stay",
                    $"{avgLengthOfStay:F1} nights",
                    "Useful for planning housekeeping, staffing, and room operations.",
                    Color.SteelBlue,
                    Math.Min(avgLengthOfStay * 10, 100)
                ),

                (
                    "Most Common Staff Interaction",
                    topInteractionType,
                    "Shows the interaction type most frequently recorded by staff.",
                    Color.SteelBlue,
                    100
                )
            };

            // =====================================================
            // ADD KPI CARDS
            // =====================================================

            int y = 0;

            foreach (var kpi in kpis)
            {
                var card = BuildKpiCard(
                    kpi.Title,
                    kpi.Value,
                    kpi.Interpretation,
                    kpi.BarColor,
                    kpi.BarPercent);

                card.Location = new Point(0, y);

                pnlKpiContainer.Controls.Add(card);

                y += card.Height + 10;
            }

            // =====================================================
            // RESERVATION STATUS CHART
            // =====================================================

            var statusGroups = reservations
                .GroupBy(r => r.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToList();

            var chartLabel = new Label
            {
                Text = "Reservations by Status",
                Font = new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            pnlChartContainer.Controls.Add(chartLabel);

            int chartY = 32;

            int maxCount =
                statusGroups.Count > 0
                    ? statusGroups.Max(g => g.Count)
                    : 1;

            const int labelColumnWidth = 150;
            const int maxBarTrackWidth = 500;
            const int barHeight = 22;
            const int rowSpacing = 32;

            foreach (var group in statusGroups)
            {
                var rowLabel = new Label
                {
                    Text = $"{group.Status} ({group.Count})",
                    AutoSize = true,
                    Location = new Point(
                        0,
                        chartY + 4)
                };

                int barWidth =
                    maxCount == 0
                        ? 2
                        : Math.Max(
                            2,
                            (int)(
                                (double)group.Count /
                                maxCount *
                                maxBarTrackWidth));

                var barTrack = new Panel
                {
                    BackColor = Color.WhiteSmoke,
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(
                        labelColumnWidth,
                        chartY),
                    Size = new Size(
                        maxBarTrackWidth,
                        barHeight)
                };

                var barFillChart = new Panel
                {
                    BackColor = Color.SteelBlue,
                    Location = new Point(0, 0),
                    Size = new Size(
                        barWidth,
                        barHeight)
                };

                barTrack.Controls.Add(barFillChart);

                pnlChartContainer.Controls.Add(rowLabel);
                pnlChartContainer.Controls.Add(barTrack);

                chartY += rowSpacing;
            }
        }

        // =========================================================
        // KPI CARD
        // =========================================================

        private Panel BuildKpiCard(
            string title,
            string value,
            string interpretation,
            Color barColor,
            double barPercent)
        {
            int cardWidth =
                Math.Min(
                    Math.Max(
                        pnlKpiContainer.ClientSize.Width - 20,
                        790),
                    900);

            const int rightBlockWidth = 200;

            int rightBlockX =
                cardWidth -
                rightBlockWidth -
                15;

            int leftTextWidth =
                Math.Max(
                    rightBlockX - 25,
                    300);

            var card = new Panel
            {
                Size = new Size(
                    cardWidth,
                    80),

                BorderStyle =
                    BorderStyle.FixedSingle,

                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            var lblTitleKpi = new Label
            {
                Text = title,
                Font = new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold),
                Location = new Point(10, 8),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font(
                    "Segoe UI",
                    14F,
                    FontStyle.Bold),
                ForeColor = barColor,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblValue.Location =
                new Point(
                    rightBlockX,
                    8);

            var lblInterpretation = new Label
            {
                Text = interpretation,
                Font = new Font(
                    "Segoe UI",
                    8.5F),
                ForeColor = Color.DimGray,
                Location = new Point(10, 34),
                Size = new Size(
                    leftTextWidth,
                    40)
            };

            double clampedPercent =
                Math.Max(
                    0,
                    Math.Min(
                        barPercent,
                        100));

            var barBackground = new Panel
            {
                BackColor = Color.Gainsboro,
                Location = new Point(
                    rightBlockX,
                    45),
                Size = new Size(
                    rightBlockWidth - 20,
                    12)
            };

            var barFill = new Panel
            {
                BackColor = barColor,
                Location = new Point(0, 0),
                Size = new Size(
                    (int)(
                        (rightBlockWidth - 20) *
                        clampedPercent /
                        100),
                    12)
            };

            barBackground.Controls.Add(barFill);

            card.Controls.Add(lblTitleKpi);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblInterpretation);
            card.Controls.Add(barBackground);

            return card;
        }
    }
}

