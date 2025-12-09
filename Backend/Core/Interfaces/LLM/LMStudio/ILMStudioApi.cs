using Core.Dtos;

namespace Core.Interfaces.LLM.LMStudio
{
    public interface ILMStudioApi
    {
        Task<LMStudioResponse> SendMessageAsync(LMStudioRequest request, string settingsKey);
    }
}
