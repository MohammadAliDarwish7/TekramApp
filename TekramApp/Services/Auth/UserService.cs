using Microsoft.EntityFrameworkCore;
using TekramApp.Context;
using TekramApp.Interfaces;
using TekramApp.Models;
using static TekramApp.Helpers.JWTHelper;
using static TekramApp.Helpers.PasswordHelper;

namespace TekramApp.Services.Auth
{
    public class UserService : IUsers
    {
        private readonly TekramDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public UserService(TekramDbContext context, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<User> GetUserByIdAsync(Guid id) =>
            await _context.Users.Include(u => u.Role)
                                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User> GetUserByUsernameAsync(string username) =>
            await _context.Users.Include(u => u.Role)
                                .FirstOrDefaultAsync(u => u.Username == username);

        public async Task<IEnumerable<User>> GetAllUsersAsync() =>
            await _context.Users.Include(u => u.Role).ToListAsync();

        public async Task CreateUserAsync(User user, string password)
        {
            user.PasswordHash = _passwordHasher.HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null) throw new KeyNotFoundException("User not found");

            existing.Username = user.Username;
            existing.Email = user.Email;
            existing.RoleId = user.RoleId;
            existing.IsActive = user.IsActive;

            _context.Users.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public Task<bool> VerifyPasswordAsync(User user, string password)
        {
            if (user == null) return Task.FromResult(false);
            bool result = _passwordHasher.VerifyPassword(user.PasswordHash, password);
            return Task.FromResult(result);
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _context.Users
       .Include(u => u.Role) // load the Role
           .ThenInclude(r => r.RolePermissions) // load RolePermissions
               .ThenInclude(rp => rp.Permission) // load Permission
       .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, password))
                throw new UnauthorizedAccessException("Invalid username or password");

            return _jwtTokenService.GenerateToken(user);
        }
    }
}
