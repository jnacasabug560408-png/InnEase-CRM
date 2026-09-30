namespace InnEase_CRM
{
    partial class SalesReportForm
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
            lblTitle = new Label();
            lblReportTitle = new Label();
            pnlControls = new Panel();
            btnDaily = new Button();
            btnWeekly = new Button();
            btnMonthly = new Button();
            btnRefresh = new Button();
            lblStatus = new Label();
            pnlChart = new Panel();
            dgvSalesReport = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)dgvSalesReport).BeginInit();
            SuspendLayout();

            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(200, 30);
            lblTitle.Text = "Sales Report Module";

            // lblReportTitle
            lblReportTitle.AutoSize = true;
            lblReportTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblReportTitle.Location = new Point(20, 60);
            lblReportTitle.Name = "lblReportTitle";
            lblReportTitle.Size = new Size(150, 25);
            lblReportTitle.Text = "Daily Sales Report";

            // pnlControls
            pnlControls.Location = new Point(20, 95);
            pnlControls.Name = "pnlControls";
            pnlControls.Size = new Size(800, 40);
            pnlControls.BackColor = Color.White;

            // btnDaily
            btnDaily.Location = new Point(0, 5);
            btnDaily.Name = "btnDaily";
            btnDaily.Size = new Size(100, 30);
            btnDaily.Text = "Daily";
            btnDaily.BackColor = Color.FromArgb(146, 30, 98);
            btnDaily.ForeColor = Color.White;
            btnDaily.FlatStyle = FlatStyle.Flat;
            btnDaily.UseVisualStyleBackColor = false;

            // btnWeekly
            btnWeekly.Location = new Point(110, 5);
            btnWeekly.Name = "btnWeekly";
            btnWeekly.Size = new Size(100, 30);
            btnWeekly.Text = "Weekly";
            btnWeekly.BackColor = Color.White;
            btnWeekly.ForeColor = Color.FromArgb(146, 30, 98);
            btnWeekly.FlatStyle = FlatStyle.Flat;
            btnWeekly.UseVisualStyleBackColor = false;

            // btnMonthly
            btnMonthly.Location = new Point(220, 5);
            btnMonthly.Name = "btnMonthly";
            btnMonthly.Size = new Size(100, 30);
            btnMonthly.Text = "Monthly";
            btnMonthly.BackColor = Color.White;
            btnMonthly.ForeColor = Color.FromArgb(146, 30, 98);
            btnMonthly.FlatStyle = FlatStyle.Flat;
            btnMonthly.UseVisualStyleBackColor = false;

            // btnRefresh
            btnRefresh.Location = new Point(330, 5);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.Text = "Refresh";
            btnRefresh.BackColor = Color.White;
            btnRefresh.ForeColor = Color.FromArgb(30, 34, 58);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.UseVisualStyleBackColor = false;

            // lblStatus
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(440, 10);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 20);
            lblStatus.Text = "";

            pnlControls.Controls.Add(btnDaily);
            pnlControls.Controls.Add(btnWeekly);
            pnlControls.Controls.Add(btnMonthly);
            pnlControls.Controls.Add(btnRefresh);
            pnlControls.Controls.Add(lblStatus);

            // pnlChart
            pnlChart.Location = new Point(20, 145);
            pnlChart.Name = "pnlChart";
            pnlChart.Size = new Size(1160, 300);
            pnlChart.BackColor = Color.White;
            pnlChart.BorderStyle = BorderStyle.FixedSingle;

            // dgvSalesReport
            dgvSalesReport.AllowUserToAddRows = false;
            dgvSalesReport.AllowUserToDeleteRows = false;
            dgvSalesReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSalesReport.BackgroundColor = Color.White;
            dgvSalesReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSalesReport.Location = new Point(20, 455);
            dgvSalesReport.Name = "dgvSalesReport";
            dgvSalesReport.ReadOnly = true;
            dgvSalesReport.RowHeadersVisible = false;
            dgvSalesReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSalesReport.Size = new Size(1160, 250);
            dgvSalesReport.TabIndex = 0;

            // SalesReportForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 720);
            Controls.Add(lblTitle);
            Controls.Add(lblReportTitle);
            Controls.Add(pnlControls);
            Controls.Add(pnlChart);
            Controls.Add(dgvSalesReport);
            Font = new Font("Segoe UI", 9F);
            Name = "SalesReportForm";
            Text = "InnEase CRM - Sales Report";
            WindowState = FormWindowState.Maximized;

            ((System.ComponentModel.ISupportInitialize)dgvSalesReport).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitle;
        private Label lblReportTitle;
        private Panel pnlControls;
        private Button btnDaily;
        private Button btnWeekly;
        private Button btnMonthly;
        private Button btnRefresh;
        private Label lblStatus;
        private Panel pnlChart;
        private DataGridView dgvSalesReport;
    }
}
