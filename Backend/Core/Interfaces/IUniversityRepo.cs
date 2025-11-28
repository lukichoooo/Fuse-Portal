using Core.Entities;

namespace Core.Interfaces;

public interface IUniversityRepo
{
    Task<University?> GetByIdAsync(int id);
    Task<University> CreateAsync(University university);
    Task<University> UpdateAsync(University university);
    Task<University> DeleteByIdAsync(int id);

    Task<List<University>> GetPageAsync(int lastId, int pageSize);
    Task<List<University>> PageByNameAsync(string name, int lastId = -1, int pageSize = 16);
    Task<List<User>> GetUsersPageAsync(int id, int LastId, int pageSize);
}
