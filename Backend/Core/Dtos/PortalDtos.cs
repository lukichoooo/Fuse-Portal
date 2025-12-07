
namespace Core.Dtos;

public record TestDto(
        int Id,
        string Name
        );

public record LecturerDto(
        int Id,
        string Name
        );

public record TestFullDto(
        int Id,
        string Name,
        string Content,
        string? Metadata
        );

public record ScheduleDto(
        int Id,
        DateTime Start,
        DateTime End,
        int SubjectId,
        string? Metadata
        );

public record SubjectDto(
        int Id,
        string Name,
        string? Metadata
        );

public record SubjectFullDto
{
    public required int Id { get; init; }

    public required string Name { get; init; } = null!;

    public required int? Grade { get; init; }

    public required List<ScheduleDto> Schedules { get; init; } = [];
    public required List<LecturerDto> Lecturers { get; init; } = [];
    public required List<TestDto> Tests { get; init; } = [];

    public string? Metadata { get; init; }
}

public record PortalDto
{
    public List<SubjectFullDto> Subjects { get; init; } = [];
    public string? Metadata { get; set; }
}

