using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Interfaces.UserUniversityTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserUniversityController(
        IUserUniversityService service,
        IOptions<ControllerSettings> options
        ) : ControllerBase
{
    private readonly IUserUniversityService _service = service;
    private readonly ControllerSettings _settings = options.Value;

    [HttpGet("uni/{uniId:int}/users")]
    public async Task<ActionResult<List<UserDto>>> GetUsersByUniIdPageAsync(
            [FromRoute] int uniId,
            [FromQuery] int? lastId,
            [FromQuery] int? pageSize
            )
        => Ok(await _service.GetUsersByUniIdPageAsync(
                    uniId,
                    lastId,
                    pageSize ?? _settings.BigPageSize));


    [HttpGet("user/{userId:int}/unis")]
    public async Task<ActionResult<List<UniDto>>> GetUnisPageForUserIdAsync(
            [FromRoute] int userId,
            [FromQuery] int? lastId,
            [FromQuery] int? pageSize
            )
        => Ok(await _service.GetUnisPageForUserIdAsync(
                    userId,
                    lastId,
                    pageSize ?? _settings.BigPageSize));
}
