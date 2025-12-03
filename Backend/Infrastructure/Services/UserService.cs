using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UserService(IUserMapper mapper, IEncryptor encryptor, IUserRepo userRepo) : IUserService
{
    private readonly IUserRepo _repo = userRepo;
    private readonly IUserMapper _mapper = mapper;
    private readonly IEncryptor _encryptor = encryptor;

    public async Task<List<UserDto>> GetAllPageAsync(int lastId, int pageSize)
        => (await _repo.GetAllPageAsync(lastId, pageSize))
        .ConvertAll(_mapper.ToDto)
        .ToList();

    public async Task<List<UserDto>> GetPageByNameAsync(string name, int lastId, int pageSize)
        => (await _repo.PageByNameAsync(name, lastId, pageSize))
        .ConvertAll(_mapper.ToDto)
        .ToList();

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id)
            ?? throw new UserNotFoundException($"User not found with Id={id}");
        return _mapper.ToDto(user);
    }

    public async Task<UserPrivateDto> GetPrivateDtoById(int id)
    {
        var user = await _repo.GetByIdAsync(id)
            ?? throw new UserNotFoundException($"User not found with Id={id}");
        user.Password = _encryptor.Encrypt(user.Password);
        return _mapper.ToPrivateDto(user);
    }


    public async Task<UserPrivateDto> UpdateUserCredentialsAsync(UserPrivateDto info)
    {
        var user = _mapper.ToUser(info);
        user.Password = _encryptor.Encrypt(user.Password);
        return _mapper.ToPrivateDto(await _repo.UpdateUserCredentialsAsync(user));
    }

    public async Task<UserDetailsDto> DeleteByIdAsync(int id)
        => _mapper.ToDetailsDto(await _repo.DeleteByIdAsync(id));

    public async Task<UserDetailsDto> GetUserDetailsAsync(int id)
    {
        var user = await _repo.GetUserDetailsAsync(id)
            ?? throw new UserNotFoundException($"User not found with Id={id}");
        return _mapper.ToDetailsDto(user);
    }
}
