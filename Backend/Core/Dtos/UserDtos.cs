namespace Core.Dtos;

public record UserDto(
            string Name
        );

public record UserDtoWithUniversity(
            string Name,
            List<UniversityDto> Universities
        );

public record UserPrivateDto(
            string Name,
            string Email,
            string Password
        );

