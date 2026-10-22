using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using tashrif.Context;
using tashrif.Data;
using tashrif.Data.DTOs.Auth;
using tashrif.Identity.Interfaces;
using tashrif.Identity.Services;

namespace tashrif.Tests.Services;

/// <summary>
/// Spec 01 — the duplicate-email guard, verified against the real database.
///
/// The mock-based tests in AuthServiceTests cannot catch the soft-delete gap:
/// the in-memory TestAsyncQueryProvider never applies EF's global IsDeleted
/// filter, so a guard that omitted IgnoreQueryFilters would still "pass".
/// These tests run against tashrif_test, where the filter is real.
/// </summary>
[Collection("SequentialDb")]
public class AuthServiceEmailDbTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public AuthServiceEmailDbTests(TestDatabaseFixture fixture) => _fixture = fixture;

    private static AuthService BuildAuthService(IServiceProvider root)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "SuperSecretKeyForTestPurposesOnly!@#$%",
            ["Jwt:Issuer"] = "test-issuer",
            ["Jwt:Audience"] = "test-audience",
            ["Jwt:RefreshKey"] = "SuperSecretRefreshKeyForTestPurposes!@#$%",
        }).Build();

        var scope = root.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorageService>();

        return new AuthService(
            uow.UsersRepository,
            uow.Individual_profilesRepository,
            uow.Entity_profilesRepository,
            uow.Contact_personsRepository,
            uow,
            config,
            fileStorage);
    }

    private static RegisterIndividualDto MakeDto(string nationalId, string email) => new()
    {
        FirstName = "أحمد",
        LastName = "محمد",
        NationalId = nationalId,
        Password = "password123",
        Phone = "+966501234567",
        Email = email,
        Gender = "ذكر",
        Nationality = "سعودي"
    };

    [Fact]
    public async Task RegisterIndividual_SoftDeletedUserHoldsEmail_Throws409_Not500()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();
        await db.Database.ExecuteSqlRawAsync("DELETE FROM individual_profiles; DELETE FROM users;");

        // A deactivated account still holds the address in the UNIQUE index.
        db.users.Add(new users
        {
            national_id = "9501000001",
            name = "محذوف",
            email = "softdeleted@example.com",
            phone = "0500000000",
            password_hash = "hash",
            type = "individual",
            gender = "male",
            nationality = "سعودي",
            avatar_url = "",
            IsDeleted = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        var auth = BuildAuthService(_fixture.ServiceProvider);

        // Without IgnoreQueryFilters the guard would miss the soft-deleted row, the
        // INSERT would violate the UNIQUE index, and the caller would get a 500.
        var act = async () => await auth.RegisterIndividualAsync(
            MakeDto("9501000002", "softdeleted@example.com"));

        await act.Should().ThrowAsync<BadHttpRequestException>()
            .WithMessage("*البريد الإلكتروني مستخدم مسبقا*");

        // No new user row was created (IgnoreQueryFilters — the seeded user is soft-deleted).
        var count = await db.users.AsNoTracking().IgnoreQueryFilters().CountAsync();
        count.Should().Be(1);

        await db.Database.ExecuteSqlRawAsync("DELETE FROM individual_profiles; DELETE FROM users;");
    }

    [Fact]
    public async Task RegisterIndividual_LiveUserHoldsEmail_Throws409()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();
        await db.Database.ExecuteSqlRawAsync("DELETE FROM individual_profiles; DELETE FROM users;");

        db.users.Add(new users
        {
            national_id = "9601000001",
            name = "حي",
            email = "live@example.com",
            phone = "0500000000",
            password_hash = "hash",
            type = "individual",
            gender = "male",
            nationality = "سعودي",
            avatar_url = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        var auth = BuildAuthService(_fixture.ServiceProvider);

        var act = async () => await auth.RegisterIndividualAsync(
            MakeDto("9601000002", "live@example.com"));

        await act.Should().ThrowAsync<BadHttpRequestException>()
            .WithMessage("*البريد الإلكتروني مستخدم مسبقا*");

        await db.Database.ExecuteSqlRawAsync("DELETE FROM individual_profiles; DELETE FROM users;");
    }

    [Fact]
    public async Task RegisterIndividual_StoresEmailLowercased_AndUnverified()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();
        await db.Database.ExecuteSqlRawAsync("DELETE FROM individual_profiles; DELETE FROM users;");

        var auth = BuildAuthService(_fixture.ServiceProvider);

        await auth.RegisterIndividualAsync(MakeDto("9701000001", "  NEW.User@Example.COM  "));

        var stored = await db.users.AsNoTracking().FirstAsync(u => u.national_id == "9701000001");
        stored.email.Should().Be("new.user@example.com");
        stored.is_email_verified.Should().BeFalse("new registrations start unverified (D2)");

        await db.Database.ExecuteSqlRawAsync("DELETE FROM individual_profiles; DELETE FROM users;");
    }
}
