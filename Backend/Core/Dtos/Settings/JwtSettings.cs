namespace Core.Dtos;

public class JwtSettings
{
    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int AccessTokenExpiration { get; set; }
    public int RefreshTokenExpiration { get; set; }
}
