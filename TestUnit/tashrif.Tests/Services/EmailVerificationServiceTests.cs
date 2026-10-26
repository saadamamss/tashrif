using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using tashrif.Data.DTOs.Auth;
using tashrif.Data.Interfaces;
using tashrif.Data.Models;
using tashrif.Identity.Interfaces;
using tashrif.Identity.Services;
using tashrif.Tests.Fakes;
using Xunit;

namespace tashrif.Tests.Services;

/// <summary>
/// Spec-02 / plan §4 — the email-verification auth gates, at the SERVICE level
/// (mock-based; DB-level equivalents already exist in AuthServiceEmailDbTests).
///
/// Covers the plan's required cases:
///   - LoginAsync: unverified → 403; verified → session (D6/D2)
///   - RefreshTokenAsync: unverified → 403 (D6)
///   - GetCurrentUserAsync: unverified → 200 + IsEmailVerified=false, NOT an
///     exception (B6 — useAuth.init() must not log an unverified user out)
///   - VerifyEmailAsync: happy path (auto-login, used_at set, siblings
///     invalidated, user verified), unknown / used / expired tokens (D4/D5)
///   - ResendVerificationAsync: unknown / soft-deleted / already-verified →
///     ShouldSend=false (no enumeration oracle, D8); unverified → rotate (old
///     token soft-deleted) + a fresh 24h token (D5/B5)
/// </summary>
public class EmailVerificationServiceTests
{
    private static readonly DateTime FixedNow =
        new(2026, 10, 22, 12, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IusersRepository> _userRepo = new();
    private readonly Mock<Iindividual_profilesRepository> _profileRepo = new();
    private readonly Mock<Ientity_profilesRepository> _entityProfileRepo = new();
    private readonly Mock<Icontact_personsRepository> _contactPersonRepo = new();
    private readonly Mock<IGenericRepository<email_verification_tokens>> _tokenRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IConfiguration> _config = new();

    private AuthService BuildSut()
    {
        _config.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyForTestPurposesOnly!@#$%");
        _config.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _config.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
        _config.Setup(c => c["Jwt:RefreshKey"]).Returns("SuperSecretRefreshKeyForTestPurposes!@#$%");
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        return new AuthService(
            _userRepo.Object,
            _profileRepo.Object,
            _entityProfileRepo.Object,
            _contactPersonRepo.Object,
            _tokenRepo.Object,
            _uow.Object,
            _config.Object,
            Mock.Of<IFileStorageService>(),
            new FakeClock(FixedNow));
    }

    private static users MakeUser(bool verified = false, bool deleted = false) => new()
    {
        Id = 42,
        national_id = "1010101010",
        name = "مستخدم اختبار",
        email = "gate.user@test.com",
        phone = "+966500000000",
        password_hash = BCrypt.Net.BCrypt.HashPassword("password123"),
        type = "individual",
        gender = "male",
        nationality = "سعودي",
        avatar_url = "",
        is_email_verified = verified,
        IsDeleted = deleted,
        CreatedAt = FixedNow,
        UpdatedAt = FixedNow,
    };

    private static email_verification_tokens MakeToken(
        users user, string hash = "hash-of-somewhere", bool used = false, bool expired = false, int id = 7) =>
        new()
        {
            Id = id,
            user_id = user.Id,
            token_hash = hash,
            expires_at = expired ? FixedNow.AddHours(-1) : FixedNow.AddHours(24),
            used_at = used ? FixedNow.AddMinutes(-5) : null,
            CreatedAt = FixedNow.AddDays(-1),
            UpdatedAt = FixedNow.AddDays(-1),
        };

    /// <summary>Asserts the action throws BadHttpRequestException with the exact status —
    /// the status matters because ExceptionMiddleware trusts ex.StatusCode (D6).</summary>
    private static async Task<BadHttpRequestException> ThrowsWithStatus(Func<Task> act, int status, string because)
    {
        var ex = (await act.Should().ThrowAsync<BadHttpRequestException>(because)).Which;
        ex.StatusCode.Should().Be(status);
        return ex;
    }

    // ---- Login gate (D6) ----

    [Fact]
    public async Task Login_UnverifiedUser_Throws403_WithArabicMessage()
    {
        var user = MakeUser(verified: false);
        _userRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { user }.AsQueryable().BuildMock());
        var sut = BuildSut();

        var ex = await ThrowsWithStatus(
            () => sut.LoginAsync(new LoginDto { NationalId = "1010101010", Password = "password123" }),
            403, "the 403 gate MUST be BadHttpRequestException so ExceptionMiddleware renders 403, not 401 (D6)");

        ex.Message.Should().Be("يجب تفعيل بريدك الإلكتروني قبل تسجيل الدخول");
    }

