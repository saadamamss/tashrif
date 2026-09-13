namespace tashrif.Data.DTOs;

public class MessageResponseDto
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public long SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SendMessageDto
{
    [Required]
    [MaxLength(2000)]
    public string Body { get; set; } = string.Empty;
}
