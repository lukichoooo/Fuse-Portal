using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
public class UniversityControlle(IUniversityService service) : ControllerBase
{
    private readonly IUniversityService _service = service;


    [HttpGet("all")]
    public async Task<IActionResult> GetAllAsync()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
        => Ok(await _service.GetAsync(id));

    [HttpGet("search/{name}")]
    public async Task<IActionResult> SearchByNameAsync(string name)
        => Ok(await _service.SearchAsync(name));

    [HttpGet("{id}/users")]
    public async Task<IActionResult> GetUsersAsync(int id)
        => Ok(await _service.GetUsersAsync(id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByIdAsync(int id)
        => Ok(await _service.DeleteAsync(id));

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] University university)
        => Ok(await _service.UpdateAsync(university));

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] University university)
        => Ok(await _service.CreateAsync(university));
}
