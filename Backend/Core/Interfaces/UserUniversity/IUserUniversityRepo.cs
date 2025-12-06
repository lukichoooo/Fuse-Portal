using Core.Entities;

namespace Core.Interfaces.UserUniversity
{
    public interface IUserUniversityRepo
    {
        Task<List<User>> GetUsersByUniIdPageAsync(int id, int? lastId, int pageSize);
        Task<List<University>> GetUnisPageForUserIdAsync(int userId, int? lastId, int pageSize);
    }
}
