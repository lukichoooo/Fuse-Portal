using Core.Dtos;

namespace Core.Interfaces.Portal
{
    public interface IPortalService
    {
        Task<List<SubjectDto>> GetSubjectsPageForCurrentUserAsync(int? lastSubjectId, int pageSize);
        Task<SubjectFullDto> GetFullSubjectById(int subjectId);

        Task<SubjectDto> AddSubjectForCurrentUser(SubjectDto dto);
        Task<SubjectDto> RemoveSubjectById(int subjectId);
    }
}
