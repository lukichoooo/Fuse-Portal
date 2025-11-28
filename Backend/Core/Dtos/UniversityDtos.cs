namespace Core.Dtos;

public record UniDto(
        int Id,
        string Name
        );

public record UniDtoWIthUsers(
        int Id,
        string Name,
        List<UserDto> Users
    );
