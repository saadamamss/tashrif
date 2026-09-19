using Microsoft.EntityFrameworkCore;
using tashrif.Data.DTOs.Analytics;
using tashrif.Data.Interfaces;

namespace tashrif.Core;

public class analyticsService(IUnitOfWork unitOfWork) : IanalyticsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private static readonly string[] AllStatuses =
        ["new", "shortlisted", "interview", "contract_sent", "accepted", "refused", "withdrawn"];

    public async Task<EntityAnalyticsDto> GetEntityAnalyticsAsync(long entityId)
    {
        var dto = new EntityAnalyticsDto();

        // --- 1. Job counts (sequential — shared DbContext is not thread-safe) ---
        var jobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var entityJobs = await jobsQuery
            .Where(j => j.entity_id == entityId && !j.IsDeleted)
            .ToListAsync();

        dto.TotalJobs = entityJobs.Count;
        dto.ActiveJobs = entityJobs.Count(j => j.status == "active");

        var entityJobIds = entityJobs.Select(j => j.Id).ToHashSet();

        // --- 2. Applications for this entity's jobs ---
        var appsQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var entityApplications = await appsQuery
            .Where(a => entityJobIds.Contains(a.job_id) && !a.IsDeleted)
            .ToListAsync();

        dto.TotalApplications = entityApplications.Count;

        // --- 3. Applications by status (all 7 statuses, zeros included) ---
        dto.ApplicationsByStatus = AllStatuses.ToDictionary(s => s, _ => 0);
        foreach (var g in entityApplications.GroupBy(a => a.status))
            dto.ApplicationsByStatus[g.Key] = g.Count();

        // --- 4. Conversion rate ---
        var acceptedCount = dto.ApplicationsByStatus.GetValueOrDefault("accepted", 0);
        dto.ConversionRate = dto.TotalApplications == 0
            ? 0
            : Math.Round((double)acceptedCount / dto.TotalApplications * 100, 1);

        // --- 5. Average time-to-hire (D4: from application_status_history) ---
        {
            var entityAppIds = entityApplications.Select(a => a.Id).ToHashSet();
            var historyQuery = await _unitOfWork.Application_status_historyRepository.GetQueryable();
            var acceptedHistory = await historyQuery
                .Where(h => h.new_status == "accepted" && !h.IsDeleted && entityAppIds.Contains(h.application_id))
                .ToListAsync();

            if (acceptedHistory.Count > 0)
            {
                var appCreatedMap = entityApplications.ToDictionary(a => a.Id, a => a.CreatedAt);
                dto.AverageTimeToHireDays = Math.Round(
                    acceptedHistory
                        .Select(h => (h.changed_at - appCreatedMap[h.application_id]).TotalDays)
                        .Average(),
                    1);
            }
        }

        // --- 6. Top 5 jobs by application count ---
        if (entityApplications.Count > 0)
        {
            var topGroups = entityApplications
                .GroupBy(a => a.job_id)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .ToList();

            var jobTitleMap = entityJobs.ToDictionary(j => j.Id, j => j.title);

            dto.TopJobs = topGroups.Select(g => new TopJobDto
            {
                JobId = g.Key,
                Title = jobTitleMap.GetValueOrDefault(g.Key, string.Empty),
                ApplicationCount = g.Count(),
                HiredCount = g.Count(a => a.status == "accepted"),
            }).ToList();
        }

        // --- 7. Applications over time — continuous 30-day series, zero-filled ---
        {
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddDays(-29);

            // Count per UTC day for this entity's applications
            var dailyCounts = entityApplications
                .Where(a => a.CreatedAt.Date >= startDate)
                .GroupBy(a => a.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            dto.ApplicationsOverTime = Enumerable.Range(0, 30).Select(i =>
            {
                var date = startDate.AddDays(i);
                return new DailyCountDto
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    Count = dailyCounts.GetValueOrDefault(date, 0),
                };
            }).ToList();
        }

        // --- 8. Applicant demographics (distinct applicants only) ---
        if (entityApplications.Count > 0)
        {
            var distinctUserIds = entityApplications
                .Select(a => a.user_id)
                .Distinct()
                .ToList();

            var usersQuery = await _unitOfWork.UsersRepository.GetQueryable();
            var distinctUsers = await usersQuery
                .Where(u => distinctUserIds.Contains(u.Id) && !u.IsDeleted)
                .ToListAsync();

            // By gender — DB stores English (male/female), grouped as-is
            dto.ApplicantDemographics.ByGender = distinctUsers
                .GroupBy(u => string.IsNullOrWhiteSpace(u.gender) ? "غير محدد" : u.gender)
                .ToDictionary(g => g.Key, g => g.Count());

            // By nationality — top 5 + "أخرى", empty/null → "غير محدد"
            var nationalityGroups = distinctUsers
                .GroupBy(u => string.IsNullOrWhiteSpace(u.nationality) ? "غير محدد" : u.nationality)
                .OrderByDescending(g => g.Count())
                .ToList();

            if (nationalityGroups.Count <= 5)
            {
                dto.ApplicantDemographics.ByNationality = nationalityGroups
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            else
            {
                var top5 = nationalityGroups.Take(5).ToDictionary(g => g.Key, g => g.Count());
                var otherCount = nationalityGroups.Skip(5).Sum(g => g.Count());
                top5["أخرى"] = otherCount;
                dto.ApplicantDemographics.ByNationality = top5;
            }
        }

        return dto;
    }
}
