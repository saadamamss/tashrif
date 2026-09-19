using tashrif.Data.Interfaces;

namespace tashrif.Core;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
