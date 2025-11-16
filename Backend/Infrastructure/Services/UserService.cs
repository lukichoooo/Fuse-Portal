using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepo _userRepo;

    public UserService(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<User> CreateAsync(User user)
        => await _userRepo.CreateAsync(user);

    public async Task<User> DeleteAsync(int id)
        => await _userRepo.DeleteAsync(id);

    public async Task<IEnumerable<User>> GetAllAsync()
        => await _userRepo.GetAllAsync();

    public async Task<User> GetAsync(int id)
        => await _userRepo.GetAsync(id);

    public async Task<University> GetUniversityAsync(int id)
        => await _userRepo.GetUniversityAsync(id);

    public async Task<IEnumerable<User>> SearchAsync(string name)
        => await _userRepo.SearchAsync(name);

    public async Task<User> UpdateAsync(User user)
        => await _userRepo.UpdateAsync(user);
}
