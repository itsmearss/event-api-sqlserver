using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Services;

namespace TestProjectAnnur.Controllers
{
    [ApiController]
    [Route("api/registerEvent")]
    [Authorize]
    public class RegisterEventController : ControllerBase
    {
        private readonly IRegisterEventService _reService;

        public RegisterEventController(IRegisterEventService reService)
        {
            _reService = reService;
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAllRegisteredEvents()
        {
            try
            {
                var registerEvents = await _reService.GetAllRegisterEventsAsync();
                return Ok(registerEvents);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRegisterEventById(int id)
        {
            var registerEvent = await _reService.GetRegisterEventById(id);

            if (registerEvent == null)
                return NotFound($"Register Event dengan ID: {id} tidak ditemukan");

            return Ok(registerEvent);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRegisterEvent(int id, [FromForm] RegisterEventDTO registerEventDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedRegisterEvent = await _reService.UpdateRegisterEventAsync(id, registerEventDTO);

            if(updatedRegisterEvent == null)
            {
                return NotFound($"RegisterEvent dengan ID: {id} tidak ditemukan");
            }

            return Ok(updatedRegisterEvent);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var deleteRe = await _reService.DeleteRegisterEventAsync(id);

            if(!deleteRe)
                return NotFound($"RegisterEvent dengan ID: {id} tidak ditemukan");

            return Ok(deleteRe);
        }

        [HttpGet("getById")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetEventsByUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var registerEvent = await _reService.GetRegisterEventsByUserIdAsync(Int32.Parse(id));

            if (registerEvent == null)
                return NotFound($"Register Event dengan ID: {id} tidak ditemukan");

            return Ok(registerEvent);

        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RegisterEvent([FromBody] RegisterEventDTO registerEventDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            registerEventDTO.UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var createdRegisterEvent = await _reService.CreateRegisterEventAsync(registerEventDTO);
            return CreatedAtAction(nameof(GetRegisterEventById), new { id = createdRegisterEvent.Id }, createdRegisterEvent);
        }

        [HttpGet("getByEventId/{eventId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetEventByUserIdAndEventId(int eventId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var registerEvent = await _reService.GetRegisterEventAsync(userId, eventId);

            if (registerEvent == null)
                return NotFound($"Register Event tidak ditemukan");

            return Ok(registerEvent);
        }
    }
}
