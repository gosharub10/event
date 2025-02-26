using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOrUser")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IAuthServices _authServices;

        public UserController(IUserServices userServices, IAuthServices authServices)
        {
            _userServices = userServices;
            _authServices = authServices;
        }

        [HttpGet("get/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            return Ok(await _userServices.GetByIdAsync(id));
        }

        [HttpGet("get/{title}")]
        public async Task<IActionResult> GetUserByEventTitle(string title)
        {
            return Ok(await _userServices.GetByEventName(title));
        }

        [HttpPost("registrationParticipants/{eventId:int}/{userId:int}")]
        public async Task<IActionResult> RegistrationParticipant(int eventId, int userId)
        {
            await _userServices.AddUserToEvent(eventId, userId);
            return Ok("registration participants success");
        }

        [HttpDelete("removeParticipant/{eventId:int}/{userId:int}")]
        public async Task<IActionResult> RemoveParticipant(int eventId, int userId)
        {
            await _userServices.RemoveUserFromEvent(eventId, userId);
            return Ok("removal participants success");
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO user)
        {
            await _authServices.Registration(user);
            
            return Ok("registration success");
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Register([FromBody] LoginDTO user)
        {
            var token = await _authServices.Login(user);
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(TokenDTO tokenDto)
        {
            var newTokenDto = await _authServices.RefreshToken(tokenDto);
            return Ok(newTokenDto);
        }
    }
}
