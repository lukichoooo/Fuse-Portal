using Core.Dtos;

namespace Core.Interfaces.LLM.LMStudio
{
    public interface ILMStudioMapper
    {
        MessageDto ToMessage(LMStudioResponse response, int chatId);
        LMStudioRequest ToNextRequest(MessageDto msg, string previousResponseId);
    }
}
