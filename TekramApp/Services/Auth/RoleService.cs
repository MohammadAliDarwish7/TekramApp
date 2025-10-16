using Microsoft.EntityFrameworkCore;
using TekramApp.Context;
using TekramApp.Interfaces;
using TekramApp.Models;

namespace TekramApp.Services.Auth
{
    public class RoleService : IRoles
    {
        private readonly TekramDbContext _context;

        public RoleService(TekramDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(Guid id)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddRoleAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoleAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId)
        {
            var exists = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (!exists)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
        {
            var rolePerm = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (rolePerm != null)
            {
                _context.RolePermissions.Remove(rolePerm);
                await _context.SaveChangesAsync();
            }
        }
    }
}