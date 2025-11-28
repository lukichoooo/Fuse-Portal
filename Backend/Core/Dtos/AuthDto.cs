namespace Core.Dtos
{
    public record LoginRequest(
            string Email,
            string Password
            );

    public record RegisterRequest(
            string Name,
            string Email,
            string Password,
            List<string> Faculties
            );

    public record AuthResponse(
            string AccessToken,
            string RefreshToken
            );
}


