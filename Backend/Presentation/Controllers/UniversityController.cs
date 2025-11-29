using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
public class UniversityController(IUniversityService service) : ControllerBase
{
    private readonly IUniversityService _service = service;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync([FromRoute] int id)
        => Ok(await _service.GetAsync(id));

    [HttpGet("search/{name}")]
    public async Task<IActionResult> GetPageByNameAsync(
            [FromRoute] string name,
            [FromQuery] int lastId = int.MinValue,
            [FromQuery] int pageSize = 16
            )
        => Ok(await _service.GetPageByNameAsync(name, lastId, pageSize));

    [HttpGet("{id}/users")]
    public async Task<IActionResult> GetUsersPageAsync(
            [FromRoute] int id,
            [FromQuery] int lastId = int.MinValue,
            [FromQuery] int pageSize = 16
            )
        => Ok(await _service.GetUsersPageAsync(id, lastId, pageSize));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByIdAsync(int id)
        => Ok(await _service.DeleteByIdAsync(id));

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UniRequestDto uni)
        => Ok(await _service.UpdateAsync(uni));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] UniRequestDto uni)
        => Ok(await _service.CreateAsync(uni));
}
