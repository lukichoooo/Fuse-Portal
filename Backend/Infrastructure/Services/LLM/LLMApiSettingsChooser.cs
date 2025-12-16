using Core.Dtos.Settings;
using Core.Dtos.Settings.Infrastructure;
using Core.Exceptions;
using Core.Interfaces.LLM;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM
{
    public class LLMApiSettingsChooser : ILLMApiSettingsChooser
    {
        private readonly LLMApiSettingKeys _apiKeys;
        private readonly LLMApiSettings _chatSettings;
        private readonly LLMApiSettings _parserSettings;
        private readonly LLMApiSettings _examSettings;

        public LLMApiSettingsChooser(
            IOptions<LLMApiSettingKeys> keyOptions,
            IOptionsMonitor<LLMApiSettings> apiOptionsMonitor
                )
        {
            _apiKeys = keyOptions.Value;
            _chatSettings = apiOptionsMonitor.Get(_apiKeys.Chat);
            _parserSettings = apiOptionsMonitor.Get(_apiKeys.Parser);
            _examSettings = apiOptionsMonitor.Get(_apiKeys.Exam);
        }

        public LLMApiSettings GetSettings(string settingsKey)
            => settingsKey switch
            {
                var key when key == _apiKeys.Chat => _chatSettings,
                var key when key == _apiKeys.Parser => _parserSettings,
                var key when key == _apiKeys.Exam => _examSettings,
                _ => throw new LMStudioApiException($"Invalid settings key: {settingsKey}")
            };

    }
}
