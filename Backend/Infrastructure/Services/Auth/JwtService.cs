using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces;

namespace Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IUserRepo _userRepo;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly IUserMapper _userMapper;

    public JwtService(IUserRepo userRepo, IJwtTokenGenerator jwtGenerator, IUserMapper userMapper)
    {
        _userMapper = userMapper;
        _userRepo = userRepo;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<RegisterResponse> GenerateTokenAsync(RegisterRequest request)
    {
        if (await _userRepo.ExistsAsync(request.Email))
            throw new UserAlreadyExistsException($"User Already Exists With Email {request.Email}");

        var user = _userMapper.ToUser(request);
        var token = _jwtGenerator.GenerateToken(user);
        await _userRepo.CreateAsync(user);
        return new RegisterResponse(token, ""); // TODO: Add refresh token
    }

    public async Task<LoginResponse> GenerateTokenAsync(LoginRequest user)
    {
        if (!await _userRepo.ExistsAsync(user.Email))
            throw new UserNotFoundException($"User Not Found With Email {user.Email}");

        var onDbUser = await _userRepo.GetAsync(user.Email);
        var token = _jwtGenerator.GenerateToken(onDbUser);
        return new LoginResponse(token, ""); // TODO: Add refresh token
    }
}
