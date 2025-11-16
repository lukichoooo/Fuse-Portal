using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> Get()
        => Ok(await _userService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
        => Ok(await _userService.GetAsync(id));

    [HttpGet("search/{name}")]
    public async Task<IActionResult> SearchAsync(string name)
        => Ok(await _userService.SearchAsync(name));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
        => Ok(await _userService.DeleteAsync(id));
}
