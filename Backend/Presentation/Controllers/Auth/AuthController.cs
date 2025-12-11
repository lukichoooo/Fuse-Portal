namespace Presentation.Controllers;

using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest userLogin)
        => Ok(await _authService.LoginAsync(userLogin));

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest userRegister)
        => Ok(await _authService.RegisterAsync(userRegister));

}
