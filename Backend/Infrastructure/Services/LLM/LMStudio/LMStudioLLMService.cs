using Core.Dtos;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.LMStudio;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioLLMService(
            ILMStudioApi api,
            ILMStudioMapper mapper
            ) : ILLMService
    {
        private readonly ILMStudioApi _api = api;
        private readonly ILMStudioMapper _mapper = mapper;

        public async Task<MessageDto> SendMessageAsync(MessageDto msg)
        {
            var chatId = msg.ChatId;
            var response = await _api.SendMessageAsync(_mapper.ToRequest(msg));
            return _mapper.ToMessageDto(response, chatId);
        }
    }
}
