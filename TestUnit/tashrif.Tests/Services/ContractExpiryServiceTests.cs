using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using tashrif.API.Services;
using tashrif.Context;
using tashrif.Core;
using tashrif.Data;
using tashrif.Data.Models;
using tashrif.Tests.Fakes;

namespace tashrif.Tests.Services;

// Spec 1 (contract expiry): overdue `sent` contracts flip to `expired` with one
// DB notification per side. Hub/email are absent from the fixture (API/Email layers)
// so fan-out is skipped there and asserted only via the notifications table.
// Real-DB fixture classes wipe shared tables in seed — serialize them.
[CollectionDefinition("SequentialDb", DisableParallelization = true)]
public class SequentialDbCollection { }

[Collection("SequentialDb")]
public class ContractExpiryServiceTests : IClassFixture<TestDatabaseFixture>
{
    private static readonly DateTime Now = new(2026, 10, 6, 12, 0, 0, DateTimeKind.Utc);
    private readonly TestDatabaseFixture _fixture;

    public ContractExpiryServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private static async Task SeedAsync(tashrifDBContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            DELETE FROM notifications;
            DELETE FROM contracts;
            DELETE FROM interviews;
            DELETE FROM applications;
            DELETE FROM jobs;
            DELETE FROM users;
        ");

        var entity = new users
        {
            national_id = "2020202020", name = "شركة تجريبية", email = "entity@test.com",
            phone = "0500000000", password_hash = "hash", type = "entity",
            gender = "male", nationality = "سعودي", avatar_url = "",
            CreatedAt = Now, UpdatedAt = Now,
        };
        var individual = new users
        {
            national_id = "1010101010", name = "أحمد", email = "ahmed@test.com",
            phone = "0511111111", password_hash = "hash", type = "individual",
            gender = "male", nationality = "سعودي", avatar_url = "",
            CreatedAt = Now, UpdatedAt = Now,
        };
        db.users.AddRange(entity, individual);
        await db.SaveChangesAsync();

        var job = new jobs
        {
            title = "مبرمج", description = "وصف", location = "makkah", work_type = "full-time",
            target = "both", vacancies = 1, qualification = "بكالوريوس", salary_text = "8000",
            gender = "both", hours = "8", duration = "month", status = "active",
            entity_id = entity.Id, CreatedAt = Now, UpdatedAt = Now,
        };
        db.jobs.Add(job);
        await db.SaveChangesAsync();

        var app = new applications
        {
            job_id = job.Id, user_id = individual.Id, qualification = "بكالوريوس",
            experience = "سنة", cover_letter = "أرغب في العمل",
            status = "contract_sent", CreatedAt = Now, UpdatedAt = Now,
        };
        db.applications.Add(app);
        await db.SaveChangesAsync();

        contracts Row(string status, DateTime? endDate) => new()
        {
            application_id = app.Id, job_id = job.Id, user_id = individual.Id, entity_id = entity.Id,
            file_url = "", status = status, end_date = endDate, CreatedAt = Now, UpdatedAt = Now,
        };

        db.contracts.AddRange(
            Row("sent", Now.AddDays(-1)),    // overdue → flips
            Row("sent", Now.AddDays(2)),     // live → stays
            Row("signed", Now.AddDays(-1)),  // signed past deadline → stays signed
            Row("expired", Now.AddDays(-1))  // already expired → stays, no duplicate notify
        );
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }

    [Fact]
    public async Task ExpireContracts_FlipsOnlyOverdueSent()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();
        await SeedAsync(db);

        var sut = new ContractExpiryService(
            _fixture.ServiceProvider, new FakeClock(Now), NullLogger<ContractExpiryService>.Instance);

        var flipped = await sut.ExpireContractsAsync();

        flipped.Should().Be(1);
        db.ChangeTracker.Clear();
        var statuses = await db.contracts.OrderBy(c => c.Id).Select(c => c.status).ToListAsync();
        statuses.Should().Equal("expired", "sent", "signed", "expired");
    }

    [Fact]
    public async Task ExpireContracts_SharedUsersAcrossOverdue_DoesNotThrowTrackingConflict()
    {
        // Regression: overdue rows sharing one user/entity/job must not trip the
        // AsNoTracking + Update() duplicate-tracking trap (thrown hourly in prod).
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();
        await SeedAsync(db);

        var job = await db.jobs.FirstAsync();
        var individual = await db.users.FirstAsync(u => u.type == "individual");
        var app2 = new applications
        {
            job_id = job.Id, user_id = individual.Id, qualification = "بكالوريوس",
            experience = "سنة", cover_letter = "أرغب في العمل",
            status = "contract_sent", CreatedAt = Now, UpdatedAt = Now,
        };
        db.applications.Add(app2);
        await db.SaveChangesAsync();
        db.contracts.Add(new contracts
        {
            application_id = app2.Id, job_id = job.Id, user_id = individual.Id,
            entity_id = (await db.users.FirstAsync(u => u.type == "entity")).Id,
            file_url = "", status = "sent", end_date = Now.AddDays(-2),
            CreatedAt = Now, UpdatedAt = Now,
        });
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var sut = new ContractExpiryService(
            _fixture.ServiceProvider, new FakeClock(Now), NullLogger<ContractExpiryService>.Instance);

        var flipped = await sut.ExpireContractsAsync();

        flipped.Should().Be(2);
        db.ChangeTracker.Clear();
        (await db.contracts.CountAsync(c => c.status == "expired")).Should().Be(3);
    }

    [Fact]
    public async Task ExpireContracts_NotifiesBothSidesOnce()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();
        await SeedAsync(db);

        var sut = new ContractExpiryService(
            _fixture.ServiceProvider, new FakeClock(Now), NullLogger<ContractExpiryService>.Instance);

        await sut.ExpireContractsAsync();

        var notes = await db.notifications.Where(n => n.type == "contract_expired").ToListAsync();
        notes.Should().HaveCount(2);
        var individualId = db.users.Where(u => u.type == "individual").Select(u => u.Id).Single();
        var entityId = db.users.Where(u => u.type == "entity").Select(u => u.Id).Single();
        notes.Select(n => n.user_id).Should().BeEquivalentTo(new[] { individualId, entityId });

        // Second run finds nothing overdue — no duplicate notifications
        var again = await sut.ExpireContractsAsync();
        again.Should().Be(0);
        (await db.notifications.CountAsync(n => n.type == "contract_expired")).Should().Be(2);
    }
}
