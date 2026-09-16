using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using tashrif.Data.DTOs.Auth;
using tashrif.Data.Interfaces;
using tashrif.Data.Models;
using tashrif.Core;
using tashrif.Identity.Interfaces;

namespace tashrif.Identity.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<users> _userRepo;
    private readonly IGenericRepository<individual_profiles> _individualProfileRepo;
    private readonly IGenericRepository<entity_profiles> _entityProfileRepo;
    private readonly IGenericRepository<contact_persons> _contactPersonRepo;
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;
    private readonly IFileStorageService _fileStorage;

    public AuthService(
        IGenericRepository<users> userRepo,
        IGenericRepository<individual_profiles> individualProfileRepo,
        IGenericRepository<entity_profiles> entityProfileRepo,
        IGenericRepository<contact_persons> contactPersonRepo,
        IUnitOfWork uow,
        IConfiguration config,
        IFileStorageService fileStorage)
    {
        _userRepo = userRepo;
        _individualProfileRepo = individualProfileRepo;
        _entityProfileRepo = entityProfileRepo;
        _contactPersonRepo = contactPersonRepo;
        _uow = uow;
        _config = config;
        _fileStorage = fileStorage;
    }

    public async Task<AuthResponseDto> RegisterIndividualAsync(RegisterIndividualDto dto)
    {
        var query = await _userRepo.GetQueryable();
        var existing = await query.FirstOrDefaultAsync(u => u.national_id == dto.NationalId && !u.IsDeleted);
        if (existing != null)
            throw new BadHttpRequestException("رقم الهوية مسجل مسبقاً", 409);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new users
        {
            national_id = dto.NationalId,
            password_hash = passwordHash,
            name = $"{dto.FirstName} {dto.LastName}",
            email = dto.Email,
            phone = dto.Phone,
            type = "individual",
            gender = dto.Gender,
            nationality = dto.Nationality,
            avatar_url = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user);
        await _uow.SaveChangesAsync();

        var cvUrl = dto.CvFile != null ? await _fileStorage.SaveFileAsync(dto.CvFile, "cvs") : null;
        var idUrl = dto.IdFile != null ? await _fileStorage.SaveFileAsync(dto.IdFile, "ids") : null;

        var profile = new individual_profiles
        {
            user_id = user.Id,
            city = "",
            zone = "",
            district = "",
            street = "",
            zipcode = "",
            job_title = "",
            cv_file = cvUrl ?? "",
            id_file = idUrl ?? "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        profile.profile_completion_pct = IndividualCompletionCalculator.Calculate(user, profile);
        await _individualProfileRepo.AddAsync(profile);
        await _uow.SaveChangesAsync();

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> RegisterEntityAsync(RegisterEntityDto dto)
    {
        var query = await _userRepo.GetQueryable();
        var existing = await query.FirstOrDefaultAsync(u => u.national_id == dto.NationalId && !u.IsDeleted);
        if (existing != null)
            throw new BadHttpRequestException("رقم الهوية مسجل مسبقاً", 409);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new users
        {
            national_id = dto.NationalId,
            password_hash = passwordHash,
            name = dto.CompanyName,
            email = dto.Email,
            phone = dto.Phone,
            type = "entity",
            gender = "",
            nationality = "",
            avatar_url = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user);
        await _uow.SaveChangesAsync();

        var logoUrl = dto.CompanyLogo != null ? await _fileStorage.SaveFileAsync(dto.CompanyLogo, "logos") : null;

        if (logoUrl != null)
            user.avatar_url = logoUrl;

        var profile = new entity_profiles
        {
            user_id = user.Id,
            logo_url = logoUrl ?? "",
            company_field = dto.FieldName,
            sector = dto.Sector,
            company_size = "",
            commercial_reg = "",
            country = dto.Country,
            city = "",
            zone = "",
            district = "",
            street = "",
            zipcode = "",
            province = dto.Province,
            company_desc = dto.CompanyDesc,
            website = dto.CompanyWebsite ?? "",
            twitter_url = dto.TwitterAccount ?? "",
            facebook_url = dto.FacebookAccount ?? "",
            youtube_url = dto.YoutubeAccount ?? "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        profile.profile_completion_pct = CalculateEntityCompletion(profile);
        await _entityProfileRepo.AddAsync(profile);
        await _uow.SaveChangesAsync();

        var contactPerson = new contact_persons
        {
            entity_id = profile.Id,
            name = dto.Name,
            email = dto.Email,
            phone = dto.Phone,
            role = dto.Role,
            nationality = dto.Nationality,
            is_primary = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _contactPersonRepo.AddAsync(contactPerson);
        await _uow.SaveChangesAsync();

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var query = await _userRepo.GetQueryable();
        var user = await query.FirstOrDefaultAsync(u => u.national_id == dto.NationalId && !u.IsDeleted);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.password_hash))
            throw new BadHttpRequestException("بيانات الدخول غير صحيحة", 401);

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var principal = ValidateToken(refreshToken);
        if (principal == null)
            throw new BadHttpRequestException("رمز التحديث غير صالح", 401);

        var userId = long.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null || user.IsDeleted)
            throw new BadHttpRequestException("المستخدم غير موجود", 404);

        return new AuthResponseDto
        {
            User = MapToUserDto(user),
            Token = GenerateJwtToken(user),
            RefreshToken = GenerateRefreshToken(user)
        };
    }

    public async Task<UserDto> GetCurrentUserAsync(long userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            throw new BadHttpRequestException("المستخدم غير موجود", 404);

        return MapToUserDto(user);
    }

    public async Task<UserDto> ChangePasswordAsync(long userId, ChangePasswordDto dto)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            throw new BadHttpRequestException("المستخدم غير موجود", 404);

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.password_hash))
            throw new BadHttpRequestException("كلمة المرور الحالية غير صحيحة", 401);

        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 8)
            throw new BadHttpRequestException("كلمة المرور الجديدة يجب أن تكون 8 أحرف على الأقل", 400);

        user.password_hash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        _userRepo.Update(user);
        await _uow.SaveChangesAsync();

        return MapToUserDto(user);
    }

    private AuthResponseDto GenerateAuthResponse(users user)
    {
        return new AuthResponseDto
        {
            User = MapToUserDto(user),
            Token = GenerateJwtToken(user),
            RefreshToken = GenerateRefreshToken(user)
        };
    }

    private static UserDto MapToUserDto(users user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.name,
            Email = user.email,
            Type = user.type,
            Phone = user.phone,
            NationalId = user.national_id,
            Gender = user.gender,
            Nationality = user.nationality,
            AvatarUrl = user.avatar_url
        };
    }

    private string GenerateJwtToken(users user)
    {
        var jwtKey = GetRequiredConfig("Jwt:Key");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.name),
            new Claim(ClaimTypes.Email, user.email),
            new Claim(ClaimTypes.Role, user.type),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "tashrif-api",
            audience: _config["Jwt:Audience"] ?? "tashrif-client",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken(users user)
    {
        var refreshKey = GetRequiredConfig("Jwt:RefreshKey");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "tashrif-api",
            audience: _config["Jwt:Audience"] ?? "tashrif-client",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var refreshKey = GetRequiredConfig("Jwt:RefreshKey");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshKey));
            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"] ?? "tashrif-api",
                ValidAudience = _config["Jwt:Audience"] ?? "tashrif-client",
                IssuerSigningKey = key,
            }, out _);
        }
        catch
        {
            return null;
        }
    }

    private string GetRequiredConfig(string key) =>
        _config[key] is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException($"Missing required configuration: {key}");

    private static short CalculateEntityCompletion(entity_profiles profile)
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
