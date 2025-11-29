using Core.Dtos;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UniversityMapper : IUniversityMapper
{
    public UniDto ToDto(University uni)
        => new(
                Id: uni.Id,
                Name: uni.Name
                );

    public UniDtoWithUsers ToDtoWithUsers(University uni)
        => new(
                Id: uni.Id,
                Name: uni.Name,
                Users: uni.Users.ConvertAll(u => new UserDto(u.Id, u.Name))
                );

    public University ToUniversity(UniDto dto)
        => new()
        {
            Id = dto.Id,
            Name = dto.Name,
        };

    public University ToUniversity(UniDtoWithUsers dto)
        => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Users = dto.Users.ConvertAll(d => new User
            {
                Id = d.Id,
                Name = d.Name,
            })
        };

    public University ToUniversity(UniRequestDto info)
        => new()
        {
            Id = info.Id,
            Name = info.Name,
            Address = info.Address
        };
}
