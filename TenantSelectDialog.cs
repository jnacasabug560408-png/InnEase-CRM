using System;
using System.Drawing;
using System.Windows.Forms;

namespace InnEase_CRM
{
    // Clean modal shown when Admin / Manager / Staff is clicked on the Quick Demo panel.
    // Replaces the old ContextMenuStrip with the same three choices, styled with the
    // shared TileCard/Theme helpers from InnEaseShared.cs.
    public class TenantSelectDialog : Form
    {
        public int SelectedCompanyId { get; private set; }
        public string SelectedSuffix { get; private set; }   // "tenanta" / "tenantb" / "tenantc"

        public TenantSelectDialog(string roleLabel)
        {
            Build(roleLabel);
        }

        private void Build(string roleLabel)
        {
            Text = "Select Tenant";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = Theme.Bg;
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(420, 372);

            Controls.Add(new Label
            {
                Text = "Quick Demo - " + roleLabel,
                Font = new Font("Segoe UI Semibold", 14F), ForeColor = Theme.Ink,
                AutoSize = true, Location = new Point(24, 20)
            });
            Controls.Add(new Label
            {
                Text = "Choose which tenant to sign in as.",
                ForeColor = Theme.Muted, AutoSize = true, Location = new Point(26, 50)
            });

            AddOption("Tenant A", "Basic - Main Transactions & Data Collection", Theme.Blue, 88, 1, "tenanta");
            AddOption("Tenant B", "Pro - Business Intelligence & Actions", Theme.Green, 168, 2, "tenantb");
            AddOption("Tenant C", "Enterprise - Multi-Branching, Global BI & Actions", Theme.Brand, 248, 3, "tenantc");

            var cancel = Theme.MakeButton("Cancel", Theme.Muted, 100);
            cancel.Location = new Point(296, 328);
            cancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(cancel);

            AcceptButton = null;
            CancelButton = cancel;
        }

        private void AddOption(string title, string sub, Color accent, int y, int companyId, string suffix)
        {
            var card = new TileCard(accent, true) { Location = new Point(24, y), Width = 372 };
            card.SetContent(title, sub, null, null);
            card.Click += (s, e) =>
            {
                SelectedCompanyId = companyId;
                SelectedSuffix = suffix;
                DialogResult = DialogResult.OK;
                Close();
            };
            Controls.Add(card);
        }
    }
}
