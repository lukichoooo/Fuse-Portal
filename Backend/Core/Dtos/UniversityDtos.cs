namespace Core.Dtos;

public record UniversityDto(
        string Name
        );

public record UniversityDtoWithUsers(
        string Name,
        IEnumerable<UserDto> Users
    );
