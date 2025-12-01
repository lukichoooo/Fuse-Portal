namespace Core.Dtos;

public record ChatDto(
        int Id,
        string Name
        );

public record MessageDto(
        int Id,
        string Text,
        DateTime CreatedAt,
        int ChatId
        );

public record MessageWithFileDto(
        int Id,
        string Text,
        DateTime CreatedAt,
        object File, // TODO give type
        int ChatId
        );

public record ChatFullDto(
        int Id,
        string Name,
        List<MessageDto> Messages
        );
