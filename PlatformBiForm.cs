using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace InnEase_CRM
{
    public partial class PlatformBiForm : Form
    {
        private readonly HttpClient _client = ApiClient.Instance;

        private Label lblTotalActive;
        private Label lblSuspended;
        private Label lblTierDist;
        private Label lblHealth;
        private Panel dotHealth;

        // ---- Palette (matches Admin Panel / Subscriptions) ----
        private static readonly Color DeepNavy = ColorTranslator.FromHtml("#1E1B4B");
        private static readonly Color SoftBackground = ColorTranslator.FromHtml("#F8FAFC");
        private static readonly Color WhiteCard = ColorTranslator.FromHtml("#FFFFFF");
        private static readonly Color BorderColor = ColorTranslator.FromHtml("#E2E8F0");
        private static readonly Color HealthyGreen = ColorTranslator.FromHtml("#22C55E");
        private static readonly Color UnhealthyRed = ColorTranslator.FromHtml("#EF4444");

        public PlatformBiForm()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var metrics = await _client.GetFromJsonAsync<BiMetricsDto>("api/master/bi");
                if (metrics != null)
                {
                    lblTotalActive.Text = metrics.TotalActiveTenants.ToString();
                    lblSuspended.Text = metrics.SuspendedTenants.ToString();
                    lblHealth.Text = $"Status: {metrics.SystemHealth}   |   Load: {metrics.PlatformLoad}";

                    dotHealth.BackColor = string.Equals(metrics.SystemHealth, "Healthy", StringComparison.OrdinalIgnoreCase)
                        ? HealthyGreen
                        : UnhealthyRed;

                    var sb = new StringBuilder();
                    foreach (var tier in metrics.SubscriptionTierDistribution)
                        sb.AppendLine($"{tier.Tier}: {tier.Count}");

                    lblTierDist.Text = sb.Length == 0 ? "-" : sb.ToString().TrimEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading BI metrics: " + ex.Message, "Platform Business Intelligence",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class BiMetricsDto
        {
            public int TotalActiveTenants { get; set; }
            public int SuspendedTenants { get; set; }
            public List<TierDto> SubscriptionTierDistribution { get; set; } = new List<TierDto>();
            public string SystemHealth { get; set; } = string.Empty;
            public string PlatformLoad { get; set; } = string.Empty;
        }

        public class TierDto
        {
            public string Tier { get; set; } = string.Empty;
            public int Count { get; set; }
        }

        // =========================================================
        // LAYOUT
        // =========================================================

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScroll = true;
            this.BackColor = SoftBackground;
            this.Font = new Font("Segoe UI", 9F);

            // ---- Header ----
            var lblHeader = new Label
            {
                Text = "Platform Business Intelligence",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = DeepNavy,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var lblSubHeader = new Label
            {
                Text = "SaaS-level metrics across every tenant",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(22, 20 + lblHeader.PreferredHeight + 2)
            };

            // ---- Metric cards ----
            var flowCards = new FlowLayoutPanel
            {
                Location = new Point(20, lblSubHeader.Bottom + 20),
                Size = new Size(900, 160),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                BackColor = SoftBackground
            };

            lblTotalActive = new Label { Text = "0" };
            lblSuspended = new Label { Text = "0" };
            lblTierDist = new Label { Text = "-" };

            var c1 = MakeMetricCard("Total Active Tenants", lblTotalActive, ColorTranslator.FromHtml("#DCFCE7"));
            var c2 = MakeMetricCard("Suspended Tenants", lblSuspended, ColorTranslator.FromHtml("#FEE2E2"));
            var c3 = MakeTierCard("Subscription Tiers", lblTierDist, ColorTranslator.FromHtml("#FEF9C3"));
            
            c1.Click += (s, e) => { (Application.OpenForms.OfType<MainForm>().FirstOrDefault())?.NavigateTo("AdminPanel", "Active"); };
            c2.Click += (s, e) => { (Application.OpenForms.OfType<MainForm>().FirstOrDefault())?.NavigateTo("AdminPanel", "Suspended"); };
            lblTotalActive.Click += (s, e) => { (Application.OpenForms.OfType<MainForm>().FirstOrDefault())?.NavigateTo("AdminPanel", "Active"); };
            lblSuspended.Click += (s, e) => { (Application.OpenForms.OfType<MainForm>().FirstOrDefault())?.NavigateTo("AdminPanel", "Suspended"); };
            
            flowCards.Controls.Add(c1);
            flowCards.Controls.Add(c2);
            flowCards.Controls.Add(c3);

            var btnRefresh = new Button
            {
                Text = "Refresh Metrics",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Size = new Size(150, 40),
                Location = new Point(770, lblSubHeader.Top - 10),
                BackColor = DeepNavy,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadData();
            this.Controls.Add(btnRefresh);

            // ---- System Health panel ----
            var pnlHealth = new Panel
            {
                Location = new Point(20, flowCards.Bottom + 20),
                Size = new Size(900, 130),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = WhiteCard,
                Padding = new Padding(20),
                BorderStyle = BorderStyle.None
            };
            pnlHealth.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, pnlHealth.Width - 1, pnlHealth.Height - 1);

            var lblHealthTitle = new Label
            {
                Text = "System Health & Aggregate Platform Load",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = DeepNavy,
                AutoSize = true,
                Location = new Point(20, 16)
            };

            dotHealth = new Panel
            {
                Size = new Size(14, 14),
                Location = new Point(20, lblHealthTitle.Bottom + 16),
                BackColor = HealthyGreen
            };
            dotHealth.Paint += (s, e) =>
            {
                using (var brush = new SolidBrush(dotHealth.BackColor))
                    e.Graphics.FillEllipse(brush, 0, 0, dotHealth.Width, dotHealth.Height);
            };

            lblHealth = new Label
            {
                Text = "Loading...",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(dotHealth.Right + 10, lblHealthTitle.Bottom + 13)
            };

            pnlHealth.Controls.Add(lblHealthTitle);
            pnlHealth.Controls.Add(dotHealth);
            pnlHealth.Controls.Add(lblHealth);

            this.Controls.Add(lblHeader);
            this.Controls.Add(lblSubHeader);
            this.Controls.Add(flowCards);
            this.Controls.Add(pnlHealth);

            this.ClientSize = new Size(960, pnlHealth.Bottom + 40);
            this.MinimumSize = new Size(700, 500);
            this.Name = "PlatformBiForm";
            this.Text = "InnEase CRM - Platform Business Intelligence";
            this.ResumeLayout(false);
        }

        // A metric card: colored background, bold 24pt figure, muted label underneath.
        private Panel MakeMetricCard(string title, Label valueLabel, Color backColor)
        {
            var panel = new Panel
            {
                BackColor = backColor,
                MinimumSize = new Size(220, 110),
                Size = new Size(220, 110),
                Margin = new Padding(0, 0, 20, 0),
                Padding = new Padding(16, 14, 16, 14),
                Cursor = Cursors.Hand
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(90, 90, 90),
                AutoSize = true,
                MaximumSize = new Size(190, 0),
                Location = new Point(16, 14)
            };

            valueLabel.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            valueLabel.ForeColor = DeepNavy;
            valueLabel.AutoSize = true;
            valueLabel.Location = new Point(16, lblTitle.Bottom + 6);

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(valueLabel);
            return panel;
        }

        // Subscription tiers card: same shell, smaller multi-line text instead of one big number.
        private Panel MakeTierCard(string title, Label listLabel, Color backColor)
        {
            var panel = new Panel
            {
                BackColor = backColor,
                MinimumSize = new Size(220, 150),
                Size = new Size(220, 150),
                Margin = new Padding(0, 0, 20, 0),
                Padding = new Padding(16, 14, 16, 14),
                Cursor = Cursors.Hand
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(90, 90, 90),
                AutoSize = true,
                MaximumSize = new Size(190, 0),
                Location = new Point(16, 14)
            };

            listLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            listLabel.ForeColor = DeepNavy;
            listLabel.AutoSize = true;
            listLabel.Location = new Point(16, lblTitle.Bottom + 8);

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(listLabel);
            return panel;
        }
    }
}
