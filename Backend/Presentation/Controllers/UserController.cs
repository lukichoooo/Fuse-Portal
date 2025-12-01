using Core.Dtos;
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
    public async Task<ActionResult<UserDto>> GetAllPageAsync(
            [FromQuery] int lastId = int.MinValue,
            [FromQuery] int pageSize = 16
            )
        => Ok(await _service.GetAllPageAsync(lastId, pageSize));

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(int id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpGet("search/{name}")]
    public async Task<ActionResult<UserDto>> GetPageByNameAsync(
            string name,
            [FromQuery] int lastId = int.MinValue,
            [FromQuery] int pageSize = 16
            )
        => Ok(await _service.GetPageByNameAsync(name, lastId, pageSize));

    [HttpGet("me")]
    public async Task<ActionResult<UserPrivateDto>> GetCurrentUserPrivateDto()
    {
        int id = int.Parse(HttpContext.User.FindFirst("id")!.Value);
        return Ok(await _service.GetPrivateDtoById(id));
    }

    [HttpDelete("me")]
    public async Task<ActionResult<UserPrivateDto>> DeleteCurrentUser()
    {
        int id = int.Parse(HttpContext.User.FindFirst("id")!.Value);
        return Ok(await _service.DeleteByIdAsync(id));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<UserDto>> DeleteByIdAsync(int id)
        => Ok(await _service.DeleteByIdAsync(id));
}
