using Core.Dtos;

namespace Core.Interfaces.LLM
{
    public interface ILLMChatService
    {
        Task<MessageDto> SendMessageAsync(MessageDto msg);
    }
}
