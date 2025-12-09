using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class UniversityRepo(MyContext context) : IUniversityRepo
{
    private readonly MyContext _context = context;

    public async Task<University> CreateAsync(University university)
    {
        await _context.Universities.AddAsync(university);
        await _context.SaveChangesAsync();
        return university;
    }

    public async Task<University> DeleteByIdAsync(int id)
    {
        var uni = await _context.Universities
            .Include(uni => uni.UserUniversities)
            .FirstOrDefaultAsync(uni => uni.Id == id)
            ?? throw new UniversityNotFoundException($"University Not Found With Id {id}");
        _context.UserUniversities.RemoveRange(uni.UserUniversities);
        _context.Universities.Remove(uni);
        await _context.SaveChangesAsync();
        return uni;
    }

    public async Task<List<University>> GetPageAsync(int? lastId, int pageSize)
    {
        IQueryable<University> query = _context.Universities;
        if (lastId is not null)
            query = query.Where(u => u.Id > lastId);

        return await query
            .OrderBy(uni => uni.Id)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<University?> GetByIdAsync(int id)
        => await _context.Universities.FindAsync(id);

    public async Task<List<University>> GetPageByNameAsync(string name, int? lastId, int pageSize)
    {
        IQueryable<University> query = _context.Universities;
        if (lastId is not null)
            query = query.Where(uni => uni.Id > lastId);

        return await query
            .Where(uni => EF.Functions.Like(uni.Name, $"{name}%"))
            .OrderBy(uni => uni.Id)
            .Take(pageSize)
            .ToListAsync();
    }


    public async Task<University> UpdateAsync(University university)
    {
        var onDbUni = await _context.Universities.FindAsync(university.Id)
            ?? throw new UniversityNotFoundException($"University Not Found With Id {university.Id}");
        onDbUni.Name = university.Name;
        await _context.SaveChangesAsync();
        return onDbUni;
    }


    public Task<University?> GetByNameAsync(string name)
        => _context.Universities
            .FirstOrDefaultAsync(uni => uni.Name == name);
}
