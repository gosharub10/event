using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOrUser")]
    public class EventController : ControllerBase
    {
        private readonly IEventServices _eventServices;

        public EventController(IEventServices eventServices)
        {
            _eventServices = eventServices;
        }

        [HttpGet("get/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetEventById(int id, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventServices.GetEventById(id, cancellationToken);
            return Ok(eventEntity);
        }

        [HttpGet("get/{title}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetEventByTitle(string title, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventServices.GetEventByName(title, cancellationToken);
            return Ok(eventEntity);
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetEventByFilter([FromQuery] QueryHelperDTO query, CancellationToken cancellationToken)
        {
            var events = await _eventServices.GetAll(query, cancellationToken);
            return Ok(events);
        }
        
        [Authorize(Policy = "Admin")]
        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> AddEvent([FromBody] EventNewDTO eventDto, CancellationToken cancellationToken)
        {
            await _eventServices.AddEvent(eventDto, cancellationToken);
            return Created();
        }
        
        [Authorize(Policy = "Admin")]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateEvent([FromBody] EventDTO eventDto, CancellationToken cancellationToken)
        {
            await _eventServices.UpdateEvent(eventDto, cancellationToken);
            return Ok("Success update");
        }
        
        [Authorize(Policy = "Admin")]
        [HttpDelete("delete/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteEvent(int id, CancellationToken cancellationToken)
        {
            await _eventServices.DeleteEvent(id, cancellationToken);
            return Ok("Success delete");
        }
        
        [Authorize(Policy = "Admin")]
        [HttpPut("upload/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateEventImage(int id, IFormFile? imageFile, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream, cancellationToken);
            await _eventServices.UploadImage(id, memoryStream, cancellationToken);
            
            return Ok("Image uploaded successfully.");
        }
    }
}