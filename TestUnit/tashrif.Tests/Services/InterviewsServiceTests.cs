using tashrif.Data.DTOs;

namespace tashrif.Tests.Services;

public class InterviewsServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IapplicationsRepository> _appsRepoMock;
    private readonly Mock<IjobsRepository> _jobsRepoMock;
    private readonly Mock<IinterviewsRepository> _interviewsRepoMock;
    private readonly Mock<IusersRepository> _usersRepoMock;
    private readonly Mock<IstatusHistoryService> _statusHistoryMock;
    private readonly interviewsService _sut;

    public InterviewsServiceTests()
    {
        _appsRepoMock = new Mock<IapplicationsRepository>();
        _jobsRepoMock = new Mock<IjobsRepository>();
        _interviewsRepoMock = new Mock<IinterviewsRepository>();
        _usersRepoMock = new Mock<IusersRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _uowMock.SetupGet(u => u.ApplicationsRepository).Returns(_appsRepoMock.Object);
        _uowMock.SetupGet(u => u.JobsRepository).Returns(_jobsRepoMock.Object);
        _uowMock.SetupGet(u => u.InterviewsRepository).Returns(_interviewsRepoMock.Object);
        _uowMock.SetupGet(u => u.UsersRepository).Returns(_usersRepoMock.Object);

        _statusHistoryMock = new Mock<IstatusHistoryService>();
        _statusHistoryMock.Setup(r => r.RecordAsync(It.IsAny<long>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<long>()))
            .Returns(Task.CompletedTask);

        _sut = new interviewsService(_uowMock.Object, _statusHistoryMock.Object);
    }

    [Fact]
    public async Task Schedule_UpdatesApplicationStatusToInterview()
    {
        var app = new applications
        {
            Id = 1,
            job_id = 1,
            user_id = 2,
            status = "shortlisted",
            qualification = "بكالوريوس",
            experience = "3 سنوات",
            cover_letter = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var job = new jobs { Id = 1, entity_id = 10, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());
        _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { job }.AsQueryable().BuildMock());
        _usersRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[]
            {
                new users { Id = 2, name = "المتقدم", avatar_url = "avatar.png" },
                new users { Id = 10, name = "الجهة", avatar_url = "logo.png" },
            }.AsQueryable().BuildMock());
        _interviewsRepoMock.Setup(r => r.AddAsync(It.IsAny<interviews>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        applications? updatedApp = null;
        _appsRepoMock.Setup(r => r.Update(It.IsAny<applications>()))
            .Callback<applications>(a => updatedApp = a);

        var dto = new ScheduleInterviewDto
        {
            ApplicationId = 1,
            Method = "online",
            Date = DateTime.UtcNow.AddDays(1),
            Time = "10:00",
        };

        var result = await _sut.ScheduleAsync(dto, 10);

        result.Should().NotBeNull();
        result.Status.Should().Be("scheduled");
        result.EntityName.Should().Be("الجهة");
        result.UserName.Should().Be("المتقدم");
        _interviewsRepoMock.Verify(r => r.AddAsync(It.IsAny<interviews>()), Times.Once);
        updatedApp.Should().NotBeNull();
        updatedApp!.status.Should().Be("interview");
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Schedule_NonOwnerThrowsUnauthorizedAccessException()
    {
        var app = new applications
        {
            Id = 1,
            job_id = 1,
            user_id = 2,
            status = "shortlisted",
            qualification = "بكالوريوس",
            experience = "3 سنوات",
            cover_letter = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var job = new jobs { Id = 1, entity_id = 10, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());
        _jobsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { job }.AsQueryable().BuildMock());

        var dto = new ScheduleInterviewDto
        {
            ApplicationId = 1,
            Method = "online",
            Date = DateTime.UtcNow.AddDays(1),
            Time = "10:00",
        };

        var act = () => _sut.ScheduleAsync(dto, 99);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _interviewsRepoMock.Verify(r => r.AddAsync(It.IsAny<interviews>()), Times.Never);
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAll_IncludesEntityAndApplicantNames()
    {
        var entityUser = new users { Id = 10, name = "الجهة", avatar_url = "logo.png" };
        var applicantUser = new users { Id = 2, name = "المتقدم", avatar_url = "avatar.png" };
        var interview = new interviews
        {
            Id = 1,
            application_id = 5,
            job_id = 1,
            user_id = 2,
            entity_id = 10,
            method = "online",
            interview_date = DateTime.UtcNow.AddDays(1),
            interview_time = "10:00",
            location = "مكة",
            link = "",
            notes = "",
            status = "scheduled",
            attendance = "pending",
            entity_Entity = entityUser,
            user_Entity = applicantUser,
            job_Entity = new jobs { Id = 1 },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _interviewsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { interview }.AsQueryable().BuildMock());

        var result = await _sut.GetAllAsync(new PaginationDto { Page = 1, Limit = 10 }, 10, "entity");

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items[0].EntityName.Should().Be("الجهة");
        result.Items[0].EntityLogo.Should().Be("logo.png");
        result.Items[0].UserName.Should().Be("المتقدم");
        result.Items[0].UserAvatar.Should().Be("avatar.png");
    }

    [Fact]
    public async Task GetAll_FiltersByApplicationId()
    {
        var applicantUser = new users { Id = 2, name = "المتقدم", avatar_url = "avatar.png" };
        var entityUser = new users { Id = 10, name = "الجهة", avatar_url = "logo.png" };
        interviews MakeInterview(long id, long appId) => new()
        {
            Id = id,
            application_id = appId,
            job_id = 1,
            user_id = 2,
            entity_id = 10,
            method = "online",
            interview_date = DateTime.UtcNow.AddDays(1),
            interview_time = "10:00",
            location = "مكة",
            link = "",
            notes = "",
            status = "scheduled",
            attendance = "pending",
            entity_Entity = entityUser,
            user_Entity = applicantUser,
            job_Entity = new jobs { Id = 1 },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _interviewsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { MakeInterview(1, 5), MakeInterview(2, 6) }.AsQueryable().BuildMock());

        var result = await _sut.GetAllAsync(new PaginationDto { Page = 1, Limit = 10 }, 10, "entity", 5);

        result.Items.Should().HaveCount(1);
        result.Items[0].ApplicationId.Should().Be(5);
    }

    [Fact]
    public async Task GetAll_UnknownApplicationId_ReturnsEmpty()
    {
        _interviewsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(Enumerable.Empty<interviews>().AsQueryable().BuildMock());

        var result = await _sut.GetAllAsync(new PaginationDto { Page = 1, Limit = 10 }, 10, "entity", 999);

        result.Items.Should().BeEmpty();
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task GetAll_SameApplication_ReturnsLatestFirst()
    {
        var applicantUser = new users { Id = 2, name = "المتقدم", avatar_url = "avatar.png" };
        var entityUser = new users { Id = 10, name = "الجهة", avatar_url = "logo.png" };
        var older = new interviews
        {
            Id = 1, application_id = 5, job_id = 1, user_id = 2, entity_id = 10,
            method = "online", interview_date = DateTime.UtcNow.AddDays(1), interview_time = "10:00",
            location = "مكة", link = "", notes = "الأولى", status = "scheduled", attendance = "pending",
            entity_Entity = entityUser, user_Entity = applicantUser, job_Entity = new jobs { Id = 1 },
            CreatedAt = DateTime.UtcNow.AddDays(-2), UpdatedAt = DateTime.UtcNow.AddDays(-2),
        };
        var newer = new interviews
        {
            Id = 2, application_id = 5, job_id = 1, user_id = 2, entity_id = 10,
            method = "online", interview_date = DateTime.UtcNow.AddDays(3), interview_time = "11:00",
            location = "جدة", link = "", notes = "الثانية", status = "scheduled", attendance = "pending",
            entity_Entity = entityUser, user_Entity = applicantUser, job_Entity = new jobs { Id = 1 },
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
        };

        _interviewsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { older, newer }.AsQueryable().BuildMock());

        var result = await _sut.GetAllAsync(new PaginationDto { Page = 1, Limit = 10 }, 10, "entity", 5);

        result.Items.Should().HaveCount(2);
        result.Items[0].Id.Should().Be(2);
    }
}
