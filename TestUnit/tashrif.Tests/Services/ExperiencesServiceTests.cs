namespace tashrif.Tests.Services;

public class ExperiencesServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IexperiencesRepository> _expsRepoMock;
    private readonly experiencesService _sut;

    public ExperiencesServiceTests()
    {
        _expsRepoMock = new Mock<IexperiencesRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _uowMock.SetupGet(u => u.ExperiencesRepository).Returns(_expsRepoMock.Object);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _sut = new experiencesService(_uowMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsMappedExperiences_OrderedByStartDateDesc()
    {
        var exps = new[]
        {
            new experiences { Id = 1, user_id = 1, job_title = "مشرف فرقة", employer = "شركة المشاعر", duration = "سنة", location = "مكة", is_current = false, start_date = new DateTime(2022, 1, 1), end_date = new DateTime(2023, 12, 31) },
            new experiences { Id = 2, user_id = 1, job_title = "منسق خدمات", employer = "مؤسسة الحج", duration = "سنتان", location = "الرياض", is_current = true, start_date = new DateTime(2024, 1, 1) },
        };
        _expsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(exps.AsQueryable().BuildMock());

        var result = (await _sut.GetAllByUserAsync(1)).ToList();

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(2);
        result[1].Id.Should().Be(1);
        result[1].JobTitle.Should().Be("مشرف فرقة");
        result[1].Employer.Should().Be("شركة المشاعر");
        result[1].Location.Should().Be("مكة");
        result[1].IsCurrent.Should().BeFalse();
        result[1].StartDate.Should().Be(new DateTime(2022, 1, 1));
    }

    [Fact]
    public async Task GetAll_ExcludesDeletedAndOtherUsers()
    {
        var exps = new[]
        {
            new experiences { Id = 1, user_id = 1, job_title = "مشرف", start_date = new DateTime(2022, 1, 1) },
            new experiences { Id = 2, user_id = 2, job_title = "منسق", start_date = new DateTime(2024, 1, 1) },
            new experiences { Id = 3, user_id = 1, job_title = "مدير", start_date = new DateTime(2023, 1, 1), IsDeleted = true },
        };
        _expsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(exps.AsQueryable().BuildMock());

        var result = await _sut.GetAllByUserAsync(1);

        result.Should().ContainSingle();
        result.Single().Id.Should().Be(1);
    }

    [Fact]
    public async Task Create_PersistsAllFields()
    {
        experiences? added = null;
        _expsRepoMock.Setup(r => r.AddAsync(It.IsAny<experiences>()))
            .Callback<experiences>(e => added = e);

        var dto = new CreateExperienceDto
        {
            JobTitle = "مشرف فرقة",
            Employer = "شركة المشاعر",
            Duration = "سنة",
            Location = "مكة",
            IsCurrent = true,
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2025, 1, 1),
        };

        var result = await _sut.CreateAsync(dto, 1);

        added.Should().NotBeNull();
        added!.user_id.Should().Be(1);
        added!.job_title.Should().Be("مشرف فرقة");
        added!.employer.Should().Be("شركة المشاعر");
        added!.duration.Should().Be("سنة");
        added!.location.Should().Be("مكة");
        added!.is_current.Should().BeTrue();
        added!.start_date.Should().Be(new DateTime(2024, 1, 1));
        added!.start_date!.Value.Kind.Should().Be(DateTimeKind.Utc);
        added!.end_date.Should().BeNull();
        result.JobTitle.Should().Be("مشرف فرقة");
        result.IsCurrent.Should().BeTrue();
    }

    [Fact]
    public async Task Update_ByOwner_UpdatesFields()
    {
        var exp = new experiences { Id = 1, user_id = 1, job_title = "مشرف", is_current = false };
        _expsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { exp }.AsQueryable().BuildMock());

        var dto = new CreateExperienceDto
        {
            JobTitle = "منسق خدمات",
            Employer = "مؤسسة الحج",
            Duration = "سنتان",
            Location = "الرياض",
            IsCurrent = true,
            StartDate = new DateTime(2024, 1, 1),
        };

        var result = await _sut.UpdateAsync(1, dto, 1);

        result.JobTitle.Should().Be("منسق خدمات");
        result.Employer.Should().Be("مؤسسة الحج");
        result.Duration.Should().Be("سنتان");
        result.Location.Should().Be("الرياض");
        result.IsCurrent.Should().BeTrue();
        exp.start_date!.Value.Kind.Should().Be(DateTimeKind.Utc);
        _expsRepoMock.Verify(r => r.Update(It.IsAny<experiences>()), Times.Once);
    }

    [Fact]
    public async Task Update_NonExisting_ThrowsKeyNotFoundException()
    {
        _expsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<experiences>().AsQueryable().BuildMock());

        var dto = new CreateExperienceDto { JobTitle = "مشرف" };

        var act = () => _sut.UpdateAsync(999, dto, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Update_WrongOwner_ThrowsUnauthorizedAccessException()
    {
        var exp = new experiences { Id = 1, user_id = 2, job_title = "مشرف" };
        _expsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { exp }.AsQueryable().BuildMock());

        var dto = new CreateExperienceDto { JobTitle = "منسق" };

        var act = () => _sut.UpdateAsync(1, dto, 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Delete_ByOwner_SoftDeletes()
    {
        var exp = new experiences { Id = 1, user_id = 1, job_title = "مشرف" };
        _expsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { exp }.AsQueryable().BuildMock());

        await _sut.DeleteAsync(1, 1);

        exp.IsDeleted.Should().BeTrue();
        exp.DeletedTime.Should().NotBeNull();
        _expsRepoMock.Verify(r => r.Update(It.IsAny<experiences>()), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExisting_ThrowsKeyNotFoundException()
    {
        _expsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<experiences>().AsQueryable().BuildMock());

        var act = () => _sut.DeleteAsync(999, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Delete_WrongOwner_ThrowsUnauthorizedAccessException()
    {
        var exp = new experiences { Id = 1, user_id = 2, job_title = "مشرف" };
        _expsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { exp }.AsQueryable().BuildMock());

        var act = () => _sut.DeleteAsync(1, 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _expsRepoMock.Verify(r => r.Update(It.IsAny<experiences>()), Times.Never);
    }
}