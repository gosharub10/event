using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
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
    }
}
