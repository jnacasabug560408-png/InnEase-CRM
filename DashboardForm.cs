#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;
using SystemAction = System.Action;

namespace InnEase_CRM
{
    // =========================================================
    // THEME + HELPERS
    // =========================================================

    internal static class DashTheme
    {
        public static readonly Color PageBg = Color.FromArgb(244, 245, 252);
        public static readonly Color Sidebar = Color.FromArgb(40, 32, 92);
        public static readonly Color SidebarHover = Color.FromArgb(58, 48, 118);
        public static readonly Color Accent = Color.FromArgb(146, 30, 98);
        public static readonly Color Ink = Color.FromArgb(30, 34, 58);
        public static readonly Color Muted = Color.FromArgb(112, 118, 142);
        public static readonly Color Border = Color.FromArgb(224, 227, 240);
        public static readonly Color Grid = Color.FromArgb(232, 234, 244);

        public static readonly Color Magenta = Color.FromArgb(176, 64, 160);
        public static readonly Color Blue = Color.FromArgb(30, 136, 229);
        public static readonly Color Teal = Color.FromArgb(38, 150, 130);
        public static readonly Color Amber = Color.FromArgb(226, 164, 52);
        public static readonly Color Violet = Color.FromArgb(108, 92, 231);
        public static readonly Color Rose = Color.FromArgb(214, 80, 110);
        public static readonly Color Green = Color.FromArgb(34, 160, 90);

        public static readonly Color[] Palette =
        {
            Magenta, Blue, Teal, Amber, Violet, Rose
        };

        // DPI-aware pixel scaling
        public static int S(Control c, int v)
        {
            return (int)Math.Round(v * c.DeviceDpi / 96.0);
        }

        public static string Peso(double v)
        {
            if (v >= 1000000) return "\u20B1" + (v / 1000000).ToString("0.#", CultureInfo.InvariantCulture) + "M";
            if (v >= 1000) return "\u20B1" + (v / 1000).ToString("0.#", CultureInfo.InvariantCulture) + "K";
            return "\u20B1" + v.ToString("0", CultureInfo.InvariantCulture);
        }
    }

    // =========================================================
    // KPI CARD (custom painted, DPI safe, clickable with hover effects)
    // =========================================================

    internal class DashKpiCard : Control
    {
        private readonly string _title;
        private readonly string _value;
        private readonly string _sub;
        private readonly Color _accent;

        private readonly Font _fTitle = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        private readonly Font _fValue = new Font("Segoe UI Semibold", 17F);
        private readonly Font _fSub = new Font("Segoe UI", 8F);

        private bool _isHovering;
        private Color _backgroundColor = Color.White;
        private Color _borderColor = DashTheme.Border;

        public DashKpiCard(string title, string value, string sub, Color accent)
        {
            _title = title;
            _value = value;
            _sub = sub;
            _accent = accent;

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardDoubleClick, true);

            // Enable hover effects
            MouseEnter += (s, e) =>
            {
                _isHovering = true;
                _backgroundColor = Color.FromArgb(248, 250, 255);
                _borderColor = _accent;
                Invalidate();
            };

