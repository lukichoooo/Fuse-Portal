using Core.Dtos;
using Core.Entities;
using Core.Enums;
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

        public UserRequestDto ToRequestDto(User user)
            => new(
                        id: user.Id,
                        name: user.Name,
                        email: user.Email,
                        password: user.Password,
                        address: user.Address
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
                Address = new()
                {
                    City = register.Address.City,
                    CountryA3 = register.Address.CountryA3
                },
                Faculties = register.Faculties
                                .ConvertAll(x => new Faculty { Name = x }),
                Universities = register.Universities
                    .ConvertAll(u => new University { Id = u.Id }),
            };

        public User ToUser(LoginRequest login)
            => new()
            {
                Email = login.Email,
                Password = login.Password
            };

        public User ToUser(UserRequestDto info)
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

