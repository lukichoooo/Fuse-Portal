using Core.Dtos;
using Core.Interfaces;

namespace Infrastructure.Services
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
