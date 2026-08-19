using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class cvsService(IUnitOfWork unitOfWork, IFileStorageService fileStorage) : IcvsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorage = fileStorage;

    public async Task<IEnumerable<CvResponseDto>> GetAllByUserAsync(long userId)
    {
        var query = await _unitOfWork.CvsRepository.GetQueryable();
        return await query
            .Where(c => c.user_id == userId && !c.IsDeleted)
            .OrderByDescending(c => c.UploadedAt)
            .Select(c => new CvResponseDto
            {
                Id = c.Id,
                UserId = c.user_id,
                FileName = c.file_name,
                FilePath = c.file_path,
                FileSize = c.file_size,
                IsDefault = c.is_default,
                UploadedAt = c.UploadedAt,
            })
            .ToListAsync();
    }

    public async Task<CvResponseDto> CreateAsync(CreateCvDto dto, long userId)
    {
        if (dto.File == null || dto.File.Length == 0)
            throw new InvalidOperationException("الملف مطلوب");

        var filePath = await _fileStorage.SaveFileAsync(dto.File, "cvs");

        var entity = new cvs
        {
            user_id = userId,
            file_name = dto.File.FileName,
            file_path = filePath,
            file_size = dto.File.Length,
            is_default = false,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.CvsRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new CvResponseDto
        {
            Id = entity.Id,
            UserId = entity.user_id,
            FileName = entity.file_name,
            FilePath = entity.file_path,
            FileSize = entity.file_size,
            IsDefault = entity.is_default,
            UploadedAt = entity.UploadedAt,
        };
    }

    public async Task DeleteAsync(long id, long userId)
    {
        var query = await _unitOfWork.CvsRepository.GetQueryable();
        var entity = await query
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted)
            ?? throw new KeyNotFoundException("السيرة الذاتية غير موجودة");

        if (entity.user_id != userId)
            throw new UnauthorizedAccessException("لا تملك صلاحية حذف هذه السيرة الذاتية");

        await _fileStorage.DeleteFileAsync(entity.file_path);

        entity.IsDeleted = true;
        entity.DeletedTime = DateTime.UtcNow;
        _unitOfWork.CvsRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
