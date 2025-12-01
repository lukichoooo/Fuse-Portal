using Core.Dtos;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);

        Task<UserDetailsDto> GetUserDetailsAsync(int id);
        Task<UserDetailsDto> DeleteByIdAsync(int id);

        Task<UserPrivateDto> UpdateUserCredentialsAsync(UserPrivateDto dto);
        Task<UserPrivateDto> GetPrivateDtoById(int id);

        Task<List<UserDto>> GetAllPageAsync(int lastId, int pageSize);
        Task<List<UserDto>> GetPageByNameAsync(string name, int lastId, int pageSize);
    }
}

