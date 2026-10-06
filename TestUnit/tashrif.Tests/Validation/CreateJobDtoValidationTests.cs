using System.ComponentModel.DataAnnotations;

namespace tashrif.Tests.Validation;

// CreateJobDto Gender/Type/Location are allow-listed to canonical English codes
// (gender: male/female/both; type: full-time/part-time/seasonal; location: 16 city codes).
// Publish is the only job write path and goes through [ApiController] auto-validation,
// so DataAnnotations are the enforcement point — these tests pin the attribute behavior.
public class CreateJobDtoValidationTests
{
    private static CreateJobDto ValidDto(string gender) => new()
    {
        Title = "مشرف حجاج",
        Description = "وصف الوظيفة",
        Location = "makkah",
        Type = "full-time",
        Salary = "8000",
        Vacancies = 5,
        Target = "both",
        Qualification = "بكالوريوس",
        Gender = gender,
        Hours = "8",
        Duration = "month",
    };

    private static List<ValidationResult> Validate(CreateJobDto dto)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);
        return results;
    }

    [Theory]
    [InlineData("male")]
    [InlineData("female")]
    [InlineData("both")]
    public void Gender_CanonicalValues_PassValidation(string gender)
    {
        Validate(ValidDto(gender)).Should().BeEmpty();
    }

    [Theory]
    [InlineData("any")]
    [InlineData("ذكر")]
    [InlineData("male ")]
    public void Gender_NonCanonicalValues_FailWithArabicMessage(string gender)
    {
        var results = Validate(ValidDto(gender));
        results.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("الجنس غير صحيح");
    }

    [Theory]
    [InlineData("full-time")]
    [InlineData("part-time")]
    [InlineData("seasonal")]
    public void Type_CanonicalValues_PassValidation(string type)
    {
        var dto = ValidDto("both");
        dto.Type = type;
        Validate(dto).Should().BeEmpty();
    }

    [Theory]
    [InlineData("دوام كامل")]
    [InlineData("contract")]
    public void Type_NonCanonicalValues_FailWithArabicMessage(string type)
    {
        var dto = ValidDto("both");
        dto.Type = type;
        Validate(dto).Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("نوع العمل غير صحيح");
    }

    [Theory]
    [InlineData("makkah")]
    [InlineData("madinah")]
    [InlineData("jeddah")]
    [InlineData("taif")]
    [InlineData("mina")]
    [InlineData("arafat")]
    [InlineData("muzdalifah")]
    [InlineData("rabigh")]
    [InlineData("khulais")]
    [InlineData("bahrah")]
    [InlineData("jumum")]
    [InlineData("allith")]
    [InlineData("qunfudhah")]
    [InlineData("yanbu")]
    [InlineData("badr")]
    [InlineData("riyadh")]
    public void Location_CanonicalValues_PassValidation(string location)
    {
        var dto = ValidDto("both");
        dto.Location = location;
        Validate(dto).Should().BeEmpty();
    }

    [Theory]
    [InlineData("مكة المكرمة")]
    [InlineData("test")]
    public void Location_NonCanonicalValues_FailWithArabicMessage(string location)
    {
        var dto = ValidDto("both");
        dto.Location = location;
        Validate(dto).Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("الموقع غير صحيح");
    }

    [Theory]
    [InlineData("month")]
    [InlineData("2months")]
    [InlineData("3months")]
    [InlineData("6months")]
    [InlineData("year")]
    [InlineData("2years")]
    [InlineData("continuous")]
    public void Duration_CanonicalValues_PassValidation(string duration)
    {
        var dto = ValidDto("both");
        dto.Duration = duration;
        Validate(dto).Should().BeEmpty();
    }

    [Theory]
    [InlineData("شهر")]
    [InlineData("موسم الحج")]
    public void Duration_NonCanonicalValues_FailWithArabicMessage(string duration)
    {
        var dto = ValidDto("both");
        dto.Duration = duration;
        Validate(dto).Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("المدة غير صحيحة");
    }

    [Theory]
    [InlineData("6")]
    [InlineData("8")]
    [InlineData("10")]
    [InlineData("12")]
    public void Hours_CanonicalValues_PassValidation(string hours)
    {
        var dto = ValidDto("both");
        dto.Hours = hours;
        Validate(dto).Should().BeEmpty();
    }

    [Theory]
    [InlineData("8 ساعات")]
    [InlineData("8h")]
    public void Hours_NonCanonicalValues_FailWithArabicMessage(string hours)
    {
        var dto = ValidDto("both");
        dto.Hours = hours;
        Validate(dto).Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("ساعات العمل غير صحيحة");
    }
}
