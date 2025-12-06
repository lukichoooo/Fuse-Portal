using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Interfaces.UserUniversityTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Controllers
{
    // Manages: Subject, Course, Test
    public class PortalController(
            IUserUniversityService service,
            IOptions<ControllerSettings> options
            ) : ControllerBase
    {
        private readonly IUserUniversityService _service = service;
        private readonly ControllerSettings _settings = options.Value;

        [HttpGet("{uniId:int}/users")]
        public async Task<ActionResult<List<UserDto>>> GetUsersPageAsync(
                [FromRoute] int uniId,
                [FromQuery] int? lastId,
                [FromQuery] int? pageSize
                )
            => Ok(await _service.GetUsersByUniIdPageAsync(
                        uniId,
                        lastId,
                        pageSize ?? _settings.BigPageSize));
    }
}
