using Core.Entities.Portal;
using Core.Exceptions;
using Core.Interfaces.Portal;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class PortalRepo(MyContext context) : IPortalRepo
    {
        private readonly MyContext _context = context;

        public async Task<Subject> AddSubjectForUserAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<Subject> RemoveSubjectById(int subjectId, int userId)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Id == subjectId && s.UserId == userId)
                ?? throw new SubjectNotFoundException(
                        $" subject Not Found With Id={subjectId} and UserId={userId}");

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<List<Subject>> GetSubjectsPageForUserAsync(
                int userId,
                int? lastSubjectId,
                int pageSize)
        {
            IQueryable<Subject> userSubjects = _context.Subjects
                .Where(s => s.UserId == userId);
            if (lastSubjectId is not null)
            {
                userSubjects = userSubjects
                    .Where(ss => ss.Id > lastSubjectId);
            }
            return await userSubjects
                .Include(s => s.Schedules)
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async ValueTask<Subject?> GetFullSubjectByIdAsync(int subjectId, int userId)
            => await _context.Subjects
                .Include(s => s.Schedules)
                .Include(s => s.Syllabuses)
                .Include(s => s.Lecturers)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == subjectId);

        public async Task<Schedule> AddScheduleForSubjectAsync(Schedule schedule, int userId)
        {
            bool subjectExists = await _context.Subjects
                .AnyAsync(s => s.Id == schedule.SubjectId
                        && s.UserId == userId);

            if (!subjectExists)
            {
                throw new SubjectNotFoundException(
                    $"Subject not found: Id={schedule.SubjectId}, UserId={userId}");
            }

            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<Schedule> RemoveScheduleByIdAsync(int scheduleId, int userId)
        {
            var schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == scheduleId
                        && s.Subject!.UserId == userId)
                ?? throw new ScheduleNotFoundException(
                        $"Schedule not found with Id={scheduleId}, UserId={userId}");

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<Lecturer> AddLecturerToSubjectAsync(Lecturer lecturer, int userId)
        {
            bool subjectExists = await _context.Subjects
                .AnyAsync(l => l.Id == lecturer.SubjectId
                        && l.UserId == userId);

            if (!subjectExists)
            {
                throw new SubjectNotFoundException(
                    $"Subject not found: Id={lecturer.SubjectId}, UserId={userId}");
            }

            await _context.Lecturers.AddAsync(lecturer);
            await _context.SaveChangesAsync();
            return lecturer;
        }

        public async Task<Lecturer> RemoveLecturerByIdAsync(int lecturerId, int userId)
        {
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == lecturerId
                        && l.Subject!.UserId == userId)
                ?? throw new LecturerNotFoundException(
                        $"Lecturer not found with Id={lecturerId}, UserId={userId}");

            _context.Lecturers.Remove(lecturer);
            await _context.SaveChangesAsync();
            return lecturer;
        }

        public async Task<Syllabus> AddSyllabusForSubjectAsync(Syllabus sylabus, int userId)
        {
            bool subjectExists = await _context.Subjects
                .AnyAsync(t => t.Id == sylabus.SubjectId
                        && t.UserId == userId);

            if (!subjectExists)
            {
                throw new SubjectNotFoundException(
                    $"Subject not found: Id={sylabus.SubjectId}, UserId={userId}");
            }

            await _context.Sylabuses.AddAsync(sylabus);
            await _context.SaveChangesAsync();
            return sylabus;
        }

        public async Task<Syllabus> RemoveSyllabusByIdAsync(int sylabusId, int userId)
        {
            var sylabus = await _context.Sylabuses
                .FirstOrDefaultAsync(t => t.Id == sylabusId
                        && t.Subject!.UserId == userId)
                ?? throw new SylabusNotFoundException(
                        $"Sylabus not found with Id={sylabusId}, UserId={userId}");

            _context.Sylabuses.Remove(sylabus);
            await _context.SaveChangesAsync();
            return sylabus;
        }

        public async Task<Syllabus> GetFullSyllabusByIdAsync(int sylabusId, int userId)
            => await _context.Sylabuses
                .FirstOrDefaultAsync(t => t.Id == sylabusId
                        && t.Subject!.UserId == userId)
                ?? throw new SylabusNotFoundException(
                        $"Sylabus not found with Id={sylabusId}, UserId={userId}");
    }
}
