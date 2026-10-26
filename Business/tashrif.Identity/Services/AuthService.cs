using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using tashrif.Data.DTOs.Auth;
using tashrif.Data.Interfaces;
using tashrif.Data.Models;
using tashrif.Data.Validation;
using tashrif.Core;
using tashrif.Identity.Interfaces;

namespace tashrif.Identity.Services;

public class AuthService : IAuthService
{
    /// <summary>How long a raw verification token stays usable (D5).</summary>
    private const int VerificationTokenLifetimeHours = 24;

    /// <summary>Arabic message returned when the email is not verified yet (D6 — 403).</summary>
    private const string NotVerifiedMessage = "يجب تفعيل بريدك الإلكتروني قبل تسجيل الدخول";

    private readonly IGenericRepository<users> _userRepo;
    private readonly IGenericRepository<individual_profiles> _individualProfileRepo;
    private readonly IGenericRepository<entity_profiles> _entityProfileRepo;
    private readonly IGenericRepository<contact_persons> _contactPersonRepo;
    private readonly IGenericRepository<email_verification_tokens> _tokenRepo;
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;
    private readonly IFileStorageService _fileStorage;
    private readonly IClock _clock;

    public AuthService(
        IGenericRepository<users> userRepo,
        IGenericRepository<individual_profiles> individualProfileRepo,
        IGenericRepository<entity_profiles> entityProfileRepo,
        IGenericRepository<contact_persons> contactPersonRepo,
        IGenericRepository<email_verification_tokens> tokenRepo,
        IUnitOfWork uow,
        IConfiguration config,
        IFileStorageService fileStorage,
        IClock clock)
    {
        _userRepo = userRepo;
        _individualProfileRepo = individualProfileRepo;
        _entityProfileRepo = entityProfileRepo;
        _contactPersonRepo = contactPersonRepo;
        _tokenRepo = tokenRepo;
        _uow = uow;
        _config = config;
        _fileStorage = fileStorage;
        _clock = clock;
    }

    public async Task<RegisterResponseDto> RegisterIndividualAsync(RegisterIndividualDto dto)
    {
        var query = await _userRepo.GetQueryable();
        var existing = await query.FirstOrDefaultAsync(u => u.national_id == dto.NationalId && !u.IsDeleted);
        if (existing != null)
            throw new BadHttpRequestException("رقم الهوية مسجل مسبقاً", 409);

        // D12: users.email carries a UNIQUE index, so a duplicate registration would
        // otherwise surface as a raw DbUpdateException → HTTP 500. Guard explicitly and
        // return 409, mirroring the national_id conflict above.
        await EnsureEmailIsAvailableAsync(dto.Email);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Gap #1 (2026-10-12): trim user-entered text before persisting. Validation ran on the
        // RAW values in the controller (RequestValidator), so this only affects stored data —
        // it cannot make a previously-invalid input pass. Password is deliberately NOT trimmed:
        // its exact bytes are the credential. ID file / avatar paths are not user-typed text.
        dto.FirstName = dto.FirstName.Trim();
        dto.LastName = dto.LastName.Trim();
        dto.Email = dto.Email.Trim();
        dto.Phone = dto.Phone.Trim();
        dto.Gender = dto.Gender.Trim();
        dto.Nationality = dto.Nationality.Trim();

        // D12: emails are stored lowercase. The DB has a UNIQUE index on users.email and a
        // BEFORE-trigger that lowercases on write; lowercasing here as well keeps the
        // in-memory entity (and the value echoed back to the caller) consistent with what
        // the database actually stores — EF would otherwise keep the un-normalized value.
        dto.Email = dto.Email.ToLowerInvariant();

        // Nit fix (2026-10-12): re-validate AFTER trimming. A whitespace-only value
        // ("   ") passes [Required] on the raw input, then trims to "" — without this,
        // an empty string would be stored in a required column. Trimming only strips,
        // so this can only newly fail values that are effectively empty (same Arabic
        // 400 style as the controller's pre-validation, via ExceptionMiddleware).
        RequestValidator.Validate(dto);

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
            id_file = idUrl ?? "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        profile.profile_completion_pct = IndividualCompletionCalculator.Calculate(user, profile);
        await _individualProfileRepo.AddAsync(profile);
        await _uow.SaveChangesAsync();

        // D13: issue the verification token. The raw token is returned to the controller
        // (in memory) so it can build the email link — it is never persisted in plaintext.
        return await IssueVerificationTokenAsync(user, $"{dto.FirstName} {dto.LastName}");
    }

