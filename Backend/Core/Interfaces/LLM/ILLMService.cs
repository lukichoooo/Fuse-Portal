using Core.Dtos;

namespace Core.Interfaces.LLM
{
    public interface ILLMMessageService
    {
        Task<MessageDto> SendMessageAsync(MessageDto msg);
    }
}
