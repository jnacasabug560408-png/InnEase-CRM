using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
// removed alias

namespace InnEase_CRM
{
    // =========================================================
    // API Client for shared JWT authentication
    // =========================================================
    public static class ApiClient
    {
        public static readonly HttpClient Instance = new HttpClient { BaseAddress = new Uri("https://localhost:7129/") };

        public static void Configure(string token, int? tenantId = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                Instance.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Instance.DefaultRequestHeaders.Authorization = null;
            }

            Instance.DefaultRequestHeaders.Remove("TenantId");
            if (tenantId.HasValue)
            {
                Instance.DefaultRequestHeaders.Add("TenantId", tenantId.Value.ToString());
            }
        }

        /// <summary>
        /// Parses HTTP error responses and returns a user-friendly error message
        /// </summary>
        public static async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
        {
            try
            {
                string content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    return $"HTTP {response.StatusCode}: {response.ReasonPhrase}";
                }

                // Try to parse as JSON
                try
                {
                    using (var doc = JsonDocument.Parse(content))
                    {
                        if (doc.RootElement.ValueKind == JsonValueKind.Object)
                        {
                            // Look for common error properties
                            if (doc.RootElement.TryGetProperty("message", out var messageProp))
                            {
                                var message = messageProp.GetString();
                                if (doc.RootElement.TryGetProperty("detail", out var detailProp))
                                {
                                    return $"{message}\n\nDetails: {detailProp.GetString()}";
                                }
                                return message;
                            }
                            if (doc.RootElement.TryGetProperty("error", out var errorProp))
                            {
                                return errorProp.GetString();
                            }
                            if (doc.RootElement.TryGetProperty("title", out var titleProp))
                            {
                                var title = titleProp.GetString();
                                if (doc.RootElement.TryGetProperty("errors", out var errorsProp) && errorsProp.ValueKind == JsonValueKind.Object)
                                {
                                    var errors = new System.Text.StringBuilder();
                                    foreach (var error in errorsProp.EnumerateObject())
                                    {
                                        var errorArray = error.Value;
                                        if (errorArray.ValueKind == JsonValueKind.Array)
                                        {
                                            foreach (var err in errorArray.EnumerateArray())
                                            {
                                                errors.AppendLine($"  • {err.GetString()}");
                                            }
                                        }
                                    }
                                    return $"{title}\n\n{errors}";
                                }
                                return title;
                            }
                        }
                    }
                }
                catch
                {
                    // JSON parsing failed, return raw content
                }

                return $"HTTP {response.StatusCode}: {content}";
            }
            catch
            {
                return $"HTTP {response.StatusCode}: {response.ReasonPhrase}";
            }
        }
    }

    // =========================================================
    // DTOs
    // =========================================================
    public class TenantDto
    {
        public int CompanyId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string PlanTier { get; set; } = string.Empty;
        public string ActiveStatus { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
    }

    public class BiSnapshot
    {
        public int Active;
        public int Suspended;
        public Dictionary<string, int> Tiers = new Dictionary<string, int>();
        public bool FromServer;   // true when /api/master/bi supplied the figures
    }

    // =========================================================
    // Navigation hook: MainForm subscribes once, controls just call AppNav.Go("Admin" | "BI" | "Subscriptions")
    // =========================================================
    public static class AppNav
    {
        public static event System.Action<string> Requested;
        public static void Go(string page) { Requested?.Invoke(page); }
    }

    // =========================================================
    // One shared data store so all three screens stay in sync
    // =========================================================
    public static class TenantStore
    {
        private static HttpClient Client => ApiClient.Instance;
        private static Task _refresh;
        private static List<TenantDto> _tenants = new List<TenantDto>();

        public static IReadOnlyList<TenantDto> Tenants { get { return _tenants; } }
        public static event EventHandler Changed;

        // Shared status filter ("All" | "Active" | "Suspended"); BI cards set it, Admin Panel applies it.
        public static string StatusFilter { get; private set; } = "All";
        public static event EventHandler FilterChanged;

        public static void SetStatusFilter(string value)
        {
            if (string.Equals(StatusFilter, value, StringComparison.OrdinalIgnoreCase)) return;
            StatusFilter = value;
            FilterChanged?.Invoke(null, EventArgs.Empty);
        }

        public static bool IsActive(TenantDto t) =>
            string.Equals(t.ActiveStatus, "Active", StringComparison.OrdinalIgnoreCase);

        public static bool IsSuspended(TenantDto t) =>
            string.Equals(t.ActiveStatus, "Suspended", StringComparison.OrdinalIgnoreCase);

        // GET /api/master/tenants  (concurrent callers share one request)
        public static Task RefreshAsync()
        {
            if (_refresh != null && !_refresh.IsCompleted) return _refresh;
            _refresh = DoRefreshAsync();
            return _refresh;
        }

        private static async Task DoRefreshAsync()
        {
            var list = await Client.GetFromJsonAsync<List<TenantDto>>("api/master/tenants");
            _tenants = list ?? new List<TenantDto>();
            Changed?.Invoke(null, EventArgs.Empty);
        }

        // PUT /api/master/tenants/{id}/status
        // ASSUMPTION: body is a JSON object. Both "status" and "activeStatus" are sent (extra properties are
        // ignored by ASP.NET Core), so it works whichever name your request model uses. Adjust if yours differs.
        public static async Task SetStatusAsync(int companyId, string status)
        {
            using (var resp = await Client.PutAsJsonAsync("api/master/tenants/" + companyId + "/status",
                new { status = status, activeStatus = status }))
            {
                await EnsureOkAsync(resp);
            }
            var t = _tenants.FirstOrDefault(x => x.CompanyId == companyId);
            if (t != null) t.ActiveStatus = status;
            Changed?.Invoke(null, EventArgs.Empty);
        }

        // PUT /api/master/tenants/{id}/plan   (same assumption: "plan" and "planTier" are both sent)
        public static async Task SetPlanAsync(int companyId, string plan)
        {
            using (var resp = await Client.PutAsJsonAsync("api/master/tenants/" + companyId + "/plan",
                new { plan = plan, planTier = plan }))
            {
                await EnsureOkAsync(resp);
            }
            var t = _tenants.FirstOrDefault(x => x.CompanyId == companyId);
            if (t != null) t.PlanTier = plan;
            Changed?.Invoke(null, EventArgs.Empty);
        }

        private static async Task EnsureOkAsync(HttpResponseMessage resp)
        {
            if (resp.IsSuccessStatusCode) return;
            string body = "";
            try { body = await resp.Content.ReadAsStringAsync(); } catch { }
            throw new HttpRequestException((int)resp.StatusCode + " " + resp.ReasonPhrase +
                (string.IsNullOrWhiteSpace(body) ? "" : ": " + body));
        }

        // ---------- BI ----------
        public static BiSnapshot LocalSnapshot()
        {
            var s = new BiSnapshot
            {
                Active = _tenants.Count(IsActive),
                Suspended = _tenants.Count(IsSuspended)
            };
            foreach (var g in _tenants.GroupBy(t => string.IsNullOrWhiteSpace(t.PlanTier) ? "(none)" : t.PlanTier))
                s.Tiers[g.Key] = g.Count();
            return s;
        }

        // GET /api/master/tenants then GET /api/master/bi. The BI JSON shape is unknown, so parsing is tolerant;
        // anything it can't read falls back to figures computed from the tenant list.
        public static async Task<BiSnapshot> GetBiAsync()
        {
            await RefreshAsync();
            var snap = LocalSnapshot();
            try
            {
                using (var resp = await Client.GetAsync("api/master/bi"))
                {
                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();
                        using (var doc = JsonDocument.Parse(json))
                            snap.FromServer = ApplyServerBi(doc.RootElement, snap);
                    }
                }
            }
            catch { /* endpoint missing or different shape: keep local figures */ }
            return snap;
        }

        private static readonly string[] TierKeys = { "tiers", "subscriptionTiers", "planCounts", "tierCounts", "plans" };

        private static bool ApplyServerBi(JsonElement root, BiSnapshot snap)
        {
            if (root.ValueKind != JsonValueKind.Object) return false;
            bool any = false;
            int a, s;
            if (TryInt(root, out a, "totalActiveTenants", "activeTenants", "active")) { snap.Active = a; any = true; }
            if (TryInt(root, out s, "suspendedTenants", "totalSuspendedTenants", "suspended")) { snap.Suspended = s; any = true; }

            foreach (var p in root.EnumerateObject())
            {
                if (!TierKeys.Contains(p.Name, StringComparer.OrdinalIgnoreCase)) continue;
                var tiers = new Dictionary<string, int>();
                if (p.Value.ValueKind == JsonValueKind.Object)
                {
                    foreach (var q in p.Value.EnumerateObject())
                    {
                        int n;
                        if (q.Value.ValueKind == JsonValueKind.Number && q.Value.TryGetInt32(out n)) tiers[q.Name] = n;
                    }
                }
                else if (p.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in p.Value.EnumerateArray())
                    {
                        string name = TryStr(el, "tier", "plan", "planTier", "name");
                        int c;
                        if (name != null && TryInt(el, out c, "count", "total", "tenants")) tiers[name] = c;
                    }
                }
                if (tiers.Count > 0) { snap.Tiers = tiers; any = true; }
            }
            return any;
        }

        private static bool TryInt(JsonElement o, out int value, params string[] names)
        {
            value = 0;
            if (o.ValueKind != JsonValueKind.Object) return false;
            foreach (var p in o.EnumerateObject())
            {
                bool match = names.Any(n => string.Equals(n, p.Name, StringComparison.OrdinalIgnoreCase));
                if (match && p.Value.ValueKind == JsonValueKind.Number && p.Value.TryGetInt32(out value)) return true;
            }
            return false;
        }

        private static string TryStr(JsonElement o, params string[] names)
        {
            if (o.ValueKind != JsonValueKind.Object) return null;
            foreach (var p in o.EnumerateObject())
            {
                bool match = names.Any(n => string.Equals(n, p.Name, StringComparison.OrdinalIgnoreCase));
                if (match && p.Value.ValueKind == JsonValueKind.String) return p.Value.GetString();
            }
            return null;
        }
    }

    // =========================================================
    // Theme + helpers
    // =========================================================
    internal static class Theme
    {
        public static readonly Color Ink = ColorTranslator.FromHtml("#1E1B4B");
        public static readonly Color Brand = ColorTranslator.FromHtml("#831843");
        public static readonly Color Bg = ColorTranslator.FromHtml("#F8FAFC");
        public static readonly Color Line = ColorTranslator.FromHtml("#E2E8F0");
        public static readonly Color RowAlt = ColorTranslator.FromHtml("#F1F5F9");
        public static readonly Color Selection = ColorTranslator.FromHtml("#EDE9FE");
        public static readonly Color Muted = ColorTranslator.FromHtml("#6B7280");
        public static readonly Color Blue = ColorTranslator.FromHtml("#3B82F6");
        public static readonly Color Green = ColorTranslator.FromHtml("#10B981");
        public static readonly Color Red = ColorTranslator.FromHtml("#EF4444");
        public static readonly Color Amber = ColorTranslator.FromHtml("#F59E0B");

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

        public static Color Tint(Color c, double whiteWeight)
        {
            return Color.FromArgb(
                (int)(c.R * (1 - whiteWeight) + 255 * whiteWeight),
                (int)(c.G * (1 - whiteWeight) + 255 * whiteWeight),
                (int)(c.B * (1 - whiteWeight) + 255 * whiteWeight));
        }

        public static Button MakeButton(string text, Color back, int width)
        {
            var b = new Button
            {
                Text = text, Width = width, Height = 38,
                FlatStyle = FlatStyle.Flat, BackColor = back, ForeColor = Color.White,
                UseVisualStyleBackColor = false, Cursor = Cursors.Hand,
                Font = new Font("Segoe UI Semibold", 10F), TextAlign = ContentAlignment.MiddleCenter
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        // Fill-mode grid; the empty area below the rows uses the page background so there is no white gap.
        public static void StyleGrid(DataGridView g)
        {
            g.Dock = DockStyle.Fill;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = Bg;
            g.EnableHeadersVisualStyles = false;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.GridColor = Line;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 42;
            g.RowTemplate.Height = 40;

            g.ColumnHeadersDefaultCellStyle.BackColor = Ink;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Ink;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F);
            g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);

            g.DefaultCellStyle.BackColor = Color.White;
            g.DefaultCellStyle.ForeColor = Ink;
            g.DefaultCellStyle.SelectionBackColor = Selection;
            g.DefaultCellStyle.SelectionForeColor = Ink;
            g.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            g.AlternatingRowsDefaultCellStyle.BackColor = RowAlt;
        }

        private static readonly Font BoldCell = new Font("Segoe UI Semibold", 9.5F);

        // Colors the Plan (bold) and Status (green/red) columns
        public static void FormatCell(DataGridViewCellFormattingEventArgs e, int planCol, int statusCol)
        {
            if (e.RowIndex < 0 || e.Value == null) return;
            if (e.ColumnIndex == statusCol)
            {
                bool active = string.Equals(e.Value.ToString(), "Active", StringComparison.OrdinalIgnoreCase);
                e.CellStyle.ForeColor = active ? Green : Red;
                e.CellStyle.Font = BoldCell;
            }
            else if (e.ColumnIndex == planCol)
            {
                e.CellStyle.Font = BoldCell;
            }
        }
    }

    // =========================================================
    // Card: title + optional subtitle / big number / list of lines.
    // Height is computed from the content, so nothing is ever clipped.
    // =========================================================
    internal class TileCard : Control
    {
        private string _title = "", _sub, _big;
        private string[] _lines;
        private readonly Color _accent;
        private readonly bool _clickable;
        private bool _hover, _selected;

        private static readonly Font TitleFont = new Font("Segoe UI Semibold", 11.5F);
        private static readonly Font SubFont = new Font("Segoe UI", 9F);
        private static readonly Font BigFont = new Font("Segoe UI Semibold", 24F);
        private static readonly Font LineFont = new Font("Segoe UI Semibold", 10.5F);

        public TileCard(Color accent, bool clickable)
        {
            _accent = accent;
            _clickable = clickable;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            MinimumSize = new Size(240, 70);
            Size = new Size(260, 70);
            if (clickable) Cursor = Cursors.Hand;
        }

        public void SetContent(string title, string subtitle, string bigValue, string[] lines)
        {
            _title = title ?? ""; _sub = subtitle; _big = bigValue; _lines = lines;
            int h = 12 + 26;
            if (!string.IsNullOrEmpty(_sub)) h += 22;
            if (!string.IsNullOrEmpty(_big)) h += 48;
            if (_lines != null && _lines.Length > 0) h += _lines.Length * 24 + 4;
            h += 14;
            Height = Math.Max(MinimumSize.Height, h);
            Invalidate();
        }

        public void SetSelected(bool selected) { _selected = selected; Invalidate(); }

        protected override void OnMouseEnter(EventArgs e) { _hover = _clickable; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var r = new Rectangle(1, 1, Width - 3, Height - 3);
            Color fill = _selected ? Theme.Tint(_accent, 0.88) : _hover ? Color.FromArgb(243, 244, 246) : Color.White;
            using (var path = Theme.Round(r, 12))
            using (var br = new SolidBrush(fill))
            using (var pen = new Pen(_selected ? _accent : Theme.Line, _selected ? 2F : 1F))
            {
                g.FillPath(br, path);
                g.DrawPath(pen, path);
            }
            using (var bar = new SolidBrush(_accent))
            using (var bp = Theme.Round(new Rectangle(14, 14, 6, Height - 29), 3))
                g.FillPath(bar, bp);

            using (var ink = new SolidBrush(Theme.Ink))
            using (var mu = new SolidBrush(Theme.Muted))
            {
                int y = 12;
                g.DrawString(_title, TitleFont, ink, 30, y); y += 26;
                if (!string.IsNullOrEmpty(_sub)) { g.DrawString(_sub, SubFont, mu, 31, y); y += 22; }
                if (!string.IsNullOrEmpty(_big)) { g.DrawString(_big, BigFont, ink, 28, y - 2); y += 48; }
                if (_lines != null)
                    foreach (var line in _lines) { g.DrawString(line, LineFont, ink, 31, y); y += 24; }
            }
        }
    }
}
