using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController: ControllerBase
{
    private readonly IAuthServices _authServices;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthServices authServices, ILogger<AuthController> logger)
    {
        _authServices = authServices;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDTO user)
    {
        await _authServices.Registration(user);
            
        return Ok("registration success");
    }
        
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] LoginDTO user)
    {
        var token = await _authServices.Login(user);
        Response.Cookies.Append("accessToken", token.AccessToken);
        return Ok(token);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken()
    {
        Request.Headers.TryGetValue("Authorization", out var token);
        _logger.Log(LogLevel.Information, $"Refresh token expired {token}");
        var newTokenDto = await _authServices.RefreshToken(token!);
        return Ok(newTokenDto);
    }
}