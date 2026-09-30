namespace InnEase_CRM
{
    partial class PromotionForm
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

            lblPromotionId = new Label();
            txtPromotionId = new TextBox();

            lblPromoCode = new Label();
            txtPromoCode = new TextBox();

            lblDescription = new Label();
            txtDescription = new TextBox();

            lblDiscountType = new Label();
            cmbDiscountType = new ComboBox();

            lblDiscountValue = new Label();
            txtDiscountValue = new TextBox();

            lblLoyaltyPoints = new Label();
            txtLoyaltyPoints = new TextBox();

            lblStartDate = new Label();
            dtpStartDate = new DateTimePicker();

            lblEndDate = new Label();
            dtpEndDate = new DateTimePicker();

            lblIsActive = new Label();
            chkIsActive = new CheckBox();

            // ---- Governance / approval fields (QA requirement) ----
            lblReason = new Label();
            txtReason = new TextBox();

            lblImplementedBy = new Label();
            txtImplementedBy = new TextBox();

            lblApprovalStatus = new Label();
            txtApprovalStatus = new TextBox();

            lblApprovedBy = new Label();
            txtApprovedBy = new TextBox();

            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnApprove = new Button();
            btnReject = new Button();

            dgvPromotions = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)dgvPromotions).BeginInit();
            SuspendLayout();

            //
            // PromotionForm
            //
            Font = new Font("Segoe UI", 9F);
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            WindowState = FormWindowState.Maximized;
            Name = "PromotionForm";
            Text = "InnEase CRM - Promotions & Loyalty";

            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font(
                "Segoe UI",
                14F,
                FontStyle.Bold);

            lblTitle.Location = new Point(30, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 25);
            lblTitle.Text = "InnEase CRM - Promotions & Loyalty";

            //
            // lblPromotionId
            //
            lblPromotionId.AutoSize = true;
            lblPromotionId.Location = new Point(30, 73);
            lblPromotionId.Name = "lblPromotionId";
            lblPromotionId.Size = new Size(90, 15);
            lblPromotionId.Text = "Promotion ID:";

            //
            // txtPromotionId
            //
            txtPromotionId.Location = new Point(150, 70);
            txtPromotionId.Name = "txtPromotionId";
            txtPromotionId.Size = new Size(90, 25);
            txtPromotionId.ReadOnly = true;
            txtPromotionId.BackColor = Color.WhiteSmoke;
            txtPromotionId.ForeColor = Color.Black;

            //
            // lblPromoCode
            //
            lblPromoCode.AutoSize = true;
            lblPromoCode.Location = new Point(350, 73);
            lblPromoCode.Name = "lblPromoCode";
            lblPromoCode.Size = new Size(80, 15);
            lblPromoCode.Text = "Promo Code:";

            //
            // txtPromoCode
            //
            txtPromoCode.Location = new Point(450, 70);
            txtPromoCode.Name = "txtPromoCode";
            txtPromoCode.Size = new Size(220, 25);

            //
            // lblDescription
            //
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(30, 113);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(75, 15);
            lblDescription.Text = "Description:";

            //
            // txtDescription
            //
            txtDescription.Location = new Point(150, 110);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(520, 25);

            //
            // lblDiscountType
            //
            lblDiscountType.AutoSize = true;
            lblDiscountType.Location = new Point(30, 153);
            lblDiscountType.Name = "lblDiscountType";
            lblDiscountType.Size = new Size(90, 15);
            lblDiscountType.Text = "Discount Type:";

            //
            // cmbDiscountType
            //
            cmbDiscountType.Location = new Point(150, 150);
            cmbDiscountType.Name = "cmbDiscountType";
            cmbDiscountType.Size = new Size(180, 25);
            cmbDiscountType.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbDiscountType.Items.AddRange(
                new object[]
                {
                    "Percentage",
                    "FixedAmount"
                });

            //
            // lblDiscountValue
            //
            lblDiscountValue.AutoSize = true;
            lblDiscountValue.Location = new Point(350, 153);
            lblDiscountValue.Name = "lblDiscountValue";
            lblDiscountValue.Size = new Size(90, 15);
            lblDiscountValue.Text = "Discount Value:";

            //
            // txtDiscountValue
            //
            txtDiscountValue.Location = new Point(450, 150);
            txtDiscountValue.Name = "txtDiscountValue";
            txtDiscountValue.Size = new Size(120, 25);

            //
            // lblLoyaltyPoints
            //
            lblLoyaltyPoints.AutoSize = true;
            lblLoyaltyPoints.Location = new Point(30, 193);
            lblLoyaltyPoints.Name = "lblLoyaltyPoints";
            lblLoyaltyPoints.Size = new Size(90, 15);
            lblLoyaltyPoints.Text = "Loyalty Points:";

            //
            // txtLoyaltyPoints
            //
            txtLoyaltyPoints.Location = new Point(150, 190);
            txtLoyaltyPoints.Name = "txtLoyaltyPoints";
            txtLoyaltyPoints.Size = new Size(120, 25);

            //
            // lblStartDate
            //
            lblStartDate.AutoSize = true;
            lblStartDate.Location = new Point(350, 193);
            lblStartDate.Name = "lblStartDate";
            lblStartDate.Size = new Size(65, 15);
            lblStartDate.Text = "Start Date:";

            //
            // dtpStartDate
            //
            dtpStartDate.Location = new Point(450, 190);
            dtpStartDate.Name = "dtpStartDate";
            dtpStartDate.Size = new Size(220, 25);
            dtpStartDate.Format =
                DateTimePickerFormat.Short;

            //
            // lblEndDate
            //
            lblEndDate.AutoSize = true;
            lblEndDate.Location = new Point(30, 233);
            lblEndDate.Name = "lblEndDate";
            lblEndDate.Size = new Size(65, 15);
            lblEndDate.Text = "End Date:";

            //
            // dtpEndDate
            //
            dtpEndDate.Location = new Point(150, 230);
            dtpEndDate.Name = "dtpEndDate";
            dtpEndDate.Size = new Size(180, 25);
            dtpEndDate.Format =
                DateTimePickerFormat.Short;

            //
            // lblIsActive
            //
            lblIsActive.AutoSize = true;
            lblIsActive.Location = new Point(350, 233);
            lblIsActive.Name = "lblIsActive";
            lblIsActive.Size = new Size(45, 15);
            lblIsActive.Text = "Active:";

            //
            // chkIsActive
            //
            chkIsActive.Location = new Point(450, 230);
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Size = new Size(80, 25);
            chkIsActive.Text = "Yes";
            chkIsActive.Checked = true;

            // =========================================================
            // GOVERNANCE / APPROVAL FIELDS  (QA requirement)
            // =========================================================

            //
            // lblReason
            //
            lblReason.AutoSize = true;
            lblReason.Location = new Point(30, 273);
            lblReason.Name = "lblReason";
            lblReason.Size = new Size(110, 15);
            lblReason.Text = "Reason (why?):";

            //
            // txtReason
            //
            txtReason.Location = new Point(150, 270);
            txtReason.Name = "txtReason";
            txtReason.Size = new Size(520, 50);
            txtReason.Multiline = true;
            txtReason.ScrollBars = ScrollBars.Vertical;

            //
            // lblImplementedBy
            //
            lblImplementedBy.AutoSize = true;
            lblImplementedBy.Location = new Point(30, 333);
            lblImplementedBy.Name = "lblImplementedBy";
            lblImplementedBy.Size = new Size(120, 15);
            lblImplementedBy.Text = "Requested/Implemented By:";

            //
            // txtImplementedBy
            //
            txtImplementedBy.Location = new Point(220, 330);
            txtImplementedBy.Name = "txtImplementedBy";
            txtImplementedBy.Size = new Size(220, 25);

            //
            // lblApprovalStatus
            //
            lblApprovalStatus.AutoSize = true;
            lblApprovalStatus.Location = new Point(460, 333);
            lblApprovalStatus.Name = "lblApprovalStatus";
            lblApprovalStatus.Size = new Size(45, 15);
            lblApprovalStatus.Text = "Status:";

            //
            // txtApprovalStatus
            //
            txtApprovalStatus.Location = new Point(520, 330);
            txtApprovalStatus.Name = "txtApprovalStatus";
            txtApprovalStatus.Size = new Size(150, 25);
            txtApprovalStatus.ReadOnly = true;
            txtApprovalStatus.BackColor = Color.WhiteSmoke;
            txtApprovalStatus.ForeColor = Color.Black;
            txtApprovalStatus.Text = "Pending";

            //
            // lblApprovedBy
            //
            lblApprovedBy.AutoSize = true;
            lblApprovedBy.Location = new Point(30, 373);
            lblApprovedBy.Name = "lblApprovedBy";
            lblApprovedBy.Size = new Size(115, 15);
            lblApprovedBy.Text = "Approved/Rejected By:";

            //
            // txtApprovedBy
            //
            txtApprovedBy.Location = new Point(220, 370);
            txtApprovedBy.Name = "txtApprovedBy";
            txtApprovedBy.Size = new Size(220, 25);
            txtApprovedBy.ReadOnly = true;
            txtApprovedBy.BackColor = Color.WhiteSmoke;
            txtApprovedBy.ForeColor = Color.Black;

            //
            // btnAdd
            //
            btnAdd.Location = new Point(30, 420);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(95, 32);
            btnAdd.Text = "ADD";
            btnAdd.UseVisualStyleBackColor = true;

            //
            // btnUpdate
            //
            btnUpdate.Location = new Point(135, 420);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(95, 32);
            btnUpdate.Text = "UPDATE";
            btnUpdate.UseVisualStyleBackColor = true;

            //
            // btnDelete
            //
            btnDelete.Location = new Point(240, 420);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(95, 32);
            btnDelete.Text = "DELETE";
            btnDelete.UseVisualStyleBackColor = true;

            //
            // btnApprove
            //
            btnApprove.Location = new Point(360, 420);
            btnApprove.Name = "btnApprove";
            btnApprove.Size = new Size(110, 32);
            btnApprove.Text = "APPROVE";
            btnApprove.BackColor = Color.FromArgb(60, 160, 90);
            btnApprove.ForeColor = Color.White;
            btnApprove.FlatStyle = FlatStyle.Flat;
            btnApprove.FlatAppearance.BorderSize = 0;
            btnApprove.UseVisualStyleBackColor = false;

            //
            // btnReject
            //
            btnReject.Location = new Point(480, 420);
            btnReject.Name = "btnReject";
            btnReject.Size = new Size(110, 32);
            btnReject.Text = "REJECT";
            btnReject.BackColor = Color.FromArgb(190, 70, 70);
            btnReject.ForeColor = Color.White;
            btnReject.FlatStyle = FlatStyle.Flat;
            btnReject.FlatAppearance.BorderSize = 0;
            btnReject.UseVisualStyleBackColor = false;

            //
            // dgvPromotions
            //
            dgvPromotions.Name = "dgvPromotions";
            dgvPromotions.Location = new Point(30, 470);

            dgvPromotions.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            dgvPromotions.Size =
                new Size(
                    ClientSize.Width - 60,
                    ClientSize.Height - 500);

            dgvPromotions.AllowUserToAddRows = false;
            dgvPromotions.ReadOnly = true;
            dgvPromotions.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvPromotions.MultiSelect = false;
            dgvPromotions.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvPromotions.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            //
            // Controls
            //
            Controls.Add(lblTitle);

            Controls.Add(lblPromotionId);
            Controls.Add(txtPromotionId);

            Controls.Add(lblPromoCode);
            Controls.Add(txtPromoCode);

            Controls.Add(lblDescription);
            Controls.Add(txtDescription);

            Controls.Add(lblDiscountType);
            Controls.Add(cmbDiscountType);

            Controls.Add(lblDiscountValue);
            Controls.Add(txtDiscountValue);

            Controls.Add(lblLoyaltyPoints);
            Controls.Add(txtLoyaltyPoints);

            Controls.Add(lblStartDate);
            Controls.Add(dtpStartDate);

            Controls.Add(lblEndDate);
            Controls.Add(dtpEndDate);

            Controls.Add(lblIsActive);
            Controls.Add(chkIsActive);

            Controls.Add(lblReason);
            Controls.Add(txtReason);

            Controls.Add(lblImplementedBy);
            Controls.Add(txtImplementedBy);

            Controls.Add(lblApprovalStatus);
            Controls.Add(txtApprovalStatus);

            Controls.Add(lblApprovedBy);
            Controls.Add(txtApprovedBy);

            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnApprove);
            Controls.Add(btnReject);

            Controls.Add(dgvPromotions);

            ((System.ComponentModel.ISupportInitialize)dgvPromotions).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitle;
        private Label lblPromotionId;
        private TextBox txtPromotionId;

        private Label lblPromoCode;
        private TextBox txtPromoCode;

        private Label lblDescription;
        private TextBox txtDescription;

        private Label lblDiscountType;
        private ComboBox cmbDiscountType;

        private Label lblDiscountValue;
        private TextBox txtDiscountValue;

        private Label lblLoyaltyPoints;
        private TextBox txtLoyaltyPoints;

        private Label lblStartDate;
        private DateTimePicker dtpStartDate;

        private Label lblEndDate;
        private DateTimePicker dtpEndDate;

        private Label lblIsActive;
        private CheckBox chkIsActive;

        private Label lblReason;
        private TextBox txtReason;

        private Label lblImplementedBy;
        private TextBox txtImplementedBy;

        private Label lblApprovalStatus;
        private TextBox txtApprovalStatus;

        private Label lblApprovedBy;
        private TextBox txtApprovedBy;

        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnApprove;
        private Button btnReject;

        private DataGridView dgvPromotions;
    }
}
