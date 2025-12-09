using Core.Dtos;
using Core.Entities.JoinTables;
using Core.Interfaces;
using Core.Interfaces.Auth;
using Core.Interfaces.UserUniversityTable;

namespace Infrastructure.Services
{
    public class UserUniversityService(
            IUserUniversityRepo repo,
            IUserMapper userMapper,
            IUniversityMapper uniMapper,
            ICurrentContext currentContext
            ) : IUserUniversityService
    {
        private readonly IUserUniversityRepo _repo = repo;
        private readonly IUserMapper _userMapper = userMapper;
        private readonly IUniversityMapper _uniMapper = uniMapper;
        private readonly ICurrentContext _currentContext = currentContext;

        public async Task<UserUniversity> AddCurrentUserToUniversityAsync(int uniId)
            => await _repo.AddUserToUniversityAsync(new UserUniversity
            {
                UniversityId = uniId,
                UserId = _currentContext.GetCurrentUserId()
            });

        public async Task<UserUniversity> RemoveCurrentUserFromUniversityAsync(int uniId)
            => await _repo.RemoveUserFromUniversityAsync(
                    _currentContext.GetCurrentUserId(),
                    uniId);

        public async Task<List<UniDto>> GetUnisPageForUserIdAsync(
                int userId,
                int? lastId,
                int pageSize)
            => (await _repo.GetUnisPageForUserIdAsync(userId, lastId, pageSize))
            .ConvertAll(_uniMapper.ToDto);

        public async Task<List<UserDto>> GetUsersByUniIdPageAsync(
                int uniId,
                int? lastId,
                int pageSize)
            => (await _repo.GetUsersByUniIdPageAsync(uniId, lastId, pageSize))
            .ConvertAll(_userMapper.ToDto);
    }
}
