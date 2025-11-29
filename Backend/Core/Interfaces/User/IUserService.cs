using Core.Dtos;

namespace Core.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllPageAsync(int lastId, int pageSize);
    Task<UserDto> GetAsync(int id);
    Task<UserDetailsDto> GetUserDetailsAsync(int id);
    Task<UserRequestDto> UpdateUserCredentialsAsync(UserRequestDto dto);
    Task<UserRequestDto> DeleteByIdAsync(int id);

    Task<List<UserDto>> PageByNameAsync(string name, int lastId, int pageSize);
    Task<UserRequestDto> GetPrivateDtoById(int id);
}
