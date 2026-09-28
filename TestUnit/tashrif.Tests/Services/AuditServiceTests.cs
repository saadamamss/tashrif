using Moq;
using tashrif.Data.Constants;
using tashrif.Core.Services;
using tashrif.Tests.Fakes;

namespace tashrif.Tests.Services;

// Spec 04 (phase3-5-hardening): the audit WRITER. Pure mock — no database.
public class AuditServiceTests
{
    private readonly Mock<IGenericRepository<audit_logs>> _auditRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly FakeClock _clock = new(); // frozen at 2026-09-19 12:00 UTC
    private readonly AuditService _sut;

    public AuditServiceTests()
    {
        _auditRepoMock.Setup(r => r.AddAsync(It.IsAny<audit_logs>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _sut = new AuditService(_auditRepoMock.Object, _uowMock.Object, _clock);
    }

    [Fact]
    public async Task LogAsync_PersistsAllFields()
    {
        await _sut.LogAsync(9, AuditActions.AdminUserDeactivated, "users", 3,
            "active", "deactivated", "127.0.0.1", "test-agent");

        _auditRepoMock.Verify(r => r.AddAsync(It.Is<audit_logs>(l =>
            l.user_id == 9 &&
            l.action == "admin.user_deactivated" &&
            l.entity_type == "users" &&
            l.entity_id == 3 &&
            l.old_value == "active" &&
            l.new_value == "deactivated" &&
            l.ip_address == "127.0.0.1" &&
            l.user_agent == "test-agent"
        )), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task LogAsync_TruncatesLongValues()
    {
        var longValue = new string('x', 5000);

        await _sut.LogAsync(9, AuditActions.AdminProfileUpdated, "users", 9, longValue, longValue);

        _auditRepoMock.Verify(r => r.AddAsync(It.Is<audit_logs>(l =>
            l.old_value!.Length == 1000 && l.new_value!.Length == 1000
        )), Times.Once);
    }

    [Fact]
    public async Task LogAsync_NullOptionalFields_StillPersists()
    {
        await _sut.LogAsync(9, AuditActions.AuthLogin, "users", 9);

        _auditRepoMock.Verify(r => r.AddAsync(It.Is<audit_logs>(l =>
            l.old_value == null && l.new_value == null &&
            l.ip_address == null && l.user_agent == null &&
            l.CreatedAt == _clock.UtcNow && l.UpdatedAt == _clock.UtcNow
        )), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task LogAsync_UsesUtcTimestamps()
    {
        await _sut.LogAsync(9, AuditActions.AuthLogin, "users", 9);

        // Level-Up 3.4: IClock is the sole time source — never DateTime.UtcNow.
        _auditRepoMock.Verify(r => r.AddAsync(It.Is<audit_logs>(l =>
            l.CreatedAt == _clock.UtcNow
        )), Times.Once);
    }

    [Fact]
    public async Task LogAsync_InvalidEntityType_ThrowsLoudly()
    {
        // Silent bad data in an audit trail is the worst outcome — the UI can never filter it.
        var act = () => _sut.LogAsync(9, AuditActions.AuthLogin, "user", 9); // singular — invalid

        await act.Should().ThrowAsync<InvalidOperationException>();
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
