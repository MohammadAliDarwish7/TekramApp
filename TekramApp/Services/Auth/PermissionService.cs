using Microsoft.EntityFrameworkCore;
using TekramApp.Context;
using TekramApp.Interfaces;
using TekramApp.Models;

namespace TekramApp.Services.Auth
{
    public class PermissionService : IPermissionService
    {
        private readonly TekramDbContext _context;

        public PermissionService(TekramDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task<Permission> GetPermissionByIdAsync(Guid id)
        {
            return await _context.Permissions.FindAsync(id);
        }

        public async Task AddPermissionAsync(Permission permission)
        {
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePermissionAsync(Permission permission)
        {
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePermissionAsync(Guid id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(Guid roleId)
        {
            return await _context.RolePermissions
                                 .Where(rp => rp.RoleId == roleId)
                                 .Include(rp => rp.Permission)
                                 .Select(rp => rp.Permission)
                                 .ToListAsync();
        }
    }
}
