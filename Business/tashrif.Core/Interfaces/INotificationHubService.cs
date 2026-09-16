namespace tashrif.Core;

public interface INotificationHubService
{
    Task SendToUserAsync(long userId, string method, object data);
}
