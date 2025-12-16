namespace Core.Interfaces.Convo
{
    public interface IMessageStreamer
    {
        Task StreamAsync(string chatId, string chunk);
    }
}
