namespace tashrif.Tests.Services;

public class AnalyticsServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IjobsRepository> _jobsRepoMock;
    private readonly Mock<IapplicationsRepository> _appsRepoMock;
    private readonly Mock<Iapplication_status_historyRepository> _historyRepoMock;
    private readonly Mock<IusersRepository> _usersRepoMock;
    private readonly analyticsService _sut;

    public AnalyticsServiceTests()
    {
        _jobsRepoMock = new Mock<IjobsRepository>();
        _appsRepoMock = new Mock<IapplicationsRepository>();
        _historyRepoMock = new Mock<Iapplication_status_historyRepository>();
        _usersRepoMock = new Mock<IusersRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _uowMock.SetupGet(u => u.JobsRepository).Returns(_jobsRepoMock.Object);
        _uowMock.SetupGet(u => u.ApplicationsRepository).Returns(_appsRepoMock.Object);
        _uowMock.SetupGet(u => u.Application_status_historyRepository).Returns(_historyRepoMock.Object);
        _uowMock.SetupGet(u => u.UsersRepository).Returns(_usersRepoMock.Object);

        _sut = new analyticsService(_uowMock.Object);
    }

    private void SetupJobs(params jobs[] data)
        => _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(data.AsQueryable().BuildMock());

    private void SetupApplications(params applications[] data)
        => _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(data.AsQueryable().BuildMock());

    private void SetupHistory(params application_status_history[] data)
        => _historyRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(data.AsQueryable().BuildMock());

    private void SetupUsers(params users[] data)
        => _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(data.AsQueryable().BuildMock());

    // ---- 1. Returns correct counts ----

    [Fact]
    public async Task GetEntityAnalyticsAsync_ReturnsCorrectCounts()
    {
        SetupJobs(
            new jobs { Id = 1, entity_id = 10, status = "active" },
            new jobs { Id = 2, entity_id = 10, status = "active" },
            new jobs { Id = 3, entity_id = 10, status = "closed" },
            new jobs { Id = 4, entity_id = 99, status = "active" } // other entity
        );
        SetupApplications(
            new applications { Id = 1, job_id = 1, user_id = 100, status = "new" },
            new applications { Id = 2, job_id = 2, user_id = 101, status = "accepted" },
            new applications { Id = 3, job_id = 4, user_id = 102, status = "new" } // other entity
        );
        SetupHistory();
        SetupUsers(
            new users { Id = 100, gender = "male", nationality = "سعودي" },
            new users { Id = 101, gender = "female", nationality = "مصري" }
        );

        var result = await _sut.GetEntityAnalyticsAsync(10);

        result.TotalJobs.Should().Be(3);
        result.ActiveJobs.Should().Be(2);
        result.TotalApplications.Should().Be(2);
    }

    // ---- 2. Conversion rate ----

    [Fact]
    public async Task GetEntityAnalyticsAsync_CalculatesConversionRate()
    {
        SetupJobs(new jobs { Id = 1, entity_id = 10, status = "active" });
        SetupApplications(
            new applications { Id = 1, job_id = 1, user_id = 100, status = "new" },
            new applications { Id = 2, job_id = 1, user_id = 101, status = "accepted" },
            new applications { Id = 3, job_id = 1, user_id = 102, status = "new" },
            new applications { Id = 4, job_id = 1, user_id = 103, status = "accepted" },
            new applications { Id = 5, job_id = 1, user_id = 104, status = "shortlisted" },
            new applications { Id = 6, job_id = 1, user_id = 105, status = "new" },
            new applications { Id = 7, job_id = 1, user_id = 106, status = "new" },
            new applications { Id = 8, job_id = 1, user_id = 107, status = "new" },
            new applications { Id = 9, job_id = 1, user_id = 108, status = "new" },
            new applications { Id = 10, job_id = 1, user_id = 109, status = "new" }
        );
        SetupHistory();
        SetupUsers(Enumerable.Range(100, 10).Select(i => new users { Id = i, gender = "male", nationality = "سعودي" }).ToArray());

        var result = await _sut.GetEntityAnalyticsAsync(10);

        result.ConversionRate.Should().Be(20.0); // 2 / 10 * 100
    }

    [Fact]
    public async Task GetEntityAnalyticsAsync_ZeroApplications_ConversionRateIsZero()
    {
        SetupJobs(new jobs { Id = 1, entity_id = 10, status = "active" });
        SetupApplications();
        SetupHistory();
        SetupUsers();

        var result = await _sut.GetEntityAnalyticsAsync(10);

        result.ConversionRate.Should().Be(0);
        result.TotalApplications.Should().Be(0);
    }

    // ---- 3. Groups by status (all 7 present) ----

    [Fact]
    public async Task GetEntityAnalyticsAsync_GroupsByStatus_AllSevenPresent()
    {
        SetupJobs(new jobs { Id = 1, entity_id = 10, status = "active" });
        SetupApplications(
            new applications { Id = 1, job_id = 1, user_id = 100, status = "new" },
            new applications { Id = 2, job_id = 1, user_id = 101, status = "accepted" }
        );
        SetupHistory();
        SetupUsers(
            new users { Id = 100, gender = "male", nationality = "سعودي" },
            new users { Id = 101, gender = "female", nationality = "مصري" }
        );

        var result = await _sut.GetEntityAnalyticsAsync(10);

        result.ApplicationsByStatus.Should().ContainKeys("new", "shortlisted", "interview", "contract_sent", "accepted", "refused", "withdrawn");
        result.ApplicationsByStatus["new"].Should().Be(1);
        result.ApplicationsByStatus["accepted"].Should().Be(1);
        result.ApplicationsByStatus["shortlisted"].Should().Be(0);
        result.ApplicationsByStatus["interview"].Should().Be(0);
        result.ApplicationsByStatus["refused"].Should().Be(0);
    }

    // ---- 4. Top jobs ----

    [Fact]
    public async Task GetEntityAnalyticsAsync_ReturnsTopJobs_SortedDesc()
    {
        SetupJobs(
            new jobs { Id = 1, entity_id = 10, status = "active", title = "حارس أمن" },
            new jobs { Id = 2, entity_id = 10, status = "active", title = "سائق" },
            new jobs { Id = 3, entity_id = 10, status = "active", title = "طباخ" }
        );
        SetupApplications(
            new applications { Id = 1, job_id = 1, user_id = 100, status = "new" },
            new applications { Id = 2, job_id = 1, user_id = 101, status = "accepted" },
            new applications { Id = 3, job_id = 1, user_id = 102, status = "new" },
            new applications { Id = 4, job_id = 2, user_id = 103, status = "new" },
            new applications { Id = 5, job_id = 3, user_id = 104, status = "new" },
            new applications { Id = 6, job_id = 3, user_id = 105, status = "new" }
        );
        SetupHistory();
        SetupUsers(Enumerable.Range(100, 6).Select(i => new users { Id = i, gender = "male", nationality = "سعودي" }).ToArray());

        var result = await _sut.GetEntityAnalyticsAsync(10);

        result.TopJobs.Should().HaveCount(3);
        result.TopJobs[0].JobId.Should().Be(1); // 3 apps
        result.TopJobs[0].ApplicationCount.Should().Be(3);
        result.TopJobs[0].HiredCount.Should().Be(1);
        result.TopJobs[1].JobId.Should().Be(3); // 2 apps
        result.TopJobs[2].JobId.Should().Be(2); // 1 app
    }

    // ---- 5. Time-to-hire calculation ----

    [Fact]
    public async Task GetEntityAnalyticsAsync_CalculatesTimeToHire()
    {
        var createdAt1 = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc);
        var createdAt2 = new DateTime(2026, 9, 5, 10, 0, 0, DateTimeKind.Utc);

        SetupJobs(new jobs { Id = 1, entity_id = 10, status = "active", title = "حارس" });
        SetupApplications(
            new applications { Id = 1, job_id = 1, user_id = 100, status = "accepted", CreatedAt = createdAt1 },
            new applications { Id = 2, job_id = 1, user_id = 101, status = "accepted", CreatedAt = createdAt2 }
        );
        SetupHistory(
            new application_status_history { Id = 1, application_id = 1, old_status = "contract_sent", new_status = "accepted", changed_by = 10, changed_at = new DateTime(2026, 9, 11, 10, 0, 0, DateTimeKind.Utc) }, // 10 days
            new application_status_history { Id = 2, application_id = 2, old_status = "interview", new_status = "accepted", changed_by = 10, changed_at = new DateTime(2026, 9, 15, 10, 0, 0, DateTimeKind.Utc) }  // 10 days
        );
        SetupUsers(
            new users { Id = 100, gender = "male", nationality = "سعودي" },
            new users { Id = 101, gender = "female", nationality = "مصري" }
        );

        var result = await _sut.GetEntityAnalyticsAsync(10);

        result.AverageTimeToHireDays.Should().Be(10.0);
    }

    // ---- 6. No accepted apps → null ----

    [Fact]
    public async Task GetEntityAnalyticsAsync_NoAcceptedApps_TimeToHireIsNull()
    {
        SetupJobs(new jobs { Id = 1, entity_id = 10, status = "active", title = "حارس" });
        SetupApplications(
            new applications { Id = 1, job_id = 1, user_id = 100, status = "new" },
            new applications { Id = 2, job_id = 1, user_id = 101, status = "shortlisted" }
        );
        SetupHistory();
        SetupUsers(
            new users { Id = 100, gender = "male", nationality = "سعودي" },
            new users { Id = 101, gender = "female", nationality = "مصري" }
        );

        var result = await _sut.GetEntityAnalyticsAsync(10);

        result.AverageTimeToHireDays.Should().BeNull();
    }

    // ---- 7. Continuous 30-day series ----

    [Fact]
    public async Task GetEntityAnalyticsAsync_FillsMissingDays_Always30Entries()
    {
        SetupJobs(new jobs { Id = 1, entity_id = 10, status = "active", title = "حارس" });
        SetupApplications(
            new applications { Id = 1, job_id = 1, user_id = 100, status = "new", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new applications { Id = 2, job_id = 1, user_id = 101, status = "new", CreatedAt = DateTime.UtcNow.AddDays(-1) }
        );
        SetupHistory();
        SetupUsers(
            new users { Id = 100, gender = "male", nationality = "سعودي" },
            new users { Id = 101, gender = "female", nationality = "مصري" }
        );

        var result = await _sut.GetEntityAnalyticsAsync(10);

        result.ApplicationsOverTime.Should().HaveCount(30);
        result.ApplicationsOverTime.Count(d => d.Count > 0).Should().BeGreaterThan(0);
        result.ApplicationsOverTime.Count(d => d.Count == 0).Should().BeGreaterThan(0);
    }
}
