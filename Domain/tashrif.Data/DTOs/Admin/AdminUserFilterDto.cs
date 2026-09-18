namespace tashrif.Data.DTOs.Admin;

public class AdminUserFilterDto : PaginationDto
{
    public string? Type { get; set; }
    public string? Search { get; set; }
    /// <summary>"active" = live users only, "deactivated" = soft-deleted only, null/empty = all (D10)</summary>
    public string? Status { get; set; }
}
