using Core.Dtos;

namespace Core.Interfaces.LLM
{
    public interface ILLMInputGenerator
    {
        string GenerateInput(MessageDto msg, string rules = "");
    }
}
