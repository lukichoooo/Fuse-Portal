using Core.Exceptions;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class UserRepo : IUserRepo
{
    private readonly Context _context;

    public UserRepo(Context context)
    {
        _context = context;
    }

    public async Task<User> GetAsync(int id)
        => await _context.Users.FindAsync(id) ?? throw new UserNotFoundException($"User Not Found With Id {id}");

    public async Task<User> GetAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(x => x.Email == email) ?? throw new UserNotFoundException($"User Not Found With Email {email}");

    public async Task<bool> ExistsAsync(string email) => await _context.Users.AnyAsync(x => x.Email == email);

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }


    public async Task<User> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id) ?? throw new UserNotFoundException($"User Not Found With Id {id}");
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();


    public async Task<University> GetUniversityAsync(int id)
        => await _context.Universities.FindAsync(id) ?? throw new UniversityNotFoundException($"University Not Found With Id {id}");

    public async Task<IEnumerable<User>> SearchAsync(string name)
        => await _context.Users.Where(u => u.Name.Contains(name)).ToListAsync();

    public async Task<User> UpdateAsync(User user)
    {
        var onDbUser = await _context.Users.FindAsync(user.Email) ?? throw new UserNotFoundException($"User Not Found With Email {user.Email}");
        onDbUser.Name = user.Name;
        onDbUser.Email = user.Email;
        onDbUser.Password = user.Password;
        await _context.SaveChangesAsync();
        return onDbUser;
    }
}
