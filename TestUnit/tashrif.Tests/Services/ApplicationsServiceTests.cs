namespace tashrif.Tests.Services;

public class ApplicationsServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IapplicationsRepository> _appsRepoMock;
    private readonly Mock<IjobsRepository> _jobsRepoMock;
    private readonly Mock<IusersRepository> _usersRepoMock;
    private readonly Mock<Iindividual_profilesRepository> _profilesRepoMock;
    private readonly Mock<IqualificationsRepository> _qualsRepoMock;
    private readonly Mock<Ijob_benefitsRepository> _benefitsRepoMock;
    private readonly Mock<Ijob_conditionsRepository> _conditionsRepoMock;
    private readonly Mock<Ijob_responsibilitiesRepository> _responsibilitiesRepoMock;
    private readonly applicationsService _sut;

    public ApplicationsServiceTests()
    {
        _appsRepoMock = new Mock<IapplicationsRepository>();
        _jobsRepoMock = new Mock<IjobsRepository>();
        _usersRepoMock = new Mock<IusersRepository>();
        _profilesRepoMock = new Mock<Iindividual_profilesRepository>();
        _qualsRepoMock = new Mock<IqualificationsRepository>();
        _benefitsRepoMock = new Mock<Ijob_benefitsRepository>();
        _conditionsRepoMock = new Mock<Ijob_conditionsRepository>();
        _responsibilitiesRepoMock = new Mock<Ijob_responsibilitiesRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _uowMock.SetupGet(u => u.ApplicationsRepository).Returns(_appsRepoMock.Object);
        _uowMock.SetupGet(u => u.JobsRepository).Returns(_jobsRepoMock.Object);
        _uowMock.SetupGet(u => u.UsersRepository).Returns(_usersRepoMock.Object);
        _uowMock.SetupGet(u => u.Individual_profilesRepository).Returns(_profilesRepoMock.Object);
        _uowMock.SetupGet(u => u.QualificationsRepository).Returns(_qualsRepoMock.Object);
        _uowMock.SetupGet(u => u.Job_benefitsRepository).Returns(_benefitsRepoMock.Object);
        _uowMock.SetupGet(u => u.Job_conditionsRepository).Returns(_conditionsRepoMock.Object);
        _uowMock.SetupGet(u => u.Job_responsibilitiesRepository).Returns(_responsibilitiesRepoMock.Object);

        _benefitsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<job_benefits>().AsQueryable().BuildMock());
        _conditionsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<job_conditions>().AsQueryable().BuildMock());
        _responsibilitiesRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<job_responsibilities>().AsQueryable().BuildMock());
        _profilesRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<individual_profiles>().AsQueryable().BuildMock());
        _qualsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { new qualifications { Id = 1, type = "بكالوريوس", user_id = 1 } }.AsQueryable().BuildMock());

        _sut = new applicationsService(_uowMock.Object);
    }

    [Fact]
    public async Task Apply_CreatesApplicationWithNewStatus()
    {
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<applications>().AsQueryable().BuildMock());
        _appsRepoMock.Setup(r => r.AddAsync(It.IsAny<applications>())).Returns(Task.CompletedTask);
        _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { new users { Id = 1, name = "User" } }.AsQueryable().BuildMock());
        _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { new jobs { Id = 1 } }.AsQueryable().BuildMock());
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new ApplyJobDto
        {
            JobId = 1,
            QualificationId = 1,
            Experience = "3 سنوات"
        };

        var result = await _sut.ApplyAsync(dto, 1);

        result.Should().NotBeNull();
        result.Status.Should().Be("new");
        _appsRepoMock.Verify(r => r.AddAsync(It.IsAny<applications>()), Times.Once);
    }

    [Fact]
    public async Task Apply_DuplicateApplication_ThrowsInvalidOperationException()
    {
        var existingApp = new applications { job_id = 1, user_id = 1 };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { existingApp }.AsQueryable().BuildMock());

        var dto = new ApplyJobDto { JobId = 1 };

        var act = () => _sut.ApplyAsync(dto, 1);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*مسبقاً*");
    }

    [Fact]
    public async Task GetAll_IndividualSeesOnlyOwnApplications()
    {
        var apps = new List<applications>
        {
            new() { Id = 1, job_id = 1, user_id = 1, user_Entity = new users { name = "User1", gender = "ذكر" }, job_Entity = new jobs(), status = "new", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, job_id = 2, user_id = 1, user_Entity = new users { name = "User1", gender = "ذكر" }, job_Entity = new jobs(), status = "pending", CreatedAt = DateTime.UtcNow },
            new() { Id = 3, job_id = 3, user_id = 1, user_Entity = new users { name = "User1", gender = "ذكر" }, job_Entity = new jobs(), status = "accepted", CreatedAt = DateTime.UtcNow },
            new() { Id = 4, job_id = 4, user_id = 2, user_Entity = new users { name = "User2", gender = "أنثى" }, job_Entity = new jobs(), status = "new", CreatedAt = DateTime.UtcNow },
            new() { Id = 5, job_id = 5, user_id = 2, user_Entity = new users { name = "User2", gender = "أنثى" }, job_Entity = new jobs(), status = "new", CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();

        _appsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(apps);
        _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<jobs>().AsQueryable().BuildMock());

        var result = await _sut.GetAllAsync(new PaginationDto { Page = 1, Limit = 10 }, 1, "individual", null);

        result.Should().NotBeNull();
        result.Total.Should().Be(3);
    }

    [Fact]
    public async Task GetAll_EntitySeesApplicationsForOwnJobs()
    {
        var apps = new List<applications>
        {
            new() { Id = 1, job_id = 1, user_id = 1, user_Entity = new users { name = "User1" }, job_Entity = new jobs { entity_id = 1 }, status = "new", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, job_id = 2, user_id = 2, user_Entity = new users { name = "User2" }, job_Entity = new jobs { entity_id = 1 }, status = "new", CreatedAt = DateTime.UtcNow },
            new() { Id = 3, job_id = 3, user_id = 3, user_Entity = new users { name = "User3" }, job_Entity = new jobs { entity_id = 2 }, status = "new", CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();

        var entityJobs = new List<jobs>
        {
            new() { Id = 1, entity_id = 1 },
            new() { Id = 2, entity_id = 1 },
        }.AsQueryable().BuildMock();

        _appsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(apps);
        _jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(entityJobs);

        var result = await _sut.GetAllAsync(new PaginationDto { Page = 1, Limit = 10 }, 1, "entity", null);

        result.Total.Should().Be(2);
    }

    [Fact]
    public async Task Delete_EntityOwnerCanDeleteApplication()
    {
        var app = new applications
        {
            Id = 1, job_id = 1, user_id = 2, status = "new"
        };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());
        _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { new jobs { Id = 1, entity_id = 1 } }.AsQueryable().BuildMock());
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteAsync(1, 1);

        app.IsDeleted.Should().BeTrue();
        app.DeletedTime.Should().NotBeNull();
        _appsRepoMock.Verify(r => r.Update(app), Times.Once);
    }

    [Fact]
    public async Task Delete_UnauthorizedUserThrowsException()
    {
        var app = new applications
        {
            Id = 1, job_id = 1, user_id = 2, status = "new"
        };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());
        _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { new jobs { Id = 1, entity_id = 3 } }.AsQueryable().BuildMock());

        var act = () => _sut.DeleteAsync(1, 99);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    // ── BulkActionAsync tests ──────────────────────────────────────────

    [Fact]
    public async Task BulkAction_Shortlist_ValidApps_AllSucceed()
    {
        var apps = new List<applications>
        {
            new() { Id = 10, job_id = 1, user_id = 2, status = "new",
                     user_Entity = new users { name = "A" }, job_Entity = new jobs { entity_id = 1 } },
            new() { Id = 11, job_id = 1, user_id = 3, status = "new",
                     user_Entity = new users { name = "B" }, job_Entity = new jobs { entity_id = 1 } },
        };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(apps.AsQueryable().BuildMock());
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.BulkActionAsync(new List<long> { 10, 11 }, "shortlist", 1);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => r.Success);
        result.Should().OnlyContain(r => r.NewStatus == "shortlisted");
        apps.Should().OnlyContain(a => a.status == "shortlisted");
        // ExecuteUpdateAsync writes directly to DB — SaveChangesAsync is not called
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task BulkAction_Refuse_ValidApps_AllSucceed()
    {
        var apps = new List<applications>
        {
            new() { Id = 10, job_id = 1, user_id = 2, status = "shortlisted",
                     user_Entity = new users { name = "A" }, job_Entity = new jobs { entity_id = 1 } },
        };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(apps.AsQueryable().BuildMock());
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.BulkActionAsync(new List<long> { 10 }, "refuse", 1);

        result.Should().HaveCount(1);
        result.First().Success.Should().BeTrue();
        result.First().NewStatus.Should().Be("refused");
        result.First().OldStatus.Should().Be("shortlisted");
    }

    [Fact]
    public async Task BulkAction_SomeNotFound_ReturnsPartialResults()
    {
        var apps = new List<applications>
        {
            new() { Id = 10, job_id = 1, user_id = 2, status = "new",
                     user_Entity = new users { name = "A" }, job_Entity = new jobs { entity_id = 1 } },
        };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(apps.AsQueryable().BuildMock());
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // IDs: 10 exists, 99 does not
        var result = await _sut.BulkActionAsync(new List<long> { 10, 99 }, "shortlist", 1);

        result.Should().HaveCount(2);
        result.Should().ContainSingle(r => r.ApplicationId == 10 && r.Success);
        result.Should().ContainSingle(r => r.ApplicationId == 99 && !r.Success && r.Error != null);
    }

    [Fact]
    public async Task BulkAction_UnauthorizedEntity_ReturnsPartialResults()
    {
        var apps = new List<applications>
        {
            new() { Id = 10, job_id = 1, user_id = 2, status = "new",
                     user_Entity = new users { name = "A" }, job_Entity = new jobs { entity_id = 1 } },
            new() { Id = 11, job_id = 2, user_id = 3, status = "new",
                     user_Entity = new users { name = "B" }, job_Entity = new jobs { entity_id = 2 } },
        };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(apps.AsQueryable().BuildMock());
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Entity 1 owns job 1, but not job 2
        var result = await _sut.BulkActionAsync(new List<long> { 10, 11 }, "shortlist", 1);

        result.Should().HaveCount(2);
        result.Should().ContainSingle(r => r.ApplicationId == 10 && r.Success);
        result.Should().ContainSingle(r => r.ApplicationId == 11 && !r.Success);
    }

    [Fact]
    public async Task BulkAction_InvalidAction_ThrowsInvalidOperationException()
    {
        var act = () => _sut.BulkActionAsync(new List<long> { 1 }, "invalid_action", 1);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task BulkAction_EmptyIds_ReturnsEmptyList()
    {
        var result = await _sut.BulkActionAsync(new List<long>(), "shortlist", 1);

        result.Should().BeEmpty();
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task BulkAction_AllNotFound_AllFail()
    {
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<applications>().AsQueryable().BuildMock());

        var result = await _sut.BulkActionAsync(new List<long> { 99, 100 }, "shortlist", 1);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => !r.Success);
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task BulkAction_AlreadyInTargetStatus_StillUpdates()
    {
        var apps = new List<applications>
        {
            new() { Id = 10, job_id = 1, user_id = 2, status = "shortlisted",
                     user_Entity = new users { name = "A" }, job_Entity = new jobs { entity_id = 1 } },
        };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(apps.AsQueryable().BuildMock());
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.BulkActionAsync(new List<long> { 10 }, "shortlist", 1);

        result.Should().HaveCount(1);
        result.First().Success.Should().BeTrue();
        result.First().OldStatus.Should().Be("shortlisted");
        result.First().NewStatus.Should().Be("shortlisted");
    }

    [Fact]
    public async Task BulkAction_Restore_RefusedApplicantReturnsToNew()
    {
        var apps = new List<applications>
        {
            new() { Id = 10, job_id = 1, user_id = 2, status = "refused",
                     user_Entity = new users { name = "A" }, job_Entity = new jobs { entity_id = 1 } },
        };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(apps.AsQueryable().BuildMock());
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.BulkActionAsync(new List<long> { 10 }, "restore", 1);

        result.Should().HaveCount(1);
        result.First().Success.Should().BeTrue();
        result.First().OldStatus.Should().Be("refused");
        result.First().NewStatus.Should().Be("new");
    }
}
