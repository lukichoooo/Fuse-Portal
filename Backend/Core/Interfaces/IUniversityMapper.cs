using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces;

public interface IUniversityMapper
{
    UniversityDto ToDto(University university);
    University ToUniversity(UniversityDto universityDto);
    UniversityDtoWithUsers ToDtoWithUsers(University university);
}
