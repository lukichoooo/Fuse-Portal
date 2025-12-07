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

        public async Task<Subject> AddSubjectForUser(Subject subject)
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
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async ValueTask<Subject?> GetFullSubjectById(int subjectId, int userId)
            => await _context.Subjects
                .Include(s => s.Schedules)
                .Include(s => s.Tests)
                .Include(s => s.Lecturers)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == subjectId);
    }
}
