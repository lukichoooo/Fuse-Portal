using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
public class UniversityController(
        IUniversityService service,
        IOptions<ControllerSettings> options
        ) : ControllerBase
{
    private readonly IUniversityService _service = service;
    private readonly ControllerSettings _settings = options.Value;

    [HttpGet("{id}")]
    public async Task<ActionResult<UniDto>> GetByIdAsync([FromRoute] int id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpGet("search/{name}")]
    public async Task<ActionResult<List<UniDto>>> GetPageByNameLikeAsync(
            [FromRoute] string name,
            [FromQuery] int? lastId,
            [FromQuery] int? pageSize
            )
        => Ok(await _service.GetPageByNameLikeAsync(
                    name,
                    lastId,
                    pageSize ?? _settings.DefaultPageSize));

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
