namespace Core.Dtos;


public record LoginRequest(
        string Email,
        string Password
        );

public record RegisterRequest(
        string Name,
        string Email,
        string Password
        );

public record LoginResponse(
        string accessToken,
        string refreshToken
        );

public record RegisterResponse(
        string accessToken,
        string refreshToken
        );
