using Microsoft.AspNetCore.Hosting;

namespace tashrif.Tests.Services;

public class FileStorageServiceTests : IDisposable
{
    private readonly string _webRoot;
    private readonly FileStorageService _sut;

    public FileStorageServiceTests()
    {
        _webRoot = Path.Combine(Path.GetTempPath(), "tashrif_fs_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_webRoot);

        var env = new Mock<IWebHostEnvironment>();
        env.SetupGet(e => e.WebRootPath).Returns(_webRoot);

        _sut = new FileStorageService(env.Object);
    }

    public void Dispose()
    {
        if (Directory.Exists(_webRoot))
            Directory.Delete(_webRoot, true);
    }

    private static IFormFile CreateFile(string fileName, byte[]? content = null, string contentType = "application/pdf")
    {
        var stream = new MemoryStream(content ?? new byte[] { 0x25, 0x50, 0x44, 0x46 });
        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType,
        };
    }

    [Fact]
    public async Task Save_ValidFile_StoresInsideUploadsAndReturnsCleanUrl()
    {
        var result = await _sut.SaveFileAsync(CreateFile("contract-2026.pdf"), "contracts");

        result.Should().StartWith("/uploads/contracts/");
        result.Should().EndWith(".pdf");
        result.Should().NotContain("contract-2026");

        var storedPath = Path.Combine(_webRoot, result.TrimStart('/'));
        File.Exists(storedPath).Should().BeTrue();
    }

    [Fact]
    public async Task Save_PathTraversalFileName_StaysInsideUploadsFolder()
    {
        var result = await _sut.SaveFileAsync(CreateFile("../../../../evil.pdf"), "contracts");

        result.Should().StartWith("/uploads/contracts/");
        result.Should().NotContain("..");

        var storedPath = Path.Combine(_webRoot, result.TrimStart('/'));
        File.Exists(storedPath).Should().BeTrue();

        Directory.EnumerateFiles(_webRoot).Should().BeEmpty();
        Directory.EnumerateFiles(Path.Combine(_webRoot, "uploads")).Should().BeEmpty();
        Directory.EnumerateFiles(Path.Combine(_webRoot, "uploads", "contracts")).Should().HaveCount(1);
    }

    [Fact]
    public async Task Save_DisallowedExtension_ThrowsArgumentException()
    {
        var act = () => _sut.SaveFileAsync(CreateFile("evil.exe"), "contracts");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Save_EmptyFileName_ThrowsArgumentException()
    {
        var act = () => _sut.SaveFileAsync(CreateFile(""), "contracts");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Save_OversizedFile_ThrowsArgumentException()
    {
        var content = new byte[10 * 1024 * 1024 + 1];
        var act = () => _sut.SaveFileAsync(CreateFile("big.pdf", content), "contracts");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Save_NullFile_ThrowsArgumentException()
    {
        var act = () => _sut.SaveFileAsync(null, "contracts");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Delete_ExistingFile_RemovesIt()
    {
        var result = await _sut.SaveFileAsync(CreateFile("to-delete.pdf"), "cvs");
        var storedPath = Path.Combine(_webRoot, result.TrimStart('/'));
        File.Exists(storedPath).Should().BeTrue();

        await _sut.DeleteFileAsync(result);

        File.Exists(storedPath).Should().BeFalse();
    }
}