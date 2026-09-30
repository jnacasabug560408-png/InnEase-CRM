using System;
using System.Net.Http.Json;
using System.Windows.Forms;
using Microsoft.Win32;

namespace InnEase_CRM
{
    public partial class LoginForm : Form
    {
        private const string RegKey = @"SOFTWARE\InnEaseCRM";

        public LoginForm()
        {
            InitializeComponent();

            // Allow pressing Enter to login
            this.AcceptButton = btnLogin;
        }

        // ── SIGN IN ────────────────────────────────────────────────────────────

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            // NOTE: this still reads from txtUsername (the control's designer name
            // is unchanged so the project keeps compiling without editing
            // LoginForm.Designer.cs) - but its contents are now treated as an
            // email address, matching the new email-based accounts. If you want
            // the on-screen label/placeholder to say "Email" instead of
            // "Username", update that Label's Text property in the designer -
            // it's a cosmetic change only, safe to do anytime.
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowStatus("Please enter your email.", error: true);
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Signing in…";
            ShowStatus("", error: false);

            try
            {
                var payload = new { Email = txtUsername.Text.Trim(), Password = txtPassword.Text };
                var response = await ApiClient.Instance.PostAsJsonAsync("api/auth/login", payload);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null)
                    {
                        UserSession.Role = result.Role;
                        UserSession.FullName = result.FullName;
                        UserSession.Token = result.Token;

                        // Ensure TenantSession also has the role and tier for consistency
                        TenantSession.UserRole = result.Role;
                        TenantSession.UserName = result.Email;
                        TenantSession.TenantTier = result.TenantTier;   // e.g. "TenantA" / "TenantB" / "TenantC" / "Master"
                        TenantSession.TenantId = result.TenantId;

                        // Update IsMaster flag based on role
                        bool isMaster = string.Equals(result.Role, "master", StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(result.Role, "superadmin", StringComparison.OrdinalIgnoreCase);
                        UserSession.IsMaster = isMaster;
                        TenantSession.IsMaster = isMaster;

                        // Configure shared client
                        InnEase_CRM.ApiClient.Configure(result.Token, result.TenantId);

                        // Subscription plan decides which modules this tenant can see
                        FeatureAccess.PlanTier = result.Plan;

                        this.Hide();
                        var mainForm = new MainForm();
                        mainForm.FormClosed += (s, _) => this.Close();
                        mainForm.Show();
                    }
                }
                else
                {
                    ShowStatus("Invalid email or password.", error: true);
                }
            }
            catch (Exception ex)
            {
                ShowStatus("Cannot reach server: " + ex.Message, error: true);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Sign In";
            }
        }

        // ── HELPERS ────────────────────────────────────────────────────────────

        private void ShowStatus(string message, bool error)
        {
            lblStatus.ForeColor = error
                ? System.Drawing.Color.FromArgb(220, 38, 38)
                : System.Drawing.Color.FromArgb(5, 150, 105);
            lblStatus.Text = message;
        }


    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Plan { get; set; } = "Starter";
        public string TenantTier { get; set; } = string.Empty;
        public int? TenantId { get; set; }
    }

    public static class UserSession
    {
        public static string Token { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;
        public static string FullName { get; set; } = string.Empty;
        public static bool IsMaster { get; set; } = false; // Now settable
    }
}
