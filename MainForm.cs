#nullable disable

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace InnEase_CRM
{
    public partial class MainForm : Form
    {
        // The screen currently shown in the content area (sidebar always stays visible)
        private Form _activeChild;
        private Func<Form> _activeFactory;
        private Button _activeButton;

        private readonly Button[] _navButtons;
        private Button[] _masterButtons;   // the only buttons Super Admin may use
        private HashSet<Button> _allowedButtons;   // universal navigation gate for both master and tenant sessions

        private readonly Font _navFont = new Font("Segoe UI", 9.5F);
        private readonly Font _navFontActive = new Font("Segoe UI", 9.5F, FontStyle.Bold);

        public MainForm()
        {
            InitializeComponent();

            _navButtons = new[]
            {
                btnHome, btnDashboard, btnGuests, btnReservations, btnFeedback,
                btnConsent, btnStaff, btnPromotions, btnInteractions,
                btnReports, btnAnalytics, btnCustomerFeedback, btnSystemAdmin
            };

            // Add custom navigation for Inquiry and Complaint forms
            // These will be accessible from the TenantA interface

            // Shared: home navigation via logo / header / Home button
            btnHome.Click += (s, e) => ShowHome();
            lblLogo.Click += (s, e) => ShowHome();
            lblLogoSub.Click += (s, e) => ShowHome();
            pnlSidebarHeader.Click += (s, e) => ShowHome();

            // Log out handler
            btnLogout.Click += (s, e) => LogOut();

            // Super Admin (Master) and tenant users get completely separate wiring,
            // so a Master session never has a handler that opens a tenant screen.
            if (UserSession.IsMaster) WireMasterNavigation();
            else
            {
                WireTenantNavigation();
            }

            ShowHome();
            ApplyRolePermissions();
        }



        // =====================================================
        // LOGOUT WIRING
        // =====================================================
        private void LogOut()
        {
            // Clear user session state
            UserSession.Token = string.Empty;
            UserSession.Role = string.Empty;
            UserSession.FullName = string.Empty;
            UserSession.IsMaster = false;
            
            TenantSession.UserRole = string.Empty;
            TenantSession.UserName = string.Empty;
            TenantSession.TenantTier = string.Empty;
            TenantSession.TenantId = null;
            TenantSession.IsMaster = false;
            
            // Clear API Client token
            ApiClient.Configure(string.Empty);
            
            // Close active forms and clear child UI
            CloseActiveChild();
            
            // Re-open LoginForm
            this.Hide();
            var loginForm = new LoginForm();
            loginForm.FormClosed += (s, args) => this.Close();
            loginForm.Show();
        }

        // =====================================================
        // NAVIGATION WIRING
        // =====================================================

        private void WireTenantNavigation()
        {
            // SIDEBAR BUTTONS (each one opens inside this window)
            btnDashboard.Click += (s, e) => Open(btnDashboard, () => new DashboardForm(true));
            btnGuests.Click += (s, e) => Open(btnGuests, () => new Form1());
            btnReservations.Click += (s, e) => Open(btnReservations, () => new ReservationForm());
            btnFeedback.Click += (s, e) => Open(btnFeedback, () => new GuestFeedbackForm());
            btnConsent.Click += (s, e) => Open(btnConsent, () => new CustomerConsentForm());
            btnStaff.Click += (s, e) => Open(btnStaff, () => new StaffForm());
            btnPromotions.Click += (s, e) => Open(btnPromotions, () => new PromotionForm());
            btnInteractions.Click += (s, e) => Open(btnInteractions, () => new GuestInteractionForm());
            btnReports.Click += (s, e) => Open(btnReports, () => new SalesReportForm());
            btnAnalytics.Click += (s, e) => Open(btnAnalytics, () => new ReportsForm());
            btnCustomerFeedback.Click += (s, e) => Open(btnCustomerFeedback, () => new GuestFeedbackForm());
            btnSystemAdmin.Click += (s, e) => Open(btnSystemAdmin, () => new SystemAdminForm());

            // HOME CARDS (click to open)
            MakeCardClickable(pnlInfo1, (s, e) => Open(btnGuests, () => new Form1()));
            MakeCardClickable(pnlInfo2, (s, e) => Open(btnReservations, () => new ReservationForm()));
            MakeCardClickable(pnlInfo3, (s, e) => Open(btnDashboard, () => new DashboardForm(true)));
        }

        // Public method to open Inquiry form from TenantA interface
        public void OpenInquiryForm()
        {
            Open(btnInteractions, () => new InquiryForm());
        }

        // Public method to open Complaint form from TenantA interface
        public void OpenComplaintForm()
        {
            Open(btnInteractions, () => new ComplaintForm());
        }

        private void WireMasterNavigation()
        {
            // Reuse three existing buttons; the tenant Click handlers are never attached in this mode.
            btnReports.Text = "System Administration";
            btnAnalytics.Text = "Business Intelligence";
            btnSystemAdmin.Text = "Subscriptions";

            _masterButtons = new[] { btnReports, btnAnalytics, btnSystemAdmin };

            btnReports.Click += (s, e) => Open(btnReports, () => new AdminPanelForm());
            btnAnalytics.Click += (s, e) => Open(btnAnalytics, () => new PlatformBiForm());
            btnSystemAdmin.Click += (s, e) => Open(btnSystemAdmin, () => new SubscriptionsForm());

            // Home dashboard: cards
            lblInfo1Title.Text = "SYSTEM ADMINISTRATION";
            lblInfo1Text.Text = "Full system configuration, tenant management, and subscriptions.";
            lblInfo2Title.Text = "PLATFORM BI";
            lblInfo2Text.Text = "Global platform-level analytics across all tenants.";
            lblInfo3Title.Text = "SUBSCRIPTIONS MANAGEMENT";
            lblInfo3Text.Text = "Track active plans, renewal dates, and feature access.";

            MakeCardClickable(pnlInfo1, (s, e) => Open(btnReports, () => new AdminPanelForm()));
            MakeCardClickable(pnlInfo2, (s, e) => Open(btnAnalytics, () => new PlatformBiForm()));
            MakeCardClickable(pnlInfo3, (s, e) => Open(btnSystemAdmin, () => new SubscriptionsForm()));

            // Home dashboard: header and welcome banner
            lblTitle.Text = $"Welcome, {UserSession.FullName}";
            lblSubtitle.Text = "System Administration & Platform Management";
            lblWelcomeTitle.Text = "Your Platform Administration Hub";
            lblWelcomeText.Text = "Use the navigation menu to manage tenants and subscriptions " +
                                  "and to monitor platform-level metrics. Private guest and " +
                                  "reservation data of individual tenants is not accessible here.";
            lblLogoSub.Text = "Platform Administration";
            Text = "InnEase CRM - Platform Administration";
        }

        // =====================================================
        // ROLE-BASED MENU VISIBILITY
        // =====================================================

        private void ApplyRolePermissions()
        {
            if (UserSession.IsMaster)
            {
                ApplySuperAdminMenu();
                _allowedButtons = new HashSet<Button>(_masterButtons);
                return;
            }

            // Role-based permissions from Use Case Diagram
            // Use TenantSession.UserRole for tenant users, fallback to UserSession.Role
            var role = (!string.IsNullOrEmpty(TenantSession.UserRole) ? TenantSession.UserRole : UserSession.Role)?.ToLowerInvariant() ?? "";

            switch (role)
            {
                case "admin":
                    // Admin: Full access to all tenant modules (Staff, Guests, Transactions, BI, etc.)
                    SetButtonVisibility(new[] { btnStaff, btnGuests, btnPromotions, btnReports, btnReservations, 
                        btnFeedback, btnConsent, btnInteractions, btnCustomerFeedback, btnDashboard, btnAnalytics }, true);
                    SetButtonVisibility(new[] { btnSystemAdmin }, false);
                    break;

                case "manager":
                    // Manager: Access to Transactions, Data Collections, BI, and Promotions. No Staff or System Admin.
                    SetButtonVisibility(new[] { btnGuests, btnReservations, btnFeedback, btnConsent,
                        btnInteractions, btnCustomerFeedback, btnDashboard, btnAnalytics, btnReports, btnPromotions }, true);
                    SetButtonVisibility(new[] { btnStaff, btnSystemAdmin }, false);
                    break;

                case "staff":
                    // Staff: Guest Interactions, Reservations, Guest Requests & Feedback, Customer Consent
                    SetButtonVisibility(new[] { btnInteractions, btnReservations, btnCustomerFeedback, btnFeedback, btnConsent }, true);
                    SetButtonVisibility(new[] { btnDashboard, btnGuests, btnStaff, btnPromotions, btnReports, btnAnalytics, btnSystemAdmin }, false);
                    break;

                default:
                    // Unknown role - hide everything
                    SetButtonVisibility(_navButtons, false);
                    break;
            }

            // Subscription gate: runs AFTER the role rules, so a button is visible
            // only if BOTH the role and the tenant's plan allow it.
            ApplyPlanGate();

            // Show branch selector for Tenant C (Enterprise Tier)
            if (string.Equals(TenantSession.TenantTier, "Enterprise", StringComparison.OrdinalIgnoreCase))
            {
                cboBranchSelector.Visible = true;
                cboBranchSelector.Items.AddRange(new object[] { "All Branches", "Branch 1 - Manila", "Branch 2 - Cebu", "Branch 3 - Davao" });
                cboBranchSelector.SelectedIndex = 0;
            }
        }

        // Hides modules that the tenant's subscription plan does not include.
        //   Starter    (Tenant A): Data Collection only
        //   Standard   (Tenant B): + Promotions, Business Intelligence
        //   Enterprise (Tenant C): + Branching (branch filter, to be added)
        private void ApplyPlanGate()
        {
            // Apply subscription-based feature access to sidebar
            FeatureAccess.ApplyToSidebar(this);
        }

        private void SetButtonVisibility(Button[] buttons, bool visible)
        {
            foreach (var btn in buttons)
            {
                btn.Visible = visible;
            }
        }

        private void ApplySuperAdminMenu()
        {
            // Super Admin: System Administration, Business Intelligence, Subscriptions
            // Hide every tenant button AND every section label ("DASHBOARD", "MANAGEMENT", ...)
            foreach (Control c in pnlMenu.Controls)
                c.Visible = false;

            // One section label, followed by the three Super Admin items, moved to the top of the flow
            var platformLabel = MakeSectionLabel("PLATFORM");
            pnlMenu.Controls.Add(platformLabel);

            var ordered = new Control[] { platformLabel, btnReports, btnAnalytics, btnSystemAdmin };
            for (int i = 0; i < ordered.Length; i++)
            {
                ordered[i].Visible = true;
                pnlMenu.Controls.SetChildIndex(ordered[i], i);
            }
        }

        // =====================================================
        // SHOW A SCREEN INSIDE THE MAIN WINDOW
        // =====================================================

        private void Open(Button navButton, Func<Form> factory)
        {
            // Hard gate: only a button in the current role's allowed set may open a screen —
            // sidebar click, home-page card, or KPI shortcut all funnel through here.
            // If the button is hidden (either by role or plan), the feature is disabled.
            if (!navButton.Visible && !UserSession.IsMaster)
                return;

            CloseActiveChild();

            _activeFactory = factory;
            _activeButton = navButton;

            Form child = factory();

            if (child is DashboardForm dashboard)
                dashboard.KpiClicked += NavigateFromKpi;

            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.MinimumSize = Size.Empty;
            child.WindowState = FormWindowState.Normal;
            child.Dock = DockStyle.Fill;

            pnlContentHost.Controls.Add(child);

            SetHomeVisible(false);
            pnlMain.Padding = Padding.Empty;
            pnlContentHost.Visible = true;

            child.Show();

            _activeChild = child;
            SetActiveNav(navButton);
        }

        private void ShowHome()
        {
            CloseActiveChild();
            pnlContentHost.Visible = false;
            pnlMain.Padding = new Padding(25);
            SetHomeVisible(true);
            SetActiveNav(btnHome);
        }

        public void NavigateTo(string dest, string arg = null)
        {
            if (dest == "Subscriptions") Open(btnSystemAdmin, () => new SubscriptionsForm());
            else if (dest == "BI") Open(btnAnalytics, () => new PlatformBiForm());
            else if (dest == "AdminPanel") Open(btnReports, () => new AdminPanelForm { InitialFilter = arg });
        }

        private void CloseActiveChild()
        {
            if (_activeChild != null)
            {
                pnlContentHost.Controls.Remove(_activeChild);
                _activeChild.Dispose();
                _activeChild = null;
            }

            _activeFactory = null;
            _activeButton = null;
        }

        private void SetHomeVisible(bool visible)
        {
            pnlHeader.Visible = visible;
            pnlWelcome.Visible = visible;
            pnlInfo1.Visible = visible;
            pnlInfo2.Visible = visible;
            pnlInfo3.Visible = visible;
        }

        // Lets a KPI card on the dashboard jump straight to the matching screen.
        private void NavigateFromKpi(string target)
        {
            switch (target)
            {
                case "Guests":
                    Open(btnGuests, () => new Form1());
                    break;

                case "Reservations":
                    Open(btnReservations, () => new ReservationForm());
                    break;

                case "Feedback":
                    Open(btnFeedback, () => new GuestFeedbackForm());
                    break;

                case "Interactions":
                    Open(btnInteractions, () => new GuestInteractionForm());
                    break;
            }
        }

        // =====================================================
        // SIDEBAR HIGHLIGHT
        // =====================================================

        private void SetActiveNav(Button active)
        {
            foreach (var b in _navButtons)
            {
                bool on = b == active;

                b.BackColor = on ? DashTheme.Accent : DashTheme.Sidebar;
                b.ForeColor = on ? Color.White : Color.FromArgb(210, 205, 240);
                b.Font = on ? _navFontActive : _navFont;

                b.FlatAppearance.MouseOverBackColor = on ? DashTheme.Accent : DashTheme.SidebarHover;
                b.FlatAppearance.MouseDownBackColor = on ? DashTheme.Accent : DashTheme.SidebarHover;
            }
        }

        private void MakeCardClickable(Control card, EventHandler handler)
        {
            card.Cursor = Cursors.Hand;
            card.Click += handler;

            foreach (Control c in card.Controls)
            {
                c.Cursor = Cursors.Hand;
                c.Click += handler;
            }
        }
    }
}
