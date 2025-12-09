using Core.Entities;
using Core.Entities.JoinTables;
using Core.Exceptions;
using Core.Interfaces.UserUniversityTable;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class UserUniversityRepo(MyContext context) : IUserUniversityRepo
    {
        private readonly MyContext _context = context;

        public async Task<UserUniversity> AddUserToUniversityAsync(UserUniversity uu)
        {
            await _context.UserUniversities.AddAsync(uu);
            await _context.SaveChangesAsync();
            return uu;
        }

        public async Task<UserUniversity> RemoveUserFromUniversityAsync(int userId, int uniId)
        {
            var uu = await _context.UserUniversities
                    .FirstOrDefaultAsync(uu => uu.UniversityId == uniId && uu.UserId == userId)
                ?? throw new UserUniversityNotFoundException(
                        $"UserUniversity Not Found UserId={userId}, UniId={uniId}");
            _context.Remove(uu);
            await _context.SaveChangesAsync();
            return uu;
        }

        public async Task<List<University>> GetUnisPageForUserIdAsync(int userId, int? lastUniId, int pageSize)
        {
            IQueryable<UserUniversity> userUnis = _context.UserUniversities
                .Where(uu => uu.UserId == userId);

            if (lastUniId is not null)
                userUnis = userUnis.Where(uu => uu.UniversityId > lastUniId);

            return await userUnis
                .Include(uu => uu.University)
                .OrderBy(uu => uu.UniversityId)
                .Take(pageSize)
                .Select(uu => uu.University!)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByUniIdPageAsync(int uniId, int? lastUserId, int pageSize)
        {
            IQueryable<UserUniversity> userUnis = _context.UserUniversities
                .Where(uu => uu.UniversityId == uniId);

            if (lastUserId is not null)
                userUnis = userUnis.Where(uu => uu.UserId > lastUserId);

            return await userUnis
                .Include(uu => uu.User)
                .OrderBy(uu => uu.UserId)
                .Take(pageSize)
                .Select(uu => uu.User!)
                .ToListAsync();
        }
    }
}
