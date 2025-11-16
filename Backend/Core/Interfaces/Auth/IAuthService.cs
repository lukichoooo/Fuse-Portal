using Core.Dtos;

namespace Core.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest userRegister);
    Task<LoginResponse> LoginAsync(LoginRequest userLogin);
}
