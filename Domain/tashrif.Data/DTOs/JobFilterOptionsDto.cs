namespace tashrif.Data.DTOs;

using System.Text.Json.Serialization;

public class JobFilterOptionsDto
{
    [JsonPropertyName("workTypes")]
    public List<string> Types { get; set; } = new();
    public List<string> Locations { get; set; } = new();
    public List<string> Genders { get; set; } = new();
    public List<FilterOptionDto> Entities { get; set; } = new();
    public List<FilterOptionDto> Statuses { get; set; } = new();
}

public class FilterOptionDto
{
    public object? Value { get; set; }
    public string Label { get; set; } = string.Empty;
}