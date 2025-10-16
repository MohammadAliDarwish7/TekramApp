using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TekramApp.Attribute;
using TekramApp.Interfaces;
using TekramApp.Models;

namespace TekramApp.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        [UsePermissions("ManagePermissions")]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        [HttpGet("{id}")]
        [UsePermissions("ManagePermissions")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var perm = await _permissionService.GetPermissionByIdAsync(id);
            if (perm == null) return NotFound();
            return Ok(perm);
        }

        [HttpPost]
        [UsePermissions("ManagePermissions")]
        public async Task<IActionResult> Create([FromBody] PermissionCreateDto dto)
        {
            var perm = new Permission { Name = dto.Name, Description = dto.Description };
            await _permissionService.AddPermissionAsync(perm);
            return CreatedAtAction(nameof(GetById), new { id = perm.Id }, perm);
        }

        [HttpPut("{id}")]
        [UsePermissions("ManagePermissions")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PermissionUpdateDto dto)
        {
            var perm = await _permissionService.GetPermissionByIdAsync(id);
            if (perm == null) return NotFound();

            perm.Name = dto.Name;
            perm.Description = dto.Description;
            await _permissionService.UpdatePermissionAsync(perm);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [UsePermissions("ManagePermissions")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _permissionService.DeletePermissionAsync(id);
            return NoContent();
        }
    }

    public record PermissionCreateDto(string Name, string Description);
    public record PermissionUpdateDto(string Name, string Description);
}