using Core.Dtos;

namespace Core.Interfaces.Portal
{
    public interface IMockExamService
    {
        Task<string> GenerateExamQuestionsAsync(string syllabus);
        Task<ExamDto> GetExamResultsAsync(ExamDto examDto);
    }
}
