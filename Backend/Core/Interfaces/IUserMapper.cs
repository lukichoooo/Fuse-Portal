using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces;

public interface IUserMapper
{
    UserDto ToDto(User user);
    UserPrivateInfo ToPrivateInfo(User user);
    UserDetailsDto ToDetailsDto(User user);
    User ToUser(UserDto dto);
    User ToUser(UserPrivateInfo dto);
    User ToUser(RegisterRequest register);
    User ToUser(LoginRequest login);
}
