namespace tashrif.Tests.Services;

// Spec 01 (phase3-6-remove-registration-cv): cv_file was removed from the completion fields,
// so the denominator is now 10 (3 user fields + 7 profile fields), not 11.
public class IndividualCompletionCalculatorTests
{
    [Fact]
    public void Calculate_AllFieldsFilled_Returns100()
    {
        var user = new users { phone = "0500000000", gender = "male", nationality = "سعودي" };
        var profile = new individual_profiles
        {
            city = "الرياض",
            zone = "منطقة الرياض",
            district = "حي",
            street = "شارع",
            zipcode = "12345",
            job_title = "مهندس",
            id_file = "/uploads/ids/x.jpg",
        };

        IndividualCompletionCalculator.Calculate(user, profile).Should().Be(100);
    }

    [Fact]
    public void Calculate_NothingFilled_Returns0()
    {
        var user = new users { phone = "", gender = "", nationality = "" };
        var profile = new individual_profiles();

        IndividualCompletionCalculator.Calculate(user, profile).Should().Be(0);
    }

    [Fact]
    public void Calculate_OnlyIdFileMissing_IsNot100()
    {
        var user = new users { phone = "0500000000", gender = "male", nationality = "سعودي" };
        var profile = new individual_profiles
        {
            city = "الرياض",
            zone = "منطقة الرياض",
            district = "حي",
            street = "شارع",
            zipcode = "12345",
            job_title = "مهندس",
            id_file = "",
        };

        IndividualCompletionCalculator.Calculate(user, profile).Should().Be(90);
    }
}
