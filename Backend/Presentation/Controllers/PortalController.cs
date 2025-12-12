using System.Text;
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

    [HttpPost("upload-html")]
    public async Task<ActionResult> AddLecturerAsync()
    {
        var rawHtmlPage = await Request.GetRawBodyAsync();
        await _service.ParseAndSavePortalAsync(rawHtmlPage);
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

    [HttpPost("sylabus")]
    public async Task<ActionResult<SyllabusDto>> AddSylabusAsync(
            [FromBody] SyllabusRequestDto request)
        => Ok(await _service.AddSylabusForSubjectAsync(request));

    [HttpDelete("sylabus/{sylabusId}")]
    public async Task<ActionResult<SyllabusDto>> RemoveSylabusAsync(
            [FromRoute] int sylabusId)
        => Ok(await _service.RemoveSyllabusByIdAsync(sylabusId));

    [HttpGet("sylabus/{sylabusId}")]
    public async Task<ActionResult<SyllabusFullDto>> GetSylabusByIdAsync(
            [FromRoute] int sylabusId)
        => Ok(await _service.GetFullSyllabusByIdAsync(sylabusId));

}

public static class PortalControllerExtensions
{
    // Helper
    public static async Task<string> GetRawBodyAsync(
        this HttpRequest request,
        Encoding? encoding = null)
    {
        if (!request.Body.CanSeek)
        {
            // We only do this if the stream isn't *already* seekable,
            // as EnableBuffering will create a new stream instance
            // each time it's called
            request.EnableBuffering();
        }

        request.Body.Position = 0;

        var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

        var body = await reader.ReadToEndAsync().ConfigureAwait(false);

        request.Body.Position = 0;

        return body;
    }
}
