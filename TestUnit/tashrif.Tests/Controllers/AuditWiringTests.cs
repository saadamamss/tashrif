using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using tashrif.API.Controllers;
using tashrif.Data.Constants;
using tashrif.Data.DTOs.Admin;
using tashrif.Data.DTOs.Auth;
using tashrif.Data.DTOs;

namespace tashrif.Tests.Controllers;

// Spec 04 (phase3-5-hardening): prove the audit WIRING exists in controllers —
// the smallest harness that does it (direct instantiation + mocked services, no web host).
public class AuditWiringTests
{
    private readonly Mock<IAdminService> _adminServiceMock = new();
    private readonly Mock<IFileStorageService> _fileStorageMock = new();
    private readonly Mock<IAuditService> _auditMock = new();

    private AdminController CreateSut()
    {
        var controller = new AdminController(_adminServiceMock.Object, _fileStorageMock.Object, _auditMock.Object)
        {
            ControllerContext = FakeContext(adminId: 1),
        };
        return controller;
    }

    [Fact]
    public async Task AdminDeactivateUser_WritesAuditRow()
    {
        _adminServiceMock
            .Setup(s => s.DeactivateUserAsync(9, 1))
            .Returns(Task.CompletedTask);

        var sut = CreateSut();
        var result = await sut.DeactivateUser(9);

        result.Should().BeOfType<OkObjectResult>();
        _auditMock.Verify(a => a.LogAsync(
            1, AuditActions.AdminUserDeactivated, "users", 9, "active", "deactivated",
            It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task AdminDeactivateUser_ServiceThrows_LogsNothing()
    {
        // Audit records SUCCESSFUL actions only — a failed operation must write no row.
        _adminServiceMock
            .Setup(s => s.DeactivateUserAsync(9, 1))
            .ThrowsAsync(new KeyNotFoundException("المستخدم غير موجود"));

        var sut = CreateSut();
        var act = () => sut.DeactivateUser(9);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _auditMock.Verify(a => a.LogAsync(
            It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()),
            Times.Never);
    }

    [Fact]
    public async Task AdminActivateUser_WritesAuditRow()
    {
        _adminServiceMock
            .Setup(s => s.ActivateUserAsync(9))
            .Returns(Task.CompletedTask);

        var sut = CreateSut();
        await sut.ActivateUser(9);

        _auditMock.Verify(a => a.LogAsync(
            1, AuditActions.AdminUserActivated, "users", 9, "deactivated", "active",
            It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task AdminDeactivateJob_WritesAuditRowWithRealPreviousStatus()
    {
        _adminServiceMock.Setup(s => s.GetJobStatusAsync(7)).ReturnsAsync("active");
        _adminServiceMock.Setup(s => s.DeactivateJobAsync(7)).Returns(Task.CompletedTask);

        var sut = CreateSut();
        await sut.DeactivateJob(7);

        _auditMock.Verify(a => a.LogAsync(
            1, AuditActions.AdminJobDeactivated, "jobs", 7, "active", "closed",
            It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task AdminDeactivateJob_ReadsStatusBeforeUpdate()
    {
        // Ordering IS the correctness guarantee: old_value must be read before the mutation.
        var callOrder = new List<string>();
        _adminServiceMock.Setup(s => s.GetJobStatusAsync(7))
            .Callback(() => callOrder.Add("read")).ReturnsAsync("active");
        _adminServiceMock.Setup(s => s.DeactivateJobAsync(7))
            .Callback(() => callOrder.Add("update")).Returns(Task.CompletedTask);

        var sut = CreateSut();
        await sut.DeactivateJob(7);

        callOrder.Should().Equal("read", "update");
    }

    [Fact]
    public async Task AdminUpdateProfile_WritesAuditRow()
    {
        _adminServiceMock.Setup(s => s.UpdateProfileAsync(1, It.IsAny<UpdateAdminProfileDto>()))
            .ReturnsAsync(new UserDto());

        var sut = CreateSut();
        await sut.UpdateProfile(new UpdateAdminProfileDto());

        _auditMock.Verify(a => a.LogAsync(
            1, AuditActions.AdminProfileUpdated, "users", 1, null, null,
            It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    private static ControllerContext FakeContext(long adminId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, adminId.ToString()),
            new(ClaimTypes.Role, "admin"),
        };
        var http = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test")) };
        return new ControllerContext { HttpContext = http };
    }
}
