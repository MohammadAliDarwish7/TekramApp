using TekramApp.Models;

namespace TekramApp.Interfaces
{
    public interface IUsers
    {
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task CreateUserAsync(User user, string password);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
        Task<bool> VerifyPasswordAsync(User user, string password);
        Task<string> LoginAsync(string username, string password);

    }
}
