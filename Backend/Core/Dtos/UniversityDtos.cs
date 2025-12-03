namespace Core.Dtos;

public record UniDto(
        int Id,
        string Name
        );

public record UniDtoWithUsers(
        int Id,
        string Name,
        List<UserDto> Users
    );


public record UniRequestDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public AddressDto Address { get; init; } = null!;
}

