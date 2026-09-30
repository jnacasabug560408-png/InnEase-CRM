namespace InnEase_CRM
{
    public static class TenantSession
    {
        public static int CompanyId { get; set; } = 1;
        public static int? TenantId { get; set; } = 1;
        public static int? BranchId { get; set; } = null;
        public static bool IsMaster { get; set; } = false;
        public static string UserRole { get; set; } = "staff";
        public static string UserName { get; set; } = string.Empty;
        public static string TenantTier { get; set; } = "Starter";

        public static bool IsGlobalView => IsMaster && TenantId == null;
        public static bool SupportsBranching => TenantId != null && TenantId == 3; // Tenant C supports branching
        public static string TenantCode => TenantId switch
        {
            1 => "A",
            2 => "B", 
            3 => "C",
            _ => "Unknown"
        };

        public static void Reset()
        {
            TenantId = 1;
            BranchId = null;
            IsMaster = false;
            UserRole = "staff";
            UserName = string.Empty;
        }

        public static string GetContextLabel()
        {
            if (IsGlobalView) return "Master Perspective";
            if (TenantId.HasValue) return $"Tenant {TenantCode}";
            return "No Tenant";
        }
    }
}
