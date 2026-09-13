using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class UpdateIndividualProfileDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [Phone(ErrorMessage = "رقم الجوال غير صحيح")]
    public string? Phone { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [MaxLength(100)]
    public string? Nationality { get; set; }

    public DateTime? BirthDate { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Zone { get; set; }

    [MaxLength(100)]
    public string? District { get; set; }

    [MaxLength(200)]
    public string? Street { get; set; }

    [MaxLength(10)]
    public string? Zipcode { get; set; }

    [MaxLength(100)]
    public string? JobTitle { get; set; }

    public string? AvatarUrl { get; set; }
}
