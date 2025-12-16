using Core.Dtos;

namespace Core.Interfaces.LLM
{
    public interface ILLMApiResponseStreamer
    {
        Task<LMStudioResponse?> ReadResponseAsStreamAsync(
                HttpResponseMessage responseMessage,
                Func<string, Task>? onReceived);
    }
}
