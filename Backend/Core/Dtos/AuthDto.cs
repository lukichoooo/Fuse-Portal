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
            AddressDto Address,
            List<UniDto> Universities
            );

    public record AuthResponse(
            string AccessToken,
            string RefreshToken
            );
}


