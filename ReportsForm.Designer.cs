namespace InnEase_CRM
{
    partial class ReportsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            tabControl1 = new TabControl();

            tabReports = new TabPage();
            dgvReports = new DataGridView();

            tabRelationships = new TabPage();
            dgvRelationships = new DataGridView();

            tabRetention = new TabPage();
            dgvRetention = new DataGridView();

            tabAnalytics = new TabPage();
            pnlKpiContainer = new Panel();
            pnlChartContainer = new Panel();

            btnRefresh = new Button();

            ((System.ComponentModel.ISupportInitialize)dgvReports).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRelationships).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRetention).BeginInit();

            tabControl1.SuspendLayout();
            tabReports.SuspendLayout();
            tabRelationships.SuspendLayout();
            tabRetention.SuspendLayout();
            tabAnalytics.SuspendLayout();

            SuspendLayout();

            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font(
                "Segoe UI",
                14F,
                FontStyle.Bold);

            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(500, 25);
            lblTitle.Text = "InnEase CRM - Strategic Reports and Analytics";

            // 
            // btnRefresh
            // 
            btnRefresh.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            btnRefresh.Location = new Point(
                1100,
                15);

            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(110, 30);
            btnRefresh.Text = "REFRESH";
            btnRefresh.UseVisualStyleBackColor = true;

            // 
            // tabControl1
            // 
            tabControl1.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            tabControl1.Location = new Point(20, 55);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;

            tabControl1.Size = new Size(
                1180,
                650);

            tabControl1.Controls.Add(tabReports);
            tabControl1.Controls.Add(tabRelationships);
            tabControl1.Controls.Add(tabRetention);
            tabControl1.Controls.Add(tabAnalytics);

            // 
            // tabReports
            // 
            tabReports.Name = "tabReports";
            tabReports.Text = "Customer Reports";
            tabReports.Padding = new Padding(10);
            tabReports.Controls.Add(dgvReports);

            // 
            // dgvReports
            // 
            dgvReports.Dock = DockStyle.Fill;
            dgvReports.Name = "dgvReports";
            dgvReports.ReadOnly = true;
            dgvReports.AllowUserToAddRows = false;
            dgvReports.AllowUserToDeleteRows = false;
            dgvReports.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvReports.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvReports.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // 
            // tabRelationships
            // 
            tabRelationships.Name = "tabRelationships";
            tabRelationships.Text = "Relationships";
            tabRelationships.Padding = new Padding(10);
            tabRelationships.Controls.Add(dgvRelationships);

            // 
            // dgvRelationships
            // 
            dgvRelationships.Dock = DockStyle.Fill;
            dgvRelationships.Name = "dgvRelationships";
            dgvRelationships.ReadOnly = true;
            dgvRelationships.AllowUserToAddRows = false;
            dgvRelationships.AllowUserToDeleteRows = false;

            dgvRelationships.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvRelationships.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvRelationships.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // 
            // tabRetention
            // 
            tabRetention.Name = "tabRetention";
            tabRetention.Text = "Retention";
            tabRetention.Padding = new Padding(10);
            tabRetention.Controls.Add(dgvRetention);

            // 
            // dgvRetention
            // 
            dgvRetention.Dock = DockStyle.Fill;
            dgvRetention.Name = "dgvRetention";
            dgvRetention.ReadOnly = true;
            dgvRetention.AllowUserToAddRows = false;
            dgvRetention.AllowUserToDeleteRows = false;

            dgvRetention.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvRetention.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvRetention.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // 
            // tabAnalytics
            // 
            tabAnalytics.Name = "tabAnalytics";
            tabAnalytics.Text = "Analytics (KPIs)";
            tabAnalytics.Padding = new Padding(15);
            tabAnalytics.AutoScroll = true;

            tabAnalytics.Controls.Add(pnlKpiContainer);
            tabAnalytics.Controls.Add(pnlChartContainer);

            // 
            // pnlKpiContainer
            // 
            pnlKpiContainer.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            pnlKpiContainer.Location = new Point(15, 15);

            pnlKpiContainer.Name =
                "pnlKpiContainer";

            pnlKpiContainer.Size =
                new Size(1100, 340);

            pnlKpiContainer.AutoScroll = true;

            // 
            // pnlChartContainer
            // 
            pnlChartContainer.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            pnlChartContainer.Location =
                new Point(15, 365);

            pnlChartContainer.Name =
                "pnlChartContainer";

            pnlChartContainer.Size =
                new Size(1100, 180);

            // 
            // ReportsForm
            // 
            AutoScaleDimensions =
                new SizeF(7F, 15F);

            AutoScaleMode =
                AutoScaleMode.Font;

            ClientSize =
                new Size(1200, 750);

            Controls.Add(tabControl1);
            Controls.Add(btnRefresh);
            Controls.Add(lblTitle);

            Name = "ReportsForm";
            Text = "InnEase CRM - Reports and Analytics";

            ((System.ComponentModel.ISupportInitialize)dgvReports).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRelationships).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRetention).EndInit();

            tabReports.ResumeLayout(false);
            tabRelationships.ResumeLayout(false);
            tabRetention.ResumeLayout(false);
            tabAnalytics.ResumeLayout(false);

            tabControl1.ResumeLayout(false);

            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitle;
        private Button btnRefresh;

        private TabControl tabControl1;

        private TabPage tabReports;
        private DataGridView dgvReports;

        private TabPage tabRelationships;
        private DataGridView dgvRelationships;

        private TabPage tabRetention;
        private DataGridView dgvRetention;

        private TabPage tabAnalytics;
        private Panel pnlKpiContainer;
        private Panel pnlChartContainer;
    }
}
