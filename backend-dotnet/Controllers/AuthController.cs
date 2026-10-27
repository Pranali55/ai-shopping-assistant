using Microsoft.AspNetCore.Mvc;
using ShoppingAssistant.DTOs;
using ShoppingAssistant.Services;

namespace ShoppingAssistant.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _auth.RegisterAsync(dto);
        return result == null ? BadRequest("Email already exists.") : Ok(result);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var result = _auth.Login(dto);
        return result == null ? Unauthorized("Invalid credentials.") : Ok(result);
    }
}
