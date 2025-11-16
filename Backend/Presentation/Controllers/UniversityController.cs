using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
public class UniversityController : ControllerBase
{
    private readonly IUniversityService _universityService;

    public UniversityController(IUniversityService universityService)
    {
        _universityService = universityService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> Get()
        => Ok(await _universityService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
        => Ok(await _universityService.GetAsync(id));

    [HttpGet("search/{name}")]
    public async Task<IActionResult> SearchAsync(string name)
        => Ok(await _universityService.SearchAsync(name));

    [HttpGet("{id}/users")]
    public async Task<IActionResult> GetUsersAsync(int id)
        => Ok(await _universityService.GetUsersAsync(id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
        => Ok(await _universityService.DeleteAsync(id));

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] University university)
        => Ok(await _universityService.UpdateAsync(university));

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] University university)
        => Ok(await _universityService.CreateAsync(university));
}
