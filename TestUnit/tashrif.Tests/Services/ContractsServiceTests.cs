using Microsoft.EntityFrameworkCore;
using tashrif.Context;
using tashrif.Core;
using tashrif.Data;
using tashrif.Data.DTOs;

namespace tashrif.Tests.Services;

public class ContractsServiceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public ContractsServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task SeedTestData(tashrifDBContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            DELETE FROM contracts;
            DELETE FROM interviews;
            DELETE FROM applications;
            DELETE FROM jobs;
            DELETE FROM users;
        ");

        var entity = new users
        {
            national_id = "2020202020",
            name = "شركة تجريبية",
            email = "entity@test.com",
            phone = "0500000000",
            password_hash = "hash",
            type = "entity",
            gender = "male",
            nationality = "سعودي",
            avatar_url = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        db.users.Add(entity);
        await db.SaveChangesAsync();

        var individual = new users
        {
            national_id = "1010101010",
            name = "أحمد",
            email = "ahmed@test.com",
            phone = "0511111111",
            password_hash = "hash",
            type = "individual",
            gender = "male",
            nationality = "سعودي",
            avatar_url = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        db.users.Add(individual);
        await db.SaveChangesAsync();

        var job = new jobs
        {
            title = "مبرمج",
            description = "وصف الوظيفة",
            location = "الرياض",
            work_type = "full_time",
            target = "individuals",
            vacancies = 5,
            qualification = "بكالوريوس",
            salary_min = 5000,
            salary_max = 10000,
            salary_text = "5000-10000",
            gender = "male",
            hours = "8",
            duration = "6months",
            status = "published",
            entity_id = entity.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        db.jobs.Add(job);
        await db.SaveChangesAsync();

        var app = new applications
        {
            job_id = job.Id,
            user_id = individual.Id,
            qualification = "بكالوريوس",
            experience = "3 سنوات",
            cover_letter = "أرغب في العمل",
            status = "new",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        db.applications.Add(app);
        await db.SaveChangesAsync();

        // Clear change tracker so seed entities don't conflict with service-loaded entities
        // (GetQueryable() returns AsNoTracking, creating a second instance of the same entity)
        db.ChangeTracker.Clear();
    }

    [Fact]
    public async Task Send_CreatesContractWithSentStatus()
    {
        var (sut, db, scope) = _fixture.CreateScopedService();
        try
        {
            await SeedTestData(db);

            var app = await db.applications.FirstAsync();
            var entity = await db.users.FirstAsync(u => u.type == "entity");

            var result = await sut.SendAsync(new SendContractDto { ApplicationId = app.Id }, entity.Id);

            result.Should().NotBeNull();
            result.Status.Should().Be("sent");
            result.JobTitle.Should().Be("مبرمج");
            result.UserName.Should().Be("أحمد");
            result.UserEmail.Should().Be("ahmed@test.com");

            // ExecuteUpdateAsync bypasses change tracker — clear it to read fresh from DB
            db.ChangeTracker.Clear();
            var updatedApp = await db.applications.FindAsync(app.Id);
            updatedApp!.status.Should().Be("contract_sent");
        }
        finally { scope.Dispose(); }
    }

    [Fact]
    public async Task Send_NonExistingApplication_ThrowsKeyNotFoundException()
    {
        var (sut, db, _) = _fixture.CreateScopedService();
        try
        {
            var act = () => sut.SendAsync(new SendContractDto { ApplicationId = 99999 }, 1);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
        finally { }
    }

    [Fact]
    public async Task Send_JobOwnedByAnotherEntity_ThrowsUnauthorizedAccessException()
    {
        var (sut, db, _) = _fixture.CreateScopedService();
        try
        {
            await SeedTestData(db);
            var app = await db.applications.FirstAsync();
            var act = () => sut.SendAsync(new SendContractDto { ApplicationId = app.Id }, 99999);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
        finally { }
    }

    [Fact]
    public async Task Send_ApplicationAlreadyContractSent_ThrowsBadHttpRequestException()
    {
        var (sut, db, _) = _fixture.CreateScopedService();
        try
        {
            await SeedTestData(db);
            var app = await db.applications.FirstAsync();
            var entity = await db.users.FirstAsync(u => u.type == "entity");

            await sut.SendAsync(new SendContractDto { ApplicationId = app.Id }, entity.Id);

            var act = () => sut.SendAsync(new SendContractDto { ApplicationId = app.Id }, entity.Id);
            await act.Should().ThrowAsync<BadHttpRequestException>();
        }
        finally { }
    }

    [Fact]
    public async Task Sign_UpdatesStatusToSigned()
    {
        var (sut, db, _) = _fixture.CreateScopedService();
        try
        {
            await SeedTestData(db);
            var app = await db.applications.FirstAsync();
            var individual = await db.users.FirstAsync(u => u.type == "individual");
            var entity = await db.users.FirstAsync(u => u.type == "entity");

            var contractResult = await sut.SendAsync(new SendContractDto { ApplicationId = app.Id }, entity.Id);
            var result = await sut.SignAsync(contractResult.Id, individual.Id);

            result.Should().NotBeNull();
            result.Status.Should().Be("signed");
            result.SignedAt.Should().NotBeNull();
        }
        finally { }
    }

    [Fact]
    public async Task Send_WithNotesAndEndDate_PersistsThem()
    {
        var (sut, db, _) = _fixture.CreateScopedService();
        try
        {
            await SeedTestData(db);
            var app = await db.applications.FirstAsync();
            var entity = await db.users.FirstAsync(u => u.type == "entity");
            var endDate = DateTime.UtcNow.AddDays(7);

            var dto = new SendContractDto
            {
                ApplicationId = app.Id,
                Notes = "يرجى التوقيع قبل بداية الموسم",
                EndDate = endDate,
            };

            var result = await sut.SendAsync(dto, entity.Id);

            result.Notes.Should().Be("يرجى التوقيع قبل بداية الموسم");
            result.EndDate.Should().Be(endDate);

            var savedContract = await db.contracts.FindAsync(result.Id);
            savedContract!.notes.Should().Be("يرجى التوقيع قبل بداية الموسم");
            savedContract.end_date.Should().NotBeNull();
        }
        finally { }
    }

    [Fact]
    public async Task Sign_SetsApplicationAccepted()
    {
        var (sut, db, _) = _fixture.CreateScopedService();
        try
        {
            await SeedTestData(db);
            var app = await db.applications.FirstAsync();
            var individual = await db.users.FirstAsync(u => u.type == "individual");
            var entity = await db.users.FirstAsync(u => u.type == "entity");

            var contractResult = await sut.SendAsync(new SendContractDto { ApplicationId = app.Id }, entity.Id);
            var result = await sut.SignAsync(contractResult.Id, individual.Id);

            result.Status.Should().Be("signed");
            db.ChangeTracker.Clear();
            var updatedApp = await db.applications.FindAsync(app.Id);
            updatedApp!.status.Should().Be("accepted");
        }
        finally { }
    }

    [Fact]
    public async Task Sign_AlreadySigned_ThrowsBadHttpRequestException()
    {
        var (sut, db, _) = _fixture.CreateScopedService();
        try
        {
            await SeedTestData(db);
            var app = await db.applications.FirstAsync();
            var individual = await db.users.FirstAsync(u => u.type == "individual");
            var entity = await db.users.FirstAsync(u => u.type == "entity");

            var contractResult = await sut.SendAsync(new SendContractDto { ApplicationId = app.Id }, entity.Id);
            await sut.SignAsync(contractResult.Id, individual.Id);

            var act = () => sut.SignAsync(contractResult.Id, individual.Id);
            await act.Should().ThrowAsync<BadHttpRequestException>();
        }
        finally { }
    }

    [Fact]
    public async Task Sign_WrongUser_ThrowsUnauthorizedAccessException()
    {
        var (sut, db, _) = _fixture.CreateScopedService();
        try
        {
            await SeedTestData(db);
            var app = await db.applications.FirstAsync();
            var entity = await db.users.FirstAsync(u => u.type == "entity");

            var contractResult = await sut.SendAsync(new SendContractDto { ApplicationId = app.Id }, entity.Id);

            var act = () => sut.SignAsync(contractResult.Id, entity.Id);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
        finally { }
    }
}
