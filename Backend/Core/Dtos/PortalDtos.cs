
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
        DateTime Date,
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


public record LecturerRequestDto
{
    public required string Name { get; set; }
    public required int SubjectId { get; set; }
}

public record ScheduleRequestDto
{
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public required int SubjectId { get; set; }
    public string? Location { get; set; }
    public string? MetaData { get; set; }
}

public record TestRequestDto
{
    public required string Name { get; set; } = null!;
    public required string Content { get; set; } = null!;
    public required int SubjectId { get; set; }
    public DateTime? Date { get; set; }
    public string? Metadata { get; set; }
}


public record SubjectRequestDto
{
    public required string Name { get; init; } = null!;
    public required int? Grade { get; init; }
    public string? Metadata { get; init; }
}

public record PortalDto
{
    public List<SubjectFullDto> Subjects { get; init; } = [];
    public string? Metadata { get; set; }
}

