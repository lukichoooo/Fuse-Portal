using Core.Dtos;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UniversityMapper() : IUniversityMapper
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
                Users: uni.UserUniversities
                .ConvertAll(uu => new UserDto(uu.UserId, uu.User.Name))
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
            UserUniversities = dto.Users
                .ConvertAll(u => new UserUniversity
                {
                    UserId = u.Id,
                    UniversityId = dto.Id
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
