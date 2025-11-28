
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

public class UserPrivateInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public UserPrivateInfo() { }

    public UserPrivateInfo(int id, string name, string email, string password)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
    }
}

