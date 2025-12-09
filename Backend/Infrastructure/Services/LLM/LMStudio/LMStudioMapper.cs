using Core.Dtos;
using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.LMStudio;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioMapper : ILMStudioMapper
    {

        private readonly LLMApiSettingKeys _keySettings;
        private readonly ILLMApiSettingsChooser _settingsChooser;
        private readonly ILLMInputGenerator _inputGenerator;

        public LMStudioMapper(
                ILLMInputGenerator inputGenerator,
                ILLMApiSettingsChooser apiSettingsChooser,
                IOptions<LLMApiSettingKeys> keyOptions
                )
        {
            _inputGenerator = inputGenerator;
            _settingsChooser = apiSettingsChooser;
            _keySettings = keyOptions.Value;
        }

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

        public LMStudioRequest ToRequest(
                MessageDto msg,
                string? previousResponseId = null,
                string? rulesPrompt = null)
            => new()
            {
                Model = _settingsChooser.GetSettings(_keySettings.Chat).Model,
                Input = _inputGenerator.GenerateInput(msg, rulesPrompt),
                PreviousResponseId = previousResponseId
            };

        public LMStudioRequest ToRequest(
                string html,
                string? rulesPrompt = null)
            => new()
            {
                Model = _settingsChooser.GetSettings(_keySettings.Parser).Model,
                Input = _inputGenerator.GenerateInput(html, rulesPrompt),
            };

        public string ToOutputText(LMStudioResponse response)
            => response.Output[0].Content[0].Text;

    }
}
