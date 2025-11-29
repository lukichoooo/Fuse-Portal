
using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces;

public interface IUniversityService
{
    Task<UniDto> GetAsync(int id);
    Task<UniDto> CreateAsync(UniRequestDto info);
    Task<UniDto> UpdateAsync(UniRequestDto info);
    Task<UniDto> DeleteByIdAsync(int id);

    Task<List<UniDto>> GetPageByNameAsync(string name, int lastId, int pageSize);
    Task<List<UserDto>> GetUsersPageAsync(int id, int lastId, int pageSize);
}
