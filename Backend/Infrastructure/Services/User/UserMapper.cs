using Core.Dtos;
using Core.Entities;
using Core.Entities.JoinTables;
using Core.Interfaces;
using Core.Interfaces.Portal;

namespace Infrastructure.Services
{
    public class UserMapper(
            IPortalMapper portalMapper,
            IUniversityMapper uniMapper) : IUserMapper
    {
        private readonly IPortalMapper _portalMapper = portalMapper;
        private readonly IUniversityMapper _uniMapper = uniMapper;

        public UserDto ToDto(User user)
            => new(
                    Id: user.Id,
                    Name: user.Name
                    );

        public UserDetailsDto ToDetailsDto(User user)
            => new(
                Id: user.Id,
                Name: user.Name,
                Universities: user.UserUniversities
                    .ConvertAll(uu =>
                        new UniDto(uu.UniversityId, uu.University?.Name!)),
                Subjects: user.Subjects
                    .ConvertAll(_portalMapper.ToSubjectDto)
                    );

        public UserPrivateDto ToPrivateDto(User user)
            => new()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Address = user.Address,

                Subjects = user.Subjects.ConvertAll(_portalMapper.ToSubjectDto),
                Universities = user.UserUniversities.ConvertAll(uu => _uniMapper.ToDto(uu.University!))
            };

        public User ToUser(UserUpdateRequest request, int userId)
            => new()
            {
                Id = userId,
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                Address = new()
                {
                    City = request.Address.City,
                    CountryA3 = request.Address.CountryA3
                },

                UserUniversities = request.Universities?.ConvertAll(
                        uni => new UserUniversity { UniversityId = uni.Id, UserId = userId }) ?? [],

                Subjects = request.Subjects?
                    .ConvertAll(s => _portalMapper.ToSubject(s, userId)) ?? [],
            };



        public User ToUser(UserPrivateDto info)
            => new()
            {
                Id = info.Id,
                Name = info.Name,
                Email = info.Email,
                Password = info.Password,
                Address = info.Address
            };
    }
}

