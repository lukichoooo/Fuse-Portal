namespace Core.Dtos.Settings
{
    public class FileProcessingSettings
    {
        public Dictionary<string, string> Handlers { get; set; } = [];
        public int MaxFileSizeBytes { get; set; }
    }
}
