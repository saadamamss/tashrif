namespace tashrif.Tests.Services;

public class QualificationsServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IqualificationsRepository> _qualsRepoMock;
    private readonly qualificationsService _sut;

    public QualificationsServiceTests()
    {
        _qualsRepoMock = new Mock<IqualificationsRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _uowMock.SetupGet(u => u.QualificationsRepository).Returns(_qualsRepoMock.Object);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _sut = new qualificationsService(_uowMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsMappedQualifications_OrderedByYearDesc()
    {
        var quals = new[]
        {
            new qualifications { Id = 1, user_id = 1, type = "بكلوريوس", specialization = "علوم حاسب", institution = "جامعة الملك سعود", graduation_year = 2020, grade = "ممتاز" },
            new qualifications { Id = 2, user_id = 1, type = "دبلوم", specialization = "إدارة أعمال", institution = "معهد الإدارة", graduation_year = 2024, grade = "جيد" },
        };
        _qualsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(quals.AsQueryable().BuildMock());

        var result = (await _sut.GetAllByUserAsync(1)).ToList();

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(2);
        result[1].Id.Should().Be(1);
        result[1].Type.Should().Be("بكلوريوس");
        result[1].Specialization.Should().Be("علوم حاسب");
        result[1].Institution.Should().Be("جامعة الملك سعود");
        result[1].GraduationYear.Should().Be(2020);
        result[1].Grade.Should().Be("ممتاز");
    }

    [Fact]
    public async Task GetAll_ExcludesDeletedAndOtherUsers()
    {
        var quals = new[]
        {
            new qualifications { Id = 1, user_id = 1, type = "بكلوريوس", graduation_year = 2020 },
            new qualifications { Id = 2, user_id = 2, type = "دبلوم", graduation_year = 2024 },
            new qualifications { Id = 3, user_id = 1, type = "دبلوم", graduation_year = 2022, IsDeleted = true },
        };
        _qualsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(quals.AsQueryable().BuildMock());

        var result = await _sut.GetAllByUserAsync(1);

        result.Should().ContainSingle();
        result.Single().Id.Should().Be(1);
    }

    [Fact]
    public async Task Create_PersistsAllFields()
    {
        qualifications? added = null;
        _qualsRepoMock.Setup(r => r.AddAsync(It.IsAny<qualifications>()))
            .Callback<qualifications>(q => added = q);

        var dto = new CreateQualificationDto
        {
            QualificationType = "بكلوريوس",
            Specialization = "هندسة",
            Institution = "جامعة الملك فهد",
            GraduationYear = 2021,
            Grade = "جيد جداً",
        };

        var result = await _sut.CreateAsync(dto, 1);

        added.Should().NotBeNull();
        added!.user_id.Should().Be(1);
        added!.type.Should().Be("بكلوريوس");
        added!.specialization.Should().Be("هندسة");
        added!.institution.Should().Be("جامعة الملك فهد");
        added!.graduation_year.Should().Be(2021);
        added!.grade.Should().Be("جيد جداً");
        result.Type.Should().Be("بكلوريوس");
        result.GraduationYear.Should().Be(2021);
    }

    [Fact]
    public async Task Update_ByOwner_UpdatesFields()
    {
        var qual = new qualifications { Id = 1, user_id = 1, type = "دبلوم", graduation_year = 2020 };
        _qualsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { qual }.AsQueryable().BuildMock());

        var dto = new CreateQualificationDto
        {
            QualificationType = "بكلوريوس",
            Specialization = "علوم حاسب",
            Institution = "جامعة الملك سعود",
            GraduationYear = 2024,
            Grade = "ممتاز",
        };

        var result = await _sut.UpdateAsync(1, dto, 1);

        result.Type.Should().Be("بكلوريوس");
        result.Specialization.Should().Be("علوم حاسب");
        result.Institution.Should().Be("جامعة الملك سعود");
        result.GraduationYear.Should().Be(2024);
        result.Grade.Should().Be("ممتاز");
        _qualsRepoMock.Verify(r => r.Update(It.IsAny<qualifications>()), Times.Once);
    }

    [Fact]
    public async Task Update_NonExisting_ThrowsKeyNotFoundException()
    {
        _qualsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<qualifications>().AsQueryable().BuildMock());

        var dto = new CreateQualificationDto { QualificationType = "دبلوم" };

        var act = () => _sut.UpdateAsync(999, dto, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Update_WrongOwner_ThrowsUnauthorizedAccessException()
    {
        var qual = new qualifications { Id = 1, user_id = 2, type = "دبلوم" };
        _qualsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { qual }.AsQueryable().BuildMock());

        var dto = new CreateQualificationDto { QualificationType = "بكلوريوس" };

        var act = () => _sut.UpdateAsync(1, dto, 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Delete_ByOwner_SoftDeletes()
    {
        var qual = new qualifications { Id = 1, user_id = 1, type = "دبلوم" };
        _qualsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { qual }.AsQueryable().BuildMock());

        await _sut.DeleteAsync(1, 1);

        qual.IsDeleted.Should().BeTrue();
        qual.DeletedTime.Should().NotBeNull();
        _qualsRepoMock.Verify(r => r.Update(It.IsAny<qualifications>()), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExisting_ThrowsKeyNotFoundException()
    {
        _qualsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<qualifications>().AsQueryable().BuildMock());

        var act = () => _sut.DeleteAsync(999, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Delete_WrongOwner_ThrowsUnauthorizedAccessException()
    {
        var qual = new qualifications { Id = 1, user_id = 2, type = "دبلوم" };
        _qualsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { qual }.AsQueryable().BuildMock());

        var act = () => _sut.DeleteAsync(1, 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _qualsRepoMock.Verify(r => r.Update(It.IsAny<qualifications>()), Times.Never);
    }
}