using Core.Dtos;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UserService(IUserMapper mapper, IUniversityMapper uniMapper, IUserRepo userRepo) : IUserService
{
    private readonly IUserRepo _repo = userRepo;
    private readonly IUserMapper _mapper = mapper;
    private readonly IUniversityMapper _uniMapper = uniMapper;

    public async Task<List<UserDto>> GetAllPageAsync(int lastId, int pageSize)
        => (await _repo.GetAllPageAsync(lastId, pageSize))
        .ConvertAll(_mapper.ToDto)
        .ToList();

    public async Task<List<UserDto>> PageByNameAsync(string name, int lastId, int pageSize)
        => (await _repo.PageByNameAsync(name, lastId, pageSize))
        .ConvertAll(_mapper.ToDto)
        .ToList();

    public async Task<UserDto> GetAsync(int id)
    {
        var user = await _repo.GetAsync(id)
            ?? throw new UserNotFoundException($"User not found with Id={id}");
        return _mapper.ToDto(user);
    }

    public async Task<UserPrivateInfo> GetPrivateDtoById(int id)
    {
        var user = await _repo.GetAsync(id)
            ?? throw new UserNotFoundException($"User not found with Id={id}");
        return _mapper.ToPrivateInfo(user);
    }


    public async Task<UserPrivateInfo> UpdateUserCredentialsAsync(UserPrivateInfo info)
        => _mapper.ToPrivateInfo(await _repo.UpdateUserCredentialsAsync(_mapper.ToUser(info)));

    public async Task<UserPrivateInfo> DeleteByIdAsync(int id)
        => _mapper.ToPrivateInfo(await _repo.DeleteByIdAsync(id));

    public async Task<UserDetailsDto> GetUserDetailsAsync(int id)
    {
        var user = await _repo.GetUserDetailsAsync(id)
            ?? throw new UserNotFoundException($"User not found with Id={id}");
        return _mapper.ToDetailsDto(user);
    }
}
