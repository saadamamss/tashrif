namespace tashrif.Data.DTOs;

public class CreateQualificationDto
{
    public string QualificationType { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public short? GraduationYear { get; set; }
}