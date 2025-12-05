namespace Core.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
    }
}

