namespace Core.Interfaces.Convo.FileServices
{
    public interface IFileTextParser
    {
        Task<string> ReadDocxAsync(Stream stream);
        Task<string> ReadTextAsync(Stream stream);
    }
}
