using Core.Entities;
using Core.Interfaces.UserUniversityTable;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class UserUniversityRepo(MyContext context) : IUserUniversityRepo
    {
        private readonly MyContext _context = context;

        public async Task<List<University>> GetUnisPageForUserIdAsync(int userId, int? lastUniId, int pageSize)
        {
            IQueryable<UserUniversity> unis = _context.UserUniversities
                .Where(uu => uu.UserId == userId);

            if (lastUniId is not null)
                unis = unis.Where(uu => uu.UniversityId > lastUniId);

            return await unis
                .Include(uu => uu.University)
                .OrderBy(uu => uu.UniversityId)
                .Take(pageSize)
                .Select(uu => uu.University!)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByUniIdPageAsync(int uniId, int? lastUserId, int pageSize)
        {
            IQueryable<UserUniversity> query = _context.UserUniversities
                .Where(uu => uu.UniversityId == uniId);

            if (lastUserId is not null)
                query = query.Where(uu => uu.UserId > lastUserId);

            return await query
                .Include(uu => uu.User)
                .OrderBy(uu => uu.UserId)
                .Take(pageSize)
                .Select(uu => uu.User!)
                .ToListAsync();
        }

    }
}
