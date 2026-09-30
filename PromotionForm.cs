using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using InnEaseCRM.domain.entities;

namespace InnEase_CRM
{
    public partial class PromotionForm : Form
    {
        private readonly HttpClient _httpClient;

        // Use the currently logged-in tenant/company
        private string ApiUrl =>
            $"https://localhost:7129/api/promotions";

        public PromotionForm()
        {
            InitializeComponent();

            _httpClient = ApiClient.Instance;

            this.Load += PromotionForm_Load;

            dgvPromotions.CellClick += dgvPromotions_CellClick;

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnApprove.Click += btnApprove_Click;
            btnReject.Click += btnReject_Click;

            // Default discount type
            if (cmbDiscountType.Items.Count > 0)
            {
                cmbDiscountType.SelectedIndex = 0;
            }
        }

        // =========================================================
        // FORM LOAD
        // =========================================================

        private async void PromotionForm_Load(object sender, EventArgs e)
        {
            await LoadPromotions();
        }

        // =========================================================
        // LOAD PROMOTIONS
        // =========================================================

        private async Task LoadPromotions()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await ApiClient.GetErrorMessageAsync(response);
                    MessageBox.Show(
                        $"Unable to load promotions.\n\n{errorMessage}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                var items = await response.Content.ReadFromJsonAsync<List<Promotion>>() ?? new List<Promotion>();
                dgvPromotions.DataSource = null;
                dgvPromotions.DataSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load promotions.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // ADD PROMOTION
        // =========================================================

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate promo code
                if (string.IsNullOrWhiteSpace(txtPromoCode.Text))
                {
                    MessageBox.Show(
                        "Please enter a promo code.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtPromoCode.Focus();
                    return;
                }

                // Validate reason (QA requirement: why this promotion is needed)
                if (string.IsNullOrWhiteSpace(txtReason.Text))
                {
                    MessageBox.Show(
                        "Please enter a reason for this promotion.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtReason.Focus();
                    return;
                }

                // Validate who is requesting/implementing it
                if (string.IsNullOrWhiteSpace(txtImplementedBy.Text))
                {
                    MessageBox.Show(
                        "Please enter who is requesting/implementing this promotion.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtImplementedBy.Focus();
                    return;
                }

                // Validate discount value
                if (!decimal.TryParse(
                    txtDiscountValue.Text,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal discountValue))
                {
                    MessageBox.Show(
                        "Please enter a valid discount value.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtDiscountValue.Focus();
                    return;
                }

                // Validate discount value depending on type
                string discountType =
                    cmbDiscountType.SelectedItem?.ToString()
                    ?? "Percentage";

                if (discountType == "Percentage" &&
                    (discountValue < 0 || discountValue > 100))
                {
                    MessageBox.Show(
                        "Percentage discount must be between 0 and 100.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtDiscountValue.Focus();
                    return;
                }

                if (discountValue < 0)
                {
                    MessageBox.Show(
                        "Discount value cannot be negative.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtDiscountValue.Focus();
                    return;
                }

                // Loyalty points
                if (!int.TryParse(
                    txtLoyaltyPoints.Text,
                    out int loyaltyPoints))
                {
                    loyaltyPoints = 0;
                }

                if (loyaltyPoints < 0)
                {
                    MessageBox.Show(
                        "Loyalty points cannot be negative.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtLoyaltyPoints.Focus();
                    return;
                }

                // Validate dates
                if (dtpEndDate.Value <= dtpStartDate.Value)
                {
                    MessageBox.Show(
                        "End date must be after start date.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // Create promotion.
                // ApprovalStatus / ApprovedBy are NOT set here � the API always
                // creates new promotions as "Pending" until someone approves them.
                var promotion = new Promotion
                {
                    PromoCode = txtPromoCode.Text.Trim(),

                    Description = txtDescription.Text.Trim(),

                    DiscountType = discountType,

                    DiscountValue = discountValue,

                    LoyaltyPoints = loyaltyPoints,

                    StartDate = dtpStartDate.Value,

                    EndDate = dtpEndDate.Value,

                    IsActive = chkIsActive.Checked,

                    Reason = txtReason.Text.Trim(),

                    ImplementedBy = txtImplementedBy.Text.Trim()
                };

                // Send POST request
                var response =
                    await _httpClient.PostAsJsonAsync(
                        ApiUrl,
                        promotion);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Promotion added successfully! It is now Pending approval.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadPromotions();
                }
                else
                {
                    string errorMessage =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Failed to add promotion.\n\n" +
                        errorMessage,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while adding the promotion.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // SELECT PROMOTION FROM DATAGRIDVIEW
        // =========================================================

        private void dgvPromotions_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                var row = dgvPromotions.Rows[e.RowIndex];

                // Promotion ID
                txtPromotionId.Text =
                    row.Cells["PromotionId"].Value?.ToString() ?? "";

                // Promo Code
                txtPromoCode.Text =
                    row.Cells["PromoCode"].Value?.ToString() ?? "";

                // Description
                txtDescription.Text =
                    row.Cells["Description"].Value?.ToString() ?? "";

                // Discount Value
                txtDiscountValue.Text =
                    row.Cells["DiscountValue"].Value?.ToString() ?? "";

                // Loyalty Points
                txtLoyaltyPoints.Text =
                    row.Cells["LoyaltyPoints"].Value?.ToString() ?? "";

                // Discount Type
                var discountType =
                    row.Cells["DiscountType"].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(discountType) &&
                    cmbDiscountType.Items.Contains(discountType))
                {
                    cmbDiscountType.SelectedItem = discountType;
                }

                // Start Date
                if (DateTime.TryParse(
                    row.Cells["StartDate"].Value?.ToString(),
                    out DateTime startDate))
                {
                    dtpStartDate.Value = startDate;
                }

                // End Date
                if (DateTime.TryParse(
                    row.Cells["EndDate"].Value?.ToString(),
                    out DateTime endDate))
                {
                    dtpEndDate.Value = endDate;
                }

                // Active status
                if (bool.TryParse(
                    row.Cells["IsActive"].Value?.ToString(),
                    out bool isActive))
                {
                    chkIsActive.Checked = isActive;
                }

                // ---- Governance / approval fields ----

                txtReason.Text =
                    row.Cells["Reason"].Value?.ToString() ?? "";

                txtImplementedBy.Text =
                    row.Cells["ImplementedBy"].Value?.ToString() ?? "";

                txtApprovalStatus.Text =
                    row.Cells["ApprovalStatus"].Value?.ToString() ?? "Pending";

                txtApprovedBy.Text =
                    row.Cells["ApprovedBy"].Value?.ToString() ?? "";

                UpdateApprovalButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to select the promotion.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Approve/Reject only make sense for a selected, still-Pending promotion.
        private void UpdateApprovalButtonState()
        {
            bool hasSelection = int.TryParse(txtPromotionId.Text, out _);
            bool isPending = string.Equals(txtApprovalStatus.Text, "Pending", StringComparison.OrdinalIgnoreCase);

            btnApprove.Enabled = hasSelection && isPending;
            btnReject.Enabled = hasSelection && isPending;
        }

        // =========================================================
        // UPDATE PROMOTION
        // =========================================================

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate Promotion ID
                if (!int.TryParse(
                    txtPromotionId.Text,
                    out int promotionId))
                {
                    MessageBox.Show(
                        "Please select a promotion first.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // Validate promo code
                if (string.IsNullOrWhiteSpace(txtPromoCode.Text))
                {
                    MessageBox.Show(
                        "Please enter a promo code.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtPromoCode.Focus();
                    return;
                }

                // Validate reason
                if (string.IsNullOrWhiteSpace(txtReason.Text))
                {
                    MessageBox.Show(
                        "Please enter a reason for this promotion.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtReason.Focus();
                    return;
                }

                // Validate implementer
                if (string.IsNullOrWhiteSpace(txtImplementedBy.Text))
                {
                    MessageBox.Show(
                        "Please enter who is requesting/implementing this promotion.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtImplementedBy.Focus();
                    return;
                }

                // Validate discount
                if (!decimal.TryParse(
                    txtDiscountValue.Text,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal discountValue))
                {
                    MessageBox.Show(
                        "Please enter a valid discount value.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtDiscountValue.Focus();
                    return;
                }

                string discountType =
                    cmbDiscountType.SelectedItem?.ToString()
                    ?? "Percentage";

                // Percentage validation
                if (discountType == "Percentage" &&
                    (discountValue < 0 || discountValue > 100))
                {
                    MessageBox.Show(
                        "Percentage discount must be between 0 and 100.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtDiscountValue.Focus();
                    return;
                }

                // General discount validation
                if (discountValue < 0)
                {
                    MessageBox.Show(
                        "Discount value cannot be negative.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtDiscountValue.Focus();
                    return;
                }

                // Loyalty points
                if (!int.TryParse(
                    txtLoyaltyPoints.Text,
                    out int loyaltyPoints))
                {
                    loyaltyPoints = 0;
                }

                if (loyaltyPoints < 0)
                {
                    MessageBox.Show(
                        "Loyalty points cannot be negative.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtLoyaltyPoints.Focus();
                    return;
                }

                // Validate dates
                if (dtpEndDate.Value <= dtpStartDate.Value)
                {
                    MessageBox.Show(
                        "End date must be after start date.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // Create updated promotion.
                // ApprovalStatus / ApprovedBy are ignored by the API on Update �
                // they only change through Approve / Reject.
                var promotion = new Promotion
                {
                    PromotionId = promotionId,

                    PromoCode = txtPromoCode.Text.Trim(),

                    Description = txtDescription.Text.Trim(),

                    DiscountType = discountType,

                    DiscountValue = discountValue,

                    LoyaltyPoints = loyaltyPoints,

                    StartDate = dtpStartDate.Value,

                    EndDate = dtpEndDate.Value,

                    IsActive = chkIsActive.Checked,

                    Reason = txtReason.Text.Trim(),

                    ImplementedBy = txtImplementedBy.Text.Trim()
                };

                // Send PUT request
                var response =
                    await _httpClient.PutAsJsonAsync(
                        $"{ApiUrl}/{promotionId}",
                        promotion);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Promotion updated successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadPromotions();
                }
                else
                {
                    string errorMessage =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Failed to update promotion.\n\n" +
                        errorMessage,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while updating the promotion.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // APPROVE PROMOTION
        // =========================================================

        private async void btnApprove_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtPromotionId.Text, out int promotionId))
            {
                MessageBox.Show(
                    "Please select a promotion first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string approver = PromptForName("Approve Promotion", "Approving manager's name:");
            if (approver == null)
                return; // user cancelled

            if (string.IsNullOrWhiteSpace(approver))
            {
                MessageBox.Show(
                    "Approver name is required.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            await SendApprovalAction(promotionId, "approve", approver, "Promotion approved.");
        }

        // =========================================================
        // REJECT PROMOTION
        // =========================================================

        private async void btnReject_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtPromotionId.Text, out int promotionId))
            {
                MessageBox.Show(
                    "Please select a promotion first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "Are you sure you want to reject this promotion?",
                "Confirm Reject",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            string rejecter = PromptForName("Reject Promotion", "Rejecting manager's name:");
            if (rejecter == null)
                return; // user cancelled

            if (string.IsNullOrWhiteSpace(rejecter))
            {
                MessageBox.Show(
                    "Name is required.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            await SendApprovalAction(promotionId, "reject", rejecter, "Promotion rejected.");
        }

        private async Task SendApprovalAction(int promotionId, string action, string approvedBy, string successMessage)
        {
            try
            {
                var body = new { ApprovedBy = approvedBy };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{ApiUrl}/{promotionId}/{action}",
                    body);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        successMessage,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    await LoadPromotions();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        $"Failed to {action} promotion.\n\n{errorMessage}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while trying to {action} the promotion.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Small inline prompt so we don't need an extra Designer file or a
        // Microsoft.VisualBasic reference just to ask for one line of text.
        // Returns null if the user cancels.
        private static string PromptForName(string title, string label)
        {
            using (var dialog = new Form())
            using (var lbl = new Label())
            using (var input = new TextBox())
            using (var ok = new Button())
            using (var cancel = new Button())
            {
                dialog.Text = title;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ClientSize = new Size(360, 130);

                lbl.Text = label;
                lbl.AutoSize = true;
                lbl.Location = new Point(15, 15);

                input.Location = new Point(15, 40);
                input.Size = new Size(330, 25);

                ok.Text = "OK";
                ok.DialogResult = DialogResult.OK;
                ok.Location = new Point(185, 80);
                ok.Size = new Size(75, 30);

                cancel.Text = "Cancel";
                cancel.DialogResult = DialogResult.Cancel;
                cancel.Location = new Point(270, 80);
                cancel.Size = new Size(75, 30);

                dialog.Controls.Add(lbl);
                dialog.Controls.Add(input);
                dialog.Controls.Add(ok);
                dialog.Controls.Add(cancel);

                dialog.AcceptButton = ok;
                dialog.CancelButton = cancel;

                return dialog.ShowDialog() == DialogResult.OK ? input.Text.Trim() : null;
            }
        }

        // =========================================================
        // DELETE PROMOTION
        // =========================================================

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate Promotion ID
                if (!int.TryParse(
                    txtPromotionId.Text,
                    out int promotionId))
                {
                    MessageBox.Show(
                        "Please select a promotion first.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // Confirm deletion
                var confirm = MessageBox.Show(
                    "Are you sure you want to delete this promotion?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                // Send DELETE request
                var response =
                    await _httpClient.DeleteAsync(
                        $"{ApiUrl}/{promotionId}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Promotion deleted successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    await LoadPromotions();
                }
                else
                {
                    string errorMessage =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Failed to delete promotion.\n\n" +
                        errorMessage,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while deleting the promotion.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // CLEAR FORM
        // =========================================================

        private void ClearFields()
        {
            txtPromotionId.Clear();

            txtPromoCode.Clear();

            txtDescription.Clear();

            txtDiscountValue.Clear();

            txtLoyaltyPoints.Clear();

            if (cmbDiscountType.Items.Count > 0)
            {
                cmbDiscountType.SelectedIndex = 0;
            }

            dtpStartDate.Value = DateTime.Today;

            dtpEndDate.Value = DateTime.Today.AddMonths(1);

            chkIsActive.Checked = true;

            txtReason.Clear();
            txtImplementedBy.Clear();
            txtApprovalStatus.Text = "Pending";
            txtApprovedBy.Clear();

            UpdateApprovalButtonState();

            txtPromoCode.Focus();
        }
    }
}

