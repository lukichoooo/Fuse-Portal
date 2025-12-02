using Core.Dtos;

namespace Core.Interfaces
{
    public interface ILLMService
    {
        Task<MessageDto> SendMessageAsync(MessageDto msg);
    }
}
