using Core.Dtos;
using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.Cache;
using Core.Interfaces.LLM.LMStudio;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioMessageService(
            ILMStudioApi api,
            ILMStudioMapper mapper,
            IChatMetadataService metadataService,
            IOptions<LLMApiSettingKeys> apiKeyOptions
            ) : ILLMMessageService
    {
        private readonly ILMStudioApi _api = api;
        private readonly ILMStudioMapper _mapper = mapper;
        private readonly IChatMetadataService _metadataService = metadataService;
        private readonly LLMApiSettingKeys _apiKeys = apiKeyOptions.Value;


        public async Task<MessageDto> SendMessageAsync(MessageDto msg, Action<string>? onStreamReceived)
        {
            var chatId = msg.ChatId;
            var lastId = await _metadataService.GetLastResponseIdAsync(msg.ChatId);
            var request = _mapper.ToRequest(msg, lastId, RulesPrompt);
            LMStudioResponse response;

            if (onStreamReceived == null)
            {
                request.Stream = false;
                response = await _api.SendMessageAsync(
                        request,
                        _apiKeys.Chat);
            }
            else
            {
                request.Stream = true;
                response = await _api.SendMessageWithStreamingAsync(
                        request,
                        _apiKeys.Chat,
                        onStreamReceived);
            }

            await _metadataService.SetLastResponseIdAsync(chatId, response.Id);
            return _mapper.ToMessageDto(response, chatId);
        }

        private const string RulesPrompt = @"
You are Ruby, a fox with glasses and an AI mentor for students around the world.
Be direct, concise, and technical. Give clear answers without fluff.
When uncertain, ask for clarification.";


    }
}
