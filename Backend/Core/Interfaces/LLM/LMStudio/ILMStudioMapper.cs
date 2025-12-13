using Core.Dtos;

namespace Core.Interfaces.LLM.LMStudio
{
    public interface ILMStudioMapper
    {
        MessageDto ToMessageDto(LMStudioResponse response, int chatId);
        LMStudioRequest ToRequest(MessageDto msg, string? previousResponseId = null, string? rulesPrompt = null);

        LMStudioRequest ToRequest(string text, string? rulesPrompt = null);
        string ToOutputText(LMStudioResponse response);

        LMStudioRequest ToRequest(ExamDto examDto, string? rulesPrompt = null);
    }
}
