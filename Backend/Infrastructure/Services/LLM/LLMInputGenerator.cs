using System.Text;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.LLM;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM
{
    public class LLMInputGenerator(IOptions<LLMInputSettings> options) : ILLMInputGenerator
    {
        private readonly LLMInputSettings _settings = options.Value;

        public string GenerateInput(MessageDto msg, string rules = "")
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(rules))
                sb.AppendLine($"{_settings.SystemPromptDelimiter}\n{rules}");

            if (!string.IsNullOrWhiteSpace(msg.Text))
                sb.AppendLine($"{_settings.UserInputDelimiter}\n{msg.Text}");

            foreach (var (name, content) in msg.Files)
            {
                sb.AppendLine($"{_settings.FileNameDelimiter}\n{name}");
                sb.AppendLine($"{_settings.FileContentDelimiter}\n{content}");
            }

            return sb.ToString().Trim();
        }
    }
}
