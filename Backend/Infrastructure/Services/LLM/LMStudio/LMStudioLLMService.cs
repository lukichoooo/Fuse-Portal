using Core.Dtos;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.Cache;
using Core.Interfaces.LLM.LMStudio;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioLLMService(
            ILMStudioApi api,
            ILMStudioMapper mapper,
            IChatMetadataService metadataService
            ) : ILLMService
    {
        private readonly ILMStudioApi _api = api;
        private readonly ILMStudioMapper _mapper = mapper;
        private readonly IChatMetadataService _metadataService = metadataService;

        public async Task<MessageDto> SendMessageAsync(MessageDto msg)
        {
            var chatId = msg.ChatId;
            var request = _mapper.ToRequest(msg);
            request.PreviousResponseId = await _metadataService.GetLastResponseIdAsync(chatId);
            var response = await _api.SendMessageAsync(request);
            await _metadataService.SetLastResponseIdAsync(chatId, response.Id);

            return _mapper.ToMessageDto(response, chatId);
        }
    }
}
