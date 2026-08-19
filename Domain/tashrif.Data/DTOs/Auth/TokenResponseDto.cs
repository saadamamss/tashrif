namespace tashrif.Data.DTOs.Auth;

public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    [System.Text.Json.Serialization.JsonIgnore]
    public string RefreshToken { get; set; } = string.Empty;
}
