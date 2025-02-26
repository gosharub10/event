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
        public async Task<IActionResult> GetEventById(int id)
        {
            return Ok(await _eventServices.GetEventById(id));
        }

        [HttpGet("get/{title}")]
        public async Task<IActionResult> GetEventByTitle(string title)
        {
            return Ok(await _eventServices.GetEventByName(title));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetEventByFilter([FromQuery] QueryHelperDTO query)
        {
            var events = await _eventServices.GetAll(query);
            return Ok(events);
        }
        
        [Authorize(Policy = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddEvent([FromBody] EventNewDTO eventDto)
        {
            await _eventServices.AddEvent(eventDto);
            return Ok();
        }
        
        [Authorize(Policy = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateEvent([FromBody] EventDTO eventDto)
        {
            await _eventServices.UpdateEvent(eventDto);
            return Ok("success update");
        }
        
        [Authorize(Policy = "Admin")]
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventServices.DeleteEvent(id);
            return Ok("success delete");
        }
        
        [Authorize(Policy = "Admin")]
        [HttpPut("upload/{id:int}")]
        public async Task<IActionResult> UpdateEventImage(int id, IFormFile? imageFile)
        {
            if (imageFile == null) 
                return BadRequest();
            
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            await _eventServices.UploadImage(id, memoryStream);
            
            return Ok();
        }
    }
}
