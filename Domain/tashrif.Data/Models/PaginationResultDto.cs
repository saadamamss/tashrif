using System.Text.Json.Serialization;

namespace tashrif.Data.Models;

public class PaginationResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / Math.Max(Limit, 1));

    [JsonIgnore]
    public List<T> Data { get => Items; set => Items = value; }
    [JsonIgnore]
    public int TotalRecords { get => Total; set => Total = value; }
}