            MouseLeave += (s, e) =>
            {
                _isHovering = false;
                _backgroundColor = Color.White;
                _borderColor = DashTheme.Border;
                Invalidate();
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(_backgroundColor);

            using (var pen = new Pen(_borderColor, _isHovering ? 2f : 1f))
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            int bar = DashTheme.S(this, 4);
            using (var b = new SolidBrush(_accent))
                g.FillRectangle(b, 0, 0, bar, Height);

            int x = DashTheme.S(this, 16);
            int w = Math.Max(10, Width - x - DashTheme.S(this, 8));

            const TextFormatFlags f =
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix |
                TextFormatFlags.SingleLine;

            TextRenderer.DrawText(g, _title, _fTitle,
                new Rectangle(x, DashTheme.S(this, 8), w, DashTheme.S(this, 20)),
                DashTheme.Muted, f);

            TextRenderer.DrawText(g, _value, _fValue,
                new Rectangle(x, DashTheme.S(this, 28), w, DashTheme.S(this, 36)),
                DashTheme.Ink, f);

            TextRenderer.DrawText(g, _sub, _fSub,
                new Rectangle(x, DashTheme.S(this, 64), w, DashTheme.S(this, 20)),
                DashTheme.Muted, f);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fTitle.Dispose();
                _fValue.Dispose();
                _fSub.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // =========================================================
    // CHART CARD (column / horizontal bar / area line / donut)
    // =========================================================

    internal enum DashChartKind
    {
        Column,
        HBar,
        Area,
        Donut
    }

    internal class DashChartCard : Control
    {
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title { get; set; } = "";
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DashChartKind Kind { get; set; } = DashChartKind.Column;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<KeyValuePair<string, double>> Data { get; } = new List<KeyValuePair<string, double>>();
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<double, string> Format { get; set; } = v => v.ToString("0.#", CultureInfo.InvariantCulture);
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SingleColor { get; set; }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BaseColor { get; set; } = DashTheme.Magenta;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IntegerAxis { get; set; } = true;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EmptyText { get; set; } = "No data for the selected period";

        private readonly Font _fTitle = new Font("Segoe UI Semibold", 10.5F);
        private readonly Font _fSmall = new Font("Segoe UI", 8.5F);
        private readonly Font _fBold = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        private readonly Font _fBig = new Font("Segoe UI Semibold", 15F);

        private const TextFormatFlags CenterFlags =
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
            TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

        private const TextFormatFlags LabelFlags =
            TextFormatFlags.HorizontalCenter | TextFormatFlags.Top |
            TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis |
            TextFormatFlags.NoPrefix;

        private const TextFormatFlags RightFlags =
            TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
            TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

        private const TextFormatFlags LeftFlags =
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
            TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine |
            TextFormatFlags.EndEllipsis;

        public DashChartCard()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
        }

        private int S(int v)
        {
            return DashTheme.S(this, v);
        }

        private Color ColorAt(int i)
        {
            return SingleColor ? BaseColor : DashTheme.Palette[i % DashTheme.Palette.Length];
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            using (var pen = new Pen(DashTheme.Border))
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            TextRenderer.DrawText(g, Title, _fTitle,
                new Rectangle(S(14), S(8), Width - S(28), S(26)),
                DashTheme.Ink,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine |
                TextFormatFlags.EndEllipsis);

            var area = new Rectangle(S(14), S(40), Width - S(28), Height - S(40) - S(12));

            bool empty = Data.Count == 0 ||
                         (Kind == DashChartKind.Donut && Data.Sum(d => d.Value) <= 0);

            if (empty)
            {
                TextRenderer.DrawText(g, EmptyText, _fSmall,
                    new Rectangle(0, S(40), Width, Math.Max(10, Height - S(40))),
                    DashTheme.Muted, CenterFlags);
                return;
            }

            switch (Kind)
            {
                case DashChartKind.Column: DrawColumns(g, area); break;
                case DashChartKind.HBar: DrawHBars(g, area); break;
                case DashChartKind.Area: DrawArea(g, area); break;
                case DashChartKind.Donut: DrawDonut(g, area); break;
            }
        }

        // ---------- axis helpers ----------

        private void Axis(double max, out double step, out double top)
        {
            if (max <= 0)
            {
                step = 1;
                top = 4;
                return;
            }

            double raw = max / 4.0;
            double exp = Math.Pow(10, Math.Floor(Math.Log10(raw)));
            double f = raw / exp;
            double nice = f <= 1 ? 1 : f <= 2 ? 2 : f <= 5 ? 5 : 10;

            step = nice * exp;
            if (IntegerAxis && step < 1) step = 1;

            top = Math.Ceiling(max / step - 1e-9) * step;
        }

        private void DrawGrid(Graphics g, Rectangle a, Rectangle plot, double step, int ticks, int axisW)
        {
            using (var pen = new Pen(DashTheme.Grid))
            {
                for (int i = 0; i <= ticks; i++)
                {
                    float y = plot.Bottom - (float)plot.Height * i / ticks;
                    g.DrawLine(pen, plot.Left, y, plot.Right, y);

                    TextRenderer.DrawText(g, Format(i * step), _fSmall,
                        new Rectangle(a.Left, (int)y - S(8), axisW - S(6), S(16)),
                        DashTheme.Muted, RightFlags);
                }
            }
        }

        // ---------- COLUMN ----------

        private void DrawColumns(Graphics g, Rectangle a)
        {
            double max = Data.Max(d => d.Value);
            Axis(max, out double step, out double top);
            int ticks = Math.Max(1, (int)Math.Round(top / step));

            int axisW = TextRenderer.MeasureText(Format(top), _fSmall).Width + S(10);
            int bottomH = S(36);

            var plot = new Rectangle(
                a.Left + axisW,
                a.Top + S(16),
                a.Width - axisW,
                a.Height - S(16) - bottomH);

            if (plot.Width < S(30) || plot.Height < S(30)) return;

            DrawGrid(g, a, plot, step, ticks, axisW);

            float slot = (float)plot.Width / Data.Count;
            float bw = Math.Min(slot * 0.5f, S(58));

            for (int i = 0; i < Data.Count; i++)
            {
                double v = Data[i].Value;
                float h = (float)(v / top * plot.Height);
                float slotLeft = plot.Left + slot * i;
                float x = slotLeft + (slot - bw) / 2f;

                if (h >= 1f)
                {
                    using (var br = new SolidBrush(ColorAt(i)))
                        g.FillRectangle(br, x, plot.Bottom - h, bw, h);
                }

                TextRenderer.DrawText(g, Format(v), _fBold,
                    new Rectangle((int)slotLeft, (int)(plot.Bottom - h) - S(18), (int)slot, S(16)),
                    DashTheme.Ink, CenterFlags);

                TextRenderer.DrawText(g, Data[i].Key, _fSmall,
                    new Rectangle((int)slotLeft + S(2), plot.Bottom + S(4), (int)slot - S(4), bottomH - S(4)),
                    DashTheme.Muted, LabelFlags);
            }
        }

        // ---------- HORIZONTAL BARS ----------

        private void DrawHBars(Graphics g, Rectangle a)
        {
            double max = Math.Max(1, Data.Max(d => d.Value));

            int labelW = Math.Min(
                Data.Max(d => TextRenderer.MeasureText(d.Key, _fSmall).Width) + S(12),
                (int)(a.Width * 0.4));

            int valueW = S(56);
            int barMax = a.Width - labelW - valueW;
            if (barMax < S(20)) return;

            int n = Data.Count;
            float rowH = (float)a.Height / n;
            float barH = Math.Min(rowH * 0.55f, S(20));

            for (int i = 0; i < n; i++)
            {
                float rowTop = a.Top + rowH * i;
                float y = rowTop + (rowH - barH) / 2f;

                TextRenderer.DrawText(g, Data[i].Key, _fSmall,
                    new Rectangle(a.Left, (int)rowTop, labelW, (int)rowH),
                    DashTheme.Muted, LeftFlags);

                float w = Math.Max(2f, (float)(Data[i].Value / max * barMax));

                using (var br = new SolidBrush(ColorAt(i)))
                    g.FillRectangle(br, a.Left + labelW, y, w, barH);

                TextRenderer.DrawText(g, Format(Data[i].Value), _fBold,
                    new Rectangle((int)(a.Left + labelW + w + S(6)), (int)rowTop, valueW, (int)rowH),
                    DashTheme.Ink, LeftFlags);
            }
        }

        // ---------- AREA LINE ----------

        private void DrawArea(Graphics g, Rectangle a)
        {
            double max = Data.Max(d => d.Value);
            Axis(max, out double step, out double top);
            int ticks = Math.Max(1, (int)Math.Round(top / step));

            int axisW = TextRenderer.MeasureText(Format(top), _fSmall).Width + S(10);
            int bottomH = S(26);

            var plot = new Rectangle(
                a.Left + axisW,
                a.Top + S(16),
                a.Width - axisW,
                a.Height - S(16) - bottomH);

            if (plot.Width < S(60) || plot.Height < S(30)) return;

            DrawGrid(g, a, plot, step, ticks, axisW);

            int n = Data.Count;
            float padX = S(18);
            float x0 = plot.Left + padX;
            float x1 = plot.Right - padX;
            float stepX = n > 1 ? (x1 - x0) / (n - 1) : 0f;

            var pts = new PointF[n];
            for (int i = 0; i < n; i++)
            {
                float x = n > 1 ? x0 + stepX * i : (plot.Left + plot.Right) / 2f;
                float y = plot.Bottom - (float)(Data[i].Value / top * plot.Height);
                pts[i] = new PointF(x, y);
            }

            Color c = BaseColor;

            if (n > 1)
            {
                var poly = new PointF[n + 2];
                Array.Copy(pts, poly, n);
                poly[n] = new PointF(pts[n - 1].X, plot.Bottom);
                poly[n + 1] = new PointF(pts[0].X, plot.Bottom);

                using (var br = new LinearGradientBrush(
                    new RectangleF(plot.Left, plot.Top, plot.Width, plot.Height),
                    Color.FromArgb(120, c), Color.FromArgb(10, c), 90f))
                {
                    g.FillPolygon(br, poly);
                }

                using (var pen = new Pen(c, S(2) + 0.5f))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawLines(pen, pts);
                }
            }

            int valueEvery = n > 1 ? Math.Max(1, (int)Math.Ceiling(S(32) / stepX)) : 1;
            int maxLabelW = Data.Max(d => TextRenderer.MeasureText(d.Key, _fSmall).Width) + S(10);
            int labelEvery = n > 1 ? Math.Max(1, (int)Math.Ceiling(maxLabelW / stepX)) : 1;

            int m = S(8);
            using (var fill = new SolidBrush(Color.White))
            using (var outline = new Pen(c, S(2)))
            {
                for (int i = 0; i < n; i++)
                {
                    g.FillEllipse(fill, pts[i].X - m / 2f, pts[i].Y - m / 2f, m, m);
                    g.DrawEllipse(outline, pts[i].X - m / 2f, pts[i].Y - m / 2f, m, m);

                    if (i % valueEvery == 0)
                    {
                        TextRenderer.DrawText(g, Format(Data[i].Value), _fBold,
                            new Rectangle((int)pts[i].X - S(24), (int)pts[i].Y - S(22), S(48), S(16)),
                            DashTheme.Ink, CenterFlags);
                    }

                    if (i % labelEvery == 0)
                    {
                        TextRenderer.DrawText(g, Data[i].Key, _fSmall,
                            new Rectangle((int)pts[i].X - maxLabelW / 2 - S(4), plot.Bottom + S(4), maxLabelW + S(8), S(18)),
                            DashTheme.Muted, CenterFlags);
                    }
                }
            }
        }

        // ---------- DONUT ----------

        private void DrawDonut(Graphics g, Rectangle a)
        {
            double total = Data.Sum(d => d.Value);
            if (total <= 0) return;

            int size = Math.Min(a.Height, (int)(a.Width * 0.5));
            if (size < S(60)) return;

            var ring = new Rectangle(a.Left + S(10), a.Top + (a.Height - size) / 2, size, size);

            float start = -90f;
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Value <= 0) continue;

                float sweep = (float)(Data[i].Value / total * 360.0);

                using (var br = new SolidBrush(DashTheme.Palette[i % DashTheme.Palette.Length]))
                {
                    if (sweep >= 359.9f) g.FillEllipse(br, ring);
                    else g.FillPie(br, ring, start, sweep);
                }

                if (sweep < 359.9f)
                {
                    using (var sep = new Pen(Color.White, S(2)))
                        g.DrawPie(sep, ring, start, sweep);
                }

                start += sweep;
            }

            int hole = (int)(size * 0.58);
            var holeRect = new Rectangle(
                ring.Left + (size - hole) / 2,
                ring.Top + (size - hole) / 2,
                hole, hole);

            g.FillEllipse(Brushes.White, holeRect);

