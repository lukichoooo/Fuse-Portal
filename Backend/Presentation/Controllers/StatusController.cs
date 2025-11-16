using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
        => Ok();
}
