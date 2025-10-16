using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TekramApp.Attribute;
using TekramApp.Interfaces;
using TekramApp.Models;
using TekramApp.Services.Auth;

namespace TekramApp.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
#if !DEBUG
    [Authorize]
#endif
    public class RolesController : ControllerBase
    {
        private readonly IRoles _roleService;

        public RolesController(IRoles roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [UsePermissions("ManageRoles")]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        [UsePermissions("ManageRoles")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost]
        [UsePermissions("ManageRoles")]
        public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
        {
            var role = new Role { Name = dto.Name, Description = dto.Description };
            await _roleService.AddRoleAsync(role);
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        [UsePermissions("ManageRoles")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RoleUpdateDto dto)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();

            role.Name = dto.Name;
            role.Description = dto.Description;
            await _roleService.UpdateRoleAsync(role);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [UsePermissions("ManageRoles")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _roleService.DeleteRoleAsync(id);
            return NoContent();
        }
    }

    public record RoleCreateDto(string Name, string Description);
    public record RoleUpdateDto(string Name, string Description);
}
