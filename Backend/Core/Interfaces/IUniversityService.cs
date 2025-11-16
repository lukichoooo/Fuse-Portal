
using Core.Entities;

namespace Core.Interfaces;

public interface IUniversityService
{
    Task<IEnumerable<University>> GetAllAsync();
    Task<University> GetAsync(int id);
    Task<University> CreateAsync(University uni);
    Task<University> UpdateAsync(University uni);
    Task<University> DeleteAsync(int id);

    Task<IEnumerable<University>> SearchAsync(string name);
    Task<IEnumerable<User>> GetUsersAsync(int id);
}
