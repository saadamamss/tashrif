namespace tashrif.Tests.Services;

public class ProfileServiceTests
{
    [Fact]
    public async Task IndividualStats_ReturnsCorrectCounts()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var appsRepoMock = new Mock<IapplicationsRepository>();
        var interviewsRepoMock = new Mock<IinterviewsRepository>();
        var contractsRepoMock = new Mock<IcontractsRepository>();
        var profilesRepoMock = new Mock<Iindividual_profilesRepository>();
        var usersRepoMock = new Mock<IusersRepository>();

        var apps = new List<applications>
        {
            new() { user_id = 1, status = "new", CreatedAt = DateTime.UtcNow },
            new() { user_id = 1, status = "pending", CreatedAt = DateTime.UtcNow },
            new() { user_id = 1, status = "accepted", CreatedAt = DateTime.UtcNow },
            new() { user_id = 1, status = "rejected", CreatedAt = DateTime.UtcNow },
            new() { user_id = 1, status = "new", CreatedAt = DateTime.UtcNow },
        }.AsQueryable().BuildMock();

        var interviews = new List<interviews>
        {
            new() { user_id = 1 },
            new() { user_id = 1 },
        }.AsQueryable().BuildMock();

        var contracts = new List<contracts>
        {
            new() { user_id = 1 },
        }.AsQueryable().BuildMock();

        uowMock.SetupGet(u => u.ApplicationsRepository).Returns(appsRepoMock.Object);
        uowMock.SetupGet(u => u.InterviewsRepository).Returns(interviewsRepoMock.Object);
        uowMock.SetupGet(u => u.ContractsRepository).Returns(contractsRepoMock.Object);
        uowMock.SetupGet(u => u.Individual_profilesRepository).Returns(profilesRepoMock.Object);
        uowMock.SetupGet(u => u.UsersRepository).Returns(usersRepoMock.Object);

        appsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(apps);
        interviewsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(interviews);
        contractsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(contracts);

        var service = new individual_profilesService(uowMock.Object);
        var result = await service.GetStatsAsync(1);

