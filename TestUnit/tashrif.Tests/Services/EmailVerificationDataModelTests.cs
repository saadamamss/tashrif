using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using tashrif.Context;

namespace tashrif.Tests.Services;

/// <summary>
/// Spec 01 — email verification data model.
///
/// Covers the three things Spec 01 introduces:
///   1. the `email_verification_tokens` entity shape + its DI/repository wiring,
///   2. the `users.is_email_verified` flag (default false, backfilled true),
///   3. D12 — email uniqueness: lowercase normalization on write + a 409 guard on
///      duplicate registration (the DB has a UNIQUE index, so an unguarded duplicate
///      would surface as a raw DbUpdateException → HTTP 500).
/// </summary>
public class EmailVerificationDataModelTests
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

    public EmailVerificationDataModelTests()
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

    private static RegisterIndividualDto MakeIndividualDto(string email = "new.user@test.com") => new()
    {
        FirstName = "أحمد",
        LastName = "محمد",
        NationalId = "1234567890",
        Password = "password123",
        Phone = "+966501234567",
        Email = email,
        Gender = "ذكر",
        Nationality = "سعودي"
    };

    // ---- 1. The token entity + DI wiring ----

    [Fact]
    public void EmailVerificationToken_HasExpectedShape()
    {
        var token = new email_verification_tokens
        {
            Id = 1,
            user_id = 42,
            token_hash = "a".PadRight(64, '0'),
            expires_at = DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        token.Id.Should().Be(1);
        token.user_id.Should().Be(42);
        token.token_hash.Should().NotBeNullOrEmpty();
        token.expires_at.Should().BeAfter(DateTime.UtcNow);
        token.used_at.Should().BeNull("a fresh token has not been redeemed yet");
        token.sent_at.Should().BeNull();
        token.last_error.Should().BeNull();
        token.IsDeleted.Should().BeFalse();
        token.DeletedTime.Should().BeNull();
    }

    [Fact]
    public void EmailVerificationTokensRepository_ResolvesFromDependencyInjection()
    {
        // Proves the Spec 01 wiring end-to-end: the repository interface, its
        // implementation, and the DI registration all exist and resolve.
        var services = new ServiceCollection();
        services.AddDbContext<tashrifDBContext>(o =>
            o.UseNpgsql("Host=localhost;Database=does_not_matter;Username=u;Password=p"));
        services.AddtashrifRepositoryDependencies();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<Iemail_verification_tokensRepository>();
        repo.Should().NotBeNull();

        // The generic registration is what spec-02's AuthService will depend on.
        var generic = scope.ServiceProvider.GetRequiredService<IGenericRepository<email_verification_tokens>>();
        generic.Should().NotBeNull();
    }

    [Fact]
    public void UnitOfWork_ExposesEmailVerificationTokensRepository()
    {
        var services = new ServiceCollection();
        services.AddDbContext<tashrifDBContext>(o =>
            o.UseNpgsql("Host=localhost;Database=does_not_matter;Username=u;Password=p"));
        services.AddtashrifRepositoryDependencies();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        uow.Email_verification_tokensRepository.Should().NotBeNull();
    }

    // ---- 2. The is_email_verified flag ----

    [Fact]
    public void NewUser_DefaultsToUnverified()
    {
        var user = new users { email = "a@b.com" };

        user.is_email_verified.Should().BeFalse("new registrations start unverified (D2)");
    }

    [Fact]
    public async Task RegisterIndividual_NewUser_IsNotVerified()
    {
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<users>().AsQueryable().BuildMock());
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>())).Returns(Task.CompletedTask);
        _profileRepoMock.Setup(r => r.AddAsync(It.IsAny<individual_profiles>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        users? saved = null;
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>()))
            .Callback<users>(u => saved = u)
            .Returns(Task.CompletedTask);

        await _sut.RegisterIndividualAsync(MakeIndividualDto());

        saved.Should().NotBeNull();
        saved!.is_email_verified.Should().BeFalse();
    }

    // ---- 3. D12: email uniqueness + normalization ----

    [Fact]
    public async Task RegisterIndividual_StoresEmailLowercased()
    {
        _userRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(new List<users>().AsQueryable().BuildMock());
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>())).Returns(Task.CompletedTask);
        _profileRepoMock.Setup(r => r.AddAsync(It.IsAny<individual_profiles>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        users? saved = null;
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<users>()))
            .Callback<users>(u => saved = u)
            .Returns(Task.CompletedTask);

        await _sut.RegisterIndividualAsync(MakeIndividualDto("  MiXeD@Example.COM  "));

        saved.Should().NotBeNull();
        saved!.email.Should().Be("mixed@example.com");
    }

    [Fact]
    public async Task RegisterIndividual_DuplicateEmail_Throws409()
    {
        var existing = new users { Id = 7, email = "taken@example.com" };
        _userRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { existing }.AsQueryable().BuildMock());

        var act = () => _sut.RegisterIndividualAsync(MakeIndividualDto("taken@example.com"));

        await act.Should().ThrowAsync<BadHttpRequestException>()
            .WithMessage("*البريد الإلكتروني مستخدم مسبقا*");

        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<users>()), Times.Never,
            "a duplicate email must not create a user row");
    }

    [Fact]
    public async Task RegisterIndividual_DuplicateEmailDifferentCase_Throws409()
    {
        // Emails are stored lowercase, so a case variant is the SAME address.
        var existing = new users { Id = 7, email = "taken@example.com" };
        _userRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { existing }.AsQueryable().BuildMock());

        var act = () => _sut.RegisterIndividualAsync(MakeIndividualDto("TAKEN@EXAMPLE.COM"));

        await act.Should().ThrowAsync<BadHttpRequestException>()
            .WithMessage("*البريد الإلكتروني مستخدم مسبقا*");
    }

    [Fact]
    public async Task RegisterEntity_DuplicateEmail_Throws409()
    {
        var existing = new users { Id = 7, email = "taken@example.com" };
        _userRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { existing }.AsQueryable().BuildMock());

        var dto = new RegisterEntityDto
        {
            CompanyName = "شركة اختبار",
            NationalId = "7001234567",
            CommercialReg = "1010123456",
            Password = "TestPass123",
            FieldName = "خدمات الحج",
            Sector = "حج وعمرة",
            Country = "السعودية",
            Province = "الرياض",
            CompanyDesc = "وصف الشركة",
            CompanyWebsite = "https://example.com",
            Name = "سعد",
            Role = "مدير",
            Nationality = "سعودي",
            Email = "TAKEN@example.com",
            Phone = "+966501234567"
        };

        var act = () => _sut.RegisterEntityAsync(dto);

        await act.Should().ThrowAsync<BadHttpRequestException>()
            .WithMessage("*البريد الإلكتروني مستخدم مسبقا*");
    }

    [Fact]
    public async Task RegisterIndividual_DuplicateEmailOfSoftDeletedUser_Throws409()
    {
        // The UNIQUE index on users.email spans ALL rows, soft-deleted ones included.
        // The guard must therefore look past the global IsDeleted query filter — otherwise
        // this registration would pass the guard and fail at SaveChanges with a
        // DbUpdateException → HTTP 500 instead of a clean 409.
        var deleted = new users { Id = 9, email = "gone@example.com", IsDeleted = true };
        _userRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { deleted }.AsQueryable().BuildMock());

        var act = () => _sut.RegisterIndividualAsync(MakeIndividualDto("gone@example.com"));

        await act.Should().ThrowAsync<BadHttpRequestException>()
            .WithMessage("*البريد الإلكتروني مستخدم مسبقا*");

        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<users>()), Times.Never);
    }

    // ---- 4. The EF model itself ----

    [Fact]
    public void DbContext_Model_ContainsEmailVerificationArtifacts()
    {
        var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<tashrifDBContext>()
            .UseNpgsql("Host=localhost;Database=does_not_matter;Username=u;Password=p")
            .Options;

        using var ctx = new tashrifDBContext(options);
        var model = ctx.Model;

        var tokenType = model.FindEntityType(typeof(email_verification_tokens));
        tokenType.Should().NotBeNull("the token entity must be part of the EF model");

        tokenType!.FindProperty("token_hash")!.IsNullable.Should().BeFalse();
        tokenType.FindProperty("expires_at")!.IsNullable.Should().BeFalse();
        tokenType.FindProperty("used_at")!.IsNullable.Should().BeTrue();
        tokenType.FindProperty("sent_at")!.IsNullable.Should().BeTrue();
        tokenType.FindProperty("last_error")!.IsNullable.Should().BeTrue();

        // token_hash must be uniquely indexed — the raw token is looked up by its hash.
        var tokenHashIndex = tokenType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Count == 1 && i.Properties[0].Name == "token_hash");
        tokenHashIndex.Should().NotBeNull();
        tokenHashIndex!.IsUnique.Should().BeTrue();

        // users.email must be uniquely indexed (D12).
        var userType = model.FindEntityType(typeof(users));
        var emailIndex = userType!.GetIndexes()
            .FirstOrDefault(i => i.Properties.Count == 1 && i.Properties[0].Name == "email");
        emailIndex.Should().NotBeNull();
        emailIndex!.IsUnique.Should().BeTrue();

        // The verification flag must exist on users.
        userType.FindProperty("is_email_verified").Should().NotBeNull();
    }
}
