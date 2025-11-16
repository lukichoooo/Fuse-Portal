using Core.Entities;

namespace Core.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetAsync(int id);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<User> DeleteAsync(int id);

    Task<IEnumerable<User>> SearchAsync(string name);
    Task<University> GetUniversityAsync(int id);
}
