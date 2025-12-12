
namespace Core.Dtos;


public record MockExamResponse(
        string Exam
        );

public record SyllabusDto(
        int Id,
        string Name
        );

public record LecturerDto(
        int Id,
        string Name
        );

public record SyllabusFullDto(
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
        List<ScheduleDto> Schedules,
        string? Metadata
        );

public record SubjectFullDto
{
    public required int Id { get; init; }
    public required string Name { get; init; } = null!;
    public required int? Grade { get; init; }

    public required List<ScheduleDto> Schedules { get; init; } = [];
    public required List<LecturerDto> Lecturers { get; init; } = [];
    public required List<SyllabusDto> Syllabuses { get; init; } = [];

    public string? Metadata { get; init; }
}

// Requests

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
    public string? Metadata { get; set; }
}

public record SyllabusRequestDto
{
    public required string Name { get; set; } = null!;
    public required string Content { get; set; } = null!;
    public required int SubjectId { get; set; }
    public string? Metadata { get; set; }
}

// Without Ids 


public record LecturerRequestDtoNoSubjectId
{
    public required string Name { get; set; }
}

public record ScheduleRequestDtoNoSubjectId
{
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public string? Location { get; set; }
    public string? Metadata { get; set; }
}

public record SyllabusRequestDtoNoSubjectId
{
    public required string Name { get; set; } = null!;
    public required string Content { get; set; } = null!;
    public string? Metadata { get; set; }
}

// subject

public record SubjectRequestDto
{
    public required string Name { get; init; } = null!;
    public required int? Grade { get; init; }
    public string? Metadata { get; init; }
}


public record SubjectFullRequestDto
{
    public required string Name { get; init; } = null!;
    public required int? Grade { get; init; }
    public string? Metadata { get; init; }

    public required List<ScheduleRequestDtoNoSubjectId> Schedules { get; init; } = [];
    public required List<LecturerRequestDtoNoSubjectId> Lecturers { get; init; } = [];
    public required List<SyllabusRequestDtoNoSubjectId> Syllabuses { get; init; } = [];
}

public record PortalParserResponseDto
{
    public List<SubjectFullRequestDto> Subjects { get; init; } = [];
    public string? Metadata { get; set; }
}

