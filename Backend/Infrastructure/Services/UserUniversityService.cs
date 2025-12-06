using Core.Dtos;
using Core.Interfaces;
using Core.Interfaces.UserUniversityTable;

namespace Infrastructure.Services
{
    public class UserUniversityService(
            IUserUniversityRepo repo,
            IUserMapper userMapper,
            IUniversityMapper uniMapper
            ) : IUserUniversityService
    {
        private readonly IUserUniversityRepo _repo = repo;
        private readonly IUserMapper _userMapper = userMapper;
        private readonly IUniversityMapper _uniMapper = uniMapper;

        public Task<List<UniDto>> GetUnisPageForUserIdAsync(int userId, int? lastId, int pageSize)
            => throw new NotImplementedException();

        public async Task<List<UserDto>> GetUsersByUniIdPageAsync(int id, int? lastId, int pageSize)
            => (await _repo.GetUsersByUniIdPageAsync(id, lastId, pageSize))
            .ConvertAll(_userMapper.ToDto);

    }
}