    public async Task<RegisterResponseDto> RegisterEntityAsync(RegisterEntityDto dto)
    {
        var query = await _userRepo.GetQueryable();
        var existing = await query.FirstOrDefaultAsync(u => u.national_id == dto.NationalId && !u.IsDeleted);
        if (existing != null)
            throw new BadHttpRequestException("رقم الهوية مسجل مسبقاً", 409);

        // D12: duplicate-email guard — see the individual register branch.
        await EnsureEmailIsAvailableAsync(dto.Email);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Gap #1 (2026-10-12): trim user-entered text before persisting (see the individual
        // register branch for the rationale). Validation already ran on raw values.
        dto.CompanyName = dto.CompanyName.Trim();
        dto.Email = dto.Email.Trim();
        dto.Phone = dto.Phone.Trim();
        dto.CommercialReg = dto.CommercialReg.Trim();
        dto.FieldName = dto.FieldName.Trim();
        dto.Sector = dto.Sector.Trim();
        dto.Country = dto.Country.Trim();
        dto.Province = dto.Province.Trim();
        dto.CompanyDesc = dto.CompanyDesc.Trim();
        dto.CompanyWebsite = dto.CompanyWebsite.Trim();
        if (dto.TwitterAccount != null) dto.TwitterAccount = dto.TwitterAccount.Trim();
        if (dto.FacebookAccount != null) dto.FacebookAccount = dto.FacebookAccount.Trim();
        if (dto.YoutubeAccount != null) dto.YoutubeAccount = dto.YoutubeAccount.Trim();
        dto.Name = dto.Name.Trim();
        dto.Role = dto.Role.Trim();
        dto.Nationality = dto.Nationality.Trim();

        // D12: emails are stored lowercase — see the individual register branch.
        dto.Email = dto.Email.ToLowerInvariant();

        // Nit fix (2026-10-12): same post-trim re-validation as the individual branch.
        RequestValidator.Validate(dto);

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
            commercial_reg = dto.CommercialReg,
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

        // D13: same as the individual branch — issue the token, return it in memory.
        return await IssueVerificationTokenAsync(user, dto.CompanyName);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var query = await _userRepo.GetQueryable();
        var user = await query.FirstOrDefaultAsync(u => u.national_id == dto.NationalId && !u.IsDeleted);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.password_hash))
            throw new BadHttpRequestException("بيانات الدخول غير صحيحة", 401);

        // D6: 403 (NOT 401) so the frontend's axios interceptor never enters the
        // refresh-queue path for an unverified account — a 401 would trigger a refresh
        // attempt and a silent logout instead of a "verify your email" message.
        EnsureEmailIsVerified(user);

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

        // D6: same 403 gate as login — a refresh must not resurrect an unverified session.
        EnsureEmailIsVerified(user);

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
        // GetByIdAsync uses FindAsync, which bypasses the soft-delete filter — see known-issues.md
        if (user == null || user.IsDeleted)
            throw new BadHttpRequestException("المستخدم غير موجود", 404);

        // B6: NOT a 403 here. `useAuth.init()` treats any error as "session invalid" and
        // calls clearUser(), so an authenticated-but-unverified user would be silently
        // logged out instead of being routed to the verification page. Return 200 with
        // `isEmailVerified: false` and let the frontend middleware decide.
        return MapToUserDto(user);
    }

    public async Task<UserDto> ChangePasswordAsync(long userId, ChangePasswordDto dto)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        // Guard must precede BCrypt.Verify + Update() so a deactivated user can cause no mutation.
        if (user == null || user.IsDeleted)
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

    /// <summary>
    /// D11/B2 — redeems a raw verification token and signs the user in (D7 auto-login).
    ///
    /// <para>
    /// The token is single-use: redeeming it sets <c>used_at</c> (the replay check),
    /// and every OTHER live token for that user is soft-deleted, so only one live
    /// token exists per user (D5). The global <c>IsDeleted</c> query filter then hides
    /// the soft-deleted rows from the next lookup, which is what makes a rotated
    /// token fail.
    /// </para>
    /// </summary>
    public async Task<AuthResponseDto> VerifyEmailAsync(string rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            throw new BadHttpRequestException("رابط التحقق غير صالح", 400);

        var hash = HashToken(rawToken);
        var tokenQuery = await _tokenRepo.GetQueryable();
        var token = await tokenQuery.FirstOrDefaultAsync(t => t.token_hash == hash);

        if (token == null)
            throw new BadHttpRequestException("رابط التحقق غير صالح", 400);

        if (token.used_at != null)
            throw new BadHttpRequestException("رابط التحقق غير صالح", 400);

        if (token.expires_at <= _clock.UtcNow)
            throw new BadHttpRequestException("انتهت صلاحية رابط التحقق", 400);

        var user = await _userRepo.GetByIdAsync(token.user_id);
        if (user == null || user.IsDeleted)
            throw new BadHttpRequestException("المستخدم غير موجود", 404);

        // Mark the token as redeemed, then kill every other live token for this user.
        token.used_at = _clock.UtcNow;
        token.UpdatedAt = _clock.UtcNow;
        _tokenRepo.Update(token);

        await InvalidateSiblingTokensAsync(token.user_id, token.Id);

        user.is_email_verified = true;
        user.UpdatedAt = _clock.UtcNow;
        _userRepo.Update(user);

        await _uow.SaveChangesAsync();

        return GenerateAuthResponse(user);
    }

    /// <summary>
    /// D8/D11/B2 — re-issues a verification token for the account with the given
    /// national id.
    ///
    /// <para>
    /// The public endpoint always answers with the same generic message, so this method
    /// returns the real outcome in a separate DTO that only the controller sees. An
    /// unknown id, a soft-deleted account, and an already-verified account all produce
    /// <c>ShouldSend = false</c> — no enumeration oracle.
    /// </para>
    /// </summary>
    public async Task<ResendVerificationResultDto> ResendVerificationAsync(string nationalId)
    {
        var result = new ResendVerificationResultDto();

        if (string.IsNullOrWhiteSpace(nationalId))
            return result;

        var query = await _userRepo.GetQueryable();
        var user = await query.FirstOrDefaultAsync(u => u.national_id == nationalId && !u.IsDeleted);

        // Unknown / soft-deleted / already-verified → identical generic outcome.
        if (user == null || user.is_email_verified)
            return result;

        // Remember a previous delivery failure so the resend flow can surface it (D14).
        var tokenQuery = await _tokenRepo.GetQueryable();
        var previousTokens = await tokenQuery
            .Where(t => t.user_id == user.Id)
            .OrderByDescending(t => t.Id)
            .ToListAsync();

        result.PreviousDeliveryError = previousTokens
            .FirstOrDefault(t => !string.IsNullOrEmpty(t.last_error))?.last_error;

        // D5: single active token per user — soft-delete the old rows, then insert a new one.
        await InvalidateSiblingTokensAsync(user.Id, null);

        var issued = await IssueVerificationTokenAsync(user, user.name);

        result.ShouldSend = true;
        result.VerificationToken = issued.VerificationToken;
        result.VerificationTokenId = issued.VerificationTokenId;
        result.Email = issued.Email;
        result.Name = issued.Name;

        return result;
    }

    /// <summary>
    /// D13 — mints a fresh token for the user and stores only its SHA-256 digest.
    /// The raw token is returned to the caller (the controller) in memory.
    /// </summary>
    private async Task<RegisterResponseDto> IssueVerificationTokenAsync(users user, string displayName)
    {
        var rawToken = GenerateRawToken();

        var token = new email_verification_tokens
        {
            user_id = user.Id,
            token_hash = HashToken(rawToken),
            expires_at = _clock.UtcNow.AddHours(VerificationTokenLifetimeHours),
            CreatedAt = _clock.UtcNow,
            UpdatedAt = _clock.UtcNow,
        };

        await _tokenRepo.AddAsync(token);
        await _uow.SaveChangesAsync();

        return new RegisterResponseDto
        {
            Message = "تم إنشاء الحساب بنجاح. يرجى تفعيل بريدك الإلكتروني.",
            VerificationToken = rawToken,
            VerificationTokenId = token.Id,
            Email = user.email,
            Name = displayName,
        };
    }

    /// <summary>
    /// Soft-deletes every live token of the user except <paramref name="exceptTokenId"/>
    /// (pass null to kill them all). Soft-delete is the "token is dead" marker — the
    /// global <c>IsDeleted</c> query filter then hides those rows from lookups.
    /// </summary>
    private async Task InvalidateSiblingTokensAsync(long userId, long? exceptTokenId)
    {
        var query = await _tokenRepo.GetQueryable();
        var siblings = await query.Where(t => t.user_id == userId).ToListAsync();

        var changed = false;
        foreach (var sibling in siblings)
        {
            if (exceptTokenId.HasValue && sibling.Id == exceptTokenId.Value)
                continue;

            sibling.IsDeleted = true;
            sibling.DeletedTime = _clock.UtcNow;
            sibling.UpdatedAt = _clock.UtcNow;
            _tokenRepo.Update(sibling);
            changed = true;
        }

        if (changed)
            await _uow.SaveChangesAsync();
    }

    /// <summary>
    /// D6 — the login/refresh gate. 403 (not 401) so the frontend's axios interceptor
    /// does not treat it as an expired session.
    /// </summary>
    private static void EnsureEmailIsVerified(users user)
    {
        if (!user.is_email_verified)
            throw new BadHttpRequestException(NotVerifiedMessage, 403);
    }

    /// <summary>D4 — 256 bits of entropy, base64url-encoded (no padding, URL-safe).</summary>
    private static string GenerateRawToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    /// <summary>D4 — the raw token is never stored; only its SHA-256 hex digest is.</summary>
    private static string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    /// <summary>
    /// D12 — rejects an email that is already taken.
    /// The comparison is case-insensitive because emails are stored lowercase (both by
    /// this service and by the DB trigger), so "A@x.com" collides with "a@x.com".
    ///
    /// IgnoreQueryFilters is REQUIRED here, not an optimisation: the global IsDeleted
    /// filter would hide soft-deleted rows, but the UNIQUE index on users.email spans
    /// ALL rows regardless of IsDeleted. Without this, registering the address of a
    /// deactivated account would pass the guard and then blow up at SaveChanges with a
    /// DbUpdateException → HTTP 500 instead of a clean 409.
    /// </summary>
    private async Task EnsureEmailIsAvailableAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var query = await _userRepo.GetQueryable();
        query = query.IgnoreQueryFilters();

        var emailTaken = await query.AnyAsync(u => u.email.ToLower() == normalizedEmail);

        if (emailTaken)
            throw new BadHttpRequestException("البريد الإلكتروني مستخدم مسبقا", 409);
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
            AvatarUrl = user.avatar_url,
            IsEmailVerified = user.is_email_verified
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
