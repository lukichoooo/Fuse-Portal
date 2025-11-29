using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces;

namespace Infrastructure.Services;

public class AuthService(IUserRepo repo, IUserMapper mapper, IEncryptor encryptor, IJwtTokenGenerator jwt) : IAuthService
{
    private readonly IUserRepo _repo = repo;
    private readonly IUserMapper _mapper = mapper;
    private readonly IEncryptor _encryptor = encryptor;
    private readonly IJwtTokenGenerator _jwt = jwt;

    public async Task<AuthResponse> LoginAsync(LoginRequest login)
    {
        var user = await _repo.GetByEmailAsync(login.Email)
            ?? throw new UserNotFoundException($"User not found with Email={login.Email}");
        if (_encryptor.Encrypt(login.Password) != user.Password)
            throw new UserWrongCredentialsException($"Wrong Password={login.Password}");
        return new AuthResponse(_jwt.GenerateToken(user), null);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest register)
    {
        var user = await _repo.GetByEmailAsync(register.Email);
        if (user is not null)
            throw new UserAlreadyExistsException($"Email={register.Email} already in use.");
        user = _mapper.ToUser(register);
        user.Password = _encryptor.Encrypt(user.Password);
        await _repo.CreateAsync(user);
        return new AuthResponse(_jwt.GenerateToken(user), null);
    }
}
