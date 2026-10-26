using Microsoft.Extensions.Configuration;

namespace tashrif.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IusersRepository> _userRepoMock;
    private readonly Mock<Iindividual_profilesRepository> _profileRepoMock;
    private readonly Mock<Ientity_profilesRepository> _entityProfileRepoMock;
    private readonly Mock<Icontact_personsRepository> _contactPersonRepoMock;
    private readonly Mock<IGenericRepository<email_verification_tokens>> _tokenRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly Mock<IClock> _clockMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IusersRepository>();
        _profileRepoMock = new Mock<Iindividual_profilesRepository>();
        _entityProfileRepoMock = new Mock<Ientity_profilesRepository>();
        _contactPersonRepoMock = new Mock<Icontact_personsRepository>();
        _tokenRepoMock = new Mock<IGenericRepository<email_verification_tokens>>();
        _uowMock = new Mock<IUnitOfWork>();
        _configMock = new Mock<IConfiguration>();
        _fileStorageMock = new Mock<IFileStorageService>();
        _clockMock = new Mock<IClock>();

        _configMock.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyForTestPurposesOnly!@#$%");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
        _configMock.Setup(c => c["Jwt:RefreshKey"]).Returns("SuperSecretRefreshKeyForTestPurposes!@#$%");
        _clockMock.Setup(c => c.UtcNow).Returns(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        _sut = new AuthService(
            _userRepoMock.Object,
            _profileRepoMock.Object,
            _entityProfileRepoMock.Object,
            _contactPersonRepoMock.Object,
            _tokenRepoMock.Object,
            _uowMock.Object,
            _configMock.Object,
            _fileStorageMock.Object,
            _clockMock.Object);
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
            Password = "password123",
            Phone = "+966501234567",
            Email = "test@test.com",
            Gender = "ذكر",
            Nationality = "سعودي"
        };

        var result = await _sut.RegisterIndividualAsync(dto);

        result.Should().NotBeNull();
        result.Message.Should().NotBeNullOrEmpty();
        result.Name.Should().Be("أحمد محمد");
        result.Email.Should().Be("test@test.com");
        result.VerificationToken.Should().NotBeNullOrEmpty();
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

    // ---- Spec 01 (phase3-7): entity registration stores NationalId + CommercialReg separately ----

    [Fact]
    public async Task RegisterEntity_StoresNationalIdAndCommercialRegSeparately()
    {
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<users>().AsQueryable().BuildMock());
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>())).Returns(Task.CompletedTask);
        _entityProfileRepoMock.Setup(r => r.AddAsync(It.IsAny<entity_profiles>())).Returns(Task.CompletedTask);
        _contactPersonRepoMock.Setup(r => r.AddAsync(It.IsAny<contact_persons>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _fileStorageMock.Setup(f => f.SaveFileAsync(It.IsAny<IFormFile?>(), It.IsAny<string>()))
            .ReturnsAsync("/uploads/logos/x.png");

        entity_profiles? savedProfile = null;
        _entityProfileRepoMock.Setup(r => r.AddAsync(It.IsAny<entity_profiles>()))
            .Callback<entity_profiles>(p => savedProfile = p)
            .Returns(Task.CompletedTask);

        var dto = new RegisterEntityDto
        {
            CompanyName = "شركة اختبار",
            NationalId = "7001234567",
            CommercialReg = "1010123456",
            Password = "TestPass123",
            FieldName = "خدمات الحج",
            Sector = "حج وعمرة",
            Country = "السعودية",
            Province = "مكة المكرمة",
            CompanyDesc = "وصف تجريبي",
            CompanyWebsite = "https://example.com",
            Name = "مسؤول الاتصال",
            Email = "c@example.com",
            Phone = "+966500000002",
            Role = "مدير",
            Nationality = "سعودي",
            CompanyLogo = new Mock<IFormFile>().Object,
        };

        var result = await _sut.RegisterEntityAsync(dto);

        result.Should().NotBeNull();
        result.Email.Should().Be("c@example.com");
        savedProfile.Should().NotBeNull();
        savedProfile!.commercial_reg.Should().Be("1010123456");
        savedProfile!.user_id.Should().Be(0);
        _userRepoMock.Verify(r => r.AddAsync(It.Is<users>(u => u.national_id == "7001234567")), Times.Once);
        _contactPersonRepoMock.Verify(r => r.AddAsync(It.IsAny<contact_persons>()), Times.Once);
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
            Password = "password123",
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
            Password = "password123",
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
            nationality = "سعودي",
            is_email_verified = true
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

    // ---- Gap #1 (2026-10-12): user-entered text is trimmed before persisting ----
    // Validation ran on RAW values in the controller, so trimming here only affects stored
    // data (leading spaces were seen live in users.name). Password is never trimmed.

    [Fact]
    public async Task RegisterIndividual_TrimsUserEnteredText()
    {
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<users>().AsQueryable().BuildMock());
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>())).Returns(Task.CompletedTask);
        _profileRepoMock.Setup(r => r.AddAsync(It.IsAny<individual_profiles>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        users? savedUser = null;
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>()))
            .Callback<users>(u => savedUser = u)
            .Returns(Task.CompletedTask);

        var dto = new RegisterIndividualDto
        {
            FirstName = "  أحمد  ",
            LastName = " محمد ",
            NationalId = "1234567890",
            Password = "password123",
            Phone = " +966501234567 ",
            Email = " test@test.com ",
            Gender = " male ",
            Nationality = " سعودي ",
        };

        var result = await _sut.RegisterIndividualAsync(dto);

        savedUser.Should().NotBeNull();
        savedUser!.name.Should().Be("أحمد محمد");
        savedUser!.email.Should().Be("test@test.com");
        savedUser!.phone.Should().Be("+966501234567");
        savedUser!.gender.Should().Be("male");
        savedUser!.nationality.Should().Be("سعودي");
        result.Name.Should().Be("أحمد محمد");
    }

    [Fact]
    public async Task RegisterEntity_TrimsUserEnteredText()
    {
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<users>().AsQueryable().BuildMock());
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>())).Returns(Task.CompletedTask);
        _entityProfileRepoMock.Setup(r => r.AddAsync(It.IsAny<entity_profiles>())).Returns(Task.CompletedTask);
        _contactPersonRepoMock.Setup(r => r.AddAsync(It.IsAny<contact_persons>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _fileStorageMock.Setup(f => f.SaveFileAsync(It.IsAny<IFormFile?>(), It.IsAny<string>()))
            .ReturnsAsync("/uploads/logos/x.png");

        users? savedUser = null;
        entity_profiles? savedProfile = null;
        contact_persons? savedContact = null;
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>()))
            .Callback<users>(u => savedUser = u)
            .Returns(Task.CompletedTask);
        _entityProfileRepoMock.Setup(r => r.AddAsync(It.IsAny<entity_profiles>()))
            .Callback<entity_profiles>(p => savedProfile = p)
            .Returns(Task.CompletedTask);
        _contactPersonRepoMock.Setup(r => r.AddAsync(It.IsAny<contact_persons>()))
            .Callback<contact_persons>(c => savedContact = c)
            .Returns(Task.CompletedTask);

        var dto = new RegisterEntityDto
        {
            CompanyName = " شركة اختبار ",
            NationalId = "7001234567",
            CommercialReg = " 1010123456 ",
            Password = "TestPass123",
            FieldName = " خدمات الحج ",
            Sector = " حج وعمرة ",
            Country = " السعودية ",
            Province = " مكة المكرمة ",
            CompanyDesc = " وصف تجريبي ",
            CompanyWebsite = " https://example.com ",
            Name = " مسؤول الاتصال ",
            Email = " c@example.com ",
            Phone = " +966500000002 ",
            Role = " مدير ",
            Nationality = " سعودي ",
            CompanyLogo = new Mock<IFormFile>().Object,
        };

        var result = await _sut.RegisterEntityAsync(dto);

        savedUser.Should().NotBeNull();
        savedUser!.name.Should().Be("شركة اختبار");
        savedProfile.Should().NotBeNull();
        savedProfile!.commercial_reg.Should().Be("1010123456");
        savedProfile!.company_field.Should().Be("خدمات الحج");
        savedContact.Should().NotBeNull();
        savedContact!.name.Should().Be("مسؤول الاتصال");
        savedContact!.role.Should().Be("مدير");
        result.Name.Should().Be("شركة اختبار");
    }

    // ---- Gap #1 nit fix (2026-10-12): whitespace-only input must NOT become "" ----
    // "   " passes [Required] on the raw DTO, so without post-trim re-validation an
    // empty string would be stored in a required column. Both tests also prove the
    // row is never persisted (AddAsync never fires).

    [Fact]
    public async Task RegisterIndividual_WhitespaceOnlyName_Throws()
    {
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<users>().AsQueryable().BuildMock());

        var dto = new RegisterIndividualDto
        {
            FirstName = "   ",
            LastName = "محمد",
            NationalId = "1234567890",
            Password = "password123",
            Phone = "+966501234567",
            Email = "test@test.com",
            Gender = "male",
            Nationality = "سعودي",
        };

        Func<Task> act = () => _sut.RegisterIndividualAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("الاسم الأول مطلوب");
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<users>()), Times.Never);
    }

    [Fact]
    public async Task RegisterEntity_WhitespaceOnlyCompanyName_Throws()
    {
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<users>().AsQueryable().BuildMock());

        var dto = new RegisterEntityDto
        {
            CompanyName = "   ",
            NationalId = "7001234567",
            CommercialReg = "1010123456",
            Password = "TestPass123",
            FieldName = "خدمات الحج",
            Sector = "حج وعمرة",
            Country = "السعودية",
            Province = "مكة المكرمة",
            CompanyDesc = "وصف تجريبي",
            CompanyWebsite = "https://example.com",
            Name = "مسؤول الاتصال",
            Email = "c@example.com",
            Phone = "+966500000002",
            Role = "مدير",
            Nationality = "سعودي",
        };

        Func<Task> act = () => _sut.RegisterEntityAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("اسم الشركة مطلوب");
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<users>()), Times.Never);
    }
}
