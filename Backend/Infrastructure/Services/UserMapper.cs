using Core.Dtos;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UserMapper : IUserMapper
{
    public UserDto ToDto(User user)
        => new UserDto(user.Name);

    public UserDtoWithUniversity ToDtoWithUniversity(User user)
        => new UserDtoWithUniversity(
                        user.Name,
                        user.Universities.Select(uni => new UniversityDto(uni.Name)).ToList()
                    );

    public UserPrivateDto ToPrivateDto(User user)
        => new UserPrivateDto(
                    user.Name,
                    user.Email,
                    user.Password
                );

    public User ToUser(UserDto userDto)
        => new User()
        {
            Name = userDto.Name,
        };

    public User ToUser(RegisterRequest request)
        => new User()
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            Role = "User" // TODO: Default Role
        };
}
