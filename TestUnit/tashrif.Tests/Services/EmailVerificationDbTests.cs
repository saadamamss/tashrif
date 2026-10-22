using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;
using tashrif.Context;
using tashrif.Data;

namespace tashrif.Tests.Services;

/// <summary>
/// Spec 01 — database-level verification of the email-verification schema.
///
/// These run against the real PostgreSQL test database (tashrif_test), so they
/// exercise the things a mock cannot: the global IsDeleted query filter, the
/// UNIQUE index, the cascade FK, and the BEFORE-trigger that lowercases emails
/// on every write.
/// </summary>
[Collection("SequentialDb")]
public class EmailVerificationDbTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public EmailVerificationDbTests(TestDatabaseFixture fixture) => _fixture = fixture;

    private static users MakeUser(long id, string nationalId, string email, bool isDeleted = false) => new()
    {
        Id = id,
        national_id = nationalId,
        name = "مستخدم اختبار",
        email = email,
        phone = "0500000000",
        password_hash = "hash",
        type = "individual",
        gender = "male",
        nationality = "سعودي",
        avatar_url = "",
        IsDeleted = isDeleted,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    /// <summary>Runs a scalar query via raw ADO.NET (SqlQueryRaw would require a "Value" column alias).</summary>
    private static async Task<T> ScalarAsync<T>(tashrifDBContext db, string sql)
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        var result = await cmd.ExecuteScalarAsync();
        return (T)Convert.ChangeType(result!, typeof(T));
    }

    [Fact]
    public async Task EmailVerificationTokens_TableExists_WithExpectedColumnsAndIndexes()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();

        var tableExists = await ScalarAsync<int>(db,
            "SELECT count(*)::int FROM information_schema.tables WHERE table_name = 'email_verification_tokens'");
        tableExists.Should().Be(1);

        // token_hash must carry a UNIQUE index.
        var uniqueTokenHash = await ScalarAsync<int>(db,
            """
            SELECT count(*)::int FROM pg_indexes
            WHERE tablename = 'email_verification_tokens'
              AND indexdef LIKE '%UNIQUE%'
              AND indexdef LIKE '%token_hash%'
            """);
        uniqueTokenHash.Should().Be(1);

        // users.email must carry a UNIQUE index.
        var uniqueEmail = await ScalarAsync<int>(db,
            """
            SELECT count(*)::int FROM pg_indexes
            WHERE tablename = 'users'
              AND indexdef LIKE '%UNIQUE%'
              AND indexdef LIKE '%email%'
            """);
        uniqueEmail.Should().Be(1);

        // Cascade delete from users → tokens.
        var cascade = await ScalarAsync<string>(db,
            """
            SELECT pg_get_constraintdef(oid) FROM pg_constraint
            WHERE conrelid = 'email_verification_tokens'::regclass AND contype = 'f'
            """);
        cascade.Should().Contain("CASCADE");
    }

    [Fact]
    public async Task IsEmailVerified_ColumnDefaultsToFalse()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();

        var colDefault = await ScalarAsync<string>(db,
            """
            SELECT column_default FROM information_schema.columns
            WHERE table_name = 'users' AND column_name = 'is_email_verified'
            """);
        colDefault.Should().Contain("false");
    }

    [Fact]
    public async Task Trigger_NormalizesEmailOnInsert_AndRejectsCaseVariantDuplicate()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();

        await db.Database.ExecuteSqlRawAsync("DELETE FROM email_verification_tokens; DELETE FROM users;");

        db.users.Add(MakeUser(9001, "9001000001", "  MiXeD@Example.COM  "));
        await db.SaveChangesAsync();

        // The trigger lowercased + trimmed on write, regardless of the C# value.
        var stored = await ScalarAsync<string>(db,
            "SELECT email FROM users WHERE \"Id\" = 9001");
        stored.Should().Be("mixed@example.com");

        // A case variant of the SAME address must be rejected by the UNIQUE index
        // (this is what the BEFORE-trigger makes possible without `citext`).
        db.users.Add(MakeUser(9002, "9002000002", "MIXED@EXAMPLE.COM"));
        var act = async () => await db.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();

        await db.Database.ExecuteSqlRawAsync("DELETE FROM users;");
    }

    [Fact]
    public async Task SoftDeletedUser_StillOccupiesUniqueEmailIndex_AndIsHiddenByQueryFilter()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();

        await db.Database.ExecuteSqlRawAsync("DELETE FROM email_verification_tokens; DELETE FROM users;");

        db.users.Add(MakeUser(9101, "9101000001", "taken@example.com", isDeleted: true));
        await db.SaveChangesAsync();

        // The global IsDeleted filter hides it from ordinary queries...
        var visible = await db.users.CountAsync(u => u.email == "taken@example.com");
        visible.Should().Be(0);

        // ...but the UNIQUE index still covers it, so the address is genuinely taken.
        db.users.Add(MakeUser(9102, "9102000002", "taken@example.com"));
        var act = async () => await db.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();

        await db.Database.ExecuteSqlRawAsync("DELETE FROM users;");
    }

    [Fact]
    public async Task Token_CascadeDeletes_WhenUserRowIsHardDeleted()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();

        await db.Database.ExecuteSqlRawAsync("DELETE FROM email_verification_tokens; DELETE FROM users;");

        var user = MakeUser(9201, "9201000001", "cascade@example.com");
        db.users.Add(user);
        await db.SaveChangesAsync();

        db.email_verification_tokens.Add(new email_verification_tokens
        {
            user_id = user.Id,
            token_hash = new string('a', 64),
            expires_at = DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        await db.Database.ExecuteSqlRawAsync("DELETE FROM users WHERE \"Id\" = 9201;");

        var remaining = await ScalarAsync<int>(db,
            "SELECT count(*)::int FROM email_verification_tokens");
        remaining.Should().Be(0);
    }
}
