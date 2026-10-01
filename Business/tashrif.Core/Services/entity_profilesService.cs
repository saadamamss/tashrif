using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class entity_profilesService(IUnitOfWork unitOfWork) : Ientity_profilesService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<EntityProfileResponseDto> GetByUserIdAsync(long userId)
    {
        var userQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var user = await userQuery.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted)
            ?? throw new KeyNotFoundException("المستخدم غير موجود");

        var profileQuery = await _unitOfWork.Entity_profilesRepository.GetQueryable();
        var profile = await profileQuery.FirstOrDefaultAsync(p => p.user_id == userId && !p.IsDeleted);

        var contactQuery = await _unitOfWork.Contact_personsRepository.GetQueryable();
        var contact = profile != null
            ? await contactQuery.FirstOrDefaultAsync(c => c.entity_id == profile.Id && !c.IsDeleted)
            : null;

        return new EntityProfileResponseDto
        {
            Id = profile?.Id ?? 0,
            Name = user.name,
            Email = user.email,
            Phone = user.phone ?? "",
            Type = user.type,
            NationalId = user.national_id,
            CompanyField = profile?.company_field,
            CompanySize = profile?.company_size,
            CommercialReg = profile?.commercial_reg,
            Sector = profile?.sector,
            Country = profile?.country,
            City = profile?.city,
            Zone = profile?.zone,
            District = profile?.district,
            Street = profile?.street,
            Zipcode = profile?.zipcode,
            Region = profile?.province,
            Description = profile?.company_desc,
            Website = profile?.website,
            FacebookUrl = profile?.facebook_url,
            TwitterUrl = profile?.twitter_url,
            YoutubeUrl = profile?.youtube_url,
            LogoUrl = profile?.logo_url,
            ProfileCompletionPct = profile?.profile_completion_pct ?? 0,
            ContactPerson = contact != null ? new ContactPersonResponseDto
            {
                Id = contact.Id,
                Name = contact.name,
                Email = contact.email,
                Phone = contact.phone,
                Role = contact.role,
                Nationality = contact.nationality,
            } : null,
            Stats = await GetStatsAsync(userId),
        };
    }

    public async Task<EntityProfileResponseDto> UpdateAsync(long userId, UpdateEntityProfileDto dto)
    {
        var profileQuery = await _unitOfWork.Entity_profilesRepository.GetQueryable();
        var profile = await profileQuery.FirstOrDefaultAsync(p => p.user_id == userId && !p.IsDeleted)
            ?? throw new KeyNotFoundException("الملف الشخصي غير موجود");

        if (dto.CompanyField != null) profile.company_field = dto.CompanyField;
        if (dto.CompanySize != null) profile.company_size = dto.CompanySize;
        if (dto.CommercialReg != null) profile.commercial_reg = dto.CommercialReg;
        if (dto.Sector != null) profile.sector = dto.Sector;
        if (dto.Country != null) profile.country = dto.Country;
        if (dto.City != null) profile.city = dto.City;
        if (dto.Zone != null) profile.zone = dto.Zone;
        if (dto.District != null) profile.district = dto.District;
        if (dto.Street != null) profile.street = dto.Street;
        if (dto.Zipcode != null) profile.zipcode = dto.Zipcode;
        if (dto.Province != null) profile.province = dto.Province;
        if (dto.CompanyDesc != null) profile.company_desc = dto.CompanyDesc;
        if (dto.Website != null) profile.website = dto.Website;
        if (dto.TwitterAccount != null) profile.twitter_url = dto.TwitterAccount;
        if (dto.FacebookAccount != null) profile.facebook_url = dto.FacebookAccount;
        if (dto.YoutubeAccount != null) profile.youtube_url = dto.YoutubeAccount;
        if (dto.LogoUrl != null) profile.logo_url = dto.LogoUrl;
        profile.profile_completion_pct = CalculateCompletion(profile);
        profile.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Entity_profilesRepository.Update(profile);

        var userQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var user = await userQuery.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted)
            ?? throw new KeyNotFoundException("المستخدم غير موجود");

        if (dto.Name != null) user.name = dto.Name;
        if (dto.Phone != null) user.phone = dto.Phone;
        if (dto.LogoUrl != null) user.avatar_url = dto.LogoUrl;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.UsersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return await GetByUserIdAsync(userId);
    }

    public async Task<EntityStatsDto> GetStatsAsync(long userId)
    {
        var jobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var allJobs = await jobsQuery.Where(j => j.entity_id == userId && !j.IsDeleted).ToListAsync();

        var appsQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var jobIds = allJobs.Select(j => j.Id).ToList();
        var totalApplicants = jobIds.Any()
            ? await appsQuery.CountAsync(a => jobIds.Contains(a.job_id) && !a.IsDeleted)
            : 0;

        return new EntityStatsDto
        {
            TotalJobs = allJobs.Count,
            ActiveJobs = allJobs.Count(j => j.status == "active"),
            TotalApplicants = totalApplicants,
        };
    }

    private static short CalculateCompletion(entity_profiles profile)
    {
        var fields = new[]
        {
            profile.logo_url, profile.company_field, profile.sector, profile.company_size,
            profile.commercial_reg, profile.country, profile.city, profile.zone, profile.district,
            profile.street, profile.zipcode, profile.province, profile.company_desc, profile.website,
            profile.twitter_url, profile.facebook_url, profile.youtube_url,
        };
        var filled = fields.Count(f => !string.IsNullOrWhiteSpace(f));
        return (short)Math.Round(filled * 100.0 / fields.Length);
    }
}
