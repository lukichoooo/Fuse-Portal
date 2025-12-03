namespace Core.Dtos.Settings
{
    public class LMStudioApiSettings
    {
        public string URL { get; set; } = null!;
        public string ChatRoute { get; set; } = null!;

        public string Model { get; set; } = null!;
        public string SystemPrompt { get; set; } = null!;

        public float Temperature { get; set; }
        public int MaxTokens { get; set; }
        public bool Stream { get; set; }
    }
}
