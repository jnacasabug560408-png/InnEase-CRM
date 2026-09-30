namespace InnEase_CRM
{
    partial class CustomerConsentForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblTitle;

        private System.Windows.Forms.Label lblGuestId;
        private System.Windows.Forms.TextBox txtGuestId;

        private System.Windows.Forms.Label lblConsentType;
        private System.Windows.Forms.ComboBox cmbConsentType;

        private System.Windows.Forms.Label lblIsGiven;
        private System.Windows.Forms.CheckBox chkIsGiven;

        private System.Windows.Forms.Label lblConsentDate;
        private System.Windows.Forms.DateTimePicker dtpConsentDate;

        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;

        private System.Windows.Forms.DataGridView dgvConsents;

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
            lblTitle = new System.Windows.Forms.Label();

            lblGuestId = new System.Windows.Forms.Label();
            txtGuestId = new System.Windows.Forms.TextBox();

            lblConsentType = new System.Windows.Forms.Label();
            cmbConsentType = new System.Windows.Forms.ComboBox();

            lblIsGiven = new System.Windows.Forms.Label();
            chkIsGiven = new System.Windows.Forms.CheckBox();

            lblConsentDate = new System.Windows.Forms.Label();
            dtpConsentDate = new System.Windows.Forms.DateTimePicker();

            lblNotes = new System.Windows.Forms.Label();
            txtNotes = new System.Windows.Forms.TextBox();

            btnAdd = new System.Windows.Forms.Button();
            btnUpdate = new System.Windows.Forms.Button();
            btnDelete = new System.Windows.Forms.Button();

            dgvConsents = new System.Windows.Forms.DataGridView();

            ((System.ComponentModel.ISupportInitialize)dgvConsents).BeginInit();
            SuspendLayout();

            // =========================
            // TITLE
            // =========================

            lblTitle.AutoSize = true;

            lblTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    14F,
                    System.Drawing.FontStyle.Bold);

            lblTitle.Location =
                new System.Drawing.Point(30, 20);

            lblTitle.Name =
                "lblTitle";

            lblTitle.Size =
                new System.Drawing.Size(300, 25);

            lblTitle.Text =
                "InnEase CRM - Customer Consent";

            // =========================
            // GUEST ID
            // =========================

            lblGuestId.AutoSize = true;

            lblGuestId.Location =
                new System.Drawing.Point(30, 70);

            lblGuestId.Name =
                "lblGuestId";

            lblGuestId.Size =
                new System.Drawing.Size(55, 15);

            lblGuestId.Text =
                "Guest ID:";

            txtGuestId.Location =
                new System.Drawing.Point(30, 90);

            txtGuestId.Name =
                "txtGuestId";

            txtGuestId.Size =
                new System.Drawing.Size(220, 23);

            // =========================
            // CONSENT TYPE
            // =========================

            lblConsentType.AutoSize = true;

            lblConsentType.Location =
                new System.Drawing.Point(280, 70);

            lblConsentType.Name =
                "lblConsentType";

            lblConsentType.Size =
                new System.Drawing.Size(83, 15);

            lblConsentType.Text =
                "Consent Type:";

            cmbConsentType.DropDownStyle =
                System.Windows.Forms.ComboBoxStyle.DropDownList;

            cmbConsentType.Items.AddRange(
                new object[]
                {
                    "Marketing",
                    "DataSharing",
                    "Photography"
                });

            cmbConsentType.Location =
                new System.Drawing.Point(280, 90);

            cmbConsentType.Name =
                "cmbConsentType";

            cmbConsentType.Size =
                new System.Drawing.Size(220, 23);

            // =========================
            // CONSENT GIVEN
            // =========================

            lblIsGiven.AutoSize = true;

            lblIsGiven.Location =
                new System.Drawing.Point(530, 70);

            lblIsGiven.Name =
                "lblIsGiven";

            lblIsGiven.Size =
                new System.Drawing.Size(87, 15);

            lblIsGiven.Text =
                "Consent Given:";

            chkIsGiven.Location =
                new System.Drawing.Point(530, 90);

            chkIsGiven.Name =
                "chkIsGiven";

            chkIsGiven.Size =
                new System.Drawing.Size(80, 23);

            chkIsGiven.Text =
                "Yes";

            chkIsGiven.Checked = true;

            chkIsGiven.UseVisualStyleBackColor = true;

            // =========================
            // CONSENT DATE
            // =========================

            lblConsentDate.AutoSize = true;

            lblConsentDate.Location =
                new System.Drawing.Point(30, 135);

            lblConsentDate.Name =
                "lblConsentDate";

            lblConsentDate.Size =
                new System.Drawing.Size(79, 15);

            lblConsentDate.Text =
                "Consent Date:";

            dtpConsentDate.Format =
                System.Windows.Forms.DateTimePickerFormat.Short;

            dtpConsentDate.Location =
                new System.Drawing.Point(30, 155);

            dtpConsentDate.Name =
                "dtpConsentDate";

            dtpConsentDate.Size =
                new System.Drawing.Size(220, 23);

            // =========================
            // NOTES
            // =========================

            lblNotes.AutoSize = true;

            lblNotes.Location =
                new System.Drawing.Point(280, 135);

            lblNotes.Name =
                "lblNotes";

            lblNotes.Size =
                new System.Drawing.Size(40, 15);

            lblNotes.Text =
                "Notes:";

            txtNotes.Location =
                new System.Drawing.Point(280, 155);

            txtNotes.Multiline = true;

            txtNotes.Name =
                "txtNotes";

            txtNotes.ScrollBars =
                System.Windows.Forms.ScrollBars.Vertical;

            txtNotes.Size =
                new System.Drawing.Size(430, 60);

            // =========================
            // ADD BUTTON
            // =========================

            btnAdd.Location =
                new System.Drawing.Point(30, 235);

            btnAdd.Name =
                "btnAdd";

            btnAdd.Size =
                new System.Drawing.Size(100, 35);

            btnAdd.Text =
                "ADD";

            btnAdd.UseVisualStyleBackColor = true;

            // =========================
            // UPDATE BUTTON
            // =========================

            btnUpdate.Location =
                new System.Drawing.Point(140, 235);

            btnUpdate.Name =
                "btnUpdate";

            btnUpdate.Size =
                new System.Drawing.Size(100, 35);

            btnUpdate.Text =
                "UPDATE";

            btnUpdate.UseVisualStyleBackColor = true;

            // =========================
            // DELETE BUTTON
            // =========================

            btnDelete.Location =
                new System.Drawing.Point(250, 235);

            btnDelete.Name =
                "btnDelete";

            btnDelete.Size =
                new System.Drawing.Size(100, 35);

            btnDelete.Text =
                "DELETE";

            btnDelete.UseVisualStyleBackColor = true;

            // =========================
            // DATA GRID VIEW
            // =========================

            dgvConsents.AllowUserToAddRows = false;

            dgvConsents.AllowUserToDeleteRows = false;

            dgvConsents.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            dgvConsents.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvConsents.Location =
                new System.Drawing.Point(30, 295);

            dgvConsents.Name =
                "dgvConsents";

            dgvConsents.ReadOnly = true;

            dgvConsents.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            dgvConsents.MultiSelect = false;

            dgvConsents.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;

            dgvConsents.Size =
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

            Controls.Add(lblTitle);

            Controls.Add(lblGuestId);
            Controls.Add(txtGuestId);

            Controls.Add(lblConsentType);
            Controls.Add(cmbConsentType);

            Controls.Add(lblIsGiven);
            Controls.Add(chkIsGiven);

            Controls.Add(lblConsentDate);
            Controls.Add(dtpConsentDate);

            Controls.Add(lblNotes);
            Controls.Add(txtNotes);

            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);

            Controls.Add(dgvConsents);

            Name =
                "CustomerConsentForm";

            Text =
                "InnEase CRM - Customer Consent";

            ((System.ComponentModel.ISupportInitialize)dgvConsents).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
