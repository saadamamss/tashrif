using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace tashrif.Core;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile? file, string subfolder);
    Task DeleteFileAsync(string fileUrl);
}

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveFileAsync(IFormFile? file, string subfolder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("الملف فارغ");

        if (file.Length > 10 * 1024 * 1024)
            throw new ArgumentException("حجم الملف يتجاوز 10 ميجابايت");

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".gif" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            throw new ArgumentException("نوع الملف غير مدعوم");

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", subfolder);
        Directory.CreateDirectory(uploadsDir);

        var uniqueName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, uniqueName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{subfolder}/{uniqueName}";
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return Task.CompletedTask;

        var filePath = Path.Combine(_env.WebRootPath, fileUrl.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }
}
