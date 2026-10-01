namespace tashrif.Core;

public static class IndividualCompletionCalculator
{
    public static short Calculate(users user, individual_profiles profile)
    {
        var fields = new[]
        {
            user.phone, user.gender, user.nationality,
            profile.city, profile.zone, profile.district, profile.street, profile.zipcode,
            profile.job_title, profile.id_file,
        };
        var filled = fields.Count(f => !string.IsNullOrWhiteSpace(f));
        return (short)Math.Round(filled * 100.0 / fields.Length);
    }
}