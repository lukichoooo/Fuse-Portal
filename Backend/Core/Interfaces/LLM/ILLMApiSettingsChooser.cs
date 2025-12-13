using Core.Dtos.Settings;

namespace Core.Interfaces.LLM
{
    public interface ILLMApiSettingsChooser
    {
        LLMApiSettings GetSettings(string settingsKey);
    }
}
