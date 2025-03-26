using Microsoft.AspNetCore.Mvc;
using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Services;


namespace TestProjectAnnur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventEntity = await _eventService.GetEventByIdAsync(id);

            if (eventEntity == null)
                return NotFound($"Event dengan ID {id} tidak ditemukan");

            return Ok(eventEntity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] EventDTO eventDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdEvent = await _eventService.CreateEventAsync(eventDTO);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromForm] EventDTO eventDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedEvent = await _eventService.UpdateEventAsync(id, eventDTO);

            if (updatedEvent == null)
                return NotFound($"Event dengan ID {id} tidak ditemukan");

            return Ok(updatedEvent);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var deleteCategory = await _eventService.DeleteEventAsync(id);

            if (!deleteCategory)
                return NotFound($"Event dengan ID {id} tidak ditemukan");

            return Ok(deleteCategory);
        }
    }
}