    [Fact]
    public async Task Login_VerifiedUser_ReturnsSession()
    {
        var user = MakeUser(verified: true);
        _userRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { user }.AsQueryable().BuildMock());
        var sut = BuildSut();

        var result = await sut.LoginAsync(new LoginDto { NationalId = "1010101010", Password = "password123" });

        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("gate.user@test.com");
    }

    // ---- Refresh gate (D6) ----

    [Fact]
    public async Task Refresh_UnverifiedUser_Throws403()
    {
        var user = MakeUser(verified: true);
        _userRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { user }.AsQueryable().BuildMock());
        var sut = BuildSut();

        var login = await sut.LoginAsync(new LoginDto { NationalId = "1010101010", Password = "password123" });

        user.is_email_verified = false; // flag drops between login and refresh
        _userRepo.Setup(r => r.GetByIdAsync(42L)).ReturnsAsync(user);

        var ex = await ThrowsWithStatus(
            () => sut.RefreshTokenAsync(login.RefreshToken),
            403, "a silent logout loop must not be possible when the flag drops mid-session (D6)");

        ex.Message.Should().Be("يجب تفعيل بريدك الإلكتروني قبل تسجيل الدخول");
    }

    // ---- /auth/me stays 200 for unverified (B6) ----

    [Fact]
    public async Task CurrentUser_Unverified_ReturnsFlagFalse_AndDoesNotThrow()
    {
        var user = MakeUser(verified: false);
        _userRepo.Setup(r => r.GetByIdAsync(42L)).ReturnsAsync(user);
        var sut = BuildSut();

        var dto = await sut.GetCurrentUserAsync(42);

        dto.IsEmailVerified.Should().BeFalse(
            "B6: an authenticated-but-unverified user must get a 200; a 403 would break useAuth.init()");
    }

    [Fact]
    public async Task CurrentUser_Verified_ReturnsFlagTrue()
    {
        var user = MakeUser(verified: true);
        _userRepo.Setup(r => r.GetByIdAsync(42L)).ReturnsAsync(user);
        var sut = BuildSut();

        var dto = await sut.GetCurrentUserAsync(42);

        dto.IsEmailVerified.Should().BeTrue();
    }

    // ---- VerifyEmailAction (D4/D5/D7/D11) ----

    [Fact]
    public async Task VerifyEmail_UnknownToken_Throws400()
    {
        _tokenRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(Array.Empty<email_verification_tokens>().AsQueryable().BuildMock());
        var sut = BuildSut();

        var ex = await ThrowsWithStatus(() => sut.VerifyEmailAsync("no-such-token"), 400, "unknown token");
        ex.Message.Should().Be("رابط التحقق غير صالح");
    }

    [Fact]
    public async Task VerifyEmail_BlankToken_Throws400()
    {
        var sut = BuildSut();

        var ex = await ThrowsWithStatus(() => sut.VerifyEmailAsync("   "), 400, "blank token");
        ex.Message.Should().Be("رابط التحقق غير صالح");
    }

    [Fact]
    public async Task VerifyEmail_UsedToken_Throws400()
    {
        var user = MakeUser(verified: false);
        _tokenRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { MakeToken(user, hash: HashOf("raw-token"), used: true) }.AsQueryable().BuildMock());
        var sut = BuildSut();

        var ex = await ThrowsWithStatus(() => sut.VerifyEmailAsync("raw-token"), 400,
            "single-use: a redeemed token must never open a second session");
        ex.Message.Should().Be("رابط التحقق غير صالح");
    }

    [Fact]
    public async Task VerifyEmail_ExpiredToken_Throws400_DelayedMessage()
    {
        var user = MakeUser(verified: false);
        _tokenRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { MakeToken(user, hash: HashOf("raw-token"), expired: true) }.AsQueryable().BuildMock());
        var sut = BuildSut();

        var ex = await ThrowsWithStatus(() => sut.VerifyEmailAsync("raw-token"), 400, "expired token");
        ex.Message.Should().Be("انتهت صلاحية رابط التحقق");
    }

    [Fact]
    public async Task VerifyEmail_DeletedUser_Throws404()
    {
        var user = MakeUser(deleted: true);
        _tokenRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { MakeToken(user, hash: HashOf("raw-token")) }.AsQueryable().BuildMock());
        _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        var sut = BuildSut();

        var ex = await ThrowsWithStatus(() => sut.VerifyEmailAsync("raw-token"), 404, "soft-deleted user");
        ex.Message.Should().Be("المستخدم غير موجود");
    }

    [Fact]
    public async Task VerifyEmail_HappyPath_MarksUsed_VerifiesUser_ReturnsSession()
    {
        var user = MakeUser(verified: false);
        var token = MakeToken(user, hash: HashOf("raw-token"));
        var sibling = MakeToken(user, hash: "sibling-hash", id: 8);
        _tokenRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { token, sibling }.AsQueryable().BuildMock());
        _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        var sut = BuildSut();

        var result = await sut.VerifyEmailAsync("raw-token");

        result.Token.Should().NotBeNullOrEmpty("D7: verification auto-logs the user in");
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.IsEmailVerified.Should().BeTrue();
        result.User.Email.Should().Be("gate.user@test.com");

        var savedToken = UpdatedTokens().Single(t => t.Id == token.Id);
        savedToken.used_at.Should().Be(FixedNow, "IClock must own time, never DateTime.UtcNow");

        var savedSibling = UpdatedTokens().Single(t => t.Id == sibling.Id);
        savedSibling.IsDeleted.Should().BeTrue("D5: only one live token per user");
        savedSibling.DeletedTime.Should().NotBeNull();

        var savedUser = _userRepo.Invocations
            .Where(i => i.Method.Name == nameof(IusersRepository.Update))
            .Select(i => (users)i.Arguments[0])
            .Single();
        savedUser.is_email_verified.Should().BeTrue();
    }

    // ---- Resend (D8 / B5) — the generic-response rules ----

    [Fact]
    public async Task Resend_UnknownNationalId_NoSend()
    {
        _userRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(Array.Empty<users>().AsQueryable().BuildMock());
        var sut = BuildSut();

        var result = await sut.ResendVerificationAsync("0000000000");

        result.ShouldSend.Should().BeFalse("an unknown id must be indistinguishable from a real one (no enumeration oracle)");
        result.VerificationToken.Should().BeEmpty();
        result.Email.Should().BeEmpty();
    }

    [Fact]
    public async Task Resend_BlankNationalId_NoSend()
    {
        var sut = BuildSut();

        var result = await sut.ResendVerificationAsync("  ");

        result.ShouldSend.Should().BeFalse();
    }

    [Fact]
    public async Task Resend_SoftDeletedUser_NoSend()
    {
        _userRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(Array.Empty<users>().AsQueryable().BuildMock());
        var sut = BuildSut();

        var result = await sut.ResendVerificationAsync("1010101010");

        result.ShouldSend.Should().BeFalse("B5: deleted accounts must not be re-verified — the query filter already excludes them");
    }

    [Fact]
    public async Task Resend_AlreadyVerifiedUser_NoSend()
    {
        var user = MakeUser(verified: true);
        _userRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { user }.AsQueryable().BuildMock());
        var sut = BuildSut();

        var result = await sut.ResendVerificationAsync("1010101010");

        result.ShouldSend.Should().BeFalse("B5: re-sending a token to a verified account is pointless");
    }

    [Fact]
    public async Task Resend_UnverifiedUser_RotatesToken_IssuesNew()
    {
        var user = MakeUser(verified: false);
        var oldToken = MakeToken(user, hash: "old-hash");
        _userRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { user }.AsQueryable().BuildMock());
        _tokenRepo.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { oldToken }.AsQueryable().BuildMock());
        _tokenRepo.Setup(r => r.AddAsync(It.IsAny<email_verification_tokens>()))
            .Returns(Task.CompletedTask);
        var sut = BuildSut();

        var result = await sut.ResendVerificationAsync("1010101010");

        result.ShouldSend.Should().BeTrue();
        result.Email.Should().Be(user.email);
        result.Name.Should().Be(user.name);

        oldToken.IsDeleted.Should().BeTrue("D5: rotate = soft-delete the old row");
        oldToken.DeletedTime.Should().NotBeNull();

        var added = _tokenRepo.Invocations
            .Where(i => i.Method.Name == nameof(IGenericRepository<email_verification_tokens>.AddAsync))
            .Select(i => (email_verification_tokens)i.Arguments[0])
            .Single();
        added.user_id.Should().Be(user.Id);
        added.token_hash.Should().NotBeNullOrEmpty().And.HaveLength(64, "SHA-256 hex digest, never the raw token");
        added.expires_at.Should().Be(FixedNow.AddHours(24), "D5: 24h lifetime, set via IClock");
        added.IsDeleted.Should().BeFalse();
    }

    // ---- helpers ----

    private IEnumerable<email_verification_tokens> UpdatedTokens() =>
        _tokenRepo.Invocations
            .Where(i => i.Method.Name == nameof(IGenericRepository<email_verification_tokens>.Update))
            .Select(i => (email_verification_tokens)i.Arguments[0]);

    private static string HashOf(string raw)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
