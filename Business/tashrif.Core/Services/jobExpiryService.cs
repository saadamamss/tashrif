using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace tashrif.Core.Services;

public class JobExpiryService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<JobExpiryService> _logger;

    public JobExpiryService(IServiceProvider services, ILogger<JobExpiryService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExpireJobsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during job expiry check");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ExpireJobsAsync(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var jobsQuery = await unitOfWork.JobsRepository.GetQueryable();
        var now = DateTime.UtcNow;

        var expiredJobs = await jobsQuery
            .Where(j => !j.IsDeleted
                && j.status == "active"
                && j.end_date != null
                && j.end_date < now)
            .ToListAsync(ct);

        if (expiredJobs.Count == 0)
        {
            _logger.LogDebug("No expired jobs found");
            return;
        }

        foreach (var job in expiredJobs)
        {
            job.status = "expired";
            job.UpdatedAt = now;
            unitOfWork.JobsRepository.Update(job);
        }

        await unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Expired {Count} jobs past their end_date", expiredJobs.Count);
    }
}
