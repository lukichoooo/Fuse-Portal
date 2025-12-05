using Core.Dtos;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);

        Task<UserDetailsDto> GetUserDetailsAsync(int id);

        Task<List<UserDto>> GetAllPageAsync(int? lastId, int pageSize);
        Task<List<UserDto>> GetPageByNameAsync(string name, int? lastId, int pageSize);
        Task<UserDetailsDto> DeleteByIdAsync(int id);

        // Validation
        Task<UserPrivateDto> UpdateCurrentUserCredentialsAsync(UserUpdateRequest request);
        Task<UserPrivateDto> GetCurrentUserPrivateDtoAsync();
        Task<UserDetailsDto> DeleteCurrentUserAsync();
    }
}

