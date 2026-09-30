#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace InnEase_CRM
{
    public partial class TenantAForm : Form
    {
        private Action<string> _navigateTo;
        private MainForm? _mainForm;

        public TenantAForm(Action<string> navigateTo, MainForm? mainForm = null)
        {
            _navigateTo = navigateTo;
            _mainForm = mainForm;
            InitializeComponent();
            BuildTenantAInterface();
        }

        private void BuildTenantAInterface()
        {
            this.Text = "Tenant A - Transaction Processing Center";
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
                Text = "Tenant A - Direct Transaction Processing",
                Font = new Font("Segoe UI Semibold", 18F),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var subLabel = new Label
            {
                Text = "Azure Bay Hotel - Quick access to reservations, check-in/check-out, and point-of-sale operations",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(112, 118, 142),
                Location = new Point(20, 55),
                AutoSize = true
            };

            // Transaction Processing Section
            var transactionSection = CreateSection("Main Transaction Processing", 90);
            
            var reservationCard = CreateQuickActionCard(
                "Reservations",
                "Create and manage room reservations",
                Color.FromArgb(30, 136, 229),
                "reservations");
            
            var checkinCard = CreateQuickActionCard(
                "Guest Check-In",
                "Process guest arrivals and room assignments",
                Color.FromArgb(34, 160, 90),
                "checkin");
            
            var checkoutCard = CreateQuickActionCard(
                "Guest Check-Out",
                "Process departures and final billing",
                Color.FromArgb(214, 80, 110),
                "checkout");
            
            var posCard = CreateQuickActionCard(
                "Point of Sale",
                "Direct sales and service entries",
                Color.FromArgb(226, 164, 52),
                "pos");

            transactionSection.Controls.Add(reservationCard);
            transactionSection.Controls.Add(checkinCard);
            transactionSection.Controls.Add(checkoutCard);
            transactionSection.Controls.Add(posCard);

            // Data Collection Section
            var dataSection = CreateSection("Data Collection", 320);
            
            var guestCard = CreateQuickActionCard(
                "Guest Registration",
                "Register new guest profiles",
                Color.FromArgb(146, 30, 98),
                "guests");
            
            var feedbackCard = CreateQuickActionCard(
                "Guest Feedback",
                "Record customer feedback and reviews",
                Color.FromArgb(108, 92, 231),
                "feedback");
            
            var inquiryCard = CreateQuickActionCard(
                "Inquiry Handling",
                "Record and manage guest inquiries",
                Color.FromArgb(38, 150, 130),
                "inquiries");
            
            var complaintCard = CreateQuickActionCard(
                "Complaints Management",
                "Log and track customer complaints",
                Color.FromArgb(214, 80, 110),
                "complaints");

            dataSection.Controls.Add(guestCard);
            dataSection.Controls.Add(feedbackCard);
            dataSection.Controls.Add(inquiryCard);
            dataSection.Controls.Add(complaintCard);

            mainPanel.Controls.Add(headerLabel);
            mainPanel.Controls.Add(subLabel);
            mainPanel.Controls.Add(transactionSection);
            mainPanel.Controls.Add(dataSection);

            this.Controls.Add(mainPanel);
        }

        private Panel CreateSection(string title, int y)
        {
            var sectionPanel = new Panel
            {
                Location = new Point(20, y),
                Size = new Size(1100, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(15, 15),
                AutoSize = true
            };

            sectionPanel.Controls.Add(titleLabel);

            return sectionPanel;
        }

        private Panel CreateQuickActionCard(string title, string description, Color accentColor, string action)
        {
            var card = new Panel
            {
                Size = new Size(250, 150),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand,
                Margin = new Padding(10)
            };

            // Accent bar
            var accentBar = new Panel
            {
                BackColor = accentColor,
                Dock = DockStyle.Left,
                Width = 4
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 34, 58),
                Location = new Point(15, 20),
                AutoSize = true
            };

            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(112, 118, 142),
                Location = new Point(15, 50),
                Size = new Size(220, 60),
                TextAlign = ContentAlignment.TopLeft
            };

            var actionLabel = new Label
            {
                Text = "→",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = accentColor,
                Location = new Point(200, 100),
                AutoSize = true
            };

            card.Controls.Add(accentBar);
            card.Controls.Add(titleLabel);
            card.Controls.Add(descLabel);
            card.Controls.Add(actionLabel);

            // Hover effect
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(248, 250, 255);
            };

            card.MouseLeave += (s, e) =>
            {
                card.BackColor = Color.White;
            };

            card.Click += (s, e) =>
            {
                if (_mainForm != null)
                {
                    // Handle new Inquiry and Complaint forms
                    if (action == "inquiries")
                    {
                        _mainForm.OpenInquiryForm();
                    }
                    else if (action == "complaints")
                    {
                        _mainForm.OpenComplaintForm();
                    }
                    else
                    {
                        _navigateTo?.Invoke(action);
                    }
                }
                else
                {
                    _navigateTo?.Invoke(action);
                }
            };

            return card;
        }
    }
}
