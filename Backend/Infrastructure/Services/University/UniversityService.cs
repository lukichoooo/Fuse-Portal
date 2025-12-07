using Core.Dtos;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UniversityService(IUniversityRepo repo, IUniversityMapper mapper) : IUniversityService
{
    private readonly IUniversityRepo _repo = repo;
    private readonly IUniversityMapper _mapper = mapper;

    public async Task<UniDto> GetByIdAsync(int id)
    {
        var uni = await _repo.GetByIdAsync(id)
            ?? throw new UniversityNotFoundException($"University Not Found with Id={id}");
        return _mapper.ToDto(uni);
    }


    public async Task<List<UniDto>> GetPageByNameLikeAsync(string name, int? lastId, int pageSize)
        => (await _repo.GetPageByNameAsync(name, lastId, pageSize))
        .ConvertAll(_mapper.ToDto);

    public async Task<UniDto> CreateAsync(UniRequestDto info)
    {
        const int pageSize = 64;
        var uni = _mapper.ToUniversity(info);
        List<University> dbUnis = await _repo.GetPageByNameAsync(info.Name, int.MinValue, pageSize);
        foreach (var dbUni in dbUnis)
        {
            if (dbUni.Equals(uni))
                throw new UniversityAlreadyExists($"University Name={info.Name}, Address={info.Address} already exists");
        }
        return _mapper.ToDto(await _repo.CreateAsync(uni));
    }

    public async Task<UniDto> UpdateAsync(UniRequestDto info)
        => _mapper.ToDto(await _repo.UpdateAsync(_mapper.ToUniversity(info)));

    public async Task<UniDto> DeleteByIdAsync(int id)
        => _mapper.ToDto(await _repo.DeleteByIdAsync(id));

}
