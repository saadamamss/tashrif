namespace tashrif.Data.Constants;

/// <summary>
/// Single source of truth for audit-log `action` strings and the allowed `entity_type` values.
/// entity_type MUST stay plural and equal to the frontend `entityTypeOptions`
/// (pages/dashboard/admin/audit-logs.vue) — the admin UI filter compares exact equality.
/// </summary>
public static class AuditActions
{
    public const string AdminUserDeactivated = "admin.user_deactivated";
    public const string AdminUserActivated   = "admin.user_activated";
    public const string AdminJobDeactivated  = "admin.job_deactivated";
    public const string AdminProfileUpdated  = "admin.profile_updated";
    public const string AuthLogin            = "auth.login";
    public const string AuthPasswordChanged  = "auth.password_changed";
    public const string ProfileUpdated       = "profile.updated";
    public const string ContractSent         = "contract.sent";
    public const string ContractSigned       = "contract.signed";
    public const string InterviewScheduled   = "interview.scheduled";

    /// <summary>Must match frontend `entityTypeOptions` exactly (plural).</summary>
    public static readonly string[] EntityTypes =
        ["applications", "jobs", "contracts", "interviews", "users"];
}
