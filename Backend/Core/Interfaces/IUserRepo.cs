using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepo
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetAsync(int id);
    Task<User> GetAsync(string email);
    Task<bool> ExistsAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<User> DeleteAsync(int id);

    Task<IEnumerable<User>> SearchAsync(string name);
    Task<University> GetUniversityAsync(int id);
}
