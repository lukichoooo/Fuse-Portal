using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Interfaces.Portal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Controllers;

// Manages: Subject, Test
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PortalController(
        IPortalService service,
        IOptions<ControllerSettings> options
        ) : ControllerBase
{
    private readonly IPortalService _service = service;
    private readonly ControllerSettings _settings = options.Value;

    [HttpGet("subjects")]
    public async Task<ActionResult<List<SubjectDto>>> GetSubjectsPageAsync(
            [FromQuery] int? lastSubjectId,
            [FromQuery] int? pageSize
            )
        => Ok(await _service.GetSubjectsPageForCurrentUserAsync(
                    lastSubjectId,
                    pageSize ?? _settings.DefaultPageSize));

    [HttpGet("subject/{subjectId}")]
    public async Task<ActionResult<List<SubjectFullDto>>> GetSubjectById(
            [FromRoute] int subjectId
            )
        => Ok(await _service.GetFullSubjectById(subjectId));


    [HttpPost]
    public async Task<ActionResult<List<SubjectFullDto>>> AddSubject(
            [FromBody] SubjectRequestDto subject
            )
        => Ok(await _service.AddSubjectForCurrentUser(subject));

    [HttpDelete("subject/{subjectId}")]
    public async Task<ActionResult<List<SubjectDto>>> DeleteSubjectById(
            [FromRoute] int subjectId
            )
        => Ok(await _service.RemoveSubjectById(subjectId));

}
