namespace InnEase_CRM
{
    partial class GuestFeedbackForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblGuestId;
        private System.Windows.Forms.TextBox txtGuestId;

        private System.Windows.Forms.Label lblRequestType;
        private System.Windows.Forms.ComboBox cmbRequestType;

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtMessage;

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;

        private System.Windows.Forms.DataGridView dgvFeedback;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblGuestId = new System.Windows.Forms.Label();
            txtGuestId = new System.Windows.Forms.TextBox();

            lblRequestType = new System.Windows.Forms.Label();
            cmbRequestType = new System.Windows.Forms.ComboBox();

            lblStatus = new System.Windows.Forms.Label();
            cmbStatus = new System.Windows.Forms.ComboBox();

            lblMessage = new System.Windows.Forms.Label();
            txtMessage = new System.Windows.Forms.TextBox();

            btnAdd = new System.Windows.Forms.Button();
            btnUpdate = new System.Windows.Forms.Button();
            btnDelete = new System.Windows.Forms.Button();

            dgvFeedback = new System.Windows.Forms.DataGridView();

            ((System.ComponentModel.ISupportInitialize)dgvFeedback).BeginInit();
            SuspendLayout();

            // =========================
            // GUEST ID
            // =========================

            lblGuestId.AutoSize = true;
            lblGuestId.Location =
                new System.Drawing.Point(30, 30);
            lblGuestId.Name = "lblGuestId";
            lblGuestId.Size =
                new System.Drawing.Size(55, 15);
            lblGuestId.Text = "Guest ID:";

            txtGuestId.Location =
                new System.Drawing.Point(30, 50);
            txtGuestId.Name = "txtGuestId";
            txtGuestId.Size =
                new System.Drawing.Size(220, 23);

            // =========================
            // REQUEST TYPE
            // =========================

            lblRequestType.AutoSize = true;
            lblRequestType.Location =
                new System.Drawing.Point(280, 30);
            lblRequestType.Name = "lblRequestType";
            lblRequestType.Size =
                new System.Drawing.Size(79, 15);
            lblRequestType.Text = "Request Type:";

            cmbRequestType.DropDownStyle =
                System.Windows.Forms.ComboBoxStyle.DropDownList;

            cmbRequestType.Items.AddRange(new object[]
            {
                "Feedback",
                "Complaint",
                "Request",
                "Suggestion"
            });

            cmbRequestType.Location =
                new System.Drawing.Point(280, 50);

            cmbRequestType.Name =
                "cmbRequestType";

            cmbRequestType.Size =
                new System.Drawing.Size(220, 23);

            // =========================
            // STATUS
            // =========================

            lblStatus.AutoSize = true;
            lblStatus.Location =
                new System.Drawing.Point(530, 30);

            lblStatus.Name = "lblStatus";
            lblStatus.Size =
                new System.Drawing.Size(42, 15);

            lblStatus.Text = "Status:";

            cmbStatus.DropDownStyle =
                System.Windows.Forms.ComboBoxStyle.DropDownList;

            cmbStatus.Items.AddRange(new object[]
            {
                "Open",
                "In Progress",
                "Resolved",
                "Closed"
            });

            cmbStatus.Location =
                new System.Drawing.Point(530, 50);

            cmbStatus.Name =
                "cmbStatus";

            cmbStatus.Size =
                new System.Drawing.Size(180, 23);

            // =========================
            // MESSAGE
            // =========================

            lblMessage.AutoSize = true;
            lblMessage.Location =
                new System.Drawing.Point(30, 95);

            lblMessage.Name = "lblMessage";
            lblMessage.Size =
                new System.Drawing.Size(59, 15);

            lblMessage.Text = "Message:";

            txtMessage.Location =
                new System.Drawing.Point(30, 115);

            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";

            txtMessage.ScrollBars =
                System.Windows.Forms.ScrollBars.Vertical;

            txtMessage.Size =
                new System.Drawing.Size(680, 70);

            // =========================
            // ADD BUTTON
            // =========================

            btnAdd.Location =
                new System.Drawing.Point(30, 215);

            btnAdd.Name = "btnAdd";

            btnAdd.Size =
                new System.Drawing.Size(100, 35);

            btnAdd.Text = "ADD";

            btnAdd.UseVisualStyleBackColor = true;

            // =========================
            // UPDATE BUTTON
            // =========================

            btnUpdate.Location =
                new System.Drawing.Point(140, 215);

            btnUpdate.Name = "btnUpdate";

            btnUpdate.Size =
                new System.Drawing.Size(100, 35);

            btnUpdate.Text = "UPDATE";

            btnUpdate.UseVisualStyleBackColor = true;

            // =========================
            // DELETE BUTTON
            // =========================

            btnDelete.Location =
                new System.Drawing.Point(250, 215);

            btnDelete.Name = "btnDelete";

            btnDelete.Size =
                new System.Drawing.Size(100, 35);

            btnDelete.Text = "DELETE";

            btnDelete.UseVisualStyleBackColor = true;

            // =========================
            // DATA GRID VIEW
            // =========================

            dgvFeedback.AllowUserToAddRows = false;

            dgvFeedback.AllowUserToDeleteRows = false;

            dgvFeedback.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            dgvFeedback.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvFeedback.Location =
                new System.Drawing.Point(30, 275);

            dgvFeedback.Name =
                "dgvFeedback";

            dgvFeedback.ReadOnly = true;

            dgvFeedback.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            dgvFeedback.MultiSelect = false;

            dgvFeedback.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;

            dgvFeedback.Size =
                new System.Drawing.Size(1100, 325);

            // =========================
            // FORM
            // =========================

            AutoScaleDimensions =
                new System.Drawing.SizeF(7F, 15F);

            AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            ClientSize =
                new System.Drawing.Size(1160, 650);

            Controls.Add(lblGuestId);
            Controls.Add(txtGuestId);

            Controls.Add(lblRequestType);
            Controls.Add(cmbRequestType);

            Controls.Add(lblStatus);
            Controls.Add(cmbStatus);

            Controls.Add(lblMessage);
            Controls.Add(txtMessage);

            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);

            Controls.Add(dgvFeedback);

            Name =
                "GuestFeedbackForm";

            Text =
                "Guest Feedback";

            ((System.ComponentModel.ISupportInitialize)dgvFeedback).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
