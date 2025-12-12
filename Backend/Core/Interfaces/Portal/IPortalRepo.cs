using Core.Entities.Portal;

namespace Core.Interfaces.Portal
{
    public interface IPortalRepo
    {
        Task<List<Subject>> GetSubjectsPageForUserAsync(int userId, int? lastId, int pageSize);
        ValueTask<Subject?> GetFullSubjectByIdAsync(int subjectId, int userId);

        Task<Subject> AddSubjectForUserAsync(Subject subject);
        Task<Subject> RemoveSubjectById(int subjectId, int userId);

        Task<Schedule> AddScheduleForSubjectAsync(Schedule schedule, int userId);
        Task<Schedule> RemoveScheduleByIdAsync(int scheduleId, int userId);

        Task<Lecturer> AddLecturerToSubjectAsync(Lecturer lecturer, int userId);
        Task<Lecturer> RemoveLecturerByIdAsync(int lecturerId, int userId);

        Task<Syllabus> AddSyllabusForSubjectAsync(Syllabus syllabi, int userId);
        Task<Syllabus> RemoveSyllabusByIdAsync(int syllabiId, int userId);
        Task<Syllabus> GetFullSyllabusByIdAsync(int syllabiId, int userId);
    }
}
