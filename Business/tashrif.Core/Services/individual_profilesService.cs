using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class individual_profilesService(IUnitOfWork unitOfWork) : Iindividual_profilesService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IndividualProfileResponseDto> GetByUserIdAsync(long userId)
    {
        var userQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var user = await userQuery.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted)
            ?? throw new KeyNotFoundException("المستخدم غير موجود");

        var profileQuery = await _unitOfWork.Individual_profilesRepository.GetQueryable();
        var profile = await profileQuery.FirstOrDefaultAsync(p => p.user_id == userId && !p.IsDeleted);

        return new IndividualProfileResponseDto
        {
            Id = profile?.Id ?? 0,
            Name = user.name,
            Email = user.email,
            Phone = user.phone ?? "",
            Type = user.type,
            Gender = user.gender,
            Nationality = user.nationality,
            NationalId = user.national_id,
            BirthDate = profile?.birth_date,
            City = profile?.city,
            Zone = profile?.zone,
            District = profile?.district,
            Street = profile?.street,
            Zipcode = profile?.zipcode,
            JobTitle = profile?.job_title,
            ProfileCompletionPct = profile?.profile_completion_pct ?? 0,
            CvFile = profile?.cv_file,
            IdFile = profile?.id_file,
            Stats = await GetStatsAsync(userId),
        };
    }

    public async Task<IndividualProfileResponseDto> UpdateAsync(long userId, UpdateIndividualProfileDto dto)
    {
        var profileQuery = await _unitOfWork.Individual_profilesRepository.GetQueryable();
        var profile = await profileQuery.FirstOrDefaultAsync(p => p.user_id == userId && !p.IsDeleted)
            ?? throw new KeyNotFoundException("الملف الشخصي غير موجود");

        if (dto.City != null) profile.city = dto.City;
        if (dto.Zone != null) profile.zone = dto.Zone;
        if (dto.District != null) profile.district = dto.District;
        if (dto.Street != null) profile.street = dto.Street;
        if (dto.Zipcode != null) profile.zipcode = dto.Zipcode;
        if (dto.JobTitle != null) profile.job_title = dto.JobTitle;
        if (dto.BirthDate != null)
            profile.birth_date = DateTime.SpecifyKind(dto.BirthDate.Value, DateTimeKind.Utc);
        profile.profile_completion_pct = CalculateCompletion(profile);
        profile.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Individual_profilesRepository.Update(profile);

        var userQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var user = await userQuery.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted)
            ?? throw new KeyNotFoundException("المستخدم غير موجود");

        if (dto.Name != null) user.name = dto.Name;
        if (dto.Phone != null) user.phone = dto.Phone;
        if (dto.Gender != null) user.gender = dto.Gender;
        if (dto.Nationality != null) user.nationality = dto.Nationality;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.UsersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return await GetByUserIdAsync(userId);
    }

    public async Task<IndividualStatsDto> GetStatsAsync(long userId)
    {
        var appsQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var allApps = await appsQuery.Where(a => a.user_id == userId && !a.IsDeleted).ToListAsync();

        var interviewsQuery = await _unitOfWork.InterviewsRepository.GetQueryable();
        var interviewCount = await interviewsQuery.CountAsync(i => i.user_id == userId && !i.IsDeleted);

        var contractsQuery = await _unitOfWork.ContractsRepository.GetQueryable();
        var contractCount = await contractsQuery.CountAsync(c => c.user_id == userId && !c.IsDeleted);

        return new IndividualStatsDto
        {
            TotalApplications = allApps.Count,
            PendingApps = allApps.Count(a => a.status == "new" || a.status == "pending"),
            Interviews = interviewCount,
            Contracts = contractCount,
        };
    }

    private static short CalculateCompletion(individual_profiles profile)
    {
        var fields = new[]
        {
            profile.city, profile.zone, profile.district, profile.street, profile.zipcode,
            profile.job_title, profile.cv_file, profile.id_file,
        };
        var filled = fields.Count(f => !string.IsNullOrWhiteSpace(f));
        return (short)Math.Round(filled * 100.0 / fields.Length);
    }
}
