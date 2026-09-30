using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnEase_CRM
{
    public partial class AdminPanelForm : Form
    {
        private readonly HttpClient _client = ApiClient.Instance;
        private const string TenantsEndpoint = "api/master/tenants";

        // ---- Palette ----
        private static readonly Color DeepNavy = ColorTranslator.FromHtml("#1E1B4B");
        private static readonly Color AccentPurple = ColorTranslator.FromHtml("#831843");
        private static readonly Color SoftBackground = ColorTranslator.FromHtml("#F8FAFC");
        private static readonly Color WhiteCard = ColorTranslator.FromHtml("#FFFFFF");
        private static readonly Color RowAlt = ColorTranslator.FromHtml("#F1F5F9");
        private static readonly Color GridLine = ColorTranslator.FromHtml("#E2E8F0");
        private static readonly Color SelectionBg = ColorTranslator.FromHtml("#EDE9FE");
        private static readonly Color Muted = ColorTranslator.FromHtml("#6B7280");
        private static readonly Color Blue = ColorTranslator.FromHtml("#3B82F6");
        private static readonly Color Green = ColorTranslator.FromHtml("#10B981");
        private static readonly Color Red = ColorTranslator.FromHtml("#EF4444");
        private static readonly Color Amber = ColorTranslator.FromHtml("#F59E0B");

        // Data
        private List<TenantDto> _tenants = new List<TenantDto>();
        private readonly HashSet<int> _revealed = new HashSet<int>();

        // Controls
        private AdminNavCard cOverview, cTenants, cInfra;
        private Panel host, pOverview, pTenants, pInfra;
        private DataGridView dgvTenants;
        private TextBox txtSearch;
        private ComboBox cboStatus;
        private Button btnRefresh, btnReveal;
        private Label lblBanner;
        private AdminHealthRow rowApi, rowRegistry, rowSuspended;
        private readonly System.Windows.Forms.Timer _healthTimer = new System.Windows.Forms.Timer { Interval = 10000 };
        private bool _checking;

        // Selected tenant, exposed so other screens (e.g. Subscriptions) can read it if needed.
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int? SelectedCompanyId { get; private set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string InitialFilter { get; set; }
        
        public AdminPanelForm()
        {
            InitializeComponent();
            ShowSection(1);                 // start on Tenant Management, like before
            this.Load += (s, e) => 
            {
                if (!string.IsNullOrEmpty(InitialFilter))
                {
                    cboStatus.SelectedItem = InitialFilter;
                }
            };
            _ = LoadTenantsAsync();
        }

        // =========================================================
        // DATA
        // =========================================================

        private async Task LoadTenantsAsync()
        {
            try
            {
                var tenants = await _client.GetFromJsonAsync<List<TenantDto>>(TenantsEndpoint);
                if (IsDisposed) return;
                _tenants = tenants ?? new List<TenantDto>();
                BindGrid();
                if (pOverview.Visible) BuildOverview();
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                    MessageBox.Show("Error loading tenants: " + ex.Message, "Admin Panel",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool IsActive(TenantDto t) =>
            string.Equals(t.ActiveStatus, "Active", StringComparison.OrdinalIgnoreCase);

        private static bool IsSuspended(TenantDto t) =>
            string.Equals(t.ActiveStatus, "Suspended", StringComparison.OrdinalIgnoreCase);

        public class TenantDto
        {
            public int CompanyId { get; set; }
            public string OrganizationName { get; set; } = string.Empty;
            public string PlanTier { get; set; } = string.Empty;
            public string ActiveStatus { get; set; } = string.Empty;
            public string ConnectionString { get; set; } = string.Empty;
        }

        // =========================================================
        // NAVIGATION
        // =========================================================

        private void ShowSection(int i)
        {
            cOverview.Active = i == 0;
            cTenants.Active = i == 1;
            cInfra.Active = i == 2;
            pOverview.Visible = i == 0;
            pTenants.Visible = i == 1;
            pInfra.Visible = i == 2;
            _healthTimer.Enabled = i == 2;

            if (i == 0) BuildOverview();
            if (i == 2) _ = RunHealthCheckAsync();
        }

        // =========================================================
        // SECTION 1: SYSTEM OVERVIEW
        // =========================================================

        private Panel Tile(string caption, string value, Color accent, int x)
        {
            var p = new Panel { Location = new Point(x, 4), Size = new Size(190, 92), BackColor = WhiteCard };
            p.Paint += (s, e) =>
            {
                using (var b = new SolidBrush(accent)) e.Graphics.FillRectangle(b, 0, 0, 5, p.Height);
                using (var pen = new Pen(GridLine)) e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };
            p.Controls.Add(new Label { Text = caption, ForeColor = Muted, AutoSize = true, Location = new Point(18, 12) });
            p.Controls.Add(new Label
            {
                Text = value,
                ForeColor = DeepNavy,
                AutoSize = true,
                Location = new Point(16, 34),
                Font = new Font("Segoe UI Semibold", 24F)
            });
            return p;
        }

        private void BuildOverview()
        {
            pOverview.SuspendLayout();
            pOverview.Controls.Clear();

            int total = _tenants.Count;
            int active = _tenants.Count(IsActive);
            int suspended = _tenants.Count(IsSuspended);
            int plans = _tenants.Select(t => t.PlanTier).Distinct().Count();

            pOverview.Controls.Add(Tile("Total Tenants", total.ToString(), Blue, 0));
            pOverview.Controls.Add(Tile("Active", active.ToString(), Green, 206));
            pOverview.Controls.Add(Tile("Suspended", suspended.ToString(), Red, 412));
            pOverview.Controls.Add(Tile("Plans in use", plans.ToString(), Amber, 618));

            var groups = _tenants.GroupBy(t => t.PlanTier).OrderBy(g => g.Key).ToList();
            var mix = new Panel
            {
                Location = new Point(0, 112),
                Size = new Size(808, 70 + Math.Max(1, groups.Count) * 40),
                BackColor = WhiteCard
            };
            mix.Paint += (s, e) => { using (var pen = new Pen(GridLine)) e.Graphics.DrawRectangle(pen, 0, 0, mix.Width - 1, mix.Height - 1); };
            mix.Controls.Add(new Label
            {
                Text = "Subscription mix",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = DeepNavy,
                AutoSize = true,
                Location = new Point(20, 14)
            });

            Color[] cols = { ColorTranslator.FromHtml("#6366F1"), AccentPurple, DeepNavy, Blue, Green };
            for (int i = 0; i < groups.Count; i++)
            {
                int y = 56 + i * 40;
                int n = groups[i].Count();
                string name = string.IsNullOrWhiteSpace(groups[i].Key) ? "(none)" : groups[i].Key;
                mix.Controls.Add(new Label { Text = name, AutoSize = true, ForeColor = DeepNavy, Location = new Point(20, y) });
                mix.Controls.Add(new Panel { BackColor = Color.FromArgb(240, 241, 245), Location = new Point(130, y + 4), Size = new Size(560, 14) });
                var fill = new Panel { BackColor = cols[i % cols.Length], Location = new Point(130, y + 4), Size = new Size(560 * n / total, 14) };
                mix.Controls.Add(fill);
                fill.BringToFront();
                mix.Controls.Add(new Label { Text = n.ToString(), AutoSize = true, ForeColor = Muted, Location = new Point(706, y) });
            }
            if (groups.Count == 0)
                mix.Controls.Add(new Label { Text = "No tenant data loaded yet.", AutoSize = true, ForeColor = Muted, Location = new Point(20, 56) });

            pOverview.Controls.Add(mix);
            pOverview.ResumeLayout();
        }

        // =========================================================
        // SECTION 2: TENANT MANAGEMENT
        // =========================================================

        private IEnumerable<TenantDto> Filtered()
        {
            string q = txtSearch.Text.Trim();
            string f = (string)cboStatus.SelectedItem;
            return _tenants.Where(t =>
                (f == "All" || string.Equals(t.ActiveStatus, f, StringComparison.OrdinalIgnoreCase)) &&
                (q.Length == 0 ||
                 (t.OrganizationName ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                 (t.PlanTier ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        private TenantDto SelectedTenant() => dgvTenants.CurrentRow?.Tag as TenantDto;

        private void BindGrid()
        {
            int? keep = SelectedTenant()?.CompanyId;
            dgvTenants.Rows.Clear();
            foreach (var t in Filtered())
            {
                int idx = dgvTenants.Rows.Add(t.CompanyId, t.OrganizationName, t.PlanTier, t.ActiveStatus,
                    _revealed.Contains(t.CompanyId) ? t.ConnectionString : "••••••");
                dgvTenants.Rows[idx].Tag = t;
                if (keep == t.CompanyId) dgvTenants.CurrentCell = dgvTenants.Rows[idx].Cells[1];
            }
            UpdateRevealText();
        }

        private void UpdateRevealText()
        {
            var t = SelectedTenant();
            btnReveal.Text = t != null && _revealed.Contains(t.CompanyId) ? "Hide connection" : "Show connection";
        }

        private void DgvTenants_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTenants.CurrentRow?.Tag is TenantDto tenant)
                SelectedCompanyId = tenant.CompanyId;
            UpdateRevealText();
        }

        private void DgvTenants_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null) return;
            if (e.ColumnIndex == 3)
            {
                bool active = string.Equals(e.Value.ToString(), "Active", StringComparison.OrdinalIgnoreCase);
                e.CellStyle.ForeColor = active ? Green : Red;
                e.CellStyle.Font = new Font("Segoe UI Semibold", 9.5F);
            }
            else if (e.ColumnIndex == 2)
            {
                e.CellStyle.Font = new Font("Segoe UI Semibold", 9.5F);
            }
        }

        private void ToggleReveal()
        {
            var t = SelectedTenant();
            if (t == null) return;
            if (!_revealed.Remove(t.CompanyId)) _revealed.Add(t.CompanyId);
            BindGrid();
        }

        // =========================================================
        // SECTION 3: INFRASTRUCTURE HEALTH (real checks against your API)
        // =========================================================

        private async Task RunHealthCheckAsync()
        {
            if (_checking) return;
            _checking = true;
            bool ok = false;
            string failText = null;
            var sw = Stopwatch.StartNew();
            try
            {
                using (var resp = await _client.GetAsync(TenantsEndpoint))
                {
                    sw.Stop();
                    ok = resp.IsSuccessStatusCode;
                    if (!ok) failText = "HTTP " + (int)resp.StatusCode;
                }
            }
            catch
            {
                sw.Stop();
                failText = "Unreachable";
            }
            _checking = false;
            if (IsDisposed) return;

            long ms = sw.ElapsedMilliseconds;

            // API row: bar = latency, scaled so 500 ms fills it
            rowApi.Level = !ok ? 2 : ms > 800 ? 1 : 0;
            rowApi.StatusText = !ok ? failText : ms > 800 ? "Slow response" : "Healthy";
            rowApi.Percent = !ok ? 100 : (int)Math.Min(100, ms / 5);
            rowApi.Detail = ok ? ms + " ms" : "no response";

            int total = _tenants.Count, active = _tenants.Count(IsActive), susp = _tenants.Count(IsSuspended);

            // Registry row: bar = share of tenants that are active
            rowRegistry.Level = total > 0 ? 0 : 1;
            rowRegistry.StatusText = total > 0 ? "Loaded" : "No tenants loaded";
            rowRegistry.Percent = total == 0 ? 0 : active * 100 / total;
            rowRegistry.Detail = active + " of " + total + " active";

            // Suspended row: bar = share of tenants suspended
            rowSuspended.Level = susp == 0 ? 0 : 1;
            rowSuspended.StatusText = susp == 0 ? "None suspended" : susp + " suspended";
            rowSuspended.Percent = total == 0 ? 0 : susp * 100 / total;
            rowSuspended.Detail = "share of tenants";

            string stamp = "   ·   checked " + DateTime.Now.ToString("HH:mm:ss");
            if (rowApi.Level == 2)
            {
                lblBanner.Text = "●  Master API unreachable" + stamp;
                lblBanner.BackColor = Color.FromArgb(254, 226, 226); lblBanner.ForeColor = Color.FromArgb(153, 27, 27);
            }
            else if (rowApi.Level == 1 || rowRegistry.Level == 1 || rowSuspended.Level == 1)
            {
                lblBanner.Text = "●  Needs attention" + stamp;
                lblBanner.BackColor = Color.FromArgb(254, 243, 199); lblBanner.ForeColor = Color.FromArgb(146, 64, 14);
            }
            else
            {
                lblBanner.Text = "●  All systems operational" + stamp;
                lblBanner.BackColor = Color.FromArgb(220, 252, 231); lblBanner.ForeColor = Color.FromArgb(22, 101, 52);
            }
            rowApi.Invalidate(); rowRegistry.Invalidate(); rowSuspended.Invalidate();
        }

        // =========================================================
        // LAYOUT
        // =========================================================

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BackColor = SoftBackground;
            this.Font = new Font("Segoe UI", 10F);

            // ---- Header + cards ----
            var top = new Panel { Dock = DockStyle.Top, Height = 190, BackColor = SoftBackground };
            top.Controls.Add(new Label
            {
                Text = "Platform Administration",
                Font = new Font("Segoe UI Semibold", 22F),
                ForeColor = DeepNavy,
                AutoSize = true,
                Location = new Point(24, 14)
            });
            top.Controls.Add(new Label
            {
                Text = "System Governance",
                ForeColor = Muted,
                AutoSize = true,
                Location = new Point(27, 62)
            });

            cOverview = new AdminNavCard("System Overview", "Platform-wide summary", Blue) { Location = new Point(24, 96), MinimumSize = new Size(240, 70), Size = new Size(260, 78) };
            cTenants = new AdminNavCard("Tenant Management", "Organizations & plans", Green) { Location = new Point(300, 96), MinimumSize = new Size(240, 70), Size = new Size(260, 78) };
            cInfra = new AdminNavCard("Infrastructure Health", "API & tenant status", Red) { Location = new Point(576, 96), MinimumSize = new Size(240, 70), Size = new Size(260, 78) };
            cOverview.Click += (s, e) => { ShowSection(1); dgvTenants.Focus(); _ = LoadTenantsAsync(); };
            cTenants.Click += (s, e) => { (Application.OpenForms.OfType<MainForm>().FirstOrDefault())?.NavigateTo("Subscriptions"); };
            cInfra.Click += (s, e) => { (Application.OpenForms.OfType<MainForm>().FirstOrDefault())?.NavigateTo("BI"); };
            top.Controls.AddRange(new Control[] { cOverview, cTenants, cInfra });

            host = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 0, 24, 16), BackColor = SoftBackground };
            pOverview = new Panel { Dock = DockStyle.Fill, BackColor = SoftBackground };
            pTenants = new Panel { Dock = DockStyle.Fill, BackColor = SoftBackground };
            pInfra = new Panel { Dock = DockStyle.Fill, BackColor = SoftBackground };
            host.Controls.AddRange(new Control[] { pOverview, pTenants, pInfra });

            // ---- Tenant grid (unbound so the connection column can be masked) ----
            dgvTenants = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
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
                ColumnHeadersHeight = 42
            };
            dgvTenants.RowTemplate.Height = 40;
            dgvTenants.Columns.Add("CompanyId", "Company ID");
            dgvTenants.Columns.Add("OrganizationName", "Organization");
            dgvTenants.Columns.Add("PlanTier", "Plan");
            dgvTenants.Columns.Add("ActiveStatus", "Status");
            dgvTenants.Columns.Add("ConnectionString", "Connection");
            dgvTenants.Columns[0].FillWeight = 14;

            dgvTenants.ColumnHeadersDefaultCellStyle.BackColor = DeepNavy;
            dgvTenants.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTenants.ColumnHeadersDefaultCellStyle.SelectionBackColor = DeepNavy;
            dgvTenants.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F);
            dgvTenants.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvTenants.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            dgvTenants.DefaultCellStyle.BackColor = WhiteCard;
            dgvTenants.DefaultCellStyle.SelectionBackColor = SelectionBg;
            dgvTenants.DefaultCellStyle.SelectionForeColor = DeepNavy;
            dgvTenants.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            dgvTenants.AlternatingRowsDefaultCellStyle.BackColor = RowAlt;
            dgvTenants.SelectionChanged += DgvTenants_SelectionChanged;
            dgvTenants.CellFormatting += DgvTenants_CellFormatting;

            var toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = SoftBackground };
            txtSearch = new TextBox
            {
                Location = new Point(0, 9),
                Width = 240,
                Font = new Font("Segoe UI", 10.5F),
                PlaceholderText = "Search organization or plan"
            };
            txtSearch.TextChanged += (s, e) => BindGrid();
            cboStatus = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(250, 9),
                Width = 120,
                Font = new Font("Segoe UI", 10.5F)
            };
            cboStatus.Items.AddRange(new object[] { "All", "Active", "Suspended" });
            cboStatus.SelectedIndex = 0;
            cboStatus.SelectedIndexChanged += (s, e) => BindGrid();
            btnRefresh = MakeButton("Refresh", DeepNavy, 100, 385);
            btnReveal = MakeButton("Hide", Red, 150, 495);
            btnRefresh.Click += async (s, e) => await LoadTenantsAsync();
            btnReveal.Click += (s, e) => { dgvTenants.ClearSelection(); pTenants.Visible = !pTenants.Visible; };
            toolbar.Controls.AddRange(new Control[] { txtSearch, cboStatus, btnRefresh, btnReveal });

            pTenants.Controls.Add(dgvTenants);
            pTenants.Controls.Add(toolbar);

            // ---- Infrastructure rows (Dock.Top stacks last-added on top, so add bottom row first) ----
            rowApi = new AdminHealthRow { ServiceName = "Master API", Dock = DockStyle.Top };
            rowRegistry = new AdminHealthRow { ServiceName = "Tenant Registry", Dock = DockStyle.Top };
            rowSuspended = new AdminHealthRow { ServiceName = "Suspended Tenants", Dock = DockStyle.Top };
            lblBanner = new Label
            {
                Dock = DockStyle.Top,
                Height = 46,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Font = new Font("Segoe UI Semibold", 11F),
                Text = "●  Checking...",
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = DeepNavy
            };
            pInfra.Controls.Add(rowSuspended);
            pInfra.Controls.Add(rowRegistry);
            pInfra.Controls.Add(rowApi);
            pInfra.Controls.Add(lblBanner);
            _healthTimer.Tick += async (s, e) => await RunHealthCheckAsync();

            this.Controls.Add(host);   // Fill first, then Top, so docking resolves correctly
            this.Controls.Add(top);

            this.ClientSize = new Size(960, 640);
            this.MinimumSize = new Size(900, 500);
            this.Name = "AdminPanelForm";
            this.Text = "InnEase CRM - Platform Administration";
            this.ResumeLayout(false);
        }

        private static Button MakeButton(string text, Color back, int width, int x)
        {
            var b = new Button
            {
                Text = text,
                Width = width,
                Height = 34,
                Location = new Point(x, 7),
                FlatStyle = FlatStyle.Flat,
                BackColor = back,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI Semibold", 9.5F)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // _client is the shared ApiClient.Instance used across the whole app —
                // never dispose it here, or every other form's API calls will start
                // throwing ObjectDisposedException the moment this form closes.
                _healthTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // =============================================================
    // Custom controls (prefixed so they can't clash with other files)
    // =============================================================

    internal static class AdminGfx
    {
        public static GraphicsPath Round(Rectangle r, int radius)
        {
            int d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static Color Tint(Color c, double whiteWeight) => Color.FromArgb(
            (int)(c.R * (1 - whiteWeight) + 255 * whiteWeight),
            (int)(c.G * (1 - whiteWeight) + 255 * whiteWeight),
            (int)(c.B * (1 - whiteWeight) + 255 * whiteWeight));
    }

    // Clickable card with hover + selected states
    internal class AdminNavCard : Control
    {
        private readonly string _title, _sub;
        private readonly Color _accent;
        private bool _active, _hover;
        private static readonly Font TitleFont = new Font("Segoe UI Semibold", 12F);
        private static readonly Font SubFont = new Font("Segoe UI", 9F);
        private static readonly Color Ink = ColorTranslator.FromHtml("#1E1B4B");
        private static readonly Color Muted = ColorTranslator.FromHtml("#6B7280");
        private static readonly Color Line = ColorTranslator.FromHtml("#E2E8F0");

        public AdminNavCard(string title, string sub, Color accent)
        {
            _title = title; _sub = sub; _accent = accent;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Size = new Size(260, 78);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Active { get => _active; set { _active = value; Invalidate(); } }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(1, 1, Width - 3, Height - 3);
            Color fill = _active ? AdminGfx.Tint(_accent, 0.88) : _hover ? Color.FromArgb(243, 244, 246) : Color.White;
            using (var path = AdminGfx.Round(r, 12))
            using (var br = new SolidBrush(fill))
            using (var pen = new Pen(_active ? _accent : Line, _active ? 2F : 1F))
            {
                g.FillPath(br, path);
                g.DrawPath(pen, path);
            }
            using (var bar = new SolidBrush(_accent))
            using (var bp = AdminGfx.Round(new Rectangle(16, 18, 6, Height - 37), 3))
                g.FillPath(bar, bp);
            using (var ink = new SolidBrush(Ink)) g.DrawString(_title, TitleFont, ink, 32, 16);
            using (var mu = new SolidBrush(Muted)) g.DrawString(_sub, SubFont, mu, 33, 44);
        }
    }

    // One status row: dot, name, status text, bar, percent, detail
    internal class AdminHealthRow : Control
    {
        public string ServiceName = "";
        public string StatusText = "Checking...";
        public string Detail = "";
        public int Percent;
        public int Level;   // 0 = ok, 1 = warning, 2 = down

        private static readonly Font NameFont = new Font("Segoe UI Semibold", 11F);
        private static readonly Font SmallFont = new Font("Segoe UI", 9F);
        private static readonly Color Ink = ColorTranslator.FromHtml("#1E1B4B");
        private static readonly Color Muted = ColorTranslator.FromHtml("#6B7280");
        private static readonly Color Line = ColorTranslator.FromHtml("#E2E8F0");
        private static readonly Color Bg = ColorTranslator.FromHtml("#F8FAFC");

        public AdminHealthRow()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Height = 64;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Bg);
            var r = new Rectangle(0, 4, Width - 1, Height - 9);
            Color c = Level == 0 ? ColorTranslator.FromHtml("#10B981")
                    : Level == 1 ? ColorTranslator.FromHtml("#F59E0B")
                    : ColorTranslator.FromHtml("#EF4444");

            using (var p = AdminGfx.Round(r, 10))
            using (var pen = new Pen(Line))
            {
                g.FillPath(Brushes.White, p);
                g.DrawPath(pen, p);
            }
            using (var b = new SolidBrush(c)) g.FillEllipse(b, 18, Height / 2 - 6, 12, 12);
            using (var ink = new SolidBrush(Ink)) g.DrawString(ServiceName, NameFont, ink, 42, r.Y + 6);
            using (var col = new SolidBrush(c)) g.DrawString(StatusText, SmallFont, col, 43, r.Y + 27);

            int bx = 260, bw = Math.Max(60, Width - 500);
            int pct = Math.Max(0, Math.Min(100, Percent));
            using (var track = AdminGfx.Round(new Rectangle(bx, Height / 2 - 4, bw, 8), 4))
            using (var tb = new SolidBrush(Color.FromArgb(229, 231, 235)))
                g.FillPath(tb, track);
            if (pct > 0)
            {
                using (var fp = AdminGfx.Round(new Rectangle(bx, Height / 2 - 4, Math.Max(8, bw * pct / 100), 8), 4))
                using (var fb = new SolidBrush(c))
                    g.FillPath(fb, fp);
            }

            using (var mu = new SolidBrush(Muted))
            {
                g.DrawString(pct + "%", SmallFont, mu, bx + bw + 12, Height / 2 - 9);
                var fmt = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(Detail, SmallFont, mu, new RectangleF(Width - 170, 0, 150, Height), fmt);
            }
        }
    }
}
