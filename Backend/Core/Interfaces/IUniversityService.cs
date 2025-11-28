
using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces;

public interface IUniversityService
{
    Task<List<University>> GetAllPageAsync(int lastId, int pageSize);
    Task<UniDto> GetAsync(int id);
    Task<UniDto> CreateAsync(University uni);
    Task<UniDto> UpdateAsync(University uni);
    Task<UniDto> DeleteByIdAsync(int id);

    Task<List<UniDto>> PageByNameAsync(string name);
    Task<List<UserDto>> GetUsersAsync(int id);
}
