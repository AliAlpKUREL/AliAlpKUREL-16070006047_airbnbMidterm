namespace AirbnbMinimal.Options;

public class JwtTokenOptions
{
    public const string Name = "JwtToken";
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecurityKey { get; init; } = string.Empty;
    public int ExpirationMinute { get; init; }
}