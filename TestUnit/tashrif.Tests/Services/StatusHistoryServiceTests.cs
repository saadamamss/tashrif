namespace tashrif.Tests.Services;

public class StatusHistoryServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<Iapplication_status_historyRepository> _historyRepoMock;
    private readonly Mock<IapplicationsRepository> _appsRepoMock;
    private readonly Mock<IusersRepository> _usersRepoMock;
    private readonly statusHistoryService _sut;

    public StatusHistoryServiceTests()
    {
        _historyRepoMock = new Mock<Iapplication_status_historyRepository>();
        _appsRepoMock = new Mock<IapplicationsRepository>();
        _usersRepoMock = new Mock<IusersRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _uowMock.SetupGet(u => u.Application_status_historyRepository).Returns(_historyRepoMock.Object);
        _uowMock.SetupGet(u => u.ApplicationsRepository).Returns(_appsRepoMock.Object);
        _uowMock.SetupGet(u => u.UsersRepository).Returns(_usersRepoMock.Object);

        _historyRepoMock.Setup(r => r.AddAsync(It.IsAny<application_status_history>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _sut = new statusHistoryService(_uowMock.Object);
    }

    [Fact]
    public async Task RecordAsync_CreatesHistoryEntry()
    {
        application_status_history? captured = null;
        _historyRepoMock.Setup(r => r.AddAsync(It.IsAny<application_status_history>()))
            .Callback<application_status_history>(h => captured = h)
            .Returns(Task.CompletedTask);

        await _sut.RecordAsync(42, null, "new", 9);

        captured.Should().NotBeNull();
        captured!.application_id.Should().Be(42);
        captured.old_status.Should().BeNull();
        captured.new_status.Should().Be("new");
        captured.changed_by.Should().Be(9);
        captured.changed_at.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetHistoryAsync_OwnApplication_ReturnsHistory()
    {
        var app = new applications { Id = 42, user_id = 9, job_id = 1, status = "shortlisted", job_Entity = new jobs { entity_id = 3 } };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());

        var users = new[] { new users { Id = 9, name = "أحمد" }, new users { Id = 3, name = "شركة" } };
        _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(users.AsQueryable().BuildMock());

        var history = new[]
        {
            new application_status_history { Id = 1, application_id = 42, old_status = null, new_status = "new", changed_by = 9, changed_at = new DateTime(2026, 9, 15, 10, 0, 0, DateTimeKind.Utc) },
            new application_status_history { Id = 2, application_id = 42, old_status = "new", new_status = "shortlisted", changed_by = 3, changed_at = new DateTime(2026, 9, 16, 14, 0, 0, DateTimeKind.Utc) },
        };
        _historyRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(history.AsQueryable().BuildMock());

        var result = await _sut.GetHistoryAsync(42, 9);

        result.Should().HaveCount(2);
        result[0].NewStatus.Should().Be("new");
        result[0].ChangedByName.Should().Be("أحمد");
        result[1].NewStatus.Should().Be("shortlisted");
        result[1].ChangedByName.Should().Be("شركة");
    }

    [Fact]
    public async Task GetHistoryAsync_EntityOwnsJob_ReturnsHistory()
    {
        var app = new applications { Id = 42, user_id = 9, job_id = 1, status = "new", job_Entity = new jobs { entity_id = 3 } };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());

        var users = new[] { new users { Id = 9, name = "أحمد" } };
        _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(users.AsQueryable().BuildMock());

        var history = new[]
        {
            new application_status_history { Id = 1, application_id = 42, old_status = null, new_status = "new", changed_by = 9, changed_at = DateTime.UtcNow },
        };
        _historyRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(history.AsQueryable().BuildMock());

        var result = await _sut.GetHistoryAsync(42, 3);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetHistoryAsync_UnauthorizedUser_Throws()
    {
        var app = new applications { Id = 42, user_id = 9, job_id = 1, status = "new", job_Entity = new jobs { entity_id = 3 } };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());

        await _sut.Invoking(s => s.GetHistoryAsync(42, 999))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GetHistoryAsync_ReturnsOrderedByDate()
    {
        var app = new applications { Id = 42, user_id = 9, job_id = 1, status = "new", job_Entity = new jobs { entity_id = 3 } };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());

        _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { new users { Id = 9, name = "أحمد" } }.AsQueryable().BuildMock());

        var history = new[]
        {
            new application_status_history { Id = 2, application_id = 42, old_status = "new", new_status = "shortlisted", changed_by = 9, changed_at = new DateTime(2026, 9, 16, 14, 0, 0, DateTimeKind.Utc) },
            new application_status_history { Id = 1, application_id = 42, old_status = null, new_status = "new", changed_by = 9, changed_at = new DateTime(2026, 9, 15, 10, 0, 0, DateTimeKind.Utc) },
        };
        _historyRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(history.AsQueryable().BuildMock());

        var result = await _sut.GetHistoryAsync(42, 9);

        result.Should().HaveCount(2);
        result[0].NewStatus.Should().Be("new");
        result[1].NewStatus.Should().Be("shortlisted");
    }
}
