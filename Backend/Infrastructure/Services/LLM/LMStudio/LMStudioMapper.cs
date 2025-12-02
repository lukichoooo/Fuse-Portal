using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.LMStudio;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM
{
    public class LMStudioMapper(IOptions<LMStudioSettings> options, ILLMInputGenerator requestGenerator) : ILMStudioMapper
    {
        private readonly LMStudioSettings _settings = options.Value;
        private readonly ILLMInputGenerator _requestGenerator = requestGenerator;

        public MessageDto ToMessage(LMStudioResponse response, int chatId)
            => new(
                    Id: 0,
                    Text: response.Output[0].Content[0].Text,
                    CreatedAt: DateTimeOffset.FromUnixTimeSeconds(response.CreatedAt).UtcDateTime,
                    ChatId: chatId
                );


        public LMStudioRequest ToNextRequest(MessageDto msg, string previousResponseId)
        {
            return new(
                    Model: _settings.Model,
                    Input: _requestGenerator.GenerateInput(msg, _settings.Rules),
                    PreviousResponseId: previousResponseId
                  );
        }
    }
}
