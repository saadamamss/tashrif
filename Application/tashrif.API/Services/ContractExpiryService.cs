using tashrif.Email.Interfaces;
using tashrif.Email.Templates;

namespace tashrif.API.Services;

/// <summary>
/// Flips <c>sent</c> contracts past their signing deadline to <c>expired</c> and notifies
/// both sides. Lives in the API layer (not Core) because fan-out needs the SignalR hub and
/// the email service, which Core must not reference. Mirrors <see cref="tashrif.Core.Services.JobExpiryService"/>
/// but is <see cref="IClock"/>-based so the flip logic is testable.
/// </summary>
public class ContractExpiryService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IClock _clock;
    private readonly ILogger<ContractExpiryService> _logger;

    public ContractExpiryService(IServiceProvider services, IClock clock, ILogger<ContractExpiryService> logger)
    {
        _services = services;
        _clock = clock;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExpireContractsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during contract expiry check");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    /// <returns>Number of contracts flipped to <c>expired</c>.</returns>
    public async Task<int> ExpireContractsAsync(CancellationToken ct = default)
    {
        using var scope = _services.CreateScope();
        var sp = scope.ServiceProvider;
        var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
        var notifications = sp.GetRequiredService<INotificationsService>();
        // Test seam: hub + email live in the API/Email layers and are absent from the
        // test fixture — skipped when unregistered, always present in production.
        var hub = sp.GetService<INotificationHubService>();
        var email = sp.GetService<IEmailService>();
        var now = _clock.UtcNow;

        var contractsQuery = await unitOfWork.ContractsRepository.GetQueryable();
        var overdue = await contractsQuery
            .Where(c => !c.IsDeleted
                && c.status == "sent"
                && c.end_date != null
                && c.end_date < now)
            .Include(c => c.user_Entity)
            .Include(c => c.entity_Entity)
            .Include(c => c.job_Entity)
            .ToListAsync(ct);

        // Flip via ExecuteUpdateAsync, NOT Update(): the rows above are AsNoTracking and
        // several can share one user/entity/job — Update() would attach duplicate instances
        // of the same key and throw (same trap as contractsService.SendAsync documents).
        var overdueIds = overdue.Select(c => c.Id).ToList();
        var contractsFlip = await unitOfWork.ContractsRepository.GetQueryable();
        await contractsFlip.Where(c => overdueIds.Contains(c.Id)).ExecuteUpdateAsync(
            c => c.SetProperty(x => x.status, "expired")
                 .SetProperty(x => x.UpdatedAt, now), ct);

        foreach (var contract in overdue)
        {
            var jobTitle = contract.job_Entity?.title ?? "";
            var applicantName = contract.user_Entity?.name ?? "";
            var entityName = contract.entity_Entity?.name ?? "";

            var individualNote = await notifications.CreateAsync(
                contract.user_id,
                "انتهت مهلة العقد",
                $"انتهت مهلة توقيع العقد لوظيفة {jobTitle} من {entityName} دون توقيع",
                "contract_expired",
                contract.Id,
                "contract");

            var entityNote = await notifications.CreateAsync(
                contract.entity_id,
                "انتهت مهلة العقد",
                $"انتهت مهلة توقيع {applicantName} للعقد لوظيفة {jobTitle} دون توقيع",
                "contract_expired",
                contract.Id,
                "contract");

            if (hub is not null)
            {
                await hub.SendToUserAsync(contract.user_id, "ContractExpired", new
                {
                    individualNote.Id,
                    ContractId = contract.Id,
                    JobTitle = jobTitle,
                    EntityName = entityName,
                });
                await hub.SendToUserAsync(contract.entity_id, "ContractExpired", new
                {
                    entityNote.Id,
                    ContractId = contract.Id,
                    JobTitle = jobTitle,
                    UserName = applicantName,
                });
            }

            if (email is not null)
            {
                if (contract.user_Entity?.email is not null)
                    await email.SendAsync(contract.user_Entity.email, "انتهت مهلة توقيع العقد",
                        EmailTemplates.ContractExpired(applicantName, jobTitle, entityName));
                if (contract.entity_Entity?.email is not null)
                    await email.SendAsync(contract.entity_Entity.email, "انتهت مهلة توقيع العقد",
                        EmailTemplates.ContractExpired(entityName, jobTitle, applicantName));
            }
        }

        if (overdue.Count > 0)
            _logger.LogInformation("Expired {Count} contracts past their signing deadline", overdue.Count);

        return overdue.Count;
    }
}
