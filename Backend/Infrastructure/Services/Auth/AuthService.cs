using Core.Dtos;
using Core.Interfaces;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;

    public AuthService(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest userLogin)
    {
        return await _jwtService.GenerateTokenAsync(userLogin);
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest userRegister)
    {
        return await _jwtService.GenerateTokenAsync(userRegister);
    }
}
