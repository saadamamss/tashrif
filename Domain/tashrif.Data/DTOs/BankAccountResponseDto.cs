namespace tashrif.Data.DTOs;

public class BankAccountResponseDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Iban { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string IbanStatus { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
}
