using Core.Dtos;

namespace Core.Interfaces.LLM
{
    public interface ILLMInputGenerator
    {
        string GenerateInput(MessageDto msg, string? rules);
        string GenerateInput(string text, string? rules);
        string GenerateInput(ExamDto examDto, string? rules);
    }
}
