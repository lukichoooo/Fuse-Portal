using Core.Dtos;

namespace Core.Interfaces.Portal
{
    public interface IPortalService
    {
        Task<PortalParserResponseDto> ParseAndSavePortalAsync(PortalParserRequestDto request);

        Task<List<SubjectDto>> GetSubjectsPageForCurrentUserAsync(int? lastSubjectId, int pageSize);
        Task<SubjectFullDto> GetFullSubjectByIdAsync(int subjectId);

        Task<SubjectFullDto> AddSubjectForCurrentUserAsync(SubjectRequestDto dto);
        Task<SubjectDto> RemoveSubjectByIdAsync(int subjectId);

        Task<ScheduleDto> AddScheduleForSubjectAsync(ScheduleRequestDto request);
        Task<ScheduleDto> RemoveScheduleByIdAsync(int scheduleId);

        Task<LecturerDto> AddLecturerToSubjectAsync(LecturerRequestDto request);
        Task<LecturerDto> RemoveLecturerByIdAsync(int lecturerId);

        Task<TestDto> AddTestForSubjectAsync(TestRequestDto request);
        Task<TestDto> RemoveTestByIdAsync(int testId);
        Task<TestFullDto> GetFullTestByIdAsync(int testId);
    }
}
