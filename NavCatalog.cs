using System;
using System.Collections.Generic;

namespace InnEase_CRM
{
    public enum TenantTier { A = 1, B = 2, C = 3 }

    // Resolves which sidebar features a signed-in tenant user may see, combining:
    //   - Tenant Tier (from TenantSession.CompanyId: 1=A, 2=B, 3=C)
    //   - Role (Admin / Manager / Staff)
    // Master/Super Admin users are unaffected; MainForm already routes them through
    // WireMasterNavigation / ApplyMasterMenu before this is ever consulted.
    public static class NavCatalog
    {
        public const string Dashboard = "Dashboard";
        public const string Guests = "Guests";
        public const string Reservations = "Reservations";
        public const string Feedback = "Feedback";
        public const string Consent = "Consent";
        public const string Interactions = "Interactions";
        public const string CustomerFeedback = "CustomerFeedback";
        public const string Staff = "Staff";               // user management
        public const string Promotions = "Promotions";      // action triggers
        public const string Reports = "Reports";            // BI
        public const string Analytics = "Analytics";        // BI
        public const string SystemAdmin = "SystemAdmin";    // reserved: no tenant-facing screen exists yet

        // Tenant A (Basic): Main Transactions & Data Collection.
        // Staff management is here too (role-gated below, not tier-gated) since it isn't
        // called out as a BI/action feature in the spec.
        private static readonly string[] BaseFeatures =
        {
            Dashboard, Guests, Reservations, Feedback, Consent, Interactions, CustomerFeedback, Staff
        };

        // Tenant B (Pro) adds: Business Intelligence & Action Triggers.
        private static readonly string[] TierBExtra = { Reports, Analytics, Promotions };

        // Tenant C (Enterprise) adds: Multi-Branching Management.
        // No screen exists for this yet, so nothing is added here. Once a form exists,
        // add its feature key here (and give it a nav button in MainForm's designer).
        private static readonly string[] TierCExtra = { };

        public static TenantTier TierForCompany(int companyId)
        {
            if (companyId <= 1) return TenantTier.A;
            if (companyId == 2) return TenantTier.B;
            return TenantTier.C;
        }

        public static string TierLabel(TenantTier tier)
        {
            switch (tier)
            {
                case TenantTier.A: return "Tenant A - Basic";
                case TenantTier.B: return "Tenant B - Pro";
                default: return "Tenant C - Enterprise";
            }
        }

        // Strict per-role module list, from the system Use Case Diagram. A module is visible
        // only when it's in BOTH the tenant's tier feature set AND this list — the tier decides
        // what the subscription unlocks, the role decides which of those modules this person
        // personally works with.
        //   Super Admin: System Administration (tenant management, subscriptions), BI (global platform analytics)
        //   Admin:   Staff Accounts, Guest Information, Promotions & Loyalty, Customer Reports.
        //   Manager: BI Dashboard, Customer Feedback & Complaints, Customer Analytics.
        //            ("Monitor Customer Retention" has no backing screen yet.)
        //   Staff:   Guest Interactions, Reservations, Guest Requests & Feedback, Customer Consent.
        private static readonly Dictionary<string, string[]> RoleModules =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["superadmin"] = new[] { SystemAdmin, Reports, Analytics }, // Super Admin role (handled separately in MainForm)
                ["admin"] = new[] { Staff, Guests, Promotions, Reports },
                ["manager"] = new[] { Dashboard, Feedback, Analytics },
                ["staff"] = new[] { Interactions, Reservations, CustomerFeedback, Consent },
                ["master"] = new[] { SystemAdmin, Reports, Analytics }, // Alias for superadmin
            };

        // The full set of feature keys this user may see: what their tenant's tier unlocks,
        // narrowed to only the modules their role is assigned in the Use Case Diagram.
        public static HashSet<string> Resolve(int companyId, string role)
        {
            var tier = TierForCompany(companyId);
            var tierFeatures = new HashSet<string>(BaseFeatures);
            if (tier >= TenantTier.B) foreach (var f in TierBExtra) tierFeatures.Add(f);
            if (tier >= TenantTier.C) foreach (var f in TierCExtra) tierFeatures.Add(f);

            string r = (role ?? "").Trim().ToLowerInvariant();
            if (RoleModules.TryGetValue(r, out var roleFeatures))
                tierFeatures.IntersectWith(roleFeatures);
            else
                tierFeatures.Clear();   // unrecognized role: show nothing rather than guess

            return tierFeatures;
        }
    }
        // Read-only convenience wrapper matching the Session.CurrentUser / CurrentRole / CompanyId
        // names from the spec. It does not store anything itself — it reads the values your login
        // flow already sets on TenantSession / UserSession, so there is a single source of truth.
        public static class Session
    {
        public static string CurrentUser => TenantSession.UserName;
        public static string CurrentRole => TenantSession.UserRole;
        public static int CompanyId => TenantSession.CompanyId;
        public static bool IsMaster => TenantSession.IsMaster;
    }
}
