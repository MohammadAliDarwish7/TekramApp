using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TekramApp.Attribute;
using TekramApp.Interfaces;
using TekramApp.Models;

namespace TekramApp.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsers _userService;

        public UsersController(IUsers userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        [UsePermissions("ManageUsers")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        [UsePermissions("ManageUsers")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        //[UsePermissions("ManageUsers")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                RoleId = dto.RoleId
            };
            await _userService.CreateUserAsync(user, dto.Password);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        [UsePermissions("ManageUsers")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
        {
            var existing = await _userService.GetUserByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Username = dto.Username;
            existing.Email = dto.Email;
            existing.RoleId = dto.RoleId;
            existing.IsActive = dto.IsActive;

            await _userService.UpdateUserAsync(existing);
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [UsePermissions("ManageUsers")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

    }

    // DTOs for cleaner API
    public record UserCreateDto(string Username, string Email, string Password, Guid RoleId);
    public record UserUpdateDto(string Username, string Email, Guid RoleId, bool IsActive);
}