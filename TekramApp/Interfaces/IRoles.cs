using TekramApp.Models;

namespace TekramApp.Interfaces
{
    public interface IRoles
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(Guid id);
        Task AddRoleAsync(Role role);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(Guid id);

        // Role-Permission management
        Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
        Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);
    }
}
