using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController(
        IUserService service,
        IOptions<ControllerSettings> options
        ) : ControllerBase
{
    private readonly IUserService _service = service;
    private readonly ControllerSettings _settings = options.Value;

    [HttpGet("all")]
    public async Task<ActionResult<List<UserDto>>> GetAllPageAsync(
            [FromQuery] int? lastId,
            [FromQuery] int? pageSize
            )
        => Ok(await _service.GetAllPageAsync(
                    lastId,
                    pageSize ?? _settings.DefaultPageSize));

    [HttpGet("search/{name}")]
    public async Task<ActionResult<List<UserDto>>> GetPageByNameAsync(
            string name,
            [FromQuery] int? lastId,
            [FromQuery] int? pageSize
            )
        => Ok(await _service.GetPageByNameAsync(
                    name,
                    lastId,
                    pageSize ?? _settings.DefaultPageSize));

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(int id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpPut("me")]
    public async Task<ActionResult<UserPrivateDto>> UpdateCurrentUserCredentialsAsync(
            [FromBody] UserUpdateRequest request
            )
        => Ok(await _service.UpdateCurrentUserCredentialsAsync(request));

    [HttpGet("me")]
    public async Task<ActionResult<UserPrivateDto>> GetCurrentUserPrivateDto()
        => Ok(await _service.GetCurrentUserPrivateDtoAsync());

    [HttpDelete("me")]
    public async Task<ActionResult<UserDetailsDto>> DeleteCurrentUser()
        => Ok(await _service.DeleteCurrentUserAsync());

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<UserDetailsDto>> DeleteByIdAsync(int id)
        => Ok(await _service.DeleteByIdAsync(id));


}
