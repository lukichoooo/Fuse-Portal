using Core.Entities;

namespace Core.Interfaces
{
    public interface IUserRepo
    {
        Task<User?> GetUserDetailsAsync(int id);
        ValueTask<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateUserCredentialsAsync(User user);
        Task<User> DeleteByIdAsync(int id);

        Task<List<User>> GetAllPageAsync(int lastId, int pageSize);
        Task<List<User>> PageByNameAsync(string name, int lastId, int pageSize);
        Task<List<University>> GetUnisForUserAsync(int userId);
    }
}

