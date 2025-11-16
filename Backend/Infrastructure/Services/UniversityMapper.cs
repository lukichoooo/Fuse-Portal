using Core.Dtos;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UniversityMapper : IUniversityMapper
{
    public UniversityDto ToDto(University uni)
        => new UniversityDto(uni.Name);

    public UniversityDtoWithUsers ToDtoWithUsers(University uni)
        => new UniversityDtoWithUsers(
                    uni.Name,
                    uni.Users.Select(u => new UserDto(u.Name)).ToList()
                );

    public University ToUniversity(UniversityDto universityDto)
        => new University()
        {
            Name = universityDto.Name,
            Users = new List<User>()
        };
}
