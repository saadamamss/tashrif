namespace tashrif.API.Services;

public interface IBackgroundTaskQueue
{
    void Enqueue(Func<IServiceScope, Task> workItem);
    Task<Func<IServiceScope, Task>> DequeueAsync(CancellationToken cancellationToken);
}
