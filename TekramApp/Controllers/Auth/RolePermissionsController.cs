using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TekramApp.Attribute;
using TekramApp.Interfaces;
using TekramApp.Services.Auth;

namespace TekramApp.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionsController : ControllerBase
    {
        private readonly IRoles _roleService;
        private readonly IPermissionService _permissionService;

        public RolePermissionsController(IRoles roleService, IPermissionService permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        // GET: api/rolepermissions/{roleId}
        [HttpGet("{roleId}")]
        [UsePermissions("ManageRoles")]
        public async Task<IActionResult> GetRolePermissions(Guid roleId)
        {
            var role = await _roleService.GetRoleByIdAsync(roleId);
            if (role == null) return NotFound("Role not found");

            var permissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            return Ok(permissions);
        }

        // POST: api/rolepermissions
        [HttpPost]
        [UsePermissions("ManageRoles")]
        public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionDto dto)
        {
            var role = await _roleService.GetRoleByIdAsync(dto.RoleId);
            var permission = await _permissionService.GetPermissionByIdAsync(dto.PermissionId);

            if (role == null) return NotFound("Role not found");
            if (permission == null) return NotFound("Permission not found");

            await _roleService.AssignPermissionToRoleAsync(dto.RoleId, dto.PermissionId);
            return Ok(new { Message = "Permission assigned successfully" });
        }

        // DELETE: api/rolepermissions
        [HttpDelete]
        [UsePermissions("ManageRoles")]
        public async Task<IActionResult> RemovePermission([FromBody] AssignPermissionDto dto)
        {
            var role = await _roleService.GetRoleByIdAsync(dto.RoleId);
            var permission = await _permissionService.GetPermissionByIdAsync(dto.PermissionId);

            if (role == null) return NotFound("Role not found");
            if (permission == null) return NotFound("Permission not found");

            await _roleService.RemovePermissionFromRoleAsync(dto.RoleId, dto.PermissionId);
            return Ok(new { Message = "Permission removed successfully" });
        }
    }

    // DTO
    public record AssignPermissionDto(Guid RoleId, Guid PermissionId);
}
