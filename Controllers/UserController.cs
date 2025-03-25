using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Services;

namespace TestProjectAnnur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var userEntity = await _userService.GetUserByIdAsync(id);

            if (userEntity == null)
                return NotFound($"User dengan ID {id} tidak ditemukan");

            return Ok(userEntity);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var userEntity = await _userService.GetUserByUsernameAsync(username);

            if (userEntity == null)
                return NotFound($"User dengan Username {username} tidak ditemukan");

            return Ok(userEntity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdUser = await _userService.CreateUserAsync(userDTO);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUser = await _userService.UpdateUserAsync(id, userDTO);

            if (updatedUser == null)
                return NotFound($"User dengan ID {id} tidak ditemukan");

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deletedUser = await _userService.DeleteUserAsync(id);

            if (!deletedUser)
                return NotFound($"User dengan ID {id} tidak ditemukan");

            return Ok(deletedUser);
        }
    }
}
