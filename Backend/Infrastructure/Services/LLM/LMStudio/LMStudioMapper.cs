using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.LMStudio;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioMapper(IOptions<LMStudioApiSettings> options, ILLMInputGenerator requestGenerator) : ILMStudioMapper
    {
        private readonly LMStudioApiSettings _settings = options.Value;
        private readonly ILLMInputGenerator _requestGenerator = requestGenerator;

        public MessageDto ToMessageDto(LMStudioResponse response, int chatId)
            => new()
            {
                Id = 0,
                Text = response.Output[0].Content[0].Text,
                CreatedAt = DateTimeOffset
                        .FromUnixTimeSeconds(response.CreatedAt)
                        .UtcDateTime,
                ChatId = chatId,
                Files = []
            };

        public LMStudioRequest ToRequest(MessageDto msg, string? previousResponseId = null)
        {
            return new()
            {
                Model = _settings.Model,
                Input = _requestGenerator.GenerateInput(msg, _settings.SystemPrompt),
                PreviousResponseId = previousResponseId
            };
        }
    }
}
