using Core.Entities;
using Core.Entities.JoinTables;

namespace Core.Interfaces.UserUniversityTable
{
    public interface IUserUniversityRepo
    {
        Task<List<User>> GetUsersByUniIdPageAsync(int id, int? lastId, int pageSize);
        Task<List<University>> GetUnisPageForUserIdAsync(int userId, int? lastId, int pageSize);
        Task<UserUniversity> AddUserToUniversityAsync(UserUniversity uu);
        Task<UserUniversity> RemoveUserFromUniversityAsync(int userId, int uniId);
    }
}
