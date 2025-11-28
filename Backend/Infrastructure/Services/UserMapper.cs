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
                        Faculties: user.Faculties.ConvertAll(f => f.Name)
                    );

        public UserPrivateInfo ToPrivateInfo(User user)
            => new(
                        id: user.Id,
                        name: user.Name,
                        email: user.Email,
                        password: user.Password
                    );

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
                Faculties = register.Faculties
                                .ConvertAll(x => new Faculty { Name = x })
            };

        public User ToUser(LoginRequest login)
            => new()
            {
                Email = login.Email,
                Password = login.Password
            };

        public User ToUser(UserPrivateInfo info)
            => new()
            {
                Id = info.Id,
                Name = info.Name,
                Email = info.Email,
                Password = info.Password
            };
    }
}

