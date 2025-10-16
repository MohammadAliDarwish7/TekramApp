using TekramApp.Models;

namespace TekramApp.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task<Permission> GetPermissionByIdAsync(Guid id);
        Task AddPermissionAsync(Permission permission);
        Task UpdatePermissionAsync(Permission permission);
        Task DeletePermissionAsync(Guid id);

        // Get all permissions assigned to a role
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(Guid roleId);
    }
}
