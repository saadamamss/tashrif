using Microsoft.AspNetCore.SignalR;
using tashrif.API.Hubs;
using tashrif.Core;

namespace tashrif.API.Services;

public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationHubService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(long userId, string method, object data)
    {
        await _hubContext.Clients
            .Group($"user:{userId}")
            .SendAsync(method, data);
    }
}
