using Core.Dtos;

namespace Core.Interfaces.LLM.LMStudio
{
    public interface ILMStudioMapper
    {
        MessageDto ToMessageDto(LMStudioResponse response, int chatId);
        LMStudioRequest ToRequest(MessageDto msg, string? previousResponseId = null);
    }
}
