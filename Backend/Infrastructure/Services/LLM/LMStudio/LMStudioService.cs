using Core.Dtos;
using Core.Interfaces;
using Core.Interfaces.LLM;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioService(LMStudioApi api) : ILLMService
    {
        private readonly LMStudioApi _api = api;

        public Task<MessageDto> SendMessageAsync(MessageDto msg)
        {
            throw new NotImplementedException();
        }
    }
}
