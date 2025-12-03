namespace Core.Interfaces.LLM.Cache
{
    public interface IChatMetadataCache
    {
        Task<string?> GetValueAsync(int key);
        Task SetValueAsync(int key, string value);
    }
}
