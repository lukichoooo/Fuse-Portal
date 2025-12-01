namespace Core.Interfaces
{
    public interface ILLMService<T, R>
    {
        Task<R> SendMessageAsync(T msg);
    }
}
