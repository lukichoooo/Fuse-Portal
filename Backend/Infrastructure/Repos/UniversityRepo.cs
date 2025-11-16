using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class UniversityRepo : IUniversityRepo
{
    private readonly Context _context;
    public UniversityRepo(Context context)
    {
        _context = context;
    }

    public async Task<University> CreateAsync(University university)
    {
        await _context.Universities.AddAsync(university);
        await _context.SaveChangesAsync();
        return university;
    }

    public async Task<University> DeleteAsync(int id)
    {
        var uni = await _context.Universities.FindAsync(id) ?? throw new UniversityNotFoundException($"University Not Found With Id {id}");
        _context.Universities.Remove(uni);
        await _context.SaveChangesAsync();
        return uni;
    }

    public async Task<IEnumerable<University>> GetAllAsync() => await _context.Universities.ToListAsync();

    public async Task<University> GetAsync(int id) => await _context.Universities.FindAsync(id) ?? throw new UniversityNotFoundException($"University Not Found With Id {id}");

    public async Task<IEnumerable<User>> GetUsersAsync(int id)
        => await (from uni in _context.Universities
                  from user in uni.Users
                  where uni.Id == id
                  select user
                  ).ToListAsync();

    public async Task<IEnumerable<University>> SearchAsync(string name) => await _context.Universities.Where(u => u.Name.Contains(name)).ToListAsync();

    public async Task<University> UpdateAsync(University university)
    {
        var onDbUni = await _context.Universities.FindAsync(university.Id) ?? throw new UniversityNotFoundException($"University Not Found With Id {university.Id}");
        onDbUni.Name = university.Name;
        await _context.SaveChangesAsync();
        return onDbUni;
    }
}
