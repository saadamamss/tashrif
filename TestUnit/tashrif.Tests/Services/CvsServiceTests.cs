using tashrif.Core;
using tashrif.Data.DTOs;
using tashrif.Data.Interfaces;
using tashrif.Data.Models;

namespace tashrif.Tests.Services;

public class CvsServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IcvsRepository> _cvsRepoMock;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly cvsService _sut;

    public CvsServiceTests()
    {
        _cvsRepoMock = new Mock<IcvsRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _fileStorageMock = new Mock<IFileStorageService>();

        _uowMock.SetupGet(u => u.CvsRepository).Returns(_cvsRepoMock.Object);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _sut = new cvsService(_uowMock.Object, _fileStorageMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsMappedCvs_ForUser()
    {
        var cvs = new[]
        {
            new cvs { Id = 1, user_id = 1, file_name = "cv.pdf", file_path = "/uploads/cvs/cv-1.pdf", file_size = 1200, is_default = true, UploadedAt = DateTime.UtcNow },
            new cvs { Id = 2, user_id = 2, file_name = "cv2.pdf", file_path = "/uploads/cvs/cv-2.pdf", file_size = 800, is_default = false, UploadedAt = DateTime.UtcNow },
            new cvs { Id = 3, user_id = 1, file_name = "deleted.pdf", file_path = "/uploads/cvs/cv-3.pdf", file_size = 500, is_default = false, UploadedAt = DateTime.UtcNow, IsDeleted = true },
        };
        _cvsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(cvs.AsQueryable().BuildMock());

        var result = (await _sut.GetAllByUserAsync(1)).ToList();

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(1);
        result[0].UserId.Should().Be(1);
        result[0].FileName.Should().Be("cv.pdf");
        result[0].FilePath.Should().Be("/uploads/cvs/cv-1.pdf");
        result[0].FileSize.Should().Be(1200);
        result[0].IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task Create_NoFile_ThrowsInvalidOperationException()
    {
        var dto = new CreateCvDto { File = null };

        var act = () => _sut.CreateAsync(dto, 1);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Create_SavesFile_AndPersists()
    {
        cvs? added = null;
        _cvsRepoMock.Setup(r => r.AddAsync(It.IsAny<cvs>()))
            .Callback<cvs>(c => added = c);
        _fileStorageMock.Setup(f => f.SaveFileAsync(It.IsAny<IFormFile>(), "cvs"))
            .ReturnsAsync("/uploads/cvs/new.pdf");

        var fileMock = new Mock<IFormFile>();
        fileMock.SetupGet(f => f.FileName).Returns("resume.pdf");
        fileMock.SetupGet(f => f.Length).Returns(2048);

        var dto = new CreateCvDto { File = fileMock.Object };

        var result = await _sut.CreateAsync(dto, 1);

        _fileStorageMock.Verify(f => f.SaveFileAsync(fileMock.Object, "cvs"), Times.Once);
        added.Should().NotBeNull();
        added!.user_id.Should().Be(1);
        added!.file_name.Should().Be("resume.pdf");
        added!.file_path.Should().Be("/uploads/cvs/new.pdf");
        added!.file_size.Should().Be(2048);
        added!.is_default.Should().BeFalse();
        result.FileName.Should().Be("resume.pdf");
        result.FilePath.Should().Be("/uploads/cvs/new.pdf");
        result.FileSize.Should().Be(2048);
    }

    [Fact]
    public async Task Delete_ByOwner_DeletesFileAndSoftDeletes()
    {
        var cv = new cvs { Id = 1, user_id = 1, file_name = "cv.pdf", file_path = "/uploads/cvs/cv-1.pdf" };
        _cvsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { cv }.AsQueryable().BuildMock());

        await _sut.DeleteAsync(1, 1);

        _fileStorageMock.Verify(f => f.DeleteFileAsync("/uploads/cvs/cv-1.pdf"), Times.Once);
        cv.IsDeleted.Should().BeTrue();
        cv.DeletedTime.Should().NotBeNull();
        _cvsRepoMock.Verify(r => r.Update(It.IsAny<cvs>()), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExisting_ThrowsKeyNotFoundException()
    {
        _cvsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<cvs>().AsQueryable().BuildMock());

        var act = () => _sut.DeleteAsync(999, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _fileStorageMock.Verify(f => f.DeleteFileAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WrongOwner_ThrowsUnauthorizedAccessException()
    {
        var cv = new cvs { Id = 1, user_id = 2, file_name = "cv.pdf", file_path = "/uploads/cvs/cv-1.pdf" };
        _cvsRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { cv }.AsQueryable().BuildMock());

        var act = () => _sut.DeleteAsync(1, 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _fileStorageMock.Verify(f => f.DeleteFileAsync(It.IsAny<string>()), Times.Never);
    }
}