        result.Should().NotBeNull();
        result.TotalApplications.Should().Be(5);
        result.PendingApps.Should().Be(3);
        result.Interviews.Should().Be(2);
        result.Contracts.Should().Be(1);
    }

    [Fact]
    public async Task EntityStats_ReturnsCorrectCounts()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var profilesRepoMock = new Mock<Ientity_profilesRepository>();
        var jobsRepoMock = new Mock<IjobsRepository>();
        var appsRepoMock = new Mock<IapplicationsRepository>();
        var usersRepoMock = new Mock<IusersRepository>();
        var contactsRepoMock = new Mock<Icontact_personsRepository>();

        var profile = new entity_profiles { Id = 1, user_id = 1 };
        var profiles = new[] { profile }.AsQueryable().BuildMock();

        var jobs = new List<jobs>
        {
            new() { Id = 1, entity_id = 1, status = "active" },
            new() { Id = 2, entity_id = 1, status = "active" },
            new() { Id = 3, entity_id = 1, status = "closed" },
        }.AsQueryable().BuildMock();

        var apps = new List<applications>
        {
            new() { job_id = 1 },
            new() { job_id = 1 },
            new() { job_id = 2 },
            new() { job_id = 3 },
        }.AsQueryable().BuildMock();

        uowMock.SetupGet(u => u.Entity_profilesRepository).Returns(profilesRepoMock.Object);
        uowMock.SetupGet(u => u.JobsRepository).Returns(jobsRepoMock.Object);
        uowMock.SetupGet(u => u.ApplicationsRepository).Returns(appsRepoMock.Object);
        uowMock.SetupGet(u => u.UsersRepository).Returns(usersRepoMock.Object);
        uowMock.SetupGet(u => u.Contact_personsRepository).Returns(contactsRepoMock.Object);

        profilesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(profiles);
        jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobs);
        appsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(apps);

        var service = new entity_profilesService(uowMock.Object);
        var result = await service.GetStatsAsync(1);

        result.Should().NotBeNull();
        result.TotalJobs.Should().Be(3);
        result.ActiveJobs.Should().Be(2);
        result.TotalApplicants.Should().Be(4);
    }

    [Fact]
    public async Task UpdateIndividualProfile_AppliesUserAndProfileFields()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var appsRepoMock = new Mock<IapplicationsRepository>();
        var interviewsRepoMock = new Mock<IinterviewsRepository>();
        var contractsRepoMock = new Mock<IcontractsRepository>();
        var profilesRepoMock = new Mock<Iindividual_profilesRepository>();
        var usersRepoMock = new Mock<IusersRepository>();

        var user = new users
        {
            Id = 1,
            national_id = "1012345678",
            name = "Old Name",
            email = "user@example.com",
            phone = null,
            type = "individual",
            gender = null,
            nationality = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var users = new[] { user }.AsQueryable().BuildMock();

        var profile = new individual_profiles
        {
            Id = 1,
            user_id = 1,
            profile_completion_pct = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var profiles = new[] { profile }.AsQueryable().BuildMock();

        var apps = new List<applications>().AsQueryable().BuildMock();
        var interviews = new List<interviews>().AsQueryable().BuildMock();
        var contracts = new List<contracts>().AsQueryable().BuildMock();

        uowMock.SetupGet(u => u.ApplicationsRepository).Returns(appsRepoMock.Object);
        uowMock.SetupGet(u => u.InterviewsRepository).Returns(interviewsRepoMock.Object);
        uowMock.SetupGet(u => u.ContractsRepository).Returns(contractsRepoMock.Object);
        uowMock.SetupGet(u => u.Individual_profilesRepository).Returns(profilesRepoMock.Object);
        uowMock.SetupGet(u => u.UsersRepository).Returns(usersRepoMock.Object);
        uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        usersRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(users);
        profilesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(profiles);
        appsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(apps);
        interviewsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(interviews);
        contractsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(contracts);

        usersRepoMock.Setup(r => r.Update(It.IsAny<users>()))
            .Callback<users>(updated =>
            {
                user.name = updated.name;
                user.phone = updated.phone;
                user.gender = updated.gender;
                user.nationality = updated.nationality;
            });

        profilesRepoMock.Setup(r => r.Update(It.IsAny<individual_profiles>()))
            .Callback<individual_profiles>(updated =>
            {
                profile.birth_date = updated.birth_date;
                profile.city = updated.city;
                profile.zone = updated.zone;
                profile.district = updated.district;
                profile.street = updated.street;
                profile.zipcode = updated.zipcode;
                profile.job_title = updated.job_title;
                profile.profile_completion_pct = updated.profile_completion_pct;
            });

        var service = new individual_profilesService(uowMock.Object);
        var result = await service.UpdateAsync(1, new UpdateIndividualProfileDto
        {
            Name = "New Name",
            Phone = "0500123456",
            Gender = "male",
            Nationality = "Saudi",
            BirthDate = new DateTime(1990, 5, 15),
            City = "Riyadh",
            Zipcode = "12345",
            JobTitle = "Developer",
        });

        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        result.Phone.Should().Be("0500123456");
        result.Gender.Should().Be("male");
        result.Nationality.Should().Be("Saudi");
        result.Email.Should().Be("user@example.com");
        result.BirthDate.Should().Be(new DateTime(1990, 5, 15));
        result.BirthDate!.Value.Kind.Should().Be(DateTimeKind.Utc);
        result.City.Should().Be("Riyadh");
        result.Zipcode.Should().Be("12345");
        result.JobTitle.Should().Be("Developer");
        // Spec 01 (phase3-6): cv_file removed from the completion fields → denominator is 10 (was 11),
        // so the same 6 filled fields now compute 60.
        result.ProfileCompletionPct.Should().Be(60);
    }

    [Fact]
    public async Task UpdateIndividualProfile_ReturnsEmptySectionFieldsForEmptyProfile()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var appsRepoMock = new Mock<IapplicationsRepository>();
        var interviewsRepoMock = new Mock<IinterviewsRepository>();
        var contractsRepoMock = new Mock<IcontractsRepository>();
        var profilesRepoMock = new Mock<Iindividual_profilesRepository>();
        var usersRepoMock = new Mock<IusersRepository>();

        var user = new users
        {
            Id = 1,
            national_id = "1012345678",
            name = "Name",
            email = "user@example.com",
            phone = "0500000000",
            type = "individual",
            gender = "male",
            nationality = "Saudi",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var users = new[] { user }.AsQueryable().BuildMock();
        var profiles = new List<individual_profiles>().AsQueryable().BuildMock();

        var apps = new List<applications>().AsQueryable().BuildMock();
        var interviews = new List<interviews>().AsQueryable().BuildMock();
        var contracts = new List<contracts>().AsQueryable().BuildMock();

        uowMock.SetupGet(u => u.ApplicationsRepository).Returns(appsRepoMock.Object);
        uowMock.SetupGet(u => u.InterviewsRepository).Returns(interviewsRepoMock.Object);
        uowMock.SetupGet(u => u.ContractsRepository).Returns(contractsRepoMock.Object);
        uowMock.SetupGet(u => u.Individual_profilesRepository).Returns(profilesRepoMock.Object);
        uowMock.SetupGet(u => u.UsersRepository).Returns(usersRepoMock.Object);

        usersRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(users);
        profilesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(profiles);
        appsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(apps);
        interviewsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(interviews);
        contractsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(contracts);

        var service = new individual_profilesService(uowMock.Object);
        var result = await service.GetByUserIdAsync(1);

        result.Should().NotBeNull();
        result.City.Should().BeNull();
        result.Zone.Should().BeNull();
        result.District.Should().BeNull();
        result.Street.Should().BeNull();
        result.Zipcode.Should().BeNull();
        result.JobTitle.Should().BeNull();
        result.BirthDate.Should().BeNull();
        result.ProfileCompletionPct.Should().Be(0);
    }

    [Fact]
    public async Task UpdateEntityProfile_AppliesUserAndProfileFields()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var profilesRepoMock = new Mock<Ientity_profilesRepository>();
        var usersRepoMock = new Mock<IusersRepository>();
        var contactsRepoMock = new Mock<Icontact_personsRepository>();
        var jobsRepoMock = new Mock<IjobsRepository>();
        var appsRepoMock = new Mock<IapplicationsRepository>();

        var user = new users
        {
            Id = 1,
            national_id = "2020202020",
            name = "Old Company",
            email = "info@company.com",
            phone = null,
            type = "entity",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var users = new[] { user }.AsQueryable().BuildMock();

        var profile = new entity_profiles
        {
            Id = 1,
            user_id = 1,
            profile_completion_pct = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var profiles = new[] { profile }.AsQueryable().BuildMock();

        var contacts = new List<contact_persons>().AsQueryable().BuildMock();
        var jobs = new List<jobs>().AsQueryable().BuildMock();
        var apps = new List<applications>().AsQueryable().BuildMock();

        uowMock.SetupGet(u => u.Entity_profilesRepository).Returns(profilesRepoMock.Object);
        uowMock.SetupGet(u => u.UsersRepository).Returns(usersRepoMock.Object);
        uowMock.SetupGet(u => u.Contact_personsRepository).Returns(contactsRepoMock.Object);
        uowMock.SetupGet(u => u.JobsRepository).Returns(jobsRepoMock.Object);
        uowMock.SetupGet(u => u.ApplicationsRepository).Returns(appsRepoMock.Object);
        uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        profilesRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(profiles);
        usersRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(users);
        contactsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(contacts);
        jobsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(jobs);
        appsRepoMock.Setup(r => r.GetQueryable()).ReturnsAsync(apps);

        usersRepoMock.Setup(r => r.Update(It.IsAny<users>()))
            .Callback<users>(updated =>
            {
                user.name = updated.name;
                user.phone = updated.phone;
            });

        profilesRepoMock.Setup(r => r.Update(It.IsAny<entity_profiles>()))
            .Callback<entity_profiles>(updated =>
            {
                profile.company_field = updated.company_field;
                profile.company_size = updated.company_size;
                profile.commercial_reg = updated.commercial_reg;
                profile.city = updated.city;
                profile.zipcode = updated.zipcode;
                profile.website = updated.website;
                profile.facebook_url = updated.facebook_url;
                profile.twitter_url = updated.twitter_url;
                profile.youtube_url = updated.youtube_url;
                profile.profile_completion_pct = updated.profile_completion_pct;
            });

        var service = new entity_profilesService(uowMock.Object);
        var result = await service.UpdateAsync(1, new UpdateEntityProfileDto
        {
            Name = "New Company",
            Phone = "0500999888",
            CompanyField = "خدمات الحج",
            CompanySize = "كبيرة",
            CommercialReg = "1012345678",
            City = "الرياض",
            Zipcode = "12345",
            Website = "www.new.sa",
            FacebookAccount = "fb.com/new",
            TwitterAccount = "tw.com/new",
            YoutubeAccount = "yt.com/new",
        });

        result.Should().NotBeNull();
        result.Name.Should().Be("New Company");
        result.Phone.Should().Be("0500999888");
        result.Email.Should().Be("info@company.com");
        result.CompanyField.Should().Be("خدمات الحج");
        result.CompanySize.Should().Be("كبيرة");
        result.CommercialReg.Should().Be("1012345678");
        result.City.Should().Be("الرياض");
        result.Zipcode.Should().Be("12345");
        result.Website.Should().Be("www.new.sa");
        result.FacebookUrl.Should().Be("fb.com/new");
        result.TwitterUrl.Should().Be("tw.com/new");
        result.YoutubeUrl.Should().Be("yt.com/new");
        result.ProfileCompletionPct.Should().Be(53);
    }
}
