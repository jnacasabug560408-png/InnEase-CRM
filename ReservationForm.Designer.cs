namespace InnEase_CRM
{
    partial class ReservationForm
    {
        private System.ComponentModel.IContainer components = null;


        protected override void Dispose(
            bool disposing)
        {
            if (disposing &&
                (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            lblTitle = new Label();

            lblReservationId = new Label();
            txtReservationId = new TextBox();

            lblGuestId = new Label();
            txtGuestId = new TextBox();

            lblRoomNumber = new Label();
            txtRoomNumber = new TextBox();

            lblCheckIn = new Label();
            dtpCheckIn = new DateTimePicker();

            lblCheckOut = new Label();
            dtpCheckOut = new DateTimePicker();

            lblStatus = new Label();
            cmbStatus = new ComboBox();

            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();

            dgvReservations = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)dgvReservations).BeginInit();

            SuspendLayout();


            // =====================================================
            // FORM
            // =====================================================

            AutoScaleDimensions =
                new SizeF(7F, 15F);

            AutoScaleMode =
                AutoScaleMode.Font;

            ClientSize =
                new Size(1000, 650);

            MinimumSize =
                new Size(850, 600);

            StartPosition =
                FormStartPosition.CenterScreen;

            Name =
                "ReservationForm";

            Text =
                "InnEase CRM - Reservations";

            Font =
                new Font(
                    "Segoe UI",
                    9F);


            // =====================================================
            // TITLE
            // =====================================================

            lblTitle.AutoSize = true;

            lblTitle.Font =
                new Font(
                    "Segoe UI",
                    14F,
                    FontStyle.Bold);

            lblTitle.Location =
                new Point(30, 20);

            lblTitle.Name =
                "lblTitle";

            lblTitle.Size =
                new Size(300, 25);

            lblTitle.Text =
                "InnEase CRM - Reservations";


            // =====================================================
            // RESERVATION ID
            // =====================================================

            lblReservationId.AutoSize = true;

            lblReservationId.Location =
                new Point(30, 73);

            lblReservationId.Name =
                "lblReservationId";

            lblReservationId.Size =
                new Size(110, 15);

            lblReservationId.Text =
                "Reservation ID:";


            txtReservationId.Location =
                new Point(150, 70);

            txtReservationId.Name =
                "txtReservationId";

            txtReservationId.Size =
                new Size(90, 25);

            txtReservationId.ReadOnly =
                true;

            txtReservationId.BackColor =
                Color.WhiteSmoke;

            txtReservationId.ForeColor =
                Color.Black;


            // =====================================================
            // GUEST ID
            // =====================================================

            lblGuestId.AutoSize = true;

            lblGuestId.Location =
                new Point(350, 73);

            lblGuestId.Name =
                "lblGuestId";

            lblGuestId.Size =
                new Size(60, 15);

            lblGuestId.Text =
                "Guest ID:";


            txtGuestId.Location =
                new Point(450, 70);

            txtGuestId.Name =
                "txtGuestId";

            txtGuestId.Size =
                new Size(100, 23);


            // =====================================================
            // ROOM NUMBER
            // =====================================================

            lblRoomNumber.AutoSize = true;

            lblRoomNumber.Location =
                new Point(30, 113);

            lblRoomNumber.Name =
                "lblRoomNumber";

            lblRoomNumber.Size =
                new Size(95, 15);

            lblRoomNumber.Text =
                "Room Number:";


            txtRoomNumber.Location =
                new Point(150, 110);

            txtRoomNumber.Name =
                "txtRoomNumber";

            txtRoomNumber.Size =
                new Size(110, 25);


            // =====================================================
            // CHECK-IN
            // =====================================================

            lblCheckIn.AutoSize = true;

            lblCheckIn.Location =
                new Point(350, 113);

            lblCheckIn.Name =
                "lblCheckIn";

            lblCheckIn.Size =
                new Size(60, 15);

            lblCheckIn.Text =
                "Check-In:";


            dtpCheckIn.Location =
                new Point(450, 110);

            dtpCheckIn.Name =
                "dtpCheckIn";

            dtpCheckIn.Size =
                new Size(220, 23);

            dtpCheckIn.Format =
                DateTimePickerFormat.Short;


            // =====================================================
            // CHECK-OUT
            // =====================================================

            lblCheckOut.AutoSize = true;

            lblCheckOut.Location =
                new Point(30, 153);

            lblCheckOut.Name =
                "lblCheckOut";

            lblCheckOut.Size =
                new Size(70, 15);

            lblCheckOut.Text =
                "Check-Out:";


            dtpCheckOut.Location =
                new Point(150, 150);

            dtpCheckOut.Name =
                "dtpCheckOut";

            dtpCheckOut.Size =
                new Size(180, 23);

            dtpCheckOut.Format =
                DateTimePickerFormat.Short;


            // =====================================================
            // STATUS
            // =====================================================

            lblStatus.AutoSize = true;

            lblStatus.Location =
                new Point(350, 153);

            lblStatus.Name =
                "lblStatus";

            lblStatus.Size =
                new Size(45, 15);

            lblStatus.Text =
                "Status:";


            cmbStatus.Location =
                new Point(450, 150);

            cmbStatus.Name =
                "cmbStatus";

            cmbStatus.Size =
                new Size(220, 23);

            cmbStatus.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbStatus.Items.AddRange(
                new object[]
                {
                    "Pending",
                    "Confirmed",
                    "CheckedIn",
                    "CheckedOut",
                    "Cancelled"
                });


            // =====================================================
            // ADD BUTTON
            // =====================================================

            btnAdd.Location =
                new Point(30, 195);

            btnAdd.Name =
                "btnAdd";

            btnAdd.Size =
                new Size(95, 32);

            btnAdd.Text =
                "ADD";

            btnAdd.UseVisualStyleBackColor =
                true;


            // =====================================================
            // UPDATE BUTTON
            // =====================================================

            btnUpdate.Location =
                new Point(135, 195);

            btnUpdate.Name =
                "btnUpdate";

            btnUpdate.Size =
                new Size(95, 32);

            btnUpdate.Text =
                "UPDATE";

            btnUpdate.UseVisualStyleBackColor =
                true;


            // =====================================================
            // DELETE BUTTON
            // =====================================================

            btnDelete.Location =
                new Point(240, 195);

            btnDelete.Name =
                "btnDelete";

            btnDelete.Size =
                new Size(95, 32);

            btnDelete.Text =
                "DELETE";

            btnDelete.UseVisualStyleBackColor =
                true;


            // =====================================================
            // DATA GRID
            // =====================================================

            dgvReservations.AllowUserToAddRows =
                false;

            dgvReservations.AllowUserToDeleteRows =
                false;

            dgvReservations.AllowUserToResizeRows =
                false;

            dgvReservations.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            dgvReservations.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvReservations.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvReservations.Location =
                new Point(30, 250);

            dgvReservations.Name =
                "dgvReservations";

            dgvReservations.ReadOnly =
                true;

            dgvReservations.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvReservations.MultiSelect =
                false;

            dgvReservations.Size =
                new Size(940, 360);


            // =====================================================
            // ADD CONTROLS
            // =====================================================

            Controls.Add(lblTitle);

            Controls.Add(lblReservationId);
            Controls.Add(txtReservationId);

            Controls.Add(lblGuestId);
            Controls.Add(txtGuestId);

            Controls.Add(lblRoomNumber);
            Controls.Add(txtRoomNumber);

            Controls.Add(lblCheckIn);
            Controls.Add(dtpCheckIn);

            Controls.Add(lblCheckOut);
            Controls.Add(dtpCheckOut);

            Controls.Add(lblStatus);
            Controls.Add(cmbStatus);

            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);

            Controls.Add(dgvReservations);


            ((System.ComponentModel.ISupportInitialize)dgvReservations).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }


        private Label lblTitle;

        private Label lblReservationId;
        private TextBox txtReservationId;

        private Label lblGuestId;
        private TextBox txtGuestId;

        private Label lblRoomNumber;
        private TextBox txtRoomNumber;

        private Label lblCheckIn;
        private DateTimePicker dtpCheckIn;

        private Label lblCheckOut;
        private DateTimePicker dtpCheckOut;

        private Label lblStatus;
        private ComboBox cmbStatus;

        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        private DataGridView dgvReservations;
    }
}
