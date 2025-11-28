using Core.Dtos;

namespace Core.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllPageAsync(int lastId, int pageSize);
    Task<UserDto> GetAsync(int id);
    Task<UserDetailsDto> GetUserDetailsAsync(int id);
    Task<UserPrivateInfo> UpdateUserCredentialsAsync(UserPrivateInfo dto);
    Task<UserPrivateInfo> DeleteByIdAsync(int id);

    Task<List<UserDto>> PageByNameAsync(string name, int lastId, int pageSize);
    Task<UserPrivateInfo> GetPrivateDtoById(int id);
}
