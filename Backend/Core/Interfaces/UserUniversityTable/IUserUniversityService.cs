using Core.Dtos;
using Core.Entities.JoinTables;

namespace Core.Interfaces.UserUniversityTable
{
    public interface IUserUniversityService
    {

        Task<List<UniDto>> GetUnisPageForUserIdAsync(int userId, int? lastId, int pageSize);
        Task<List<UserDto>> GetUsersByUniIdPageAsync(int uniId, int? lastId, int pageSize);
        Task<UserUniversity> AddCurrentUserToUniversityAsync(int uniId);
        Task<UserUniversity> RemoveCurrentUserFromUniversityAsync(int uniId);
    }
}
