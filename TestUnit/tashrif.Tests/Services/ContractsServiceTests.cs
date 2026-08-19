namespace tashrif.Tests.Services;

public class ContractsServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IcontractsRepository> _contractsRepoMock;
    private readonly Mock<IapplicationsRepository> _appsRepoMock;
    private readonly Mock<IinterviewsRepository> _interviewsRepoMock;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly contractsService _sut;

    public ContractsServiceTests()
    {
        _contractsRepoMock = new Mock<IcontractsRepository>();
        _appsRepoMock = new Mock<IapplicationsRepository>();
        _interviewsRepoMock = new Mock<IinterviewsRepository>();
        _fileStorageMock = new Mock<IFileStorageService>();
        _uowMock = new Mock<IUnitOfWork>();

        _uowMock.SetupGet(u => u.ContractsRepository).Returns(_contractsRepoMock.Object);
        _uowMock.SetupGet(u => u.ApplicationsRepository).Returns(_appsRepoMock.Object);
        _uowMock.SetupGet(u => u.InterviewsRepository).Returns(_interviewsRepoMock.Object);

        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<applications>().AsQueryable().BuildMock());
        _interviewsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<interviews>().AsQueryable().BuildMock());

        _sut = new contractsService(_uowMock.Object, _fileStorageMock.Object);
    }

    [Fact]
    public async Task Send_CreatesContractWithSentStatus()
    {
        var app = new applications { Id = 1, job_id = 1, user_id = 1, status = "new" };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());
        _contractsRepoMock.Setup(r => r.AddAsync(It.IsAny<contracts>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        applications? updatedApp = null;
        _appsRepoMock.Setup(r => r.Update(It.IsAny<applications>()))
            .Callback<applications>(a => updatedApp = a);

        var dto = new SendContractDto { ApplicationId = 1 };

        var result = await _sut.SendAsync(dto, 1);

        result.Should().NotBeNull();
        result.Status.Should().Be("sent");
        _contractsRepoMock.Verify(r => r.AddAsync(It.IsAny<contracts>()), Times.Once);
        updatedApp.Should().NotBeNull();
        updatedApp!.status.Should().Be("contract_sent");
    }

    [Fact]
    public async Task Send_NonExistingApplication_ThrowsKeyNotFoundException()
    {
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<applications>().AsQueryable().BuildMock());

        var dto = new SendContractDto { ApplicationId = 999 };

        var act = () => _sut.SendAsync(dto, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Sign_UpdatesStatusToSigned()
    {
        var contract = new contracts
        {
            Id = 1,
            user_id = 1,
            status = "sent",
            CreatedAt = DateTime.UtcNow,
        };
        _contractsRepoMock.Setup(r => r.GetByIdAsync(It.Is<object>(o => Convert.ToInt64(o) == 1))).ReturnsAsync(contract);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.SignAsync(1, 1);

        result.Should().NotBeNull();
        result.Status.Should().Be("signed");
        result.SignedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Send_MarksScheduledInterviewCompleted()
    {
        var app = new applications { Id = 1, job_id = 1, user_id = 1, status = "new" };
        var interview = new interviews
        {
            Id = 1,
            application_id = 1,
            job_id = 1,
            user_id = 1,
            entity_id = 2,
            status = "scheduled",
            UpdatedAt = DateTime.UtcNow,
        };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());
        _interviewsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { interview }.AsQueryable().BuildMock());
        _contractsRepoMock.Setup(r => r.AddAsync(It.IsAny<contracts>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        interviews? updatedInterview = null;
        _interviewsRepoMock.Setup(r => r.Update(It.IsAny<interviews>()))
            .Callback<interviews>(i => updatedInterview = i);

        var dto = new SendContractDto { ApplicationId = 1 };

        var result = await _sut.SendAsync(dto, 1);

        result.Status.Should().Be("sent");
        updatedInterview.Should().NotBeNull();
        updatedInterview!.status.Should().Be("completed");
    }

    [Fact]
    public async Task Send_WithFile_SavesFileAndSetsFileMeta()
    {
        var app = new applications { Id = 1, job_id = 1, user_id = 1, status = "new" };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());
        _contractsRepoMock.Setup(r => r.AddAsync(It.IsAny<contracts>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var content = new MemoryStream(new byte[] { 0x25, 0x50, 0x44, 0x46 });
        var formFile = new Microsoft.AspNetCore.Http.FormFile(content, 0, content.Length, "contractFile", "contract.pdf")
        {
            Headers = new Microsoft.AspNetCore.Http.HeaderDictionary(),
            ContentType = "application/pdf",
        };

        _fileStorageMock.Setup(f => f.SaveFileAsync(formFile, "contracts"))
            .ReturnsAsync("/uploads/contracts/abc_contract.pdf");

        contracts? added = null;
        _contractsRepoMock.Setup(r => r.AddAsync(It.IsAny<contracts>()))
            .Callback<contracts>(c => added = c);

        var dto = new SendContractDto { ApplicationId = 1, ContractFile = formFile };

        var result = await _sut.SendAsync(dto, 1);

        result.Status.Should().Be("sent");
        _fileStorageMock.Verify(f => f.SaveFileAsync(formFile, "contracts"), Times.Once);
        added.Should().NotBeNull();
        added!.file_url.Should().Be("/uploads/contracts/abc_contract.pdf");
        added!.file_size.Should().Be(4);
    }

    [Fact]
    public async Task Send_WithNotesAndEndDate_PersistsThem()
    {
        var app = new applications { Id = 1, job_id = 1, user_id = 1, status = "new" };
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());
        _contractsRepoMock.Setup(r => r.AddAsync(It.IsAny<contracts>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        contracts? added = null;
        _contractsRepoMock.Setup(r => r.AddAsync(It.IsAny<contracts>()))
            .Callback<contracts>(c => added = c);

        var endDate = DateTime.UtcNow.AddDays(7);
        var dto = new SendContractDto
        {
            ApplicationId = 1,
            Notes = "يرجى التوقيع قبل بداية الموسم",
            EndDate = endDate,
        };

        var result = await _sut.SendAsync(dto, 1);

        result.Notes.Should().Be("يرجى التوقيع قبل بداية الموسم");
        result.EndDate.Should().Be(endDate);
        added.Should().NotBeNull();
        added!.notes.Should().Be("يرجى التوقيع قبل بداية الموسم");
        added!.end_date.Should().Be(endDate);
    }

    [Fact]
    public async Task Sign_AfterEndDate_ThrowsBadHttpRequestException()
    {
        var contract = new contracts
        {
            Id = 1,
            user_id = 1,
            status = "sent",
            end_date = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow,
        };
        _contractsRepoMock.Setup(r => r.GetByIdAsync(It.Is<object>(o => Convert.ToInt64(o) == 1))).ReturnsAsync(contract);

        var act = () => _sut.SignAsync(1, 1);

        await act.Should().ThrowAsync<Microsoft.AspNetCore.Http.BadHttpRequestException>();
    }

    [Fact]
    public async Task Sign_SetsApplicationAccepted()
    {
        var contract = new contracts
        {
            Id = 1,
            application_id = 1,
            user_id = 1,
            status = "sent",
            CreatedAt = DateTime.UtcNow,
        };
        var app = new applications { Id = 1, job_id = 1, user_id = 1, status = "contract_sent" };
        _contractsRepoMock.Setup(r => r.GetByIdAsync(It.Is<object>(o => Convert.ToInt64(o) == 1))).ReturnsAsync(contract);
        _appsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { app }.AsQueryable().BuildMock());
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        applications? updatedApp = null;
        _appsRepoMock.Setup(r => r.Update(It.IsAny<applications>()))
            .Callback<applications>(a => updatedApp = a);

        var result = await _sut.SignAsync(1, 1);

        result.Status.Should().Be("signed");
        updatedApp.Should().NotBeNull();
        updatedApp!.status.Should().Be("accepted");
    }

    [Fact]
    public async Task Sign_WrongUser_ThrowsUnauthorizedAccessException()
    {
        var contract = new contracts { Id = 1, user_id = 2, status = "sent" };
        _contractsRepoMock.Setup(r => r.GetByIdAsync(It.Is<object>(o => Convert.ToInt64(o) == 1))).ReturnsAsync(contract);

        var act = () => _sut.SignAsync(1, 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
