namespace Presentation.Controllers;

using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest userLogin)
        => Ok(await _authService.LoginAsync(userLogin));

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest userRegister)
        => Ok(await _authService.RegisterAsync(userRegister));

}
