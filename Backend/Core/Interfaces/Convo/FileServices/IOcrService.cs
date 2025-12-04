namespace Core.Interfaces.Convo.FileServices
{
    public interface IOcrService
    {
        Task<string> ProcessAsync(Stream fileStream);
        Task<string> FallbackOcrAsync(Stream fileStream);
    }
}
