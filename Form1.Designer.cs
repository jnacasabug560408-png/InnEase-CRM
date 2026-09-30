namespace InnEase_CRM
{
    partial class Form1
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

            lblFullName = new Label();
            txtFullName = new TextBox();

            lblEmail = new Label();
            txtEmail = new TextBox();

            lblPhone = new Label();
            txtPhone = new TextBox();

            lblAddress = new Label();
            txtAddress = new TextBox();

            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();

            dgvGuests = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)dgvGuests).BeginInit();

            SuspendLayout();


            // =====================================================
            // lblTitle
            // =====================================================

            lblTitle.AutoSize = true;

            lblTitle.Font =
                new Font(
                    "Segoe UI",
                    18F,
                    FontStyle.Bold);

            lblTitle.Location =
                new Point(30, 25);

            lblTitle.Name =
                "lblTitle";

            lblTitle.Size =
                new Size(360, 32);

            lblTitle.TabIndex =
                0;

            lblTitle.Text =
                "Guest Information";


            // =====================================================
            // lblFullName
            // =====================================================

            lblFullName.AutoSize = true;

            lblFullName.Location =
                new Point(30, 85);

            lblFullName.Name =
                "lblFullName";

            lblFullName.Size =
                new Size(64, 15);

            lblFullName.TabIndex =
                1;

            lblFullName.Text =
                "Full Name:";


            // =====================================================
            // txtFullName
            // =====================================================

            txtFullName.Location =
                new Point(110, 82);

            txtFullName.Name =
                "txtFullName";

            txtFullName.Size =
                new Size(250, 23);

            txtFullName.TabIndex =
                2;

            txtFullName.MaxLength =
                100;


            // =====================================================
            // lblEmail
            // =====================================================

            lblEmail.AutoSize = true;

            lblEmail.Location =
                new Point(390, 85);

            lblEmail.Name =
                "lblEmail";

            lblEmail.Size =
                new Size(39, 15);

            lblEmail.TabIndex =
                3;

            lblEmail.Text =
                "Email:";


            // =====================================================
            // txtEmail
            // =====================================================

            txtEmail.Location =
                new Point(450, 82);

            txtEmail.Name =
                "txtEmail";

            txtEmail.Size =
                new Size(250, 23);

            txtEmail.TabIndex =
                4;

            txtEmail.MaxLength =
                150;


            // =====================================================
            // lblPhone
            // =====================================================

            lblPhone.AutoSize = true;

            lblPhone.Location =
                new Point(30, 125);

            lblPhone.Name =
                "lblPhone";

            lblPhone.Size =
                new Size(44, 15);

            lblPhone.TabIndex =
                5;

            lblPhone.Text =
                "Phone:";


            // =====================================================
            // txtPhone
            // =====================================================

            txtPhone.Location =
                new Point(110, 122);

            txtPhone.Name =
                "txtPhone";

            txtPhone.Size =
                new Size(250, 23);

            txtPhone.TabIndex =
                6;

            txtPhone.MaxLength =
                15;


            // =====================================================
            // lblAddress
            // =====================================================

            lblAddress.AutoSize = true;

            lblAddress.Location =
                new Point(390, 125);

            lblAddress.Name =
                "lblAddress";

            lblAddress.Size =
                new Size(52, 15);

            lblAddress.TabIndex =
                7;

            lblAddress.Text =
                "Address:";


            // =====================================================
            // txtAddress
            // =====================================================

            txtAddress.Location =
                new Point(450, 122);

            txtAddress.Name =
                "txtAddress";

            txtAddress.Size =
                new Size(250, 23);

            txtAddress.TabIndex =
                8;

            txtAddress.MaxLength =
                250;


            // =====================================================
            // btnAdd
            // =====================================================

            btnAdd.Location =
                new Point(30, 175);

            btnAdd.Name =
                "btnAdd";

            btnAdd.Size =
                new Size(110, 35);

            btnAdd.TabIndex =
                9;

            btnAdd.Text =
                "ADD";

            btnAdd.UseVisualStyleBackColor =
                true;


            // =====================================================
            // btnUpdate
            // =====================================================

            btnUpdate.Location =
                new Point(150, 175);

            btnUpdate.Name =
                "btnUpdate";

            btnUpdate.Size =
                new Size(110, 35);

            btnUpdate.TabIndex =
                10;

            btnUpdate.Text =
                "UPDATE";

            btnUpdate.UseVisualStyleBackColor =
                true;


            // =====================================================
            // btnDelete
            // =====================================================

            btnDelete.Location =
                new Point(270, 175);

            btnDelete.Name =
                "btnDelete";

            btnDelete.Size =
                new Size(110, 35);

            btnDelete.TabIndex =
                11;

            btnDelete.Text =
                "DELETE";

            btnDelete.UseVisualStyleBackColor =
                true;


            // =====================================================
            // dgvGuests
            // =====================================================

            dgvGuests.AllowUserToAddRows =
                false;

            dgvGuests.AllowUserToDeleteRows =
                false;

            dgvGuests.AllowUserToResizeRows =
                false;

            dgvGuests.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            dgvGuests.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvGuests.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvGuests.Location =
                new Point(30, 240);

            dgvGuests.MultiSelect =
                false;

            dgvGuests.Name =
                "dgvGuests";

            dgvGuests.ReadOnly =
                true;

            dgvGuests.RowHeadersWidth =
                50;

            dgvGuests.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvGuests.Size =
                new Size(690, 300);

            dgvGuests.TabIndex =
                12;


            // =====================================================
            // Form1
            // =====================================================

            AutoScaleDimensions =
                new SizeF(7F, 15F);

            AutoScaleMode =
                AutoScaleMode.Font;

            ClientSize =
                new Size(760, 600);

            MinimumSize =
                new Size(760, 600);

            Controls.Add(lblTitle);

            Controls.Add(lblFullName);
            Controls.Add(txtFullName);

            Controls.Add(lblEmail);
            Controls.Add(txtEmail);

            Controls.Add(lblPhone);
            Controls.Add(txtPhone);

            Controls.Add(lblAddress);
            Controls.Add(txtAddress);

            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);

            Controls.Add(dgvGuests);

            Name =
                "Form1";

            StartPosition =
                FormStartPosition.CenterScreen;

            Text =
                "InnEase CRM - Guest Information";

            ((System.ComponentModel.ISupportInitialize)dgvGuests).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }


        // =====================================================
        // CONTROLS
        // =====================================================

        private Label lblTitle;

        private Label lblFullName;
        private TextBox txtFullName;

        private Label lblEmail;
        private TextBox txtEmail;

        private Label lblPhone;
        private TextBox txtPhone;

        private Label lblAddress;
        private TextBox txtAddress;

        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        private DataGridView dgvGuests;
    }
}
