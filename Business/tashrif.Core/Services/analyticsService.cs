using Microsoft.EntityFrameworkCore;
using tashrif.Data.Constants;
using tashrif.Data.DTOs.Analytics;
using tashrif.Data.Interfaces;

namespace tashrif.Core;

public class analyticsService : IanalyticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public analyticsService(IUnitOfWork unitOfWork, IClock clock)
    {
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    // Backward compat for existing tests that construct with only IUnitOfWork (spec 1 → 2 gap)
    public analyticsService(IUnitOfWork unitOfWork) : this(unitOfWork, new SystemClock()) { }

    public async Task<EntityAnalyticsDto> GetEntityAnalyticsAsync(long entityId)
    {
        var entityJobs = await GetEntityJobsAsync(entityId);
        var entityJobIds = entityJobs.Select(j => j.Id).ToHashSet();
        var entityApplications = await GetEntityApplicationsAsync(entityJobIds);

        var funnel = BuildFunnel(entityApplications);

        var jobTitleMap = entityJobs.ToDictionary(j => j.Id, j => j.title);

        return new EntityAnalyticsDto
        {
            TotalJobs = entityJobs.Count,
            ActiveJobs = entityJobs.Count(j => j.status == "active"),
            TotalApplications = entityApplications.Count,
            ApplicationsByStatus = funnel,
            ConversionRate = CalcConversion(entityApplications, funnel),
            AverageTimeToHireDays = await CalcTimeToHireAsync(entityApplications),
            TopJobs = BuildTopJobs(entityApplications, jobTitleMap),
            ApplicationsOverTime = BuildTimeline(entityApplications, _clock.UtcNow.Date),
            ApplicantDemographics = await BuildDemographicsAsync(entityApplications),
        };
    }

    private async Task<List<jobs>> GetEntityJobsAsync(long entityId)
    {
        var jobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
        return await jobsQuery.Where(j => j.entity_id == entityId && !j.IsDeleted).ToListAsync();
    }

    private async Task<List<applications>> GetEntityApplicationsAsync(HashSet<long> entityJobIds)
    {
        if (entityJobIds.Count == 0) return [];
        var appsQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        return await appsQuery.Where(a => entityJobIds.Contains(a.job_id) && !a.IsDeleted).ToListAsync();
    }

    private static Dictionary<string, int> BuildFunnel(List<applications> apps)
    {
        var funnel = ApplicationStatuses.All.ToDictionary(s => s, _ => 0);
        foreach (var g in apps.GroupBy(a => a.status))
            funnel[g.Key] = g.Count();
        return funnel;
    }

    private static double CalcConversion(List<applications> apps, Dictionary<string, int> funnel)
    {
        if (apps.Count == 0) return 0;
        var acceptedCount = funnel.GetValueOrDefault("accepted", 0);
        return Math.Round((double)acceptedCount / apps.Count * 100, 1);
    }

    private async Task<double?> CalcTimeToHireAsync(List<applications> apps)
    {
        if (apps.Count == 0) return null;

        var entityAppIds = apps.Select(a => a.Id).ToHashSet();
        var historyQuery = await _unitOfWork.Application_status_historyRepository.GetQueryable();
        var acceptedHistory = await historyQuery
            .Where(h => h.new_status == "accepted" && !h.IsDeleted && entityAppIds.Contains(h.application_id))
            .ToListAsync();

        if (acceptedHistory.Count == 0) return null;

        var appCreatedMap = apps.ToDictionary(a => a.Id, a => a.CreatedAt);
        return Math.Round(
            acceptedHistory.Select(h => (h.changed_at - appCreatedMap[h.application_id]).TotalDays).Average(),
            1);
    }

    private static List<TopJobDto> BuildTopJobs(List<applications> apps, Dictionary<long, string> jobTitleMap)
    {
        if (apps.Count == 0) return [];
        return apps
            .GroupBy(a => a.job_id)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new TopJobDto
            {
                JobId = g.Key,
                Title = jobTitleMap.GetValueOrDefault(g.Key, string.Empty),
                ApplicationCount = g.Count(),
                HiredCount = g.Count(a => a.status == "accepted"),
            })
            .ToList();
    }

    private static List<DailyCountDto> BuildTimeline(List<applications> apps, DateTime today)
    {
        var startDate = today.AddDays(-29);
        var dailyCounts = apps
            .Where(a => a.CreatedAt.Date >= startDate)
            .GroupBy(a => a.CreatedAt.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        return Enumerable.Range(0, 30).Select(i =>
        {
            var date = startDate.AddDays(i);
            return new DailyCountDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                Count = dailyCounts.GetValueOrDefault(date, 0),
            };
        }).ToList();
    }

    private async Task<DemographicsDto> BuildDemographicsAsync(List<applications> apps)
    {
        var dto = new DemographicsDto();
        if (apps.Count == 0) return dto;

        var distinctUserIds = apps.Select(a => a.user_id).Distinct().ToList();
        var usersQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var distinctUsers = await usersQuery
            .Where(u => distinctUserIds.Contains(u.Id) && !u.IsDeleted)
            .ToListAsync();

        dto.ByGender = distinctUsers
            .GroupBy(u => string.IsNullOrWhiteSpace(u.gender) ? "غير محدد" : u.gender)
            .ToDictionary(g => g.Key, g => g.Count());

        var nationalityGroups = distinctUsers
            .GroupBy(u => string.IsNullOrWhiteSpace(u.nationality) ? "غير محدد" : u.nationality)
            .OrderByDescending(g => g.Count())
            .ToList();

        if (nationalityGroups.Count <= 5)
        {
            dto.ByNationality = nationalityGroups.ToDictionary(g => g.Key, g => g.Count());
        }
        else
        {
            var top5 = nationalityGroups.Take(5).ToDictionary(g => g.Key, g => g.Count());
            var otherCount = nationalityGroups.Skip(5).Sum(g => g.Count());
            top5["أخرى"] = otherCount;
            dto.ByNationality = top5;
        }

        return dto;
    }
}
