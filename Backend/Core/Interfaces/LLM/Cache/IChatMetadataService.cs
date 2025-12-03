namespace Core.Interfaces.LLM.Cache
{
    public interface IChatMetadataService
    {
        Task<string?> GetLastResponseIdAsync(int chatId);
        Task SetLastResponseIdAsync(int chatId, string responseId);
    }
}
