using Core.Dtos;

namespace Core.Interfaces.Portal
{
    public interface IPortalService
    {
        Task<PortalParserResponseDto> ParseAndSavePortalAsync(string HtmlPage);

        Task<List<SubjectDto>> GetSubjectsPageForCurrentUserAsync(
                int? lastSubjectId, int pageSize);
        Task<SubjectFullDto> GetFullSubjectByIdAsync(int subjectId);

        Task<SubjectFullDto> AddSubjectForCurrentUserAsync(SubjectRequestDto dto);
        Task<SubjectDto> RemoveSubjectByIdAsync(int subjectId);

        Task<ScheduleDto> AddScheduleForSubjectAsync(ScheduleRequestDto request);
        Task<ScheduleDto> RemoveScheduleByIdAsync(int scheduleId);

        Task<LecturerDto> AddLecturerToSubjectAsync(LecturerRequestDto request);
        Task<LecturerDto> RemoveLecturerByIdAsync(int lecturerId);

        Task<SyllabusDto> AddSyllabusForSubjectAsync(SyllabusRequestDto request);
        Task<SyllabusDto> RemoveSyllabusByIdAsync(int syllabusId);
        Task<SyllabusFullDto> GetFullSyllabusByIdAsync(int syllabusId);

        Task<ExamDto> GenerateMockExamForSyllabusAsync(int syllabusId);
        Task<ExamDto> CheckExamAnswersAsync(ExamDto examDto);
    }
}
