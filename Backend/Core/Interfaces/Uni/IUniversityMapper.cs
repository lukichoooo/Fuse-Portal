using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IUniversityMapper
    {
        UniDto ToDto(University uni);
        UniDtoWithUsers ToDtoWithUsers(University uni);
        University ToUniversity(UniDto dto);
        University ToUniversity(UniDtoWithUsers dto);
        University ToUniversity(UniRequestDto info);
    }
}
