#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnEase_CRM
{
    // Every feature a subscription plan can switch on or off.
    public enum PlanModule
    {
        // Lab 1 = Tenant A = Basic : Data Collection + main transaction
        Guests,
        Reservations,
        Consent,
        Interactions,
        Feedback,               // Inquiry / Complaints / Feedback

        // Lab 2 = Tenant B = Pro : Lab 1 + Business Intelligence + Actions (Retention)
        BusinessIntelligence,   // KPIs, Graphs
        Reports,
        Promotions,             // Retention actions (Promotions & Loyalty)

        // Lab 3 = Tenant C = Enterprise : Lab 2 + Company Branches
        Branches
    }

    // Plan -> modules. The plan comes from the master database (PlanTier), which is exactly
    // what the Super Admin changes in the Subscriptions screen.
    public static class FeatureAccess
    {
        private static readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("https://localhost:7129/") };

        public static string PlanTier { get; set; } = "Basic";
        public static bool IsSuspended { get; set; }

        // Starter = 1, Standard = 2, Enterprise = 3 (each level includes the one below it)
        public static int LevelOf(string plan)
        {
            switch ((plan ?? "").Trim().ToLowerInvariant())
            {
                case "enterprise": return 3;
                case "standard": return 2;
                case "starter": return 1;
                default: return 1;
            }
        }

        public static int Level => LevelOf(PlanTier);

        private static readonly Dictionary<PlanModule, int> RequiredLevel = new Dictionary<PlanModule, int>
        {
            { PlanModule.Guests, 1 },
            { PlanModule.Reservations, 1 },
            { PlanModule.Consent, 1 },
            { PlanModule.Interactions, 1 },
            { PlanModule.Feedback, 1 },

            { PlanModule.BusinessIntelligence, 2 },
            { PlanModule.Reports, 2 },
            { PlanModule.Promotions, 2 },

            { PlanModule.Branches, 3 }
        };

        public static bool IsEnabled(PlanModule module)
        {
            return Level >= RequiredLevel[module];
        }

        // Sidebar button (field name in MainForm) -> the module it belongs to.
        private static readonly (string ControlName, PlanModule Module)[] SidebarMap =
        {
            // Tenant A (Starter) - Basic Features
            ("btnGuests",           PlanModule.Guests),
            ("btnReservations",     PlanModule.Reservations),
            ("btnConsent",          PlanModule.Consent),
            ("btnInteractions",     PlanModule.Interactions),
            ("btnFeedback",         PlanModule.Feedback),
            ("btnCustomerFeedback", PlanModule.Feedback),

            // Tenant B (Standard) - Business Intelligence + Actions
            ("btnDashboard",        PlanModule.BusinessIntelligence),
            ("pnlInfo3",            PlanModule.BusinessIntelligence),   // "Business Intelligence" card on Home
            ("btnAnalytics",        PlanModule.BusinessIntelligence),
            ("btnReports",          PlanModule.Reports),
            ("btnSalesReport",      PlanModule.Reports),
            ("btnPromotions",       PlanModule.Promotions),

            // Tenant C (Enterprise) - Branching
            ("btnBranches",         PlanModule.Branches)
        };

        // Reads this tenant's plan from the master API. Returns false if it could not be loaded.
        public static async Task<bool> LoadAsync(int companyId)
        {
            try
            {
                var tenants = await _client.GetFromJsonAsync<List<TenantPlanDto>>("api/master/tenants");
                var tenant = tenants?.FirstOrDefault(t => t.CompanyId == companyId);
                if (tenant == null)
                    return false;

                PlanTier = tenant.PlanTier;
                IsSuspended = string.Equals(tenant.ActiveStatus, "suspended", StringComparison.OrdinalIgnoreCase);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Hides every sidebar item the current plan does not include.
        // It only ever HIDES - it never shows something a role rule already hid.
        public static void ApplyToSidebar(Control root)
        {
            foreach (var entry in SidebarMap)
            {
                if (IsEnabled(entry.Module))
                    continue;

                var control = FindControl(root, entry.ControlName);
                if (control != null)
                    control.Visible = false;
            }
        }

        // MainForm's controls are private fields, and hand-built controls often have no Name,
        // so look the field up by name first and fall back to Controls.Find.
        private static Control FindControl(Control root, string name)
        {
            var field = root.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (field?.GetValue(root) is Control fromField)
                return fromField;

            return root.Controls.Find(name, true).FirstOrDefault();
        }

        // Short text for grids.
        public static string Describe(string plan)
        {
            switch (LevelOf(plan))
            {
                case 3: return "Data Collection + BI + Promotions + Branches";
                case 2: return "Data Collection + BI + Promotions";
                case 1: return "Data Collection";
                default: return "Data Collection";
            }
        }

        // Longer text for the Subscriptions screen.
        public static string DescribeDetailed(string plan)
        {
            switch (LevelOf(plan))
            {
                case 3:
                    return "everything in Standard, plus Company Branches (branch switching / branch filter).";
                case 2:
                    return "everything in Starter, plus Business Intelligence (KPIs, Graphs, Reports) and Promotions & Loyalty (Retention).";
                case 1:
                    return "Data Collection (Inquiry, Complaints, Feedback), Guest Information, Reservations, Customer Consent, Guest Interactions.";
                default:
                    return "Data Collection (Inquiry, Complaints, Feedback), Guest Information, Reservations, Customer Consent, Guest Interactions.";
            }
        }

        private class TenantPlanDto
        {
            public int CompanyId { get; set; }
            public string PlanTier { get; set; } = string.Empty;
            public string ActiveStatus { get; set; } = string.Empty;
        }
    }
}
