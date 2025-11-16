using Core.Dtos;

namespace Core.Interfaces;

public interface IJwtService
{
    Task<RegisterResponse> GenerateTokenAsync(RegisterRequest user);
    Task<LoginResponse> GenerateTokenAsync(LoginRequest user);
}
