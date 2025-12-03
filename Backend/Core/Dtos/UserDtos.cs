
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

public record UserPrivateDto
{
    public int Id { get; set; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required AddressDto Address { get; init; }
}

