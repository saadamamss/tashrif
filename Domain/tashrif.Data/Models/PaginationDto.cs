using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.Models;

public class PaginationDto
{
    [Range(1, int.MaxValue, ErrorMessage = "الصفحة يجب أن تكون 1 على الأقل")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "الحد الأقصى 100 عنصر لكل صفحة")]
    public int Limit { get; set; } = 10;

    public string? SearchQuery { get; set; }
}