            TextRenderer.DrawText(g, total.ToString("0", CultureInfo.InvariantCulture), _fBig,
                new Rectangle(holeRect.Left, holeRect.Top + hole / 2 - S(18), hole, S(22)),
                DashTheme.Ink, CenterFlags);

            TextRenderer.DrawText(g, "total", _fSmall,
                new Rectangle(holeRect.Left, holeRect.Top + hole / 2 + S(2), hole, S(16)),
                DashTheme.Muted, CenterFlags);

            // legend
            int lx = ring.Right + S(28);
            int lw = a.Right - lx;
            if (lw < S(60)) return;

            int n = Data.Count;
            int rowH = Math.Max(S(14), Math.Min(S(24), a.Height / n));
            int y = a.Top + (a.Height - n * rowH) / 2;
            int sw = S(10);

            for (int i = 0; i < n; i++)
            {
                double pct = Data[i].Value / total * 100.0;

                using (var br = new SolidBrush(DashTheme.Palette[i % DashTheme.Palette.Length]))
                    g.FillRectangle(br, lx, y + (rowH - sw) / 2, sw, sw);

                string text = Data[i].Key + " (" + Format(Data[i].Value) + ")  " +
                              pct.ToString("0", CultureInfo.InvariantCulture) + "%";

                TextRenderer.DrawText(g, text, _fSmall,
                    new Rectangle(lx + sw + S(8), y, lw - sw - S(8), rowH),
                    DashTheme.Ink, LeftFlags);

                y += rowH;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fTitle.Dispose();
                _fSmall.Dispose();
                _fBold.Dispose();
                _fBig.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // =========================================================
    // TEXT CARD (insights + management action) - auto height
    // =========================================================

    internal class DashTextCard : Control
    {
        private class Line
        {
            public string Text;
            public Font Font;
            public Color Color;
            public int Before;
        }

        private readonly List<Line> _lines = new List<Line>();

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color AccentBar { get; set; } = Color.Empty;

        private const TextFormatFlags Flags =
            TextFormatFlags.Left | TextFormatFlags.Top |
            TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix;

        public DashTextCard()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
        }

        public void ClearLines()
        {
            foreach (var l in _lines) l.Font.Dispose();
            _lines.Clear();
            Invalidate();
        }

        public void AddLine(string text, float size, FontStyle style, Color color, int before)
        {
            _lines.Add(new Line
            {
                Text = text,
                Font = new Font("Segoe UI", size, style),
                Color = color,
                Before = before
            });
        }

        private int PadX
        {
            get { return DashTheme.S(this, 20) + (AccentBar.IsEmpty ? 0 : DashTheme.S(this, 4)); }
        }

        private int PadY
        {
            get { return DashTheme.S(this, 14); }
        }

        public int GetRequiredHeight(int width)
        {
            int textW = Math.Max(60, width - PadX - DashTheme.S(this, 20));
            int y = PadY;

            foreach (var l in _lines)
            {
                y += DashTheme.S(this, l.Before);
                y += TextRenderer.MeasureText(l.Text, l.Font, new Size(textW, int.MaxValue), Flags).Height;
            }

            return y + PadY;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            using (var pen = new Pen(DashTheme.Border))
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            if (!AccentBar.IsEmpty)
            {
                using (var b = new SolidBrush(AccentBar))
                    g.FillRectangle(b, 0, 0, DashTheme.S(this, 4), Height);
            }

            int x = PadX;
            int textW = Math.Max(60, Width - x - DashTheme.S(this, 20));
            int y = PadY;

            foreach (var l in _lines)
            {
                y += DashTheme.S(this, l.Before);

                int h = TextRenderer.MeasureText(l.Text, l.Font, new Size(textW, int.MaxValue), Flags).Height;

                TextRenderer.DrawText(g, l.Text, l.Font,
                    new Rectangle(x, y, textW, h), l.Color, Flags);

                y += h;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var l in _lines) l.Font.Dispose();
                _lines.Clear();
            }
            base.Dispose(disposing);
        }
    }

    // =========================================================
    // DASHBOARD FORM
    // =========================================================

    public class DashboardForm : Form
    {
        private readonly HttpClient _http;

        // ----- layout controls -----
        private Panel _pnlTop;
        private Panel _pnlFilter;
        private Panel _pnlContent;
        private FlowLayoutPanel _flowMain;

        private Label _lblDate;
        private Label _lblApi;
        private Label _lblTenant;
        private Label _lblShowing;

        private ComboBox _cmbPeriod;
        private DateTimePicker _dtFrom;
        private DateTimePicker _dtTo;
        private Button _btnApply;
        private Button _btnRefresh;

        private Label _lblOverview;
        private Label _lblAnalytics;
        private Panel _kpiHost;
        private Panel _chartsHost;
        private DashTextCard _insights;
        private DashTextCard _action;

        // ----- state -----
        private List<Guest> _guests = new List<Guest>();
        private List<Reservation> _reservations = new List<Reservation>();
        private List<GuestFeedback> _feedback = new List<GuestFeedback>();
        private List<GuestInteraction> _interactions = new List<GuestInteraction>();

        private bool _dataLoaded;
        private bool _suppress;
        private bool _layoutBusy;
        private int _lastWidth = -1;

        // Optional fields discovered by reflection (charts appear automatically when found)
        private readonly PropertyInfo _pResAmount;
        private readonly PropertyInfo _pFbCreated;
        private readonly PropertyInfo _pFbResolved;
        private readonly PropertyInfo _pFbRating;
        private readonly PropertyInfo _pFbPriority;
        private readonly PropertyInfo _pIntDate;

        private sealed class Stats
        {
            public DateTime From, To;
            public int TotalGuests, ActiveGuests, Reservations, Dataset, RepeatGuests, Cancelled;
            public int CheckedIn, TodayCheckIns, Upcoming;
            public int Feedback, Resolved, Unresolved, Complaints, Interactions, Rated;
            public int RoomsUsed, RoomsKnown, NewGuests;
            public double RepeatRate, CancelRate, AvgStay, ResolutionRate, ComplaintRatio, AvgValue, OccupancyRate;
            public double? Revenue, AvgRating;
            public string CommonInteraction;
            public int UnresolvedFeedback => Unresolved; // Alias for clarity
        }

        // Standalone window (maximized).
        public DashboardForm() : this(false)
        {
        }

        // embedded = true  -> the form shows inside a panel of your MainForm (no border, fills the panel).
        public DashboardForm(bool embedded)
        {
            _http = ApiClient.Instance;

            Text = "InnEase CRM - Business Intelligence Dashboard";

            if (embedded)
            {
                TopLevel = false;
                FormBorderStyle = FormBorderStyle.None;
                Dock = DockStyle.Fill;
            }
            else
            {
                WindowState = FormWindowState.Maximized;
                MinimumSize = new Size(1000, 640);
            }

            BackColor = DashTheme.PageBg;
            Font = new Font("Segoe UI", 9F);
            DoubleBuffered = true;

            // ---- Edit these name lists if your entity properties are named differently ----
            _pResAmount = FindProp(typeof(Reservation),
                "TotalAmount", "TotalPrice", "TotalCost", "TotalCharges", "GrandTotal", "Amount", "Total");

            _pFbCreated = FindProp(typeof(GuestFeedback),
                "CreatedAt", "CreatedDate", "DateCreated", "SubmittedAt", "DateSubmitted", "FeedbackDate", "CreatedOn", "Timestamp", "Date");

            _pFbResolved = FindProp(typeof(GuestFeedback),
                "ResolvedAt", "ResolvedDate", "DateResolved", "ResolutionDate", "ClosedAt", "ClosedDate");

            _pFbRating = FindProp(typeof(GuestFeedback),
                "Rating", "StarRating", "GuestRating", "Score");

            _pFbPriority = FindProp(typeof(GuestFeedback),
                "Priority", "PriorityLevel", "Urgency");

            _pIntDate = FindProp(typeof(GuestInteraction),
                "InteractionDate", "CreatedAt", "CreatedDate", "DateCreated", "InteractionDateTime", "CreatedOn", "Timestamp", "Date");

            BuildInterface();

            Load += async (s, e) => await LoadDashboard();
            FormClosed += (s, e) => _http.Dispose();
        }

        private int S(int v)
        {
            return DashTheme.S(this, v);
        }

        // Lets MainForm reload the data (e.g. after the tenant changes).
        public Task RefreshDashboardAsync()
        {
            return LoadDashboard();
        }

        // =========================================================
        // BUILD INTERFACE
        // =========================================================

        private void BuildInterface()
        {
            var main = new Panel { Dock = DockStyle.Fill, BackColor = DashTheme.PageBg };

            BuildContentArea();
            BuildFilterBar();
            BuildTopBar();

            // Dock order: Fill first, then Top panels (last added = topmost)
            main.Controls.Add(_pnlContent);
            main.Controls.Add(_pnlFilter);
            main.Controls.Add(_pnlTop);

            Controls.Add(main);
        }

        // ---------------- TOP BAR ----------------

        private void BuildTopBar()
        {
            _pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = S(80),
                BackColor = DashTheme.PageBg
            };

            var lblTitle = new Label
            {
                Text = "Business Intelligence Dashboard",
                Font = new Font("Segoe UI Semibold", 18F),
                ForeColor = DashTheme.Ink,
                Location = new Point(S(24), S(8)),
                AutoSize = true
            };

            var lblSub = new Label
            {
                Text = "Live analytics from reservations, guests, feedback and interaction data",
                Font = new Font("Segoe UI", 9F),
                ForeColor = DashTheme.Muted,
                Location = new Point(S(26), S(48)),
                AutoSize = true
            };

            _lblDate = new Label
            {
                Text = DateTime.Now.ToString("dddd, MMMM d, yyyy", CultureInfo.InvariantCulture),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = DashTheme.Muted,
                AutoSize = true
            };

            _lblTenant = new Label
            {
                Text = "Current Tenant: Loading...",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = DashTheme.Accent,
                AutoSize = true
            };

            _lblApi = new Label
            {
                Text = "\u25CF Connecting...",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = DashTheme.Muted,
                AutoSize = true
            };

            _pnlTop.Controls.Add(lblTitle);
            _pnlTop.Controls.Add(lblSub);
            _pnlTop.Controls.Add(_lblDate);
            _pnlTop.Controls.Add(_lblTenant);
            _pnlTop.Controls.Add(_lblApi);

            _pnlTop.Resize += (s, e) => PositionHeaderRight();
            PositionHeaderRight();
        }

        private void PositionHeaderRight()
        {
            if (_pnlTop == null) return;

            int right = _pnlTop.ClientSize.Width - S(24);
            _lblDate.Location = new Point(right - _lblDate.Width, S(8));
            _lblTenant.Location = new Point(right - _lblTenant.Width, S(30));
            _lblApi.Location = new Point(right - _lblApi.Width, S(52));
        }

        private void SetApiStatus(string text, Color color)
        {
            _lblApi.Text = "\u25CF " + text;
            _lblApi.ForeColor = color;
            PositionHeaderRight();
        }

        // ---------------- FILTER BAR ----------------

        private void BuildFilterBar()
        {
            _pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = S(88),
                BackColor = DashTheme.PageBg
            };

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(S(24), S(8), S(10), 0),
                BackColor = DashTheme.PageBg
            };

            _cmbPeriod = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = S(130),
                Margin = new Padding(0, S(5), S(14), 0),
                FlatStyle = FlatStyle.Flat
            };

            _cmbPeriod.Items.AddRange(new object[]
            {
                "Today", "This Week", "This Month", "This Quarter",
                "This Year", "Last 30 Days", "All Time", "Custom"
            });
            _cmbPeriod.SelectedItem = "This Year";

            _dtFrom = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                Width = S(112),
                Margin = new Padding(0, S(5), S(14), 0),
                Enabled = false
            };

            _dtTo = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                Width = S(112),
                Margin = new Padding(0, S(5), S(14), 0),
                Enabled = false
            };

            _btnApply = new Button
            {
                Text = "Apply",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = DashTheme.Accent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(S(84), S(32)),
                Margin = new Padding(0, S(1), S(8), 0),
                Cursor = Cursors.Hand
            };
            _btnApply.FlatAppearance.BorderSize = 0;

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = DashTheme.Ink,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(S(90), S(32)),
                Margin = new Padding(0, S(1), S(16), 0),
                Cursor = Cursors.Hand
            };
            _btnRefresh.FlatAppearance.BorderColor = DashTheme.Border;

            _lblShowing = new Label
            {
                Text = "",
                UseMnemonic = false,
                AutoSize = true,
                ForeColor = DashTheme.Muted,
                Margin = new Padding(S(2), S(2), 0, 0)
            };

            flow.Controls.Add(MakeFilterLabel("Period", 50));
            flow.Controls.Add(_cmbPeriod);
            flow.Controls.Add(MakeFilterLabel("From", 40));
            flow.Controls.Add(_dtFrom);
            flow.Controls.Add(MakeFilterLabel("To", 26));
            flow.Controls.Add(_dtTo);
            flow.Controls.Add(_btnApply);
            flow.Controls.Add(_btnRefresh);
            flow.SetFlowBreak(_btnRefresh, true);
            flow.Controls.Add(_lblShowing);

            _pnlFilter.Controls.Add(flow);

            _cmbPeriod.SelectedIndexChanged += (s, e) =>
            {
                if (_suppress) return;

                ApplyPreset();

                if (!string.Equals(_cmbPeriod.SelectedItem as string, "Custom", StringComparison.Ordinal))
                    Render();
            };

            _btnApply.Click += (s, e) =>
            {
                if (_dtFrom.Value.Date > _dtTo.Value.Date)
                {
                    MessageBox.Show(
                        "The 'From' date must be earlier than or equal to the 'To' date.",
                        "Invalid Date Range",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                Render();
            };

            _btnRefresh.Click += async (s, e) => await LoadDashboard();

            ApplyPreset();
        }

        private Label MakeFilterLabel(string text, int width)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                Size = new Size(S(width), S(32)),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = DashTheme.Muted,
                Margin = new Padding(0, 0, S(4), 0)
            };
        }

        private void ApplyPreset()
        {
            string p = _cmbPeriod.SelectedItem as string ?? "This Year";

            _dtFrom.Enabled = _dtTo.Enabled = string.Equals(p, "Custom", StringComparison.Ordinal);
            if (string.Equals(p, "Custom", StringComparison.Ordinal)) return;

            DateTime today = DateTime.Today;
            DateTime from;
            DateTime to;

            switch (p)
            {
                case "Today":
                    from = today;
                    to = today;
                    break;

                case "This Week":
                    {
                        int diff = ((int)today.DayOfWeek + 6) % 7; // Monday start
                        from = today.AddDays(-diff);
                        to = from.AddDays(6);
                        break;
                    }

                case "This Month":
                    from = new DateTime(today.Year, today.Month, 1);
                    to = from.AddMonths(1).AddDays(-1);
                    break;

                case "This Quarter":
                    {
                        int qm = ((today.Month - 1) / 3) * 3 + 1;
                        from = new DateTime(today.Year, qm, 1);
                        to = from.AddMonths(3).AddDays(-1);
                        break;
                    }

                case "Last 30 Days":
                    to = today;
                    from = today.AddDays(-29);
                    break;

                case "All Time":
                    if (_reservations.Count > 0)
                    {
                        from = _reservations.Min(r => r.CheckInDate).Date;
                        to = _reservations.Max(r => r.CheckInDate).Date;
                    }
                    else
                    {
                        from = today;
                        to = today;
                    }
                    break;

                default: // This Year
                    from = new DateTime(today.Year, 1, 1);
                    to = new DateTime(today.Year, 12, 31);
                    break;
            }

            _suppress = true;
            try
            {
                _dtFrom.Value = from;
                _dtTo.Value = to;
            }
            finally
            {
                _suppress = false;
            }
        }

        // ---------------- CONTENT AREA ----------------

        private void BuildContentArea()
        {
            _pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = DashTheme.PageBg,
                AutoScrollMargin = new Size(S(20), S(24))
            };

            _flowMain = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = DashTheme.PageBg,
                Location = new Point(S(20), S(6)),
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            _lblOverview = MakeSection("Business overview");
            _kpiHost = MakeHost();
            _lblAnalytics = MakeSection("Reservations, guests & feedback analytics");
            _chartsHost = MakeHost();

            _insights = new DashTextCard { Margin = new Padding(0, 0, 0, S(14)), Height = S(100) };
            _action = new DashTextCard
            {
                Margin = new Padding(0, 0, 0, S(14)),
                Height = S(100),
                AccentBar = DashTheme.Accent
            };

            _flowMain.Controls.Add(_lblOverview);
            _flowMain.Controls.Add(_kpiHost);
            _flowMain.Controls.Add(_lblAnalytics);
            _flowMain.Controls.Add(_chartsHost);
            _flowMain.Controls.Add(_insights);
            _flowMain.Controls.Add(_action);

            _pnlContent.Controls.Add(_flowMain);

            _pnlContent.Resize += (s, e) => LayoutContent();
            _pnlContent.ClientSizeChanged += (s, e) => LayoutContent();
            _pnlContent.MouseEnter += (s, e) => _pnlContent.Focus(); // mouse-wheel scrolling
        }

        private Label MakeSection(string text)
        {
            return new Label
            {
                Text = text,
                UseMnemonic = false,
                AutoSize = false,
                Height = S(36),
                Font = new Font("Segoe UI Semibold", 12.5F),
                ForeColor = DashTheme.Ink,
                TextAlign = ContentAlignment.BottomLeft,
                Margin = new Padding(0, 0, 0, S(8))
            };
        }

        private Panel MakeHost()
        {
            return new Panel
            {
                Height = S(10),
                BackColor = DashTheme.PageBg,
                Margin = new Padding(0, 0, 0, S(14))
            };
        }

        private static void ClearHost(Control host)
        {
            foreach (var c in host.Controls.Cast<Control>().ToList())
            {
                host.Controls.Remove(c);
                c.Dispose();
            }
        }

        // =========================================================
        // LAYOUT
        // =========================================================

        private void LayoutContent(bool force = false)
        {
            if (_flowMain == null || _layoutBusy) return;

            _layoutBusy = true;
            try
            {
                for (int pass = 0; pass < 3; pass++)
                {
                    int width = _pnlContent.ClientSize.Width - S(40);
                    if (width < S(360)) return;
                    if (!(force && pass == 0) && width == _lastWidth) return;

                    _lastWidth = width;

                    _flowMain.SuspendLayout();

                    foreach (Control c in _flowMain.Controls)
                        c.Width = width;

                    LayoutKpis(width);
                    LayoutCharts(width);

                    _insights.Height = _insights.GetRequiredHeight(width);
                    _action.Height = _action.GetRequiredHeight(width);

                    _flowMain.ResumeLayout(true);
                }
            }
            finally
            {
                _layoutBusy = false;
            }
        }

        private void LayoutKpis(int width)
        {
            var cards = _kpiHost.Controls.Cast<Control>().ToList();
            if (cards.Count == 0)
            {
                _kpiHost.Height = S(10);
                return;
            }

            int gap = S(12);
            int minW = S(200);
            int cols = Math.Max(1, Math.Min(6, (width + gap) / (minW + gap)));
            int cardW = (width - gap * (cols - 1)) / cols;
            int cardH = S(92);

            for (int i = 0; i < cards.Count; i++)
            {
                int row = i / cols;
                int col = i % cols;
                cards[i].SetBounds(col * (cardW + gap), row * (cardH + gap), cardW, cardH);
            }

            int rows = (cards.Count + cols - 1) / cols;
            _kpiHost.Height = rows * cardH + (rows - 1) * gap;
        }

        private void LayoutCharts(int width)
        {
            var charts = _chartsHost.Controls.Cast<Control>().ToList();
            if (charts.Count == 0)
            {
                _chartsHost.Height = S(10);
                return;
            }

            int gap = S(12);
            int cols = width >= S(900) ? 2 : 1;
            int cw = (width - gap * (cols - 1)) / cols;
            int ch = S(290);

            for (int i = 0; i < charts.Count; i++)
            {
                int row = i / cols;
                int col = i % cols;

                bool span = cols == 2 && i == charts.Count - 1 && charts.Count % 2 == 1;

                charts[i].SetBounds(
                    span ? 0 : col * (cw + gap),
                    row * (ch + gap),
                    span ? width : cw,
                    ch);
            }

            int rows = (charts.Count + cols - 1) / cols;
            _chartsHost.Height = rows * ch + (rows - 1) * gap;
        }

        // =========================================================
        // LOAD DATA
        // =========================================================

        private async Task<List<T>> GetListAsync<T>(string url)
        {
            using (var response = await _http.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();
                    if (body != null && body.Length > 500)
                        body = body.Substring(0, 500) + "...";

                    throw new InvalidOperationException(
                        $"{(int)response.StatusCode} {response.ReasonPhrase}\n{url}\n\n{body}");
                }

                var list = await response.Content.ReadFromJsonAsync<List<T>>();
                return list ?? new List<T>();
            }
        }

        private async Task LoadDashboard()
        {
            try
            {
                _btnRefresh.Enabled = false;
                SetApiStatus("Loading...", DashTheme.Muted);

                _lblTenant.Text = "InnEase CRM";
                PositionHeaderRight();

                string baseUrl = "https://localhost:7129/api";

                var guestsTask       = GetListAsync<Guest>($"{baseUrl}/guests");
                var reservationsTask = GetListAsync<Reservation>($"{baseUrl}/reservations");
                var feedbackTask     = GetListAsync<GuestFeedback>($"{baseUrl}/guestfeedback");
                var interactionsTask = GetListAsync<GuestInteraction>($"{baseUrl}/interactions");

                await Task.WhenAll(guestsTask, reservationsTask, feedbackTask, interactionsTask);

                _guests = guestsTask.Result;
                _reservations = reservationsTask.Result;
                _feedback = feedbackTask.Result;
                _interactions = interactionsTask.Result;

                _dataLoaded = true;

                ApplyPreset();
                Render();

                SetApiStatus("API connected", DashTheme.Green);
            }
            catch (Exception ex)
            {
                SetApiStatus("API disconnected", Color.IndianRed);

                // Clear old data so the previous tenant's numbers are not shown by mistake
                _guests = new List<Guest>();
                _reservations = new List<Reservation>();
                _feedback = new List<GuestFeedback>();
                _interactions = new List<GuestInteraction>();
                _dataLoaded = true;
                Render();

                // Task.WhenAll reports only the first failure; show the real reason
                string message = ex.Message;
                MessageBox.Show(
                    $"Unable to load the Business Intelligence reports.\n\n" + message,
                    "Dashboard Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _btnRefresh.Enabled = true;
            }
        }

        private string GetTenantName(int companyId)
        {
            switch (companyId)
            {
                case 1: return "Tenant A - Company 1";
                case 2: return "Tenant B - Company 2";
                case 4: return "Tenant C - Company 4";
                default: return $"Company {companyId}";
            }
        }

        // =========================================================
        // RENDER (filter -> compute -> build)
        // =========================================================

        private void Render()
        {
            if (!_dataLoaded) return;

            DateTime from = _dtFrom.Value.Date;
            DateTime to = _dtTo.Value.Date;

            var res = _reservations
                .Where(r => r.CheckInDate.Date >= from && r.CheckInDate.Date <= to)
                .ToList();

            var fb = FilterByDate(_feedback, _pFbCreated, from, to);
            var inter = FilterByDate(_interactions, _pIntDate, from, to);

            var stats = Compute(res, fb, inter, from, to);

            _lblShowing.Text =
                $"Showing {from:d MMM yyyy} to {to:d MMM yyyy}   |   " +
                $"{res.Count} of {_reservations.Count} reservation transaction(s)";

            BuildKpis(stats);
            BuildCharts(res, fb, inter, stats);
            BuildInsights(stats);
            BuildAction(stats);

            LayoutContent(true);
        }

        private static List<T> FilterByDate<T>(List<T> list, PropertyInfo prop, DateTime from, DateTime to)
        {
            if (prop == null) return list;

            return list.Where(x =>
            {
                DateTime? d = GetDate(x, prop);
                return d == null || (d.Value.Date >= from && d.Value.Date <= to);
            }).ToList();
        }

        // =========================================================
        // STATS
        // =========================================================

        private Stats Compute(
            List<Reservation> res,
            List<GuestFeedback> fb,
            List<GuestInteraction> inter,
            DateTime from,
            DateTime to)
        {
            var s = new Stats
            {
                From = from,
                To = to,
                TotalGuests = _guests.Count,
                Reservations = res.Count,
                Dataset = _reservations.Count,
                Feedback = fb.Count,
                Interactions = inter.Count
            };

            s.ActiveGuests = res.Select(r => r.GuestId).Distinct().Count();

            s.RepeatGuests = _guests.Count(g => res.Count(r => r.GuestId == g.GuestId) > 1);
            s.NewGuests = Math.Max(0, s.ActiveGuests - s.RepeatGuests);

            // Functional KPI: Room Occupancy Rate = rooms used in the period / total distinct rooms in the property
            s.RoomsUsed = res.Select(r => r.RoomNumber).Where(rm => !string.IsNullOrWhiteSpace(rm)).Distinct().Count();
            s.RoomsKnown = _reservations.Select(r => r.RoomNumber).Where(rm => !string.IsNullOrWhiteSpace(rm)).Distinct().Count();
            s.OccupancyRate = s.RoomsKnown == 0 ? 0 : (double)s.RoomsUsed / s.RoomsKnown * 100;
            s.RepeatRate = _guests.Count == 0 ? 0 : (double)s.RepeatGuests / _guests.Count * 100;

            s.Cancelled = res.Count(r => Is(r.Status, "Cancelled"));
            s.CancelRate = res.Count == 0 ? 0 : (double)s.Cancelled / res.Count * 100;

            s.AvgStay = res.Count == 0 ? 0 : res.Average(r => (r.CheckOutDate - r.CheckInDate).TotalDays);

            s.CheckedIn = res.Count(r => Is(r.Status, "CheckedIn"));

            DateTime today = DateTime.Today;
            s.TodayCheckIns = _reservations.Count(r =>
                r.CheckInDate.Date == today && !Is(r.Status, "Cancelled"));
            s.Upcoming = _reservations.Count(r =>
                r.CheckInDate.Date > today &&
                r.CheckInDate.Date <= today.AddDays(7) &&
                !Is(r.Status, "Cancelled"));

            s.Resolved = fb.Count(f => Is(f.Status, "Resolved"));
            s.Unresolved = fb.Count - s.Resolved;
            s.ResolutionRate = fb.Count == 0 ? 0 : (double)s.Resolved / fb.Count * 100;

            s.Complaints = fb.Count(f => Is(f.RequestType, "Complaint"));
            s.ComplaintRatio = fb.Count == 0 ? 0 : (double)s.Complaints / fb.Count * 100;

            s.CommonInteraction = Pretty(
                inter.GroupBy(i => i.InteractionType)
                     .OrderByDescending(g => g.Count())
                     .Select(g => g.Key)
                     .FirstOrDefault() ?? "N/A");

            if (_pResAmount != null)
            {
                var billable = res.Where(r => !Is(r.Status, "Cancelled")).ToList();
                double total = billable.Sum(r => GetNumber(r, _pResAmount) ?? 0);
                s.Revenue = total;
                s.AvgValue = billable.Count == 0 ? 0 : total / billable.Count;
            }

            if (_pFbRating != null)
            {
                var ratings = fb.Select(f => GetNumber(f, _pFbRating))
                                .Where(v => v.HasValue && v.Value > 0)
                                .Select(v => v.Value)
                                .ToList();

                s.Rated = ratings.Count;
                if (ratings.Count > 0) s.AvgRating = ratings.Average();
            }

            return s;
        }

        // =========================================================
        // KPI CARDS
        // =========================================================

        // Fires when a KPI card is clicked, so the host (MainForm) can navigate to the
        // relevant screen. Keys: "Guests", "Reservations", "Feedback", "Interactions".
        public event System.Action<string> KpiClicked;

        private void BuildKpis(Stats s)
        {
            ClearHost(_kpiHost);

            System.Action<string, string, string, Color, string> add = (title, value, sub, color, target) =>
            {
                var card = new DashKpiCard(title, value, sub, color);

                if (!string.IsNullOrEmpty(target))
                {
                    card.Cursor = Cursors.Hand;
                    card.Click += (s2, e2) => KpiClicked?.Invoke(target);
                }

                _kpiHost.Controls.Add(card);
            };

            double activePct = s.TotalGuests == 0 ? 0 : (double)s.ActiveGuests / s.TotalGuests * 100;

            add("Total Guests", s.TotalGuests.ToString("N0"), "All guest records", DashTheme.Accent, "Guests");
            add("Active Guests", s.ActiveGuests.ToString("N0"), $"{activePct:F0}% of guests", DashTheme.Teal, "Guests");
            add("Total Reservations", s.Reservations.ToString("N0"), "Selected period", DashTheme.Blue, "Reservations");
            add("Unresolved Feedback", s.UnresolvedFeedback.ToString("N0"), "Requires attention", DashTheme.Rose, "Feedback");
            add("Average Stay", s.AvgStay.ToString("F1"), "Nights per reservation", DashTheme.Amber, "Reservations");
            add("Repeat Guest Rate", $"{s.RepeatRate:F1}%", $"{s.RepeatGuests} repeat guests", DashTheme.Green, "Guests");
            add("Cancellation Rate", $"{s.CancelRate:F1}%", $"{s.Cancelled} cancelled", DashTheme.Rose, "Reservations");

            add("Room Occupancy Rate", $"{s.OccupancyRate:F1}%", $"{s.RoomsUsed} of {s.RoomsKnown} rooms used", DashTheme.Rose, "Reservations");
            add("Checked-In Now", s.CheckedIn.ToString("N0"), "Status: Checked In", DashTheme.Violet, "Reservations");
            add("Today's Check-ins", s.TodayCheckIns.ToString("N0"), "Arriving today", DashTheme.Blue, "Reservations");
            add("Upcoming Check-ins", s.Upcoming.ToString("N0"), "Next 7 days", DashTheme.Teal, "Reservations");

            if (s.Revenue.HasValue)
            {
                add("Total Revenue", "\u20B1" + s.Revenue.Value.ToString("N2"), "Excludes cancelled", DashTheme.Accent, "Reservations");
                add("Avg Reservation Value", "\u20B1" + s.AvgValue.ToString("N2"), "Per reservation", DashTheme.Amber, "Reservations");
            }

            add("Total Feedback", s.Feedback.ToString("N0"), "Created in period", DashTheme.Magenta, "Feedback");
            add("Unresolved Feedback", s.Unresolved.ToString("N0"), "Needs follow-up", DashTheme.Rose, "Feedback");
            add("Feedback Resolution", $"{s.ResolutionRate:F1}%", $"{s.Resolved} resolved", DashTheme.Violet, "Feedback");
            add("Complaint Ratio", $"{s.ComplaintRatio:F1}%", $"{s.Complaints} complaints", DashTheme.Amber, "Feedback");

            if (s.AvgRating.HasValue)
                add("Average Feedback Rating", $"{s.AvgRating.Value:F2} / 5", $"{s.Rated} rated entries", DashTheme.Green, "Feedback");

            add("Guest Interactions", s.Interactions.ToString("N0"), "Recorded touchpoints", DashTheme.Blue, "Interactions");
        }

        // =========================================================
        // CHARTS
        // =========================================================

        private void BuildCharts(
            List<Reservation> res,
            List<GuestFeedback> fb,
            List<GuestInteraction> inter,
            Stats s)
        {
            ClearHost(_chartsHost);

            var reservationOrder = new[] { "Pending", "Confirmed", "CheckedIn", "CheckedOut", "Cancelled" };
            var feedbackOrder = new[] { "Open", "InProgress", "Resolved", "Closed" };
            var priorityOrder = new[] { "Low", "Medium", "High", "Urgent", "Critical" };

            // 1. Reservation status
            var c1 = NewChart("Reservation Status", DashChartKind.Column);
            c1.Data.AddRange(CountBy(res.Select(r => r.Status), reservationOrder));
            _chartsHost.Controls.Add(c1);

            // 2. Reservations trend (by month) - area line
            var c2 = NewChart("Reservations Trend (By Month)", DashChartKind.Area);
            c2.SingleColor = true;
            c2.BaseColor = DashTheme.Magenta;
            c2.Data.AddRange(MonthlySeries(res, r => 1.0));
            _chartsHost.Controls.Add(c2);

            // Custom business category (not a raw DB column): guests split by New vs Repeat
            var cSeg = NewChart("Guests By Segment (New vs Repeat)", DashChartKind.Donut);
            cSeg.Data.Add(new KeyValuePair<string, double>("New Guests", s.NewGuests));
            cSeg.Data.Add(new KeyValuePair<string, double>("Repeat Guests", s.RepeatGuests));
            _chartsHost.Controls.Add(cSeg);

            // ---- Sales Report: Daily / Weekly / Monthly ----
            // No RoomRate field exists yet on Reservation, so these currently count bookings
            // rather than currency. Once a rate/price field is added, swap "r => 1.0" below
            // for "r => GetNumber(r, _pResAmount) ?? 0" and Format for DashTheme.Peso.

            var cSalesDaily = NewChart("Sales Report - Daily (Bookings)", DashChartKind.Area);
            cSalesDaily.SingleColor = true;
            cSalesDaily.BaseColor = DashTheme.Teal;
            cSalesDaily.Data.AddRange(DailySeries(res));
            _chartsHost.Controls.Add(cSalesDaily);

            var cSalesWeekly = NewChart("Sales Report - Weekly (Bookings)", DashChartKind.Column);
            cSalesWeekly.SingleColor = true;
            cSalesWeekly.BaseColor = DashTheme.Blue;
            cSalesWeekly.Data.AddRange(WeeklySeries(res, r => 1.0));
            _chartsHost.Controls.Add(cSalesWeekly);

            var cSalesMonthly = NewChart("Sales Report - Monthly (Bookings)", DashChartKind.Column);
            cSalesMonthly.SingleColor = true;
            cSalesMonthly.BaseColor = DashTheme.Accent;
            cSalesMonthly.Data.AddRange(MonthlySeries(res, r => 1.0));
            _chartsHost.Controls.Add(cSalesMonthly);

            // 3. Revenue by month (only when an amount field exists)
            if (_pResAmount != null)
            {
                var cRev = NewChart("Revenue By Month", DashChartKind.Column);
                cRev.Format = DashTheme.Peso;
                cRev.IntegerAxis = false;
                cRev.Data.AddRange(MonthlySeries(
                    res.Where(r => !Is(r.Status, "Cancelled")).ToList(),
                    r => GetNumber(r, _pResAmount) ?? 0));
                _chartsHost.Controls.Add(cRev);
            }

            // 4. Reservations by day
            var c4 = NewChart("Reservations By Day (Check-in)", DashChartKind.Area);
            c4.SingleColor = true;
            c4.BaseColor = DashTheme.Blue;
            c4.Data.AddRange(DailySeries(res));
            _chartsHost.Controls.Add(c4);

            // 5. Feedback status
            var c5 = NewChart("Feedback Analysis", DashChartKind.Column);
            c5.Data.AddRange(CountBy(fb.Select(f => f.Status), feedbackOrder));
            _chartsHost.Controls.Add(c5);

            // 6. Feedback by type
            var c6 = NewChart("Feedback By Type", DashChartKind.HBar);
            c6.Data.AddRange(CountBy(fb.Select(f => f.RequestType), new string[0]));
            _chartsHost.Controls.Add(c6);

            // 7. Rating distribution
            if (_pFbRating != null)
            {
                var ratings = fb.Select(f => GetNumber(f, _pFbRating))
                                .Where(v => v.HasValue && v.Value >= 1 && v.Value <= 5)
                                .Select(v => (int)Math.Round(v.Value))
                                .ToList();

                if (ratings.Count > 0)
                {
                    var c7 = NewChart("Feedback Rating Distribution", DashChartKind.Column);
                    for (int star = 1; star <= 5; star++)
                    {
                        int count = ratings.Count(r => r == star);
                        c7.Data.Add(new KeyValuePair<string, double>(
                            star + (star == 1 ? " Star" : " Stars"), count));
                    }
                    _chartsHost.Controls.Add(c7);
                }
            }

            // 8. Complaints by priority
            if (_pFbPriority != null)
            {
                var complaints = fb.Where(f => Is(f.RequestType, "Complaint")).ToList();
                bool onlyComplaints = complaints.Count > 0;
                var source = onlyComplaints ? complaints : fb;

                var c8 = NewChart(onlyComplaints ? "Complaints By Priority" : "Feedback By Priority", DashChartKind.Column);
                c8.Data.AddRange(CountBy(source.Select(f => GetText(f, _pFbPriority)), priorityOrder));
                _chartsHost.Controls.Add(c8);
            }

            // 9. Average resolution time
            if (_pFbCreated != null && _pFbResolved != null)
            {
                var resolution = fb
                    .Select(f => new
                    {
                        Type = f.RequestType,
                        Created = GetDate(f, _pFbCreated),
                        Resolved = GetDate(f, _pFbResolved)
                    })
                    .Where(x => x.Created.HasValue && x.Resolved.HasValue && x.Resolved.Value >= x.Created.Value)
                    .GroupBy(x => string.IsNullOrWhiteSpace(x.Type) ? "Unknown" : x.Type)
                    .Select(g => new KeyValuePair<string, double>(
                        Pretty(g.Key),
                        g.Average(x => (x.Resolved.Value - x.Created.Value).TotalHours)))
                    .ToList();

                if (resolution.Count > 0)
                {
                    var c9 = NewChart("Average Resolution Time (Hours)", DashChartKind.Column);
                    c9.IntegerAxis = false;
                    c9.Data.AddRange(resolution);
                    _chartsHost.Controls.Add(c9);
                }
            }

            // 10. Interactions by type (donut)
            var c10 = NewChart("Guest Interactions By Type", DashChartKind.Donut);
            c10.Data.AddRange(CountBy(inter.Select(i => i.InteractionType), new string[0]));
            _chartsHost.Controls.Add(c10);
        }

        private DashChartCard NewChart(string title, DashChartKind kind)
        {
            return new DashChartCard { Title = title, Kind = kind };
        }

        private static List<KeyValuePair<string, double>> CountBy(IEnumerable<string> values, string[] order)
        {
            var groups = values
                .Select(v => string.IsNullOrWhiteSpace(v) ? "Unknown" : v.Trim())
                .GroupBy(v => v, StringComparer.OrdinalIgnoreCase)
                .ToList();

            Func<string, int> rank = key =>
            {
                string k = key.Replace(" ", "");
                int i = Array.FindIndex(order, o => string.Equals(o.Replace(" ", ""), k, StringComparison.OrdinalIgnoreCase));
                return i < 0 ? int.MaxValue : i;
            };

            return groups
                .OrderBy(g => rank(g.Key))
                .ThenByDescending(g => g.Count())
                .Select(g => new KeyValuePair<string, double>(Pretty(g.Key), g.Count()))
                .ToList();
        }

        private static List<KeyValuePair<string, double>> MonthlySeries(
            List<Reservation> res,
            Func<Reservation, double> value)
        {
            var list = new List<KeyValuePair<string, double>>();
            if (res.Count == 0) return list;

            DateTime min = res.Min(r => r.CheckInDate);
            DateTime max = res.Max(r => r.CheckInDate);

            var cur = new DateTime(min.Year, min.Month, 1);
            var end = new DateTime(max.Year, max.Month, 1);

            var lookup = res
                .GroupBy(r => new DateTime(r.CheckInDate.Year, r.CheckInDate.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(value));

            while (cur <= end)
            {
                double v;
                if (!lookup.TryGetValue(cur, out v)) v = 0;

                list.Add(new KeyValuePair<string, double>(
                    cur.ToString("MMM yyyy", CultureInfo.InvariantCulture), v));

                cur = cur.AddMonths(1);
            }

            return list;
        }

        private static List<KeyValuePair<string, double>> DailySeries(List<Reservation> res)
        {
            var list = new List<KeyValuePair<string, double>>();
            if (res.Count == 0) return list;

            DateTime last = res.Max(r => r.CheckInDate).Date;
            DateTime first = res.Min(r => r.CheckInDate).Date;

            if ((last - first).TotalDays > 59)
                first = last.AddDays(-59);

            var lookup = res
                .GroupBy(r => r.CheckInDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            for (DateTime d = first; d <= last; d = d.AddDays(1))
            {
                int c;
                if (!lookup.TryGetValue(d, out c)) c = 0;

                list.Add(new KeyValuePair<string, double>(
                    d.ToString("d MMM", CultureInfo.InvariantCulture), c));
            }

            return list;
        }

        // Groups reservations into Monday-start weeks. Caps the range to the most recent
        // 20 weeks so the chart stays readable.
        private static List<KeyValuePair<string, double>> WeeklySeries(
            List<Reservation> res,
            Func<Reservation, double> value)
        {
            var list = new List<KeyValuePair<string, double>>();
            if (res.Count == 0) return list;

            Func<DateTime, DateTime> weekStart = d => d.Date.AddDays(-(((int)d.DayOfWeek + 6) % 7));

            DateTime min = weekStart(res.Min(r => r.CheckInDate));
            DateTime max = weekStart(res.Max(r => r.CheckInDate));

            if ((max - min).TotalDays / 7 > 20)
                min = max.AddDays(-20 * 7);

            var lookup = res
                .GroupBy(r => weekStart(r.CheckInDate))
                .Where(g => g.Key >= min)
                .ToDictionary(g => g.Key, g => g.Sum(value));

            for (DateTime d = min; d <= max; d = d.AddDays(7))
            {
                double v;
                if (!lookup.TryGetValue(d, out v)) v = 0;

                list.Add(new KeyValuePair<string, double>(
                    "Wk " + d.ToString("d MMM", CultureInfo.InvariantCulture), v));
            }

            return list;
        }

        // =========================================================
        // STRATEGIC INSIGHTS
        // =========================================================

        private void BuildInsights(Stats s)
        {
            _insights.ClearLines();

            _insights.AddLine("STRATEGIC REPORTS & BUSINESS INSIGHTS", 13F, FontStyle.Bold, DashTheme.Ink, 0);

            string[] lines =
            {
                $"Reporting Period: {s.From:d MMM yyyy} to {s.To:d MMM yyyy}",
                $"Transaction Dataset: {s.Dataset} reservation records ({s.Reservations} in the selected period)",
                $"Repeat Guest Rate: {s.RepeatRate:F1}% ({s.RepeatGuests} repeat guests)",
                $"Cancellation Rate: {s.CancelRate:F1}% ({s.Cancelled} cancelled reservations)",
                $"Feedback Resolution Rate: {s.ResolutionRate:F1}% ({s.Resolved} resolved)",
                $"Complaint Ratio: {s.ComplaintRatio:F1}% ({s.Complaints} complaints)",
                $"Unresolved Feedback: {s.Unresolved} record(s)",
                $"Most Common Interaction: {s.CommonInteraction}"
            };

            foreach (string line in lines)
                _insights.AddLine("\u2022  " + line, 9.5F, FontStyle.Regular, DashTheme.Ink, 6);

            _insights.Invalidate();
        }

        // =========================================================
        // MANAGEMENT ACTION
        // =========================================================

        private void BuildAction(Stats s)
        {
            _action.ClearLines();

            string finding;
            string basis;
            string action;

            // Threshold is checked against the FULL dataset, not the filtered period
            if (s.Dataset < 200)
            {
                finding = "The current dataset has not yet reached the 200-transaction BI threshold.";
                basis = $"Current reservation transactions: {s.Dataset}.";
                action = "Continue recording valid reservation transactions until the BI dataset reaches at least 200 records.";
            }
            else if (s.Unresolved > 0)
            {
                finding = $"{s.Unresolved} guest feedback record(s) remain unresolved.";
                basis = $"Feedback Resolution Rate: {s.ResolutionRate:F1}% with {s.Unresolved} unresolved record(s).";
                action = "Prioritize follow-up of unresolved guest feedback and assign responsible staff members to update each record after resolution.";
            }
            else if (s.Cancelled > 0)
            {
                finding = $"{s.Cancelled} reservation(s) were cancelled.";
                basis = $"Cancellation Rate: {s.CancelRate:F1}% ({s.Cancelled} of {s.Reservations} reservations).";
                action = "Review cancelled reservation records and identify recurring cancellation reasons for booking and guest-communication improvements.";
            }
            else if (s.Complaints > 0)
            {
                finding = $"{s.Complaints} complaint record(s) were recorded.";
                basis = $"Complaint Ratio: {s.ComplaintRatio:F1}% of all feedback records.";
                action = "Review complaint records for recurring concerns and assign follow-up actions to the appropriate staff.";
            }
            else if (s.RepeatGuests > 0)
            {
                finding = $"{s.RepeatGuests} guest(s) have more than one reservation.";
                basis = $"Repeat Guest Rate: {s.RepeatRate:F1}%.";
                action = "Review repeat-guest activity and use the findings to support customer retention and loyalty activities.";
            }
            else
            {
                finding = "No immediate issue was identified from the current reports.";
                basis = "Current reservation, guest, feedback, and interaction KPIs were reviewed.";
                action = "Continue monitoring the KPIs and update the strategic reports as new transactions are recorded.";
            }

            _action.AddLine("MANAGEMENT ACTION BASED ON STRATEGIC REPORTS", 13F, FontStyle.Bold, DashTheme.Ink, 0);
            _action.AddLine("The management action below is generated from the current Strategic Report findings:",
                9.5F, FontStyle.Regular, DashTheme.Muted, 6);

            _action.AddLine("FINDING", 9F, FontStyle.Bold, DashTheme.Muted, 14);
            _action.AddLine(finding, 10F, FontStyle.Regular, DashTheme.Ink, 3);

            _action.AddLine("BASIS", 9F, FontStyle.Bold, DashTheme.Muted, 12);
            _action.AddLine(basis, 10F, FontStyle.Regular, DashTheme.Ink, 3);

            _action.AddLine("MANAGEMENT ACTION", 9F, FontStyle.Bold, DashTheme.Accent, 12);
            _action.AddLine(action, 10.5F, FontStyle.Bold, DashTheme.Accent, 3);

            _action.Invalidate();
        }

        // =========================================================
        // SMALL HELPERS
        // =========================================================

        private static bool Is(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        // "CheckedIn" -> "Checked In", "InProgress" -> "In Progress"
        private static string Pretty(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "Unknown";
            return Regex.Replace(s.Trim(), "(?<=[a-z])(?=[A-Z])", " ");
        }

        private static PropertyInfo FindProp(Type type, params string[] names)
        {
            foreach (string n in names)
            {
                var p = type.GetProperty(
                    n,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (p != null) return p;
            }
            return null;
        }

        private static DateTime? GetDate(object obj, PropertyInfo prop)
        {
            if (obj == null || prop == null) return null;

            object v = prop.GetValue(obj);
            if (v is DateTime) return (DateTime)v;
            if (v is DateTimeOffset) return ((DateTimeOffset)v).DateTime;
            return null;
        }

        private static double? GetNumber(object obj, PropertyInfo prop)
        {
            if (obj == null || prop == null) return null;

            object v = prop.GetValue(obj);
            if (v == null) return null;

            try
            {
                return Convert.ToDouble(v, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private static string GetText(object obj, PropertyInfo prop)
        {
            if (obj == null || prop == null) return null;
            object v = prop.GetValue(obj);
            return v == null ? null : v.ToString();
        }
    }
}
