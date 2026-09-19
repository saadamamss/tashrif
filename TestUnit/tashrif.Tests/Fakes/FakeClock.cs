using tashrif.Data.Interfaces;

namespace tashrif.Tests.Fakes;

public sealed class FakeClock : IClock
{
    public DateTime UtcNow { get; set; }

    public FakeClock(DateTime utcNow) => UtcNow = utcNow;
    public FakeClock() => UtcNow = new DateTime(2026, 9, 19, 12, 0, 0, DateTimeKind.Utc);
}
