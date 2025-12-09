using Core.Dtos;
using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.Cache;
using Core.Interfaces.LLM.LMStudio;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioChatService(
            ILMStudioApi api,
            ILMStudioMapper mapper,
            IChatMetadataService metadataService,
            IOptions<LLMApiSettingKeys> apiKeyOptions
            ) : ILLMChatService
    {
        private readonly ILMStudioApi _api = api;
        private readonly ILMStudioMapper _mapper = mapper;
        private readonly IChatMetadataService _metadataService = metadataService;
        private readonly LLMApiSettingKeys _apiKeys = apiKeyOptions.Value;

        // TODO: write in another file
        const string rulesPrompt = @"
            you are an AI Mentor for Students all over the World, 
            Be direct, concise, and technical. Give clear answers without fluff. 
            When uncertain, ask for clarification.";

        public async Task<MessageDto> SendMessageAsync(MessageDto msg)
        {
            var chatId = msg.ChatId;

            var request = _mapper.ToRequest(
                    msg,
                    await _metadataService.GetLastResponseIdAsync(chatId),
                    rulesPrompt);

            var response = await _api.SendMessageAsync(
                    request,
                    _apiKeys.Chat);

            await _metadataService.SetLastResponseIdAsync(chatId, response.Id);

            return _mapper.ToMessageDto(response, chatId);
        }
    }
}
