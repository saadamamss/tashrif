using tashrif.Data.DTOs.Admin;

namespace tashrif.Tests.Services;

/// <summary>
/// Pure-mock tests for adminService (same pattern as JobsServiceTests).
/// Mock repos return in-memory queryables via BuildMock(); the TestAsyncQueryProvider
/// (AsyncQueryable.cs) handles ExecuteUpdate by mutating the in-memory entities, so
/// deactivate/activate are fully verifiable without a database.
/// </summary>
public class AdminServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IusersRepository> _usersRepoMock;
    private readonly Mock<IjobsRepository> _jobsRepoMock;
    private readonly Mock<IapplicationsRepository> _appsRepoMock;
    private readonly Mock<IcontractsRepository> _contractsRepoMock;
    private readonly Mock<Iaudit_logsRepository> _auditRepoMock;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly adminService _sut;

    public AdminServiceTests()
    {
        _usersRepoMock = new Mock<IusersRepository>();
        _jobsRepoMock = new Mock<IjobsRepository>();
        _appsRepoMock = new Mock<IapplicationsRepository>();
        _contractsRepoMock = new Mock<IcontractsRepository>();
        _auditRepoMock = new Mock<Iaudit_logsRepository>();
        _fileStorageMock = new Mock<IFileStorageService>();
        _uowMock = new Mock<IUnitOfWork>();

        _uowMock.SetupGet(u => u.UsersRepository).Returns(_usersRepoMock.Object);
        _uowMock.SetupGet(u => u.JobsRepository).Returns(_jobsRepoMock.Object);
        _uowMock.SetupGet(u => u.ApplicationsRepository).Returns(_appsRepoMock.Object);
        _uowMock.SetupGet(u => u.ContractsRepository).Returns(_contractsRepoMock.Object);
        _uowMock.SetupGet(u => u.Audit_logsRepository).Returns(_auditRepoMock.Object);

        _sut = new adminService(_uowMock.Object, _fileStorageMock.Object);
    }

    private static tashrif.Data.users MakeUser(long id, string name, string email, string type,
        bool isDeleted = false)
        => new()
        {
            Id = id,
            national_id = $"{id:D10}",
            name = name,
            email = email,
            phone = "0500000000",
            password_hash = "hash",
            type = type,
            gender = "male",
            nationality = "سعودي",
            avatar_url = "",
            IsDeleted = isDeleted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

    private static tashrif.Data.jobs MakeJob(long id, long entityId, string title, string status)
        => new()
        {
            Id = id,
            title = title,
            description = "وصف الوظيفة",
            location = "الرياض",
            work_type = "full_time",
            vacancies = 1,
            status = status,
            entity_id = entityId,
            entity_Entity = MakeUser(entityId, "شركة", $"entity{id}@test.com", "entity"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

    private void SetupUsers(params tashrif.Data.users[] users)
        => _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(users.AsQueryable().BuildMock());

    private void SetupJobs(params tashrif.Data.jobs[] jobs)
        => _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(jobs.AsQueryable().BuildMock());

    private void SetupAuditLogs(params tashrif.Data.audit_logs[] logs)
        => _auditRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(logs.AsQueryable().BuildMock());

    [Fact]
    public async Task UpdateProfileAsync_UpdatesAllFields_AndReturnsUserDto()
    {
        var admin = MakeUser(1, "مدير", "admin@test.com", "admin");
        users? saved = null;
        // GetByIdAsync takes `object id` — the service passes a long, so use 1L (int 1 would not match)
        _usersRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(admin);
        _usersRepoMock.Setup(r => r.Update(It.IsAny<users>())).Callback<users>(u => saved = u);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        // no other user has this email
        _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<users>().AsQueryable().BuildMock());

        var result = await _sut.UpdateProfileAsync(1, new UpdateAdminProfileDto
        {
            Name = "مدير جديد",
            Email = "newadmin@test.com",
            Phone = "0512345678",
            Gender = "male",
            Nationality = "سعودي",
        });

        result.Should().NotBeNull();
        result.Name.Should().Be("مدير جديد");
        result.Email.Should().Be("newadmin@test.com");
        result.Phone.Should().Be("0512345678");
        result.Gender.Should().Be("male");
        result.Nationality.Should().Be("سعودي");
        admin.name.Should().Be("مدير جديد");
        admin.email.Should().Be("newadmin@test.com");
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_EmailTakenByOtherUser_Throws()
    {
        var admin = MakeUser(1, "مدير", "admin@test.com", "admin");
        var other = MakeUser(2, "آخر", "taken@test.com", "individual");
        _usersRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(admin);
        _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { other }.AsQueryable().BuildMock());

        var act = () => _sut.UpdateProfileAsync(1, new UpdateAdminProfileDto
        {
            Name = "مدير",
            Email = "taken@test.com",
        });

        await act.Should().ThrowAsync<BadHttpRequestException>();
        admin.email.Should().Be("admin@test.com"); // unchanged
    }

    [Fact]
    public async Task UpdateProfileAsync_SameEmailForSelf_Allows()
    {
        var admin = MakeUser(1, "مدير", "admin@test.com", "admin");
        _usersRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(admin);
        _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { admin }.AsQueryable().BuildMock());
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.UpdateProfileAsync(1, new UpdateAdminProfileDto
        {
            Name = "مدير",
            Email = "admin@test.com", // own email — must be allowed
        });

        result.Email.Should().Be("admin@test.com");
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_NonExistingAdmin_Throws()
    {
        _usersRepoMock.Setup(r => r.GetByIdAsync(999L)).ReturnsAsync((users?)null);

        var act = () => _sut.UpdateProfileAsync(999, new UpdateAdminProfileDto
        {
            Name = "مدير",
            Email = "admin@test.com",
        });

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAvatarAsync_SetsAvatarUrl()
    {
        var admin = MakeUser(1, "مدير", "admin@test.com", "admin");
        _usersRepoMock.Setup(r => r.GetByIdAsync(1L)).ReturnsAsync(admin);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.UpdateAvatarAsync(1, "/uploads/avatars/abc.jpg");

        result.AvatarUrl.Should().Be("/uploads/avatars/abc.jpg");
        admin.avatar_url.Should().Be("/uploads/avatars/abc.jpg");
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAvatarAsync_EmptyUrl_Throws()
    {
        var act = () => _sut.UpdateAvatarAsync(1, " ");

        await act.Should().ThrowAsync<BadHttpRequestException>();
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsCorrectCounts()
    {
        var entity = MakeUser(1, "شركة", "company@test.com", "entity");
        var individual = MakeUser(2, "أحمد", "ahmed@test.com", "individual");
        var admin = MakeUser(3, "مدير", "admin@test.com", "admin");

        SetupUsers(entity, individual, admin);
        SetupJobs(MakeJob(10, 1, "وظيفة نشطة", "active"), MakeJob(11, 1, "وظيفة منتهية", "expired"));
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { new tashrif.Data.applications { Id = 20, job_id = 10, user_id = 2, status = "new" } }
                .AsQueryable().BuildMock());
        _contractsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<tashrif.Data.contracts>().AsQueryable().BuildMock());

        var result = await _sut.GetStatsAsync();

        result.TotalUsers.Should().Be(3);
        result.TotalIndividuals.Should().Be(1);
        result.TotalEntities.Should().Be(1);
        result.TotalAdmins.Should().Be(1);
        result.TotalJobs.Should().Be(2);
        result.ActiveJobs.Should().Be(1);
        result.TotalApplications.Should().Be(1);
        result.TotalContracts.Should().Be(0);
    }

    [Fact]
    public async Task GetStatsAsync_EmptyDatabase_ReturnsZeros()
    {
        SetupUsers();
        SetupJobs();
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<tashrif.Data.applications>().AsQueryable().BuildMock());
        _contractsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<tashrif.Data.contracts>().AsQueryable().BuildMock());

        var result = await _sut.GetStatsAsync();

        result.TotalUsers.Should().Be(0);
        result.TotalJobs.Should().Be(0);
        result.TotalApplications.Should().Be(0);
        result.TotalContracts.Should().Be(0);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsPaginatedResults()
    {
        var users = Enumerable.Range(1, 5)
            .Select(i => MakeUser(i, $"مستخدم {i}", $"user{i}@test.com", "individual"))
            .ToArray();
        SetupUsers(users);

        var result = await _sut.GetUsersAsync(new AdminUserFilterDto { Page = 1, Limit = 2 });

        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(5);
        result.Page.Should().Be(1);
        result.Limit.Should().Be(2);
        result.TotalPages.Should().Be(3);

        var page2 = await _sut.GetUsersAsync(new AdminUserFilterDto { Page = 2, Limit = 2 });
        page2.Items.Should().HaveCount(2);
        page2.Items.Should().NotContain(u => u.Id == result.Items[0].Id);
    }

    [Fact]
    public async Task GetUsersAsync_FiltersByType()
    {
        SetupUsers(
            MakeUser(1, "أحمد", "ahmed@test.com", "individual"),
            MakeUser(2, "شركة", "company@test.com", "entity"));

        var result = await _sut.GetUsersAsync(new AdminUserFilterDto { Type = "individual", Limit = 50 });

        result.Items.Should().OnlyContain(u => u.Type == "individual");
        result.Items.Should().Contain(u => u.Email == "ahmed@test.com");
    }

    [Fact]
    public async Task GetUsersAsync_SearchesByName()
    {
        SetupUsers(
            MakeUser(1, "أحمد محمد", "ahmed@test.com", "individual"),
            MakeUser(2, "خالد", "khaled@test.com", "individual"));

        var result = await _sut.GetUsersAsync(new AdminUserFilterDto { Search = "أحمد", Limit = 50 });

        result.Items.Should().ContainSingle(u => u.Email == "ahmed@test.com");
        result.Items.Should().NotContain(u => u.Email == "khaled@test.com");
    }

    [Fact]
    public async Task GetUsersAsync_SearchMatchesNationalId()
    {
        SetupUsers(
            MakeUser(1, "أحمد", "ahmed@test.com", "individual"),
            MakeUser(2, "خالد", "khaled@test.com", "individual"));

        var result = await _sut.GetUsersAsync(new AdminUserFilterDto { Search = "0000000002", Limit = 50 });

        result.Items.Should().ContainSingle(u => u.Id == 2);
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
    {
        SetupUsers(MakeUser(1, "أحمد", "ahmed@test.com", "individual"));

        var result = await _sut.GetUserByIdAsync(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("أحمد");
        result.Email.Should().Be("ahmed@test.com");
        result.Type.Should().Be("individual");
    }

    [Fact]
    public async Task GetUserByIdAsync_NonExistingUser_ReturnsNull()
    {
        SetupUsers();

        var result = await _sut.GetUserByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeactivateUserAsync_SetsIsDeleted()
    {
        var user = MakeUser(1, "أحمد", "ahmed@test.com", "individual");
        SetupUsers(user);

        await _sut.DeactivateUserAsync(id: 1, adminId: 99);

        user.IsDeleted.Should().BeTrue();
        user.DeletedTime.Should().NotBeNull();
    }

    [Fact]
    public async Task DeactivateUserAsync_AdminCannotDeactivateSelf_Throws()
    {
        var admin = MakeUser(1, "مدير", "admin@test.com", "admin");
        SetupUsers(admin);

        var act = () => _sut.DeactivateUserAsync(id: 1, adminId: 1);

        await act.Should().ThrowAsync<BadHttpRequestException>();
        admin.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task DeactivateUserAsync_NonExistingUser_Throws()
    {
        SetupUsers();

        var act = () => _sut.DeactivateUserAsync(id: 999, adminId: 1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ActivateUserAsync_ClearsIsDeleted()
    {
        var user = MakeUser(1, "أحمد", "ahmed@test.com", "individual", isDeleted: true);
        user.DeletedTime = DateTime.UtcNow;
        SetupUsers(user);

        await _sut.ActivateUserAsync(1);

        user.IsDeleted.Should().BeFalse();
        user.DeletedTime.Should().BeNull();
    }

    [Fact]
    public async Task GetUsersAsync_DeactivatedUsersVisible_AllAndFiltered()
    {
        var active = MakeUser(1, "أحمد", "ahmed@test.com", "individual");
        var deactivated = MakeUser(2, "سعيد", "saeed@test.com", "individual", isDeleted: true);
        SetupUsers(active, deactivated);

        // D10: admin list uses IgnoreQueryFilters — deactivated users are visible
        var all = await _sut.GetUsersAsync(new AdminUserFilterDto { Limit = 50 });
        all.Items.Should().HaveCount(2);

        var onlyActive = await _sut.GetUsersAsync(new AdminUserFilterDto { Status = "active", Limit = 50 });
        onlyActive.Items.Should().ContainSingle(u => u.Email == "ahmed@test.com");

        var onlyDeactivated = await _sut.GetUsersAsync(new AdminUserFilterDto { Status = "deactivated", Limit = 50 });
        onlyDeactivated.Items.Should().ContainSingle(u => u.Email == "saeed@test.com");
    }

    [Fact]
    public async Task GetAuditLogsAsync_ReturnsPaginatedResultsWithUserNames()
    {
        var user = MakeUser(5, "أحمد", "ahmed@test.com", "individual");
        SetupUsers(user);
        SetupAuditLogs(
            new tashrif.Data.audit_logs
            {
                Id = 1, user_id = 5, action = "status_change", entity_type = "applications",
                entity_id = 42, old_value = "new", new_value = "shortlisted",
                ip_address = "192.168.1.1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
            },
            new tashrif.Data.audit_logs
            {
                Id = 2, user_id = 5, action = "create", entity_type = "jobs",
                entity_id = 7, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
            });

        var result = await _sut.GetAuditLogsAsync(new AdminAuditLogFilterDto { Page = 1, Limit = 1 });

        result.Total.Should().Be(2);
        result.Items.Should().HaveCount(1);
        result.Items[0].UserName.Should().Be("أحمد");
    }

    [Fact]
    public async Task GetAuditLogsAsync_FiltersByUserId()
    {
        SetupUsers(
            MakeUser(1, "أحمد", "ahmed@test.com", "individual"),
            MakeUser(2, "شركة", "company@test.com", "entity"));
        SetupAuditLogs(
            new tashrif.Data.audit_logs
            {
                Id = 1, user_id = 1, action = "status_change", entity_type = "applications",
                entity_id = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
            },
            new tashrif.Data.audit_logs
            {
                Id = 2, user_id = 2, action = "status_change", entity_type = "applications",
                entity_id = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
            });

        var result = await _sut.GetAuditLogsAsync(new AdminAuditLogFilterDto { UserId = 1, Limit = 50 });

        result.Items.Should().OnlyContain(l => l.UserId == 1);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllJobsAsync_ReturnsAllStatuses()
    {
        SetupJobs(
            MakeJob(1, 10, "وظيفة نشطة", "active"),
            MakeJob(2, 10, "وظيفة منتهية", "expired"));

        var result = await _sut.GetAllJobsAsync(
            new tashrif.Data.Models.PaginationDto { Page = 1, Limit = 50 }, null, null);

        result.Total.Should().Be(2);
        result.Items.Should().Contain(j => j.Status == "active");
        result.Items.Should().Contain(j => j.Status == "expired");
        result.Items.First(j => j.Status == "active").EntityName.Should().Be("شركة");
    }

    [Fact]
    public async Task GetAllJobsAsync_FiltersByStatusAndSearch()
    {
        SetupJobs(
            MakeJob(1, 10, "حراسة المعرض", "active"),
            MakeJob(2, 10, "وظيفة أخرى", "expired"));

        var result = await _sut.GetAllJobsAsync(
            new tashrif.Data.Models.PaginationDto { Page = 1, Limit = 50 }, "active", "حراسة");

        result.Total.Should().Be(1);
        result.Items.Should().ContainSingle(j => j.Id == 1);
    }

    [Fact]
    public async Task DeactivateJobAsync_SetsStatusClosed()
    {
        var job = MakeJob(1, 10, "وظيفة", "active");
        SetupJobs(job);

        await _sut.DeactivateJobAsync(1);

        job.status.Should().Be("closed");
    }

    [Fact]
    public async Task DeactivateJobAsync_NonExistingJob_Throws()
    {
        SetupJobs();

        var act = () => _sut.DeactivateJobAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
