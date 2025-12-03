namespace Core.Dtos;

public record ChatDto(
        int Id,
        string Name
        );


public record FileDto(
        string Name,
        string Text
        );

public record MessageDto
{
    public int Id { get; init; }
    public required string Text { get; init; }
    public DateTime CreatedAt { get; init; }
    public int ChatId { get; init; }
    public List<FileDto> Files { get; init; } = [];
}

public record ClientMessage
{
    public int Id { get; init; }
    public required string Text { get; init; }
    public DateTime CreatedAt { get; init; }
    public int ChatId { get; init; }
}


public record ChatFullDto(
        int Id,
        string Name,
        List<MessageDto> Messages
        );
