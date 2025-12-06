using Core.Exceptions;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class UserRepo(MyContext context) : IUserRepo
{
    private readonly MyContext _context = context;

    public ValueTask<User?> GetByIdAsync(int id)
        => _context.Users.FindAsync(id);

    public Task<User?> GetByEmailAsync(string email)
        => _context.Users.FirstOrDefaultAsync(x => x.Email == email);

    public Task<bool> ExistsAsync(string email) => _context.Users.AnyAsync(x => x.Email == email);

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public Task<List<User>> GetAllPageAsync(int? lastId, int pageSize = 16)
    {
        IQueryable<User> query = _context.Users;
        if (lastId is not null)
            query = query.Where(u => u.Id > lastId);

        return query
            .OrderBy(u => u.Id)
            .Take(pageSize)
            .ToListAsync();
    }


    public Task<List<User>> PageByNameAsync(string name, int? lastId, int pageSize)
    {
        IQueryable<User> query = _context.Users;
        if (lastId is not null)
            query = query.Where(u => u.Id > lastId);

        return query
            .Where(u => EF.Functions.Like(u.Name, $"{name}%"))
            .OrderBy(u => u.Id)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<User> UpdateUserCredentialsAsync(User user)
    {
        var onDbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id)
            ?? throw new UserNotFoundException($"User Not Found With Id={user.Id}");
        onDbUser.Name = user.Name;
        onDbUser.Email = user.Email;
        onDbUser.Password = user.Password;
        await _context.SaveChangesAsync();
        return onDbUser;
    }


    public async Task<User> DeleteByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id)
            ?? throw new UserNotFoundException($"User Not Found With Id={id}");
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserDetailsByIdAsync(int id)
        => await _context.Users
            .Include(u => u.Courses)
            .Include(u => u.UserUniversities)
                .ThenInclude(uu => uu.University)
            .Include(u => u.SubjectEnrollments)
            .Include(u => u.TeachingSubjects)
            .FirstOrDefaultAsync(u => u.Id == id);

}
