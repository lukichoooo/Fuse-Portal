
namespace Core.Dtos;

public record UserDto(
            int Id,
            string Name
        );

public record UserDetailsDto(
            int Id,
            string Name,
            List<UniDto> Universities,
            List<string> Faculties
        );

public class UserPrivateDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public AddressDto Address { get; set; }

    public UserPrivateDto() { }

    public UserPrivateDto(int id, string name, string email, string password, AddressDto address)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Address = address;
    }
}

