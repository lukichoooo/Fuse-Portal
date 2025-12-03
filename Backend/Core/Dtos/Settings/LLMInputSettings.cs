namespace Core.Dtos.Settings
{
    public class LLMInputSettings
    {
        public string RulesDelimiter { get; set; } = null!;
        public string UserInputDelimiter { get; set; } = null!;
        public string FileNameDelimiter { get; set; } = null!;
        public string FileContentDelimiter { get; set; } = null!;
    }
}
