
using Core.Dtos;

namespace Core.Interfaces
{
    public interface IUniversityService
    {
        Task<UniDto> GetByIdAsync(int id);
        Task<UniDto> CreateAsync(UniRequestDto info);
        Task<UniDto> UpdateAsync(UniRequestDto info);
        Task<UniDto> DeleteByIdAsync(int id);
        Task<List<UniDto>> GetPageByNameLikeAsync(string name, int lastId, int pageSize);

        Task<List<UserDto>> GetUsersPageAsync(int id, int lastId, int pageSize);
    }
}
