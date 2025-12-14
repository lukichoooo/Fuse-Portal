using Core.Dtos;

namespace Core.Interfaces.LLM
{
    public interface ILLMMessageService
    {
        Task<MessageDto> SendMessageAsync(MessageDto msg);

        Task<MessageDto> SendMessageAsyncStream(
                MessageDto msg,
                Action<string>? onReceived = null);
    }
}
