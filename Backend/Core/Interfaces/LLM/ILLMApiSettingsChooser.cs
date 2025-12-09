using Core.Dtos.Settings;

namespace Core.Interfaces.LLM
{
    public interface ILLMApiSettingsChooser
    {
        public LLMApiSettings GetSettings(string settingsKey);
    }
}
