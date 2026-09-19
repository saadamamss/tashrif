using tashrif.Data.DTOs.Analytics;

namespace tashrif.Data.Interfaces;

public interface IanalyticsService
{
    Task<EntityAnalyticsDto> GetEntityAnalyticsAsync(long entityId);
}
