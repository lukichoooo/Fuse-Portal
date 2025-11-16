using Core.Entities;

namespace Core.Interfaces;

public interface IUniversityRepo
{
    Task<IEnumerable<University>> GetAllAsync();
    Task<University> GetAsync(int id);
    Task<University> CreateAsync(University university);
    Task<University> UpdateAsync(University university);
    Task<University> DeleteAsync(int id);

    Task<IEnumerable<University>> SearchAsync(string name);
    Task<IEnumerable<User>> GetUsersAsync(int id);
}
