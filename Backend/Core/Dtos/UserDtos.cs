
namespace Core.Dtos;

public record UserDto(
            int Id,
            string Name
        );

public record UserDetailsDto(
            int Id,
            string Name,
            List<UniDto> Universities,
            List<CourseDto> Courses,
            List<SubjectDto> SubjectEnrollments,
            List<SubjectDto> TeachingSubjects
        );


public record UserUpdateRequest
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required AddressDto Address { get; init; }

    public required List<SubjectDto>? SubjectEnrollments { get; init; }
    public required List<SubjectDto>? TeachingSubjects { get; init; }
    public required List<CourseDto>? Courses { get; init; }
    public required List<UniDto>? Universities { get; init; }
}

public record UserPrivateDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; set; }
    public required AddressDto Address { get; init; }


    public required List<SubjectDto>? SubjectEnrollments { get; init; }
    public required List<SubjectDto>? TeachingSubjects { get; init; }
    public required List<CourseDto>? Courses { get; init; }
    public required List<UniDto>? Universities { get; init; }
}

