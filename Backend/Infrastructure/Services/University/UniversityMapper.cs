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
                    .ConvertAll(uu => new UserDto(uu.UserId, uu.User!.Name))
                );


    public University ToUniversity(UniRequestDto info)
        => new()
        {
            Id = info.Id,
            Name = info.Name,
            Address = info.Address
        };
}
