using System.Threading.Channels;

namespace tashrif.API.Services;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<IServiceScope, Task>> _queue;

    public BackgroundTaskQueue(int capacity = 100)
    {
        _queue = Channel.CreateBounded<Func<IServiceScope, Task>>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
        });
    }

    public void Enqueue(Func<IServiceScope, Task> workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);

        if (!_queue.Writer.TryWrite(workItem))
        {
            throw new InvalidOperationException("Failed to enqueue background task — queue is full.");
        }
    }

    public async Task<Func<IServiceScope, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}
