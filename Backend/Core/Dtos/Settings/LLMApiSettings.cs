namespace Core.Dtos.Settings
{
    public class LMStudioSettings
    {
        public string URL { get; set; }
        public string ChatRoute { get; set; }

        public string Model { get; set; }
        public string Rules { get; set; }

        public int Temperature { get; set; }
        public int MaxTokens { get; set; }
        public bool Stream { get; set; }
    }
}
