using Core.Entities;

namespace Core.Interfaces
{
    public interface IUniversityRepo
    {
        Task<University?> GetByIdAsync(int id);
        Task<University?> GetByNameAsync(string name);
        Task<University> CreateAsync(University university);
        Task<University> UpdateAsync(University university);
        Task<University> DeleteByIdAsync(int id);

        Task<List<University>> GetPageAsync(int? lastId, int pageSize);
        Task<List<University>> GetPageByNameAsync(string name, int? lastId, int pageSize);
    }
}

