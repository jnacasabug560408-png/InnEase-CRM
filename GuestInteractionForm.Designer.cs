namespace InnEase_CRM
{
    partial class GuestInteractionForm
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

            lblInteractionId = new Label();
            txtInteractionId = new TextBox();

            lblGuestId = new Label();
            txtGuestId = new TextBox();

            lblInteractionType = new Label();
            cmbInteractionType = new ComboBox();

            lblHandledBy = new Label();
            txtHandledBy = new TextBox();

            lblInteractionDate = new Label();
            dtpInteractionDate = new DateTimePicker();

            lblNotes = new Label();
            txtNotes = new TextBox();

            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();

            dgvInteractions = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)dgvInteractions).BeginInit();
            SuspendLayout();

            //
            // GuestInteractionForm
            //
            Font = new Font("Segoe UI", 9F);
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;

            WindowState = FormWindowState.Maximized;

            Name = "GuestInteractionForm";
            Text = "InnEase CRM - Guest Interactions";

            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font =
                new Font(
                    "Segoe UI",
                    14F,
                    FontStyle.Bold);

            lblTitle.Location = new Point(30, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 25);
            lblTitle.Text =
                "InnEase CRM - Guest Interactions";

            //
            // lblInteractionId
            //
            lblInteractionId.AutoSize = true;
            lblInteractionId.Location = new Point(30, 73);
            lblInteractionId.Name = "lblInteractionId";
            lblInteractionId.Size = new Size(95, 15);
            lblInteractionId.Text = "Interaction ID:";

            //
            // txtInteractionId
            //
            txtInteractionId.Location = new Point(150, 70);
            txtInteractionId.Name = "txtInteractionId";
            txtInteractionId.Size = new Size(90, 25);
            txtInteractionId.ReadOnly = true;
            txtInteractionId.BackColor = Color.WhiteSmoke;
            txtInteractionId.ForeColor = Color.Black;

            //
            // lblGuestId
            //
            lblGuestId.AutoSize = true;
            lblGuestId.Location = new Point(350, 73);
            lblGuestId.Name = "lblGuestId";
            lblGuestId.Size = new Size(60, 15);
            lblGuestId.Text = "Guest ID:";

            //
            // txtGuestId
            //
            txtGuestId.Location = new Point(450, 70);
            txtGuestId.Name = "txtGuestId";
            txtGuestId.Size = new Size(120, 25);

            //
            // lblInteractionType
            //
            lblInteractionType.AutoSize = true;
            lblInteractionType.Location = new Point(30, 113);
            lblInteractionType.Name = "lblInteractionType";
            lblInteractionType.Size = new Size(60, 15);
            lblInteractionType.Text = "Type:";

            //
            // cmbInteractionType
            //
            cmbInteractionType.Location = new Point(150, 110);
            cmbInteractionType.Name = "cmbInteractionType";
            cmbInteractionType.Size = new Size(180, 25);

            cmbInteractionType.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbInteractionType.Items.AddRange(
                new object[]
                {
                    "CheckIn",
                    "CheckOut",
                    "SpecialRequest",
                    "Complaint",
                    "GeneralNote"
                });

            //
            // lblHandledBy
            //
            lblHandledBy.AutoSize = true;
            lblHandledBy.Location = new Point(350, 113);
            lblHandledBy.Name = "lblHandledBy";
            lblHandledBy.Size = new Size(75, 15);
            lblHandledBy.Text = "Handled By:";

            //
            // txtHandledBy
            //
            txtHandledBy.Location = new Point(450, 110);
            txtHandledBy.Name = "txtHandledBy";
            txtHandledBy.Size = new Size(220, 25);

            //
            // lblInteractionDate
            //
            lblInteractionDate.AutoSize = true;
            lblInteractionDate.Location = new Point(30, 153);
            lblInteractionDate.Name = "lblInteractionDate";
            lblInteractionDate.Size = new Size(30, 15);
            lblInteractionDate.Text = "Date:";

            //
            // dtpInteractionDate
            //
            dtpInteractionDate.Location = new Point(150, 150);
            dtpInteractionDate.Name = "dtpInteractionDate";
            dtpInteractionDate.Size = new Size(180, 25);

            dtpInteractionDate.Format =
                DateTimePickerFormat.Short;

            //
            // lblNotes
            //
            lblNotes.AutoSize = true;
            lblNotes.Location = new Point(30, 193);
            lblNotes.Name = "lblNotes";
            lblNotes.Size = new Size(45, 15);
            lblNotes.Text = "Notes:";

            //
            // txtNotes
            //
            txtNotes.Location = new Point(150, 190);
            txtNotes.Name = "txtNotes";
            txtNotes.Size = new Size(520, 60);
            txtNotes.Multiline = true;

            //
            // btnAdd
            //
            btnAdd.Location = new Point(30, 265);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(95, 32);
            btnAdd.Text = "ADD";
            btnAdd.UseVisualStyleBackColor = true;

            //
            // btnUpdate
            //
            btnUpdate.Location = new Point(135, 265);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(95, 32);
            btnUpdate.Text = "UPDATE";
            btnUpdate.UseVisualStyleBackColor = true;

            //
            // btnDelete
            //
            btnDelete.Location = new Point(240, 265);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(95, 32);
            btnDelete.Text = "DELETE";
            btnDelete.UseVisualStyleBackColor = true;

            //
            // dgvInteractions
            //
            dgvInteractions.Name = "dgvInteractions";
            dgvInteractions.Location = new Point(30, 320);

            dgvInteractions.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            dgvInteractions.Size =
                new Size(
                    ClientSize.Width - 60,
                    ClientSize.Height - 350);

            dgvInteractions.AllowUserToAddRows = false;
            dgvInteractions.ReadOnly = true;

            dgvInteractions.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvInteractions.MultiSelect = false;

            dgvInteractions.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvInteractions.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            //
            // Controls
            //
            Controls.Add(lblTitle);

            Controls.Add(lblInteractionId);
            Controls.Add(txtInteractionId);

            Controls.Add(lblGuestId);
            Controls.Add(txtGuestId);

            Controls.Add(lblInteractionType);
            Controls.Add(cmbInteractionType);

            Controls.Add(lblHandledBy);
            Controls.Add(txtHandledBy);

            Controls.Add(lblInteractionDate);
            Controls.Add(dtpInteractionDate);

            Controls.Add(lblNotes);
            Controls.Add(txtNotes);

            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);

            Controls.Add(dgvInteractions);

            ((System.ComponentModel.ISupportInitialize)dgvInteractions).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitle;

        private Label lblInteractionId;
        private TextBox txtInteractionId;

        private Label lblGuestId;
        private TextBox txtGuestId;

        private Label lblInteractionType;
        private ComboBox cmbInteractionType;

        private Label lblHandledBy;
        private TextBox txtHandledBy;

        private Label lblInteractionDate;
        private DateTimePicker dtpInteractionDate;

        private Label lblNotes;
        private TextBox txtNotes;

        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        private DataGridView dgvInteractions;
    }
}


