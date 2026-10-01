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

    // ---- Spec 01 (phase3-6-remove-registration-cv): registration no longer accepts a CV ----

    [Fact]
    public async Task RegisterIndividual_WithIdFile_SavesOnlyIdFile()
    {
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<users>().AsQueryable().BuildMock());
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        individual_profiles? saved = null;
        _profileRepoMock.Setup(r => r.AddAsync(It.IsAny<individual_profiles>()))
            .Callback<individual_profiles>(p => saved = p)
            .Returns(Task.CompletedTask);

        _fileStorageMock.Setup(f => f.SaveFileAsync(It.IsAny<IFormFile?>(), "ids"))
            .ReturnsAsync("/uploads/ids/x.jpg");

        var dto = new RegisterIndividualDto
        {
            FirstName = "أحمد",
            LastName = "محمد",
            NationalId = "1234567890",
            Phone = "+966501234567",
            Email = "a@b.com",
            Gender = "male",
            Nationality = "سعودي",
            IdFile = new Mock<IFormFile>().Object,
        };

        await _sut.RegisterIndividualAsync(dto);

        _fileStorageMock.Verify(f => f.SaveFileAsync(It.IsAny<IFormFile?>(), "ids"), Times.Once);
        _fileStorageMock.Verify(f => f.SaveFileAsync(It.IsAny<IFormFile?>(), "cvs"), Times.Never);
        saved.Should().NotBeNull();
        saved!.id_file.Should().Be("/uploads/ids/x.jpg");
    }

    [Fact]
    public async Task RegisterIndividual_NoFiles_DoesNotSaveAnyFile()
    {
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<users>().AsQueryable().BuildMock());
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        individual_profiles? saved = null;
        _profileRepoMock.Setup(r => r.AddAsync(It.IsAny<individual_profiles>()))
            .Callback<individual_profiles>(p => saved = p)
            .Returns(Task.CompletedTask);

        var dto = new RegisterIndividualDto
        {
            FirstName = "سارة",
            LastName = "علي",
            NationalId = "9876543210",
            Phone = "+966500000000",
            Email = "s@b.com",
            Gender = "female",
            Nationality = "سعودي",
        };

        await _sut.RegisterIndividualAsync(dto);

        _fileStorageMock.Verify(f => f.SaveFileAsync(It.IsAny<IFormFile?>(), It.IsAny<string>()), Times.Never);
        saved.Should().NotBeNull();
        saved!.id_file.Should().Be("");
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

    // ---- Spec 02 (phase3-5-hardening): soft-delete guards on /auth/me + change-password ----
    // GetByIdAsync uses FindAsync, which bypasses the global soft-delete filter; the service
    // must guard on IsDeleted itself (mirrors RefreshTokenAsync's existing guard).

    [Fact]
    public async Task GetCurrentUser_DeactivatedUser_ThrowsNotFound()
    {
        var user = new users { name = "Deactivated User", IsDeleted = true };
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<object>())).ReturnsAsync(user);

        var act = () => _sut.GetCurrentUserAsync(9);

        var ex = (await act.Should().ThrowAsync<BadHttpRequestException>()).Which;
        ex.Message.Should().Be("المستخدم غير موجود");
    }

    [Fact]
    public async Task GetCurrentUser_ActiveUser_ReturnsUserDto()
    {
        var user = new users
        {
            name = "Test User",
            email = "t@t.com",
            type = "individual",
            national_id = "1234567890",
            gender = "male",
            nationality = "سعودي",
            IsDeleted = false
        };
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<object>())).ReturnsAsync(user);

        var result = await _sut.GetCurrentUserAsync(9);

        result.Should().NotBeNull();
        result.Name.Should().Be("Test User");
        result.Type.Should().Be("individual");
    }

    [Fact]
    public async Task ChangePassword_DeactivatedUser_ThrowsAndWritesNothing()
    {
        var user = new users
        {
            password_hash = BCrypt.Net.BCrypt.HashPassword("current-pass"),
            IsDeleted = true
        };
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<object>())).ReturnsAsync(user);

        var act = () => _sut.ChangePasswordAsync(9, new ChangePasswordDto
        {
            CurrentPassword = "current-pass",
            NewPassword = "new-password-123"
        });

        var ex = (await act.Should().ThrowAsync<BadHttpRequestException>()).Which;
        ex.Message.Should().Be("المستخدم غير موجود");
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        _userRepoMock.Verify(r => r.Update(It.IsAny<users>()), Times.Never);
    }

    [Fact]
    public async Task ChangePassword_ActiveUser_WrongCurrentPassword_Throws401()
    {
        var user = new users
        {
            password_hash = BCrypt.Net.BCrypt.HashPassword("correct-pass"),
            IsDeleted = false
        };
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<object>())).ReturnsAsync(user);

        var act = () => _sut.ChangePasswordAsync(9, new ChangePasswordDto
        {
            CurrentPassword = "wrong-pass",
            NewPassword = "new-password-123"
        });

        await act.Should().ThrowAsync<BadHttpRequestException>().WithMessage("*الحالية غير صحيحة*");
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
