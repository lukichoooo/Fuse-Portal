using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IUserMapper
    {
        UserDto ToDto(User user);
        UserPrivateDto ToPrivateDto(User user);
        UserDetailsDto ToDetailsDto(User user);
        User ToUser(UserPrivateDto dto);
        User ToUser(UserUpdateRequest request, int userId);
    }
}

