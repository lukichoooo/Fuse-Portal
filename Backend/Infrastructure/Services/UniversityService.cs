using Core.Dtos;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UniversityService(IUniversityRepo repo) : IUniversityService
{
    private readonly IUniversityRepo _repo = repo;

    public Task<UniDto> CreateAsync(University uni)
    {
        throw new NotImplementedException();
    }

    public Task<UniDto> DeleteByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<University>> GetAllPageAsync(int lastId, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<UniDto> GetAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserDto>> GetUsersAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<UniDto>> PageByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<UniDto> UpdateAsync(University uni)
    {
        throw new NotImplementedException();
    }
}
