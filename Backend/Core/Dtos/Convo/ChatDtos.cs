namespace Core.Dtos;

public record ChatDto(
        int Id,
        string Name
        );

public record MessageDto(
    int Id,
    string Text,
    DateTime CreatedAt,
    int ChatId,
    Dictionary<string, string>? FileToContent = null
);


public record ChatFullDto(
        int Id,
        string Name,
        List<MessageDto> Messages
        );
