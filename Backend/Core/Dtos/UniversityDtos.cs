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


public class UniRequestDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public AddressDto Address { get; set; }

    public UniRequestDto() { }

    public UniRequestDto(int id, string name, AddressDto address)
    {
        Id = id;
        Name = name;
        Address = address;
    }
}

