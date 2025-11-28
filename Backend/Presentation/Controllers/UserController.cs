using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
public class UserController(IUserService service) : ControllerBase
{
    private readonly IUserService _service = service;

    [HttpGet("all")]
    public async Task<IActionResult> GetAllAsync()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
        => Ok(await _service.GetAsync(id));

    [HttpGet("search/{name}")]
    public async Task<IActionResult> SearchByNameAsync(string name)
        => Ok(await _service.SearchByNameAsync(name));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByIdAsync(int id)
        => Ok(await _service.DeleteByIdAsync(id));
}
