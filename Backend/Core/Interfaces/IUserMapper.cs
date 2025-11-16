using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces;

public interface IUserMapper
{
    UserDto ToDto(User user);
    User ToUser(UserDto userDto);
    UserPrivateDto ToPrivateDto(User user);
    UserDtoWithUniversity ToDtoWithUniversity(User user);
    User ToUser(RegisterRequest registerRequest);
}
