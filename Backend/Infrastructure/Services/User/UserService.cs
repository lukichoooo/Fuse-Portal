using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces;
using Core.Interfaces.Auth;

namespace Infrastructure.Services;

public class UserService(
        IUserMapper mapper,
        IEncryptor encryptor,
        IUserRepo userRepo,
        ICurrentContext currentContext
        ) : IUserService
{
    private readonly IUserRepo _repo = userRepo;
    private readonly IUserMapper _mapper = mapper;
    private readonly IEncryptor _encryptor = encryptor;
    private readonly ICurrentContext _currentContext = currentContext;

    public async Task<List<UserDto>> GetAllPageAsync(int? lastId, int pageSize)
        => (await _repo.GetAllPageAsync(lastId, pageSize))
        .ConvertAll(_mapper.ToDto)
        .ToList();

    public async Task<List<UserDto>> GetPageByNameAsync(string name, int? lastId, int pageSize)
        => (await _repo.PageByNameAsync(name, lastId, pageSize))
        .ConvertAll(_mapper.ToDto)
        .ToList();

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id)
            ?? throw new UserNotFoundException($"User not found with Id={id}");
        return _mapper.ToDto(user);
    }

    public async Task<UserPrivateDto> GetCurrentUserPrivateDtoAsync()
    {
        int id = _currentContext.GetCurrentUserId();
        var user = await _repo.GetByIdAsync(id)
            ?? throw new UserNotFoundException($"User not found with Id={id}");
        var dto = _mapper.ToPrivateDto(user);
        dto.Password = _encryptor.Decrypt(dto.Password);
        return dto;
    }


    public async Task<UserPrivateDto> UpdateCurrentUserCredentialsAsync(UserUpdateRequest request)
    {
        int userId = _currentContext.GetCurrentUserId();
        var user = _mapper.ToUser(request, userId);
        user.Password = _encryptor.Encrypt(user.Password);
        return _mapper.ToPrivateDto(await _repo.UpdateUserCredentialsAsync(user));
    }

    public async Task<UserDetailsDto> DeleteCurrentUserAsync()
    {
        int id = _currentContext.GetCurrentUserId();
        return await DeleteByIdAsync(id);
    }

    public async Task<UserDetailsDto> DeleteByIdAsync(int id)
        => _mapper.ToDetailsDto(await _repo.DeleteByIdAsync(id));

    public async Task<UserDetailsDto> GetUserDetailsAsync(int id)
    {
        var user = await _repo.GetUserDetailsByIdAsync(id)
            ?? throw new UserNotFoundException($"User not found with Id={id}");
        return _mapper.ToDetailsDto(user);
    }

}
