namespace tashrif.Tests.Validation;

// Spec 01 (phase3-7): Register() hand-builds DTOs from Request.Form, so [ApiController]
// auto-validation never fires — RequestValidator is the enforcement point (see AuthController).
public class RequestValidatorTests
{
    private static RegisterEntityDto ValidEntityDto() => new()
    {
        CompanyName = "شركة اختبار",
        NationalId = "7001234567",
        CommercialReg = "1010123456",
        Password = "TestPass123",
        FieldName = "خدمات الحج",
        Sector = "حج وعمرة",
        Country = "السعودية",
        Province = "مكة المكرمة",
        CompanyDesc = "وصف تجريبي",
        CompanyWebsite = "https://example.com",
        Name = "مسؤول الاتصال",
        Email = "c@example.com",
        Phone = "+966500000002",
        Role = "مدير",
        Nationality = "سعودي",
    };

    [Fact]
    public void Validate_FullyFilledEntityDto_DoesNotThrow()
    {
        var act = () => RequestValidator.Validate(ValidEntityDto());
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_ShortCommercialReg_Throws400WithArabicMessage()
    {
        // This is the user-14 shape ("5566") — must no longer slip through registration.
        var dto = ValidEntityDto();
        dto.CommercialReg = "5566";

        var act = () => RequestValidator.Validate(dto);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_MissingNationalId_Throws400()
    {
        var dto = ValidEntityDto();
        dto.NationalId = "";

        var act = () => RequestValidator.Validate(dto);
        act.Should().Throw<ArgumentException>()
            .WithMessage("رقم الهوية مطلوب");
    }

    [Fact]
    public void Validate_ShortIndividualNationalId_Throws400()
    {
        var dto = new RegisterIndividualDto
        {
            FirstName = "سارة",
            LastName = "علي",
            NationalId = "12345",
            Password = "TestPass123",
            Phone = "+966500000000",
            Email = "s@b.com",
            Gender = "female",
            Nationality = "سعودي",
        };

        var act = () => RequestValidator.Validate(dto);
        act.Should().Throw<ArgumentException>()
            .WithMessage("رقم الهوية يجب أن يكون 10 أرقام");
    }
}
