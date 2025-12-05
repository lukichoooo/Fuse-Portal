using Core.Dtos;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services
{
    public class UserMapper : IUserMapper
    {
        public UserDto ToDto(User user)
            => new(
                    Id: user.Id,
                    Name: user.Name
                    );

        public UserDetailsDto ToDetailsDto(User user)
            => new(
                        Id: user.Id,
                        Name: user.Name,
                        Universities: user.Universities
                            .ConvertAll(uni => new UniDto(uni.Id, uni.Name)),
                        Courses: user.Courses.ConvertAll(f => f.Name)
                    );

        public UserPrivateDto ToPrivateDto(User user)
            => new()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Address = user.Address
            };

        public User ToUser(UserUpdateRequest request)
            => new()
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                Address = new()
                {
                    City = request.Address.City,
                    CountryA3 = request.Address.CountryA3
                },

                Courses = request.Courses?
                                .ConvertAll(x => new Course { Name = x })
                                ?? [],

                Universities = request.Universities?
                                .ConvertAll(u => new University { Id = u.Id })
                                ?? [],
            };

        public User ToUser(UserDto userDto)
            => new()
            {
                Id = userDto.Id,
                Name = userDto.Name,
            };

        public User ToUser(RegisterRequest register)
            => new()
            {
                Name = register.Name,
                Email = register.Email,
                Password = register.Password,
                Address = new()
                {
                    City = register.Address.City,
                    CountryA3 = register.Address.CountryA3
                },
                Courses = register.Courses
                                .ConvertAll(x => new Course { Name = x }),
                Universities = register.Universities
                    .ConvertAll(u => new University { Id = u.Id }),
            };

        public User ToUser(LoginRequest login)
            => new()
            {
                Email = login.Email,
                Password = login.Password
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

