
namespace Core.Dtos;

public record UserDto(
            int Id,
            string Name
        );

public record UserDetailsDto(
            int Id,
            string Name,
            List<UniDto> Universities,
            List<SubjectDto> Subjects
        );


public record UserUpdateRequest
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required AddressDto Address { get; init; }

    public required List<SubjectRequestDto>? Subjects { get; init; }
    public required List<UniDto>? Universities { get; init; }
}

public record UserPrivateDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; set; }
    public required AddressDto Address { get; init; }


    public required List<SubjectDto>? Subjects { get; init; }
    public required List<UniDto>? Universities { get; init; }
}

