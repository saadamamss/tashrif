namespace tashrif.Tests.Services;

public class JobsServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IjobsRepository> _jobsRepoMock;
    private readonly Mock<IusersRepository> _usersRepoMock;
    private readonly Mock<Ijob_benefitsRepository> _benefitsRepoMock;
    private readonly Mock<Ijob_conditionsRepository> _conditionsRepoMock;
    private readonly Mock<Ijob_responsibilitiesRepository> _responsibilitiesRepoMock;
    private readonly Mock<IapplicationsRepository> _appsRepoMock;
    private readonly Mock<Iindividual_profilesRepository> _profilesRepoMock;
    private readonly Mock<IcvsRepository> _cvsRepoMock;
    private readonly jobsService _sut;

    public JobsServiceTests()
    {
        _jobsRepoMock = new Mock<IjobsRepository>();
        _usersRepoMock = new Mock<IusersRepository>();
        _benefitsRepoMock = new Mock<Ijob_benefitsRepository>();
        _conditionsRepoMock = new Mock<Ijob_conditionsRepository>();
        _responsibilitiesRepoMock = new Mock<Ijob_responsibilitiesRepository>();
        _appsRepoMock = new Mock<IapplicationsRepository>();
        _profilesRepoMock = new Mock<Iindividual_profilesRepository>();
        _cvsRepoMock = new Mock<IcvsRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _uowMock.SetupGet(u => u.JobsRepository).Returns(_jobsRepoMock.Object);
        _uowMock.SetupGet(u => u.UsersRepository).Returns(_usersRepoMock.Object);
        _uowMock.SetupGet(u => u.Job_benefitsRepository).Returns(_benefitsRepoMock.Object);
        _uowMock.SetupGet(u => u.Job_conditionsRepository).Returns(_conditionsRepoMock.Object);
        _uowMock.SetupGet(u => u.Job_responsibilitiesRepository).Returns(_responsibilitiesRepoMock.Object);
        _uowMock.SetupGet(u => u.ApplicationsRepository).Returns(_appsRepoMock.Object);
        _uowMock.SetupGet(u => u.Individual_profilesRepository).Returns(_profilesRepoMock.Object);
        _uowMock.SetupGet(u => u.CvsRepository).Returns(_cvsRepoMock.Object);

        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<applications>().AsQueryable().BuildMock());
        _profilesRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<individual_profiles>().AsQueryable().BuildMock());
        _cvsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<cvs>().AsQueryable().BuildMock());

        _sut = new jobsService(_uowMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsPaginatedResults()
    {
        var jobs = new List<jobs>
        {
            new() { Id = 1, title = "Job 1", status = "active", work_type = "دوام كامل", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, title = "Job 2", status = "active", work_type = "دوام كامل", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, title = "Job 3", status = "active", work_type = "دوام جزئي", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 4, title = "Job 4", status = "active", work_type = "دوام جزئي", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 5, title = "Job 5", status = "active", work_type = "دوام كامل", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();

        _jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobs);
        _benefitsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_benefits>().AsQueryable().BuildMock());
        _conditionsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_conditions>().AsQueryable().BuildMock());
        _responsibilitiesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_responsibilities>().AsQueryable().BuildMock());

        var result = await _sut.GetAllAsync(new PaginationDto { Page = 1, Limit = 2 }, null, null, null);

        result.Should().NotBeNull();
        result.Items.Count.Should().Be(2);
        result.Total.Should().Be(5);
        result.Page.Should().Be(1);
        result.Limit.Should().Be(2);
    }

    [Fact]
    public async Task GetAll_FiltersByType()
    {
        var jobs = new List<jobs>
        {
            new() { Id = 1, title = "Job 1", status = "active", work_type = "دوام كامل", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, title = "Job 2", status = "active", work_type = "دوام كامل", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, title = "Job 3", status = "active", work_type = "دوام كامل", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 4, title = "Job 4", status = "active", work_type = "دوام جزئي", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 5, title = "Job 5", status = "active", work_type = "دوام جزئي", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();

        _jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobs);
        _benefitsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_benefits>().AsQueryable().BuildMock());
        _conditionsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_conditions>().AsQueryable().BuildMock());
        _responsibilitiesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_responsibilities>().AsQueryable().BuildMock());

        var result = await _sut.GetAllAsync(new PaginationDto { Page = 1, Limit = 10 }, "دوام كامل", null, null);

        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAll_ReturnsApplicantCount()
    {
        var jobsList = new List<jobs>
        {
            new() { Id = 1, title = "Job 1", status = "active", work_type = "دوام كامل", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, title = "Job 2", status = "active", work_type = "دوام كامل", entity_Entity = new users { name = "Entity", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();

        _jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobsList);
        _benefitsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_benefits>().AsQueryable().BuildMock());
        _conditionsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_conditions>().AsQueryable().BuildMock());
        _responsibilitiesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_responsibilities>().AsQueryable().BuildMock());

        var apps = new List<applications>
        {
            new() { Id = 1, job_id = 1, user_id = 10, status = "new" },
            new() { Id = 2, job_id = 1, user_id = 11, status = "new" },
            new() { Id = 3, job_id = 2, user_id = 12, status = "new" },
        }.AsQueryable().BuildMock();
        _appsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(apps);

        var result = await _sut.GetAllAsync(new PaginationDto { Page = 1, Limit = 10 }, null, null, null);

        var job1 = result.Items.Single(j => j.Id == 1);
        var job2 = result.Items.Single(j => j.Id == 2);
        job1.ApplicantCount.Should().Be(2);
        job2.ApplicantCount.Should().Be(1);
    }

    [Fact]
    public async Task GetJobById_ExistingJob_ReturnsJob()
    {
        var job = new jobs
        {
            Id = 1,
            title = "Test Job",
            status = "active",
            entity_id = 1,
            entity_Entity = new users { name = "Entity", avatar_url = "" },
            CreatedAt = DateTime.UtcNow,
        };
        _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { job }.AsQueryable().BuildMock());
        _benefitsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_benefits>().AsQueryable().BuildMock());
        _conditionsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_conditions>().AsQueryable().BuildMock());
        _responsibilitiesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_responsibilities>().AsQueryable().BuildMock());

        var result = await _sut.GetJobByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Test Job");
    }

    [Fact]
    public async Task GetJobById_NonExistingJob_ThrowsKeyNotFoundException()
    {
        _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<jobs>().AsQueryable().BuildMock());

        var act = () => _sut.GetJobByIdAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Publish_CreatesJobWithActiveStatus()
    {
        var entityUser = new users { Id = 1, type = "entity", name = "Entity" };
        _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { entityUser }.AsQueryable().BuildMock());

        jobs? capturedJob = null;
        _jobsRepoMock.Setup(r => r.AddAsync(It.IsAny<jobs>()))
            .Callback<jobs>(j => { capturedJob = j; capturedJob.entity_Entity = entityUser; })
            .Returns(Task.CompletedTask);
        _benefitsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_benefits>().AsQueryable().BuildMock());
        _conditionsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_conditions>().AsQueryable().BuildMock());
        _responsibilitiesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_responsibilities>().AsQueryable().BuildMock());

        _jobsRepoMock.Setup(r => r.GetQueryable())
            .Returns(() => Task.FromResult<IQueryable<jobs>>(
                capturedJob != null
                    ? (IQueryable<jobs>)new[] { capturedJob }.AsQueryable().BuildMock()
                    : new List<jobs>().AsQueryable().BuildMock()
            ));

        _benefitsRepoMock.Setup(r => r.AddAsync(It.IsAny<job_benefits>())).Returns(Task.CompletedTask);
        _conditionsRepoMock.Setup(r => r.AddAsync(It.IsAny<job_conditions>())).Returns(Task.CompletedTask);
        _responsibilitiesRepoMock.Setup(r => r.AddAsync(It.IsAny<job_responsibilities>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new CreateJobDto
        {
            Title = "مطور",
            Description = "تطوير تطبيقات",
            Location = "مكة",
            Type = "دوام كامل",
            Target = "أفراد",
            Vacancies = 2,
            Qualification = "بكالوريوس",
            Salary = "5000",
        };

        var result = await _sut.PublishAsync(dto, 1);

        result.Should().NotBeNull();
        result.Status.Should().Be("active");
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task GetFilterOptions_ReturnsDistinctValues()
    {
        var jobs = new List<jobs>
        {
            new() { Id = 1, status = "active", work_type = "دوام كامل", location = "مكة", gender = "ذكر", entity_id = 10, entity_Entity = new users { name = "Entity A" } },
            new() { Id = 2, status = "active", work_type = "دوام جزئي", location = "مكة", gender = "أنثى", entity_id = 10, entity_Entity = new users { name = "Entity A" } },
            new() { Id = 3, status = "active", work_type = "دوام كامل", location = "جدة", gender = "ذكر", entity_id = 20, entity_Entity = new users { name = "Entity B" } },
            new() { Id = 4, status = "closed", work_type = "دوام كامل", location = "جدة", gender = "ذكر", entity_id = 20, entity_Entity = new users { name = "Entity B" } },
        }.AsQueryable().BuildMock();

        _jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobs);

        var result = await _sut.GetFilterOptionsAsync();

        result.Should().NotBeNull();
        result.Types.Should().HaveCount(2);
        result.Locations.Should().HaveCount(2);
        result.Genders.Should().HaveCount(2);
        result.Entities.Should().HaveCount(2);
        result.Entities.Select(e => (long)e.Value!).Should().BeEquivalentTo(new[] { 10L, 20L });
        result.Entities.Select(e => e.Label).Should().BeEquivalentTo(new[] { "Entity A", "Entity B" });
        result.Statuses.Should().HaveCount(3);
        result.Statuses.First().Value!.ToString().Should().BeEmpty();
        result.Statuses.Select(s => s.Value!.ToString()).Should().Contain(new[] { "active", "closed" });
    }

    [Fact]
    public async Task GetMyJobs_ReturnsOnlyCallingEntitysJobs()
    {
        var jobs = new List<jobs>
        {
            new() { Id = 1, entity_id = 5, status = "active", title = "Job 1", work_type = "دوام كامل", entity_Entity = new users { name = "Entity 5", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, entity_id = 5, status = "draft", title = "Job 2", work_type = "دوام جزئي", entity_Entity = new users { name = "Entity 5", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, entity_id = 9, status = "active", title = "Job 3", work_type = "دوام كامل", entity_Entity = new users { name = "Entity 9", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();

        _jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobs);
        _benefitsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_benefits>().AsQueryable().BuildMock());
        _conditionsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_conditions>().AsQueryable().BuildMock());
        _responsibilitiesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_responsibilities>().AsQueryable().BuildMock());

        var result = await _sut.GetMyJobsAsync(new PaginationDto { Page = 1, Limit = 10 }, 5);

        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(2);
        result.Items.Should().OnlyContain(j => j.EntityId == 5);
    }

    [Fact]
    public async Task GetMyJobs_FiltersByStatus()
    {
        var jobs = new List<jobs>
        {
            new() { Id = 1, entity_id = 5, status = "active", title = "Job 1", entity_Entity = new users { name = "Entity 5", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, entity_id = 5, status = "draft", title = "Job 2", entity_Entity = new users { name = "Entity 5", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, entity_id = 5, status = "closed", title = "Job 3", entity_Entity = new users { name = "Entity 5", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();

        _jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobs);
        _benefitsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_benefits>().AsQueryable().BuildMock());
        _conditionsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_conditions>().AsQueryable().BuildMock());
        _responsibilitiesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_responsibilities>().AsQueryable().BuildMock());

        var result = await _sut.GetMyJobsAsync(new PaginationDto { Page = 1, Limit = 10 }, 5, status: "draft");

        result.Items.Should().HaveCount(1);
        result.Items[0].Id.Should().Be(2);
        result.Items[0].Status.Should().Be("draft");
    }

    [Fact]
    public async Task GetMyJobs_FiltersByType()
    {
        var jobs = new List<jobs>
        {
            new() { Id = 1, entity_id = 5, status = "active", title = "Job 1", work_type = "دوام كامل", entity_Entity = new users { name = "Entity 5", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, entity_id = 5, status = "active", title = "Job 2", work_type = "دوام جزئي", entity_Entity = new users { name = "Entity 5", avatar_url = "" }, CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();

        _jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobs);
        _benefitsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_benefits>().AsQueryable().BuildMock());
        _conditionsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_conditions>().AsQueryable().BuildMock());
        _responsibilitiesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<job_responsibilities>().AsQueryable().BuildMock());

        var result = await _sut.GetMyJobsAsync(new PaginationDto { Page = 1, Limit = 10 }, 5, type: "دوام كامل");

        result.Items.Should().HaveCount(1);
        result.Items[0].Id.Should().Be(1);
    }

    [Fact]
    public async Task GetApplications_NonExistingJob_ThrowsKeyNotFoundException()
    {
        _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<jobs>().AsQueryable().BuildMock());

        var act = () => _sut.GetApplicationsAsync(999, 1, new PaginationDto { Page = 1, Limit = 10 });

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetApplications_JobOwnedByAnotherEntity_ThrowsUnauthorizedAccessException()
    {
        var jobs = new List<jobs>
        {
            new() { Id = 1, entity_id = 5, status = "active", title = "Job 1", CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();
        _jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobs);

        var act = () => _sut.GetApplicationsAsync(1, 9, new PaginationDto { Page = 1, Limit = 10 });

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GetApplications_OwnedJob_ReturnsApplications()
    {
        var jobs = new List<jobs>
        {
            new() { Id = 1, entity_id = 5, status = "active", title = "Job 1", CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();
        _jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobs);

        var apps = new List<applications>
        {
            new() { Id = 1, job_id = 1, user_id = 10, status = "new", experience = "سنتان خبرة", cv_id = 7, user_Entity = new users { name = "Applicant 1", gender = "ذكر", avatar_url = "/uploads/avatars/a.jpg" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, job_id = 1, user_id = 11, status = "shortlisted", user_Entity = new users { name = "Applicant 2", gender = "أنثى" }, CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();
        _appsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(apps);
        _profilesRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { new individual_profiles { user_id = 10, city = "مكة" } }.AsQueryable().BuildMock());
        _cvsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { new cvs { Id = 7, user_id = 10, file_name = "cv.pdf", file_path = "/uploads/cvs/cv.pdf" } }.AsQueryable().BuildMock());

        var result = await _sut.GetApplicationsAsync(1, 5, new PaginationDto { Page = 1, Limit = 10 });

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(2);
        result.Items.Should().OnlyContain(a => a.JobId == 1);
        var first = result.Items.Should().ContainSingle(a => a.Id == 1).Subject;
        first.Experience.Should().Be("سنتان خبرة");
        first.CvId.Should().Be(7);
        first.CvFilePath.Should().Be("/uploads/cvs/cv.pdf");
        first.UserCity.Should().Be("مكة");
        first.AvatarUrl.Should().Be("/uploads/avatars/a.jpg");
    }
}
