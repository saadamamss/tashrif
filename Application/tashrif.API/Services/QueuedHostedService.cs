namespace tashrif.API.Services;

public class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<QueuedHostedService> _logger;

    public QueuedHostedService(
        IBackgroundTaskQueue taskQueue,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<QueuedHostedService> logger)
    {
        _taskQueue = taskQueue;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("QueuedHostedService is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                using var scope = _serviceScopeFactory.CreateScope();
                await workItem(scope);
            }
            catch (OperationCanceledException)
            {
                // Expected during shutdown — stop processing.
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing background work item.");
            }
        }

        _logger.LogInformation("QueuedHostedService is stopping.");
    }
}
