using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Interfaces.Portal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Controllers;

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

    [HttpPost("parse")]
    public async Task<ActionResult> AddLecturerAsync(
            [FromBody] ParsePortalRequest request)
    {
        await _service.ParseAndSavePortalAsync(request);
        return Ok();
    }

    [HttpGet("subjects")]
    public async Task<ActionResult<List<SubjectDto>>> GetSubjectsPageAsync(
            [FromQuery] int? lastSubjectId,
            [FromQuery] int? pageSize)
        => Ok(await _service.GetSubjectsPageForCurrentUserAsync(
                    lastSubjectId,
                    pageSize ?? _settings.DefaultPageSize));

    [HttpGet("subject/{subjectId}")]
    public async Task<ActionResult<SubjectFullDto>> GetSubjectByIdAsync(
            [FromRoute] int subjectId)
        => Ok(await _service.GetFullSubjectByIdAsync(subjectId));

    [HttpPost("subject")]
    public async Task<ActionResult<SubjectFullDto>> AddSubjectAsync(
            [FromBody] SubjectRequestDto subject)
        => Ok(await _service.AddSubjectForCurrentUserAsync(subject));

    [HttpDelete("subject/{subjectId}")]
    public async Task<ActionResult<SubjectDto>> RemoveSubjectByIdAsync(
            [FromRoute] int subjectId)
        => Ok(await _service.RemoveSubjectByIdAsync(subjectId));

    [HttpPost("schedule")]
    public async Task<ActionResult<ScheduleDto>> AddScheduleAsync(
            [FromBody] ScheduleRequestDto request)
        => Ok(await _service.AddScheduleForSubjectAsync(request));

    [HttpDelete("schedule/{scheduleId}")]
    public async Task<ActionResult<ScheduleDto>> RemoveScheduleAsync(
            [FromRoute] int scheduleId)
        => Ok(await _service.RemoveScheduleByIdAsync(scheduleId));

    [HttpPost("lecturer")]
    public async Task<ActionResult<LecturerDto>> AddLecturerAsync(
            [FromBody] LecturerRequestDto request)
        => Ok(await _service.AddLecturerToSubjectAsync(request));

    [HttpDelete("lecturer/{lecturerId}")]
    public async Task<ActionResult<LecturerDto>> RemoveLecturerAsync(
            [FromRoute] int lecturerId)
        => Ok(await _service.RemoveLecturerByIdAsync(lecturerId));

    [HttpPost("test")]
    public async Task<ActionResult<TestDto>> AddTestAsync(
            [FromBody] TestRequestDto request)
        => Ok(await _service.AddTestForSubjectAsync(request));

    [HttpDelete("test/{testId}")]
    public async Task<ActionResult<TestDto>> RemoveTestAsync(
            [FromRoute] int testId)
        => Ok(await _service.RemoveTestByIdAsync(testId));

    [HttpGet("test/{testId}")]
    public async Task<ActionResult<TestFullDto>> GetTestByIdAsync(
            [FromRoute] int testId)
        => Ok(await _service.GetFullTestByIdAsync(testId));
}
