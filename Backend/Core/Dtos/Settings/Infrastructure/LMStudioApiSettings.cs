namespace Core.Dtos.Settings
{
    public record LLMApiSettings
    {
        public required string URL { get; init; }
        public required string ChatRoute { get; init; }

        public required int TimeoutMins { get; init; }
        public required string Model { get; init; }

        public float Temperature { get; init; }
        public int MaxTokens { get; init; }
        public int ContextWindow { get; init; }
        public bool Stream { get; init; }

        public string? ResponseFormatType { get; init; }
    }
}
