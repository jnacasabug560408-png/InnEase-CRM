using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace InnEase_CRM
{
    public partial class SubscriptionsForm : Form
    {
        private readonly HttpClient _client = ApiClient.Instance;
        private DataGridView dgvTenants;
        private List<TenantDto> _tenants = new List<TenantDto>();

        private Label lblSelectedTenant;
        private Button btnToggleStatus;
        private Button btnChangePlan;
        private ComboBox cmbPlan;

        // ---- Palette (matches Admin Panel / Platform BI) ----
        private static readonly Color DeepNavy = ColorTranslator.FromHtml("#1E1B4B");
        private static readonly Color AccentPurple = ColorTranslator.FromHtml("#831843");
        private static readonly Color SoftBackground = ColorTranslator.FromHtml("#F8FAFC");
        private static readonly Color WhiteCard = ColorTranslator.FromHtml("#FFFFFF");
        private static readonly Color RowAlt = ColorTranslator.FromHtml("#F1F5F9");
        private static readonly Color GridLine = ColorTranslator.FromHtml("#E2E8F0");
        private static readonly Color SelectionBg = ColorTranslator.FromHtml("#312E81");

        public SubscriptionsForm()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var tenants = await _client.GetFromJsonAsync<List<TenantDto>>("api/master/tenants");
                if (tenants != null)
                {
                    _tenants = tenants;
                    dgvTenants.DataSource = null;
                    dgvTenants.DataSource = _tenants;
                    StyleGridColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading tenants: " + ex.Message, "Subscriptions Management",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StyleGridColumns()
        {
            if (dgvTenants.Columns["CompanyId"] != null)
                dgvTenants.Columns["CompanyId"].HeaderText = "Company ID";

            if (dgvTenants.Columns["OrganizationName"] != null)
                dgvTenants.Columns["OrganizationName"].HeaderText = "Organization";

            if (dgvTenants.Columns["PlanTier"] != null)
                dgvTenants.Columns["PlanTier"].HeaderText = "Plan";

            if (dgvTenants.Columns["ActiveStatus"] != null)
                dgvTenants.Columns["ActiveStatus"].HeaderText = "Status";

            if (dgvTenants.Columns["ConnectionString"] != null)
                dgvTenants.Columns["ConnectionString"].HeaderText = "Connection";
        }

        private void DgvTenants_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTenants.CurrentRow?.DataBoundItem is TenantDto tenant)
            {
                lblSelectedTenant.Text = $"Selected: {tenant.OrganizationName}  (ID {tenant.CompanyId}, currently {tenant.ActiveStatus})";
                btnToggleStatus.Enabled = true;
                btnChangePlan.Enabled = true;
                cmbPlan.Enabled = true;
                
                string planTier = tenant.PlanTier ?? "";
                for(int i = 0; i < cmbPlan.Items.Count; i++) {
                    if (string.Equals(cmbPlan.Items[i].ToString(), planTier, StringComparison.OrdinalIgnoreCase)) {
                        cmbPlan.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                lblSelectedTenant.Text = "Select a tenant in the grid to manage its subscription.";
                btnToggleStatus.Enabled = false;
                btnChangePlan.Enabled = false;
                cmbPlan.Enabled = false;
            }
        }

        public class TenantDto
        {
            public int CompanyId { get; set; }
            public string OrganizationName { get; set; } = string.Empty;
            public string PlanTier { get; set; } = string.Empty;
            public string ActiveStatus { get; set; } = string.Empty;
            public string ConnectionString { get; set; } = string.Empty;
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
                Text = "Subscriptions Management",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = DeepNavy,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var lblSubHeader = new Label
            {
                Text = "Manage plan tiers and active status per tenant",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(22, 20 + lblHeader.PreferredHeight + 2)
            };

            // ---- Tenant grid ----
            dgvTenants = new DataGridView
            {
                Location = new Point(20, lblSubHeader.Bottom + 20),
                Size = new Size(900, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = WhiteCard,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = GridLine,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 36 }
            };

            dgvTenants.ColumnHeadersDefaultCellStyle.BackColor = DeepNavy;
            dgvTenants.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTenants.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvTenants.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvTenants.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);

            dgvTenants.DefaultCellStyle.BackColor = WhiteCard;
            dgvTenants.DefaultCellStyle.SelectionBackColor = SelectionBg;
            dgvTenants.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvTenants.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            dgvTenants.DefaultCellStyle.Font = new Font("Segoe UI", 9F);

            dgvTenants.AlternatingRowsDefaultCellStyle.BackColor = RowAlt;

            dgvTenants.SelectionChanged += DgvTenants_SelectionChanged;

            // ---- Actions toolbar (docked directly under the grid, not floating) ----
            var pnlActions = new Panel
            {
                Location = new Point(20, dgvTenants.Bottom + 16),
                Size = new Size(900, 70),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = WhiteCard,
                Padding = new Padding(16)
            };
            pnlActions.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(GridLine), 0, 0, pnlActions.Width - 1, pnlActions.Height - 1);

            lblSelectedTenant = new Label
            {
                Text = "Select a tenant in the grid to manage its subscription.",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(16, 8)
            };

            btnToggleStatus = new Button
            {
                Text = "Toggle Active / Suspended",
                Location = new Point(16, 32),
                Size = new Size(200, 30),
                BackColor = AccentPurple,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnToggleStatus.FlatAppearance.BorderSize = 0;
            btnToggleStatus.Click += async (s, e) =>
            {
                if (dgvTenants.CurrentRow?.DataBoundItem is not TenantDto tenant) return;

                var newStatus = tenant.ActiveStatus.ToLower() == "active" ? "suspended" : "active";
                await _client.PutAsJsonAsync($"api/master/tenants/{tenant.CompanyId}/status", new { Status = newStatus });
                tenant.ActiveStatus = newStatus == "active" ? "Active" : "Suspended";
                dgvTenants.Refresh();
                lblSelectedTenant.Text = $"Selected: {tenant.OrganizationName}  (ID {tenant.CompanyId}, currently {tenant.ActiveStatus})";
            };

            cmbPlan = new ComboBox
            {
                Location = new Point(btnToggleStatus.Right + 20, 32),
                Size = new Size(140, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            cmbPlan.Items.AddRange(new object[] { "Basic", "Pro", "Enterprise" });
            cmbPlan.SelectedIndex = 0;

            btnChangePlan = new Button
            {
                Text = "Change Plan Tier",
                Location = new Point(cmbPlan.Right + 12, 32),
                Size = new Size(150, 30),
                BackColor = DeepNavy,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnChangePlan.FlatAppearance.BorderSize = 0;
            btnChangePlan.Click += async (s, e) =>
            {
                if (dgvTenants.CurrentRow?.DataBoundItem is not TenantDto tenant) return;
                if (cmbPlan.SelectedItem == null) return;

                string newPlan = cmbPlan.SelectedItem.ToString();
                await _client.PutAsJsonAsync($"api/master/tenants/{tenant.CompanyId}/plan", new { PlanTier = newPlan });
                tenant.PlanTier = newPlan;
                dgvTenants.Refresh();
            };

            pnlActions.Controls.Add(lblSelectedTenant);
            pnlActions.Controls.Add(btnToggleStatus);
            pnlActions.Controls.Add(cmbPlan);
            pnlActions.Controls.Add(btnChangePlan);

            this.Controls.Add(lblHeader);
            this.Controls.Add(lblSubHeader);
            this.Controls.Add(dgvTenants);
            this.Controls.Add(pnlActions);

            this.ClientSize = new Size(960, pnlActions.Bottom + 30);
            this.MinimumSize = new Size(700, 500);
            this.Name = "SubscriptionsForm";
            this.Text = "InnEase CRM - Subscriptions Management";
            this.ResumeLayout(false);
        }
    }
}
