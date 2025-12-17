
namespace Core.Dtos;


public record ExamDto
{
    public int Id { get; init; }
    public string Questions { get; init; } = null!;
    public int? ScoreFrom100 { get; init; }
    public string Answers { get; init; } = "";
    public string? Results { get; set; }
    public int? Grade { get; set; }
    public int SubjectId { get; init; }
    public string? SubjectName { get; init; }
}

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
    public int? Grade { get; init; }
    public string? Metadata { get; init; }

    public List<ScheduleRequestDtoNoSubjectId> Schedules { get; init; } = [];
    public List<LecturerRequestDtoNoSubjectId> Lecturers { get; init; } = [];
    public List<SyllabusRequestDtoNoSubjectId> Syllabuses { get; init; } = [];
}

public record PortalParserResponseDto
{
    public List<SubjectFullRequestDto> Subjects { get; init; } = [];
    public string? Metadata { get; set; }
}

