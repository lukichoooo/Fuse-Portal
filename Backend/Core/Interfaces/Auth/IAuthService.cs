using Core.Dtos;

namespace Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest userRegister);
    Task<AuthResponse> LoginAsync(LoginRequest userLogin);
}
