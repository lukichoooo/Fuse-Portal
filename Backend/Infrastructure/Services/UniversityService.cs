using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UniversityService : IUniversityService
{
    private readonly IUniversityRepo _uniRepo;

    public UniversityService(IUniversityRepo uniRepo)
    {
        _uniRepo = uniRepo;
    }

    public async Task<University> CreateAsync(University uni)
        => await _uniRepo.CreateAsync(uni);

    public async Task<University> DeleteAsync(int id)
        => await _uniRepo.DeleteAsync(id);

    public async Task<IEnumerable<University>> GetAllAsync()
        => await _uniRepo.GetAllAsync();

    public async Task<University> GetAsync(int id)
        => await _uniRepo.GetAsync(id);

    public async Task<IEnumerable<User>> GetUsersAsync(int id)
        => await _uniRepo.GetUsersAsync(id);

    public async Task<IEnumerable<University>> SearchAsync(string name)
        => await _uniRepo.SearchAsync(name);

    public async Task<University> UpdateAsync(University uni)
        => await _uniRepo.UpdateAsync(uni);
}
