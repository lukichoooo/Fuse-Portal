using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces;

public interface IUserMapper
{
    UserDto ToDto(User user);
    UserRequestDto ToRequestDto(User user);
    UserDetailsDto ToDetailsDto(User user);
    User ToUser(UserDto dto);
    User ToUser(UserRequestDto dto);
    User ToUser(RegisterRequest register);
    User ToUser(LoginRequest login);
}
