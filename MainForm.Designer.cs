namespace InnEase_CRM
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlSidebar = new Panel();
            pnlSidebarHeader = new Panel();
            pnlSidebarFooter = new Panel();
            pnlMenu = new FlowLayoutPanel();
            lblLogo = new Label();
            lblLogoSub = new Label();

            btnHome = new Button();
            btnDashboard = new Button();
            btnGuests = new Button();
            btnReservations = new Button();
            btnFeedback = new Button();
            btnConsent = new Button();
            btnStaff = new Button();
            btnPromotions = new Button();
            btnInteractions = new Button();
            btnReports = new Button();
            btnAnalytics = new Button();
            btnCustomerFeedback = new Button();
            btnSystemAdmin = new Button();

            pnlMain = new Panel();
            pnlContentHost = new Panel();
            pnlHeader = new Panel();

            lblTitle = new Label();
            lblSubtitle = new Label();

            pnlWelcome = new Panel();
            lblWelcomeTitle = new Label();
            lblWelcomeText = new Label();

            pnlInfo1 = new Panel();
            pnlInfo2 = new Panel();
            pnlInfo3 = new Panel();

            lblInfo1Title = new Label();
            lblInfo1Text = new Label();

            lblInfo2Title = new Label();
            lblInfo2Text = new Label();

            lblInfo3Title = new Label();
            lblInfo3Text = new Label();

            pnlSidebar.SuspendLayout();
            pnlSidebarHeader.SuspendLayout();
            pnlSidebarFooter.SuspendLayout();
            pnlMenu.SuspendLayout();
            pnlMain.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlWelcome.SuspendLayout();
            pnlInfo1.SuspendLayout();
            pnlInfo2.SuspendLayout();
            pnlInfo3.SuspendLayout();

            SuspendLayout();

            // =========================================================
            // SIDEBAR  (same indigo theme as the BI dashboard)
            // =========================================================
            pnlSidebar.BackColor = DashTheme.Sidebar;
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Width = 270;

            // ---------- header (logo) ----------
            pnlSidebarHeader.BackColor = Color.FromArgb(32, 25, 76);
            pnlSidebarHeader.Dock = DockStyle.Top;
            pnlSidebarHeader.Height = 105;
            pnlSidebarHeader.Name = "pnlSidebarHeader";
            pnlSidebarHeader.Cursor = Cursors.Hand;

            lblLogo.AutoSize = true;
            lblLogo.Font = new Font("Segoe UI Semibold", 17F);
            lblLogo.ForeColor = Color.White;
            lblLogo.Location = new Point(22, 24);
            lblLogo.Text = "INNEASE CRM";
            lblLogo.Cursor = Cursors.Hand;

            lblLogoSub.AutoSize = true;
            lblLogoSub.Font = new Font("Segoe UI", 8.5F);
            lblLogoSub.ForeColor = Color.FromArgb(170, 165, 215);
            lblLogoSub.Location = new Point(24, 62);
            lblLogoSub.Text = "Customer Relationship Management";
            lblLogoSub.Cursor = Cursors.Hand;

            pnlSidebarHeader.Controls.Add(lblLogo);
            pnlSidebarHeader.Controls.Add(lblLogoSub);

            // ---------- footer (logged-in user info / logout) ----------
            pnlSidebarFooter.BackColor = Color.FromArgb(32, 25, 76);
            pnlSidebarFooter.Dock = DockStyle.Bottom;
            pnlSidebarFooter.Height = 50;
            pnlSidebarFooter.Name = "pnlSidebarFooter";

            btnLogout = new Button();
            btnLogout.Text = "Log Out";
            btnLogout.Dock = DockStyle.Fill;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnLogout.ForeColor = Color.FromArgb(239, 68, 68); // Red-ish color
            btnLogout.BackColor = Color.Transparent;
            btnLogout.Cursor = Cursors.Hand;
            pnlSidebarFooter.Controls.Add(btnLogout);

            // ---------- menu ----------
            pnlMenu.BackColor = DashTheme.Sidebar;
            pnlMenu.Dock = DockStyle.Fill;
            pnlMenu.FlowDirection = FlowDirection.TopDown;
            pnlMenu.WrapContents = false;
            pnlMenu.AutoScroll = true;
            pnlMenu.Name = "pnlMenu";
            pnlMenu.Padding = new Padding(10, 6, 10, 0);

            pnlMenu.Controls.Add(MakeSectionLabel("DASHBOARD"));
            ConfigureMenuButton(btnHome, "Home");
            ConfigureMenuButton(btnDashboard, "BI Dashboard");

            pnlMenu.Controls.Add(MakeSectionLabel("MANAGEMENT"));
            ConfigureMenuButton(btnGuests, "Guest Information");
            ConfigureMenuButton(btnReservations, "Reservations");
            ConfigureMenuButton(btnFeedback, "Guest Feedback & Requests");
            ConfigureMenuButton(btnConsent, "Customer Consent");
            ConfigureMenuButton(btnStaff, "Staff Accounts");
            ConfigureMenuButton(btnPromotions, "Promotions & Loyalty");
            ConfigureMenuButton(btnInteractions, "Guest Interactions");

            pnlMenu.Controls.Add(MakeSectionLabel("REPORTS"));
            ConfigureMenuButton(btnReports, "Sales Reports");
            ConfigureMenuButton(btnAnalytics, "Business Intelligence");
            ConfigureMenuButton(btnCustomerFeedback, "Customer Feedback & Complaints");

            pnlMenu.Controls.Add(MakeSectionLabel("SYSTEM"));
            ConfigureMenuButton(btnSystemAdmin, "System Administration");

            // Dock order: Fill first, then Bottom, then Top
            pnlSidebar.Controls.Add(pnlMenu);
            pnlSidebar.Controls.Add(pnlSidebarFooter);
            pnlSidebar.Controls.Add(pnlSidebarHeader);

            // =========================================================
            // MAIN AREA
            // =========================================================

            pnlMain.BackColor = DashTheme.PageBg;
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Name = "pnlMain";
            pnlMain.Padding = new Padding(25);

            // Host where the selected screen is embedded (sidebar stays visible)
            pnlContentHost.BackColor = DashTheme.PageBg;
            pnlContentHost.Dock = DockStyle.Fill;
            pnlContentHost.Name = "pnlContentHost";
            pnlContentHost.Visible = false;

            // Fill panel must be added first
            pnlMain.Controls.Add(pnlContentHost);

            // =========================================================
            // HEADER
            // =========================================================

            pnlHeader.BackColor = Color.White;
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 90;
            pnlHeader.Name = "pnlHeader";

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 20F);
            lblTitle.ForeColor = DashTheme.Ink;
            lblTitle.Location = new Point(25, 17);
            lblTitle.Text = "Welcome to InnEase CRM";

            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = DashTheme.Muted;
            lblSubtitle.Location = new Point(27, 54);
            lblSubtitle.Text =
                "Manage your guests, reservations, and customer relationships.";

            cboBranchSelector = new ComboBox();
            cboBranchSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBranchSelector.Font = new Font("Segoe UI", 10F);
            cboBranchSelector.FormattingEnabled = true;
            cboBranchSelector.Location = new Point(800, 30);
            cboBranchSelector.Name = "cboBranchSelector";
            cboBranchSelector.Size = new Size(200, 25);
            cboBranchSelector.Visible = false; // Hidden by default

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(cboBranchSelector);

            pnlMain.Controls.Add(pnlHeader);

            // =========================================================
            // WELCOME PANEL
            // =========================================================

            pnlWelcome.BackColor = Color.FromArgb(236, 233, 250);
            pnlWelcome.Dock = DockStyle.Top;
            pnlWelcome.Height = 150;
            pnlWelcome.Margin = new Padding(0, 20, 0, 0);
            pnlWelcome.Name = "pnlWelcome";
            pnlWelcome.Padding = new Padding(25);

            lblWelcomeTitle.AutoSize = true;
            lblWelcomeTitle.Font = new Font("Segoe UI Semibold", 18F);
            lblWelcomeTitle.ForeColor = DashTheme.Sidebar;
            lblWelcomeTitle.Location = new Point(25, 25);
            lblWelcomeTitle.Text = "Your Customer Management Hub";

            lblWelcomeText.AutoSize = false;
            lblWelcomeText.Font = new Font("Segoe UI", 10F);
            lblWelcomeText.ForeColor = Color.FromArgb(65, 70, 100);
            lblWelcomeText.Location = new Point(27, 65);
            lblWelcomeText.Size = new Size(750, 55);
            lblWelcomeText.Text =
                "Use the navigation menu to manage guest information, " +
                "reservations, feedback, customer interactions, " +
                "and business analytics.";

            pnlWelcome.Controls.Add(lblWelcomeTitle);
            pnlWelcome.Controls.Add(lblWelcomeText);

            pnlMain.Controls.Add(pnlWelcome);

            // =========================================================
            // INFORMATION CARDS  (click to open)
            // =========================================================

            ConfigureInfoCard(
                pnlInfo1,
                lblInfo1Title,
                lblInfo1Text,
                "CUSTOMER MANAGEMENT",
                "Manage guest profiles, customer consent, " +
                "feedback, and interactions.",
                25);

            ConfigureInfoCard(
                pnlInfo2,
                lblInfo2Title,
                lblInfo2Text,
                "RESERVATION MANAGEMENT",
                "Track reservations and monitor guest " +
                "booking information.",
                345);

            ConfigureInfoCard(
                pnlInfo3,
                lblInfo3Title,
                lblInfo3Text,
                "BUSINESS INTELLIGENCE",
                "View dashboards, KPIs, reports, graphs, " +
                "and business insights.",
                665);

            pnlMain.Controls.Add(pnlInfo1);
            pnlMain.Controls.Add(pnlInfo2);
            pnlMain.Controls.Add(pnlInfo3);

            // =========================================================
            // MAIN FORM
            // =========================================================

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = DashTheme.PageBg;
            ClientSize = new Size(1200, 750);

            Controls.Add(pnlMain);
            Controls.Add(pnlSidebar);

            MinimumSize = new Size(1050, 650);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "InnEase CRM - Main Menu";
            WindowState = FormWindowState.Maximized;

            pnlInfo3.ResumeLayout(false);
            pnlInfo2.ResumeLayout(false);
            pnlInfo1.ResumeLayout(false);
            pnlWelcome.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlMain.ResumeLayout(false);
            pnlMenu.ResumeLayout(false);
            pnlSidebarFooter.ResumeLayout(false);
            pnlSidebarFooter.PerformLayout();
            pnlSidebarHeader.ResumeLayout(false);
            pnlSidebarHeader.PerformLayout();
            pnlSidebar.ResumeLayout(false);

            ResumeLayout(false);
        }

        // =============================================================
        // SIDEBAR HELPERS
        // =============================================================

        private Label MakeSectionLabel(string text)
        {
            return new Label
            {
                Text = text,
                UseMnemonic = false,
                AutoSize = false,
                Size = new Size(230, 32),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 114, 180),
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(12, 0, 0, 4),
                Margin = new Padding(0, 8, 0, 0)
            };
        }

        private void ConfigureMenuButton(
            Button button,
            string text)
        {
            button.BackColor = DashTheme.Sidebar;

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = DashTheme.SidebarHover;
            button.FlatAppearance.MouseDownBackColor = DashTheme.SidebarHover;

            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Segoe UI", 9.5F);
            button.ForeColor = Color.FromArgb(210, 205, 240);
            button.Size = new Size(230, 40);
            button.Margin = new Padding(0, 1, 0, 1);
            button.Text = text;
            button.UseMnemonic = false;   // so "&" is shown as-is
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(14, 0, 5, 0);
            button.UseVisualStyleBackColor = false;
            button.Cursor = Cursors.Hand;
            button.TabStop = false;

            pnlMenu.Controls.Add(button);
        }

        // =============================================================
        // INFORMATION CARD HELPER
        // =============================================================

        private void ConfigureInfoCard(
            Panel panel,
            Label title,
            Label description,
            string titleText,
            string descriptionText,
            int left)
        {
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Location = new Point(left, 290);
            panel.Size = new Size(290, 150);

            title.AutoSize = true;
            title.Font = new Font("Segoe UI Semibold", 11F);
            title.ForeColor = DashTheme.Accent;
            title.Location = new Point(20, 20);
            title.Text = titleText;

            description.AutoSize = false;
            description.Font = new Font("Segoe UI", 9F);
            description.ForeColor = DashTheme.Muted;
            description.Location = new Point(20, 55);
            description.Size = new Size(245, 70);
            description.Text = descriptionText;

            panel.Controls.Add(title);
            panel.Controls.Add(description);
        }

        // =============================================================
        // CONTROLS
        // =============================================================

        private Panel pnlSidebar;
        private Panel pnlSidebarHeader;
        private Panel pnlSidebarFooter;
        private FlowLayoutPanel pnlMenu;
        private Panel pnlMain;
        private Panel pnlContentHost;
        private Panel pnlHeader;
        private Panel pnlWelcome;

        private Panel pnlInfo1;
        private Panel pnlInfo2;
        private Panel pnlInfo3;

        private Label lblLogo;
        private Label lblLogoSub;

        private Label lblTitle;
        private Label lblSubtitle;

        private Label lblWelcomeTitle;
        private Label lblWelcomeText;

        private Label lblInfo1Title;
        private Label lblInfo1Text;

        private Label lblInfo2Title;
        private Label lblInfo2Text;

        private Label lblInfo3Title;
        private Label lblInfo3Text;


        private Button btnHome;
        private Button btnDashboard;
        private Button btnGuests;
        private Button btnReservations;
        private Button btnFeedback;
        private Button btnConsent;
        private Button btnStaff;
        private Button btnPromotions;
        private Button btnInteractions;
        private Button btnReports;
        private Button btnAnalytics;
        private Button btnCustomerFeedback;
        private Button btnSystemAdmin;
        private Button btnLogout;
        private ComboBox cboBranchSelector;
    }
}
