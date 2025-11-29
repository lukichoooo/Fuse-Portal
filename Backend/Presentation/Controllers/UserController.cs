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
    public async Task<IActionResult> GetAllPageAsync(
            [FromQuery] int lastId = int.MinValue,
            [FromQuery] int pageSize = 16
            )
        => Ok(await _service.GetAllPageAsync(lastId, pageSize));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
        => Ok(await _service.GetAsync(id));

    [HttpGet("search/{name}")]
    public async Task<IActionResult> PageByNameAsync(
            string name,
            [FromQuery] int lastId = int.MinValue,
            [FromQuery] int pageSize = 16
            )
        => Ok(await _service.PageByNameAsync(name, lastId, pageSize));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByIdAsync(int id)
        => Ok(await _service.DeleteByIdAsync(id));
}
