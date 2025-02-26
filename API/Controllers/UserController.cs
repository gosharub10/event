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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetUserById(int id)
        {
            return Ok(await _userServices.GetByIdAsync(id));
        }

        [HttpGet("get/{title}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetUserByEventTitle(string title)
        {
            return Ok(await _userServices.GetByEventName(title));
        }

        [HttpPost("registrationParticipants/{eventId:int}/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RegistrationParticipant(int eventId, int userId)
        {
            await _userServices.AddUserToEvent(eventId, userId);
            return Ok("registration participants success");
        }

        [HttpDelete("removeParticipant/{eventId:int}/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RemoveParticipant(int eventId, int userId)
        {
            await _userServices.RemoveUserFromEvent(eventId, userId);
            return Ok("removal participants success");
        }
    }
}
