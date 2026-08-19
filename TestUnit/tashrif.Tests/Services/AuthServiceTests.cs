using Microsoft.Extensions.Configuration;

namespace tashrif.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IusersRepository> _userRepoMock;
    private readonly Mock<Iindividual_profilesRepository> _profileRepoMock;
    private readonly Mock<Ientity_profilesRepository> _entityProfileRepoMock;
    private readonly Mock<Icontact_personsRepository> _contactPersonRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IusersRepository>();
        _profileRepoMock = new Mock<Iindividual_profilesRepository>();
        _entityProfileRepoMock = new Mock<Ientity_profilesRepository>();
        _contactPersonRepoMock = new Mock<Icontact_personsRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _configMock = new Mock<IConfiguration>();
        _fileStorageMock = new Mock<IFileStorageService>();

        _configMock.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyForTestPurposesOnly!@#$%");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
        _configMock.Setup(c => c["Jwt:RefreshKey"]).Returns("SuperSecretRefreshKeyForTestPurposes!@#$%");

        _sut = new AuthService(
            _userRepoMock.Object,
            _profileRepoMock.Object,
            _entityProfileRepoMock.Object,
            _contactPersonRepoMock.Object,
            _uowMock.Object,
            _configMock.Object,
            _fileStorageMock.Object);
    }

    [Fact]
    public async Task RegisterIndividual_CreatesUserAndReturnsTokens()
    {
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<users>().AsQueryable().BuildMock());
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>())).Returns(Task.CompletedTask);
        _profileRepoMock.Setup(r => r.AddAsync(It.IsAny<individual_profiles>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new RegisterIndividualDto
        {
            FirstName = "أحمد",
            LastName = "محمد",
            NationalId = "1234567890",
            Phone = "+966501234567",
            Email = "test@test.com",
            Gender = "ذكر",
            Nationality = "سعودي"
        };

        var result = await _sut.RegisterIndividualAsync(dto);

        result.Should().NotBeNull();
        result.User.Should().NotBeNull();
        result.User.Name.Should().Be("أحمد محمد");
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<users>()), Times.Once);
    }

    [Fact]
    public async Task RegisterIndividual_DuplicateNationalId_ThrowsException()
    {
        var existingUser = new users { national_id = "1234567890" };
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new[] { existingUser }.AsQueryable().BuildMock());

        var dto = new RegisterIndividualDto
        {
            NationalId = "1234567890",
            FirstName = "أحمد",
            LastName = "محمد",
            Phone = "+966501234567",
            Email = "test@test.com",
            Gender = "ذكر",
            Nationality = "سعودي"
        };

        var act = () => _sut.RegisterIndividualAsync(dto);

        await act.Should().ThrowAsync<BadHttpRequestException>()
            .WithMessage("*مسجل مسبقاً*");
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        var password = "test123";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new users
        {
            national_id = "123",
            password_hash = hash,
            name = "Test User",
            email = "t@t.com",
            type = "individual",
            gender = "ذكر",
            nationality = "سعودي"
        };
        _userRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { user }.AsQueryable().BuildMock());

        var result = await _sut.LoginAsync(new LoginDto
        {
            NationalId = "123",
            Password = password
        });

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_InvalidPassword_ThrowsException()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("correct");
        var user = new users { national_id = "123", password_hash = hash };
        _userRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { user }.AsQueryable().BuildMock());

        var act = () => _sut.LoginAsync(new LoginDto
        {
            NationalId = "123",
            Password = "wrong"
        });

        await act.Should().ThrowAsync<BadHttpRequestException>().WithMessage("*غير صحيحة*");
    }
}
