using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO user, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return BadRequest("Registration was cancelled.");
            }
            
            await _authServices.Registration(user, cancellationToken);
            return Created();
        }
        
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Login([FromBody] LoginDTO user, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return BadRequest("Login was cancelled.");
            }

            var token = await _authServices.Login(user, cancellationToken);
            Response.Cookies.Append("accessToken", token.AccessToken);
            return Ok(token);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return BadRequest("Token refresh was cancelled.");
            }

            Request.Headers.TryGetValue("Authorization", out var token);
            
            var newTokenDto = await _authServices.RefreshToken(token!, cancellationToken);
            return Ok(newTokenDto);
        }
    }
}