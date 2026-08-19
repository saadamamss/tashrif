namespace tashrif.Data.DTOs;

public class CreateBankAccountDto
{
    public string Iban { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
}
