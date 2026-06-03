using Microsoft.AspNetCore.Mvc;
using TaskManagerApp.API.DTOs;
using TaskManagerApp.API.Services;

namespace TaskManagerApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = await _authService.RegisterAsync(dto);
        if (user is null)
            return Conflict(new { message = "Email already in use." });

        return Ok(new { user.Id, user.Email, user.Name });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto.Email, dto.Password);
        if (token is null)
            return Unauthorized(new { message = "Invalid credentials." });

        return Ok(new { token });
    }
}
