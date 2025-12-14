namespace Core.Dtos;

public record Content(
    string Type,
    string Text
);

public record Output(
    string Id,
    string Type,
    string Role,
    string Status,
    List<Content> Content
);

public record OutputTokensDetails(
    int ReasoningTokens
);

public record Usage(
    int InputTokens,
    int OutputTokens,
    int TotalTokens,
    OutputTokensDetails OutputTokensDetails
);

public record LMStudioResponse(
    string Id,
    string Object,
    int CreatedAt,
    string Status,
    string Model,
    List<Output> Output,
    Usage Usage,
    string PreviousResponseId
);

public record LMStudioRequest
{
    public string Model { get; set; } = null!;
    public string Input { get; init; } = null!;
    public bool Stream { get; set; } = false;
    public string? PreviousResponseId { get; set; }
}

public record LMStudioStreamEvent(
     string Type,
     string? ItemId = null,
     int? OutputIndex = null,
     int? ContentIndex = null,
     string? Delta = null,
     int? SequenceNumber = null,
     LMStudioResponse? Response = null
 );


