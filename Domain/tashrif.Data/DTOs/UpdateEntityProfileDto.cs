using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class UpdateEntityProfileDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [Phone(ErrorMessage = "رقم الجوال غير صحيح")]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? CompanyField { get; set; }

    [MaxLength(50)]
    public string? CompanySize { get; set; }

    [MaxLength(100)]
    public string? CommercialReg { get; set; }

    [MaxLength(100)]
    public string? Sector { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

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
    public string? Province { get; set; }

    [MaxLength(2000)]
    public string? CompanyDesc { get; set; }

    [Url(ErrorMessage = "رابط الموقع غير صحيح")]
    public string? Website { get; set; }

    [Url(ErrorMessage = "رابط تويتر غير صحيح")]
    public string? TwitterAccount { get; set; }

    [Url(ErrorMessage = "رابط فيسبوك غير صحيح")]
    public string? FacebookAccount { get; set; }

    [Url(ErrorMessage = "رابط يوتيوب غير صحيح")]
    public string? YoutubeAccount { get; set; }

    public string? LogoUrl { get; set; }
}
