namespace tashrif.Data.DTOs.Analytics;

public class EntityAnalyticsDto
{
    public int TotalJobs { get; set; }
    public int ActiveJobs { get; set; }
    public int TotalApplications { get; set; }
    public Dictionary<string, int> ApplicationsByStatus { get; set; } = new();
    public double ConversionRate { get; set; }
    public double? AverageTimeToHireDays { get; set; }
    public List<TopJobDto> TopJobs { get; set; } = new();
    public List<DailyCountDto> ApplicationsOverTime { get; set; } = new();
    public DemographicsDto ApplicantDemographics { get; set; } = new();
}

public class TopJobDto
{
    public long JobId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ApplicationCount { get; set; }
    public int HiredCount { get; set; }
}

public class DailyCountDto
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DemographicsDto
{
    public Dictionary<string, int> ByGender { get; set; } = new();
    public Dictionary<string, int> ByNationality { get; set; } = new();
}
