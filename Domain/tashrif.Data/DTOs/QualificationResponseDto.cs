namespace tashrif.Data.DTOs;

public class QualificationResponseDto
{
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public short? GraduationYear { get; set; }
    public string Grade { get; set; } = string.Empty;
}