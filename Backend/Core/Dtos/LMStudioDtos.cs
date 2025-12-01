namespace Core.Dtos;

public record Msg(
        string Role,
        string Content
        );

public record Choices(
        int Index,
        string? Logprobs,
        string FinishReason,
        Msg Message
        );

public record Usage(
        int PromptTokens,
        int CompletionTokens,
        int TotalTokens
        );

public record Stats(
        float TokensPerSecond,
        float TimeToFirstToken,
        float GenerationTime,
        string StopReason
        );

public record ModelInfo(
        string Arch,
        string Quant,
        string Format,
        int ContextLength
        );

public record Runtime(
        string Name,
        string Version,
        List<string> SupportedFormats
        );

public record LMStudioRequest(
        string Model,
        List<Msg> Messages,
        float Temperature,
        int MaxTokens,
        bool Stream
        );

public record LMStudioResponse(
        int Id,
        string Object,
        int Created,
        string Model,
        List<Choices> Choices,
        Usage Usage,
        Stats Stats,
        ModelInfo ModelInfo,
        Runtime Runtime
        );
