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
    public async Task<ActionResult<UniDto>> GetByIdAsync([FromRoute] int id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpGet("search/{name}")]
    public async Task<ActionResult<List<UniDto>>> GetPageByNameLikeAsync(
            [FromRoute] string name,
            [FromQuery] int lastId = int.MinValue,
            [FromQuery] int pageSize = 16
            )
        => Ok(await _service.GetPageByNameLikeAsync(name, lastId, pageSize));

    [HttpGet("{uniId:int}/users")]
    public async Task<ActionResult<List<UserDto>>> GetUsersPageAsync(
            [FromRoute] int uniId,
            [FromQuery] int lastId = int.MinValue,
            [FromQuery] int pageSize = 16
            )
        => Ok(await _service.GetUsersPageAsync(uniId, lastId, pageSize));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<UniDto>> DeleteByIdAsync(int id)
        => Ok(await _service.DeleteByIdAsync(id));

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<ActionResult<UniDto>> UpdateAsync([FromBody] UniRequestDto uni)
        => Ok(await _service.UpdateAsync(uni));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<UniDto>> CreateAsync([FromBody] UniRequestDto uni)
        => Ok(await _service.CreateAsync(uni));
}
