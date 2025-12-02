using Core.Dtos;

namespace Core.Interfaces.LLM
{
    public interface ILLMService
    {
        Task<MessageDto> SendMessageAsync(MessageDto msg);
    }
}
