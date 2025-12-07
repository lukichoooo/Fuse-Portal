using Core.Entities.Portal;

namespace Core.Interfaces.Portal
{
    public interface IPortalRepo
    {
        Task<List<Subject>> GetSubjectsPageForUserAsync(int userId, int? lastId, int pageSize);
        ValueTask<Subject?> GetFullSubjectById(int subjectId, int userId);

        Task<Subject> AddSubjectForUser(Subject subject);
        Task<Subject> RemoveSubjectById(int subjectId, int userId);
    }
}
