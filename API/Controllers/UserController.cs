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

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("get/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
        {
            var user = await _userServices.GetByIdAsync(id, cancellationToken);

            return Ok(user);
        }

        [HttpGet("get/{title}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetUserByEventTitle(string title, CancellationToken cancellationToken)
        {
            var users = await _userServices.GetByEventName(title, cancellationToken);
            return Ok(users);
        }

        [HttpPost("registrationParticipants/{eventId:int}/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RegistrationParticipant(int eventId, int userId, CancellationToken cancellationToken)
        {
            await _userServices.AddUserToEvent(eventId, userId, cancellationToken);
            return Ok("Registration of participants succeeded.");
        }

        [HttpDelete("removeParticipant/{eventId:int}/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RemoveParticipant(int eventId, int userId, CancellationToken cancellationToken)
        {
            await _userServices.RemoveUserFromEvent(eventId, userId, cancellationToken);
            return Ok("Removal of participants succeeded.");
        }
    }
}