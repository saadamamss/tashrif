using tashrif.Core;
using tashrif.Data.DTOs;
using tashrif.Data.Interfaces;
using tashrif.Data.Models;

namespace tashrif.Tests.Services;

public class BankAccountsServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<Ibank_accountsRepository> _bankRepoMock;
    private readonly bank_accountsService _sut;

    public BankAccountsServiceTests()
    {
        _bankRepoMock = new Mock<Ibank_accountsRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _uowMock.SetupGet(u => u.Bank_accountsRepository).Returns(_bankRepoMock.Object);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _sut = new bank_accountsService(_uowMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsMappedAccounts_ForUser()
    {
        var accounts = new[]
        {
            new bank_accounts { Id = 1, user_id = 1, iban = "SA0380000000608010167515", bank_name = "البنك الأهلي", iban_status = "verified", account_status = "active", CreatedAt = DateTime.UtcNow },
            new bank_accounts { Id = 2, user_id = 2, iban = "SA1210000001234567891234", bank_name = "الراجحي", iban_status = "pending", account_status = "active", CreatedAt = DateTime.UtcNow },
            new bank_accounts { Id = 3, user_id = 1, iban = "SA5560000000987654321098", bank_name = "الرياض", iban_status = "pending", account_status = "active", CreatedAt = DateTime.UtcNow, IsDeleted = true },
        };
        _bankRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(accounts.AsQueryable().BuildMock());

        var result = (await _sut.GetAllByUserAsync(1)).ToList();

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(1);
        result[0].UserId.Should().Be(1);
        result[0].Iban.Should().Be("SA0380000000608010167515");
        result[0].BankName.Should().Be("البنك الأهلي");
        result[0].IbanStatus.Should().Be("verified");
        result[0].AccountStatus.Should().Be("active");
    }

    [Fact]
    public async Task Create_PersistsAllFields_WithDefaultStatuses()
    {
        bank_accounts? added = null;
        _bankRepoMock.Setup(r => r.AddAsync(It.IsAny<bank_accounts>()))
            .Callback<bank_accounts>(a => added = a);

        var dto = new CreateBankAccountDto { Iban = "SA0380000000608010167515", BankName = "البنك الأهلي" };

        var result = await _sut.CreateAsync(dto, 1);

        added.Should().NotBeNull();
        added!.user_id.Should().Be(1);
        added!.iban.Should().Be("SA0380000000608010167515");
        added!.bank_name.Should().Be("البنك الأهلي");
        added!.iban_status.Should().Be("pending");
        added!.account_status.Should().Be("active");
        result.Id.Should().Be(added!.Id);
        result.Iban.Should().Be(dto.Iban);
        result.IbanStatus.Should().Be("pending");
        result.AccountStatus.Should().Be("active");
    }

    [Fact]
    public async Task Update_ByOwner_UpdatesFields()
    {
        var acc = new bank_accounts { Id = 1, user_id = 1, iban = "SA0380000000608010167515", bank_name = "البنك الأهلي" };
        _bankRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { acc }.AsQueryable().BuildMock());

        var dto = new CreateBankAccountDto { Iban = "SA1210000001234567891234", BankName = "بنك الراجحي" };

        var result = await _sut.UpdateAsync(1, dto, 1);

        result.Iban.Should().Be("SA1210000001234567891234");
        result.BankName.Should().Be("بنك الراجحي");
        acc.iban.Should().Be("SA1210000001234567891234");
        acc.bank_name.Should().Be("بنك الراجحي");
        _bankRepoMock.Verify(r => r.Update(It.IsAny<bank_accounts>()), Times.Once);
    }

    [Fact]
    public async Task Update_NonExisting_ThrowsKeyNotFoundException()
    {
        _bankRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<bank_accounts>().AsQueryable().BuildMock());

        var dto = new CreateBankAccountDto { Iban = "SA1210000001234567891234", BankName = "الراجحي" };

        var act = () => _sut.UpdateAsync(999, dto, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Update_WrongOwner_ThrowsUnauthorizedAccessException()
    {
        var acc = new bank_accounts { Id = 1, user_id = 2, iban = "SA0380000000608010167515", bank_name = "البنك الأهلي" };
        _bankRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { acc }.AsQueryable().BuildMock());

        var dto = new CreateBankAccountDto { Iban = "SA1210000001234567891234", BankName = "الراجحي" };

        var act = () => _sut.UpdateAsync(1, dto, 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Delete_ByOwner_SoftDeletes()
    {
        var acc = new bank_accounts { Id = 1, user_id = 1, iban = "SA0380000000608010167515", bank_name = "البنك الأهلي" };
        _bankRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { acc }.AsQueryable().BuildMock());

        await _sut.DeleteAsync(1, 1);

        acc.IsDeleted.Should().BeTrue();
        acc.DeletedTime.Should().NotBeNull();
        _bankRepoMock.Verify(r => r.Update(It.IsAny<bank_accounts>()), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExisting_ThrowsKeyNotFoundException()
    {
        _bankRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new List<bank_accounts>().AsQueryable().BuildMock());

        var act = () => _sut.DeleteAsync(999, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Delete_WrongOwner_ThrowsUnauthorizedAccessException()
    {
        var acc = new bank_accounts { Id = 1, user_id = 2, iban = "SA0380000000608010167515", bank_name = "البنك الأهلي" };
        _bankRepoMock.Setup(r => r.GetQueryable())
            .ReturnsAsync(new[] { acc }.AsQueryable().BuildMock());

        var act = () => _sut.DeleteAsync(1, 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}