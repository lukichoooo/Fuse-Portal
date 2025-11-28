using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces;

public interface IUniversityMapper
{
    UniDto ToDto(University university);
    UniDtoWIthUsers ToDtoWithUsers(University university);
    University ToUniversity(UniDto dto);
    University ToUniversity(UniDtoWIthUsers dto);
}
