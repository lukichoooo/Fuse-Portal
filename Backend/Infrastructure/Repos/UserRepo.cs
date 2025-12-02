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

    public Task<List<User>> GetAllPageAsync(int lastId = -1, int pageSize = 16)
        => _context.Users
            .OrderBy(u => u.Id)
            .Where(u => u.Id > lastId)
            .Take(pageSize)
            .ToListAsync();

    public async Task<List<University>> GetUnisForUserAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Universities)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new UserNotFoundException($"User Not Found With Id={userId}");
        return user.Universities.ToList();
    }

    public Task<List<User>> PageByNameAsync(string name, int lastId = -1, int pageSize = 16)
        => _context.Users
        .OrderBy(u => u.Id)
        .Where(u =>
                u.Id > lastId &&
               EF.Functions.Like(u.Name, $"%{name}%"))
        .Take(pageSize)
        .ToListAsync();

    public async Task<User> UpdateUserCredentialsAsync(User user)
    {
        var onDbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id)
            ?? throw new UserNotFoundException($"User Not Found With Id={user.Id}");
        onDbUser.Name = user.Name;
        onDbUser.Email = user.Email;
        onDbUser.Password = user.Password;
        onDbUser.Universities = user.Universities;
        onDbUser.Faculties = user.Faculties;
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

    public async Task<User?> GetUserDetailsAsync(int id)
        => await _context.Users
            .Include(u => u.Faculties)
            .Include(u => u.Universities)
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new UserNotFoundException($"User Not Found With Id={id}");

}
