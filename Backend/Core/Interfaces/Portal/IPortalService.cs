using Core.Dtos;

namespace Core.Interfaces.Portal
{
    public interface IPortalService
    {
        Task<List<SubjectDto>> GetSubjectsPageForCurrentUserAsync(int? lastSubjectId, int pageSize);
        Task<SubjectFullDto> GetFullSubjectById(int subjectId);

        Task<SubjectFullDto> AddSubjectForCurrentUser(SubjectRequestDto dto);
        Task<SubjectDto> RemoveSubjectById(int subjectId);

        Task<ScheduleDto> AddScheduleForSubjectAsync(ScheduleRequestDto request);
        Task<ScheduleDto> RemoveScheduleByIdAsync(int scheduleId);

        Task<LecturerDto> AddLecturerToSubjectAsync(LecturerRequestDto request);
        Task<LecturerDto> RemoveLecturerByIdAsync(int lecturerId);

        Task<TestDto> AddTestForSubjectAsync(TestRequestDto request);
        Task<TestDto> RemoveTestByIdAsync(int testId);
    }
}
