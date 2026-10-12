using System.Linq.Expressions;

namespace tashrif.Core;

/// <summary>
/// Single source for mapping applications → ApplicationResponseDto (senior-review gap #2).
/// Six hand-built projections already drifted once (entity job list returned null CV/city —
/// fixed by 0c943a6); this mapper kills that bug class. ListProjection is a LINQ expression
/// so it composes with Where/Include/Skip/Take in BOTH list paths
/// (applicationsService.GetAllAsync and jobsService.GetApplicationsAsync). Enrichment
/// (city/qualification/CV/job) stays per-call: list paths batch-populate via dictionary
/// queries, single reads call applicationsService.BuildSingleResponseAsync. Deliberately
/// NOT AutoMapper — this project maps manually with LINQ (conventions.md).
/// </summary>
public static class ApplicationMapper
{
    /// <summary>
    /// Base projection shared by every list query — no enrichment; callers batch-populate
    /// jobs/cities/CVs afterwards (unchanged pattern). The callers' populated user_Entity
    /// is always loaded here (Include before projection).
    /// </summary>
    public static readonly Expression<Func<applications, ApplicationResponseDto>> ListProjection =
        a => new ApplicationResponseDto
        {
            Id = a.Id,
            JobId = a.job_id,
            UserId = a.user_id,
            UserName = a.user_Entity.name,
            UserGender = a.user_Entity.gender,
            UserCity = "",
            AvatarUrl = a.user_Entity.avatar_url,
            Qualification = a.qualification,
            Experience = a.experience ?? "",
            CvId = a.cv_id,
            Status = a.status,
            CreatedAt = a.CreatedAt,
        };

    /// <summary>
    /// Base mapping for a materialized application (single reads). userFallback covers the
    /// apply path where the freshly-created application has no loaded user_Entity — name and
    /// gender come from the separately-loaded user, exactly as the old code did. City and
    /// job stay unprefilled: single-read callers enrich them right after.
    /// </summary>
    public static ApplicationResponseDto Map(applications a, users? userFallback = null)
    {
        return new ApplicationResponseDto
        {
            Id = a.Id,
            JobId = a.job_id,
            UserId = a.user_id,
            UserName = a.user_Entity?.name ?? userFallback?.name ?? "",
            UserGender = a.user_Entity?.gender ?? userFallback?.gender ?? "",
            AvatarUrl = a.user_Entity?.avatar_url,
            Qualification = a.qualification,
            Experience = a.experience ?? "",
            CvId = a.cv_id,
            Status = a.status,
            CreatedAt = a.CreatedAt,
        };
    }
}
