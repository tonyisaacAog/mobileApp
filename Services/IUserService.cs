using CompanyApi.Models;

namespace CompanyApi.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
        Task<User?> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<bool> ActivateUserAsync(int id);
        Task<bool> DeactivateUserAsync(int id);
    }
}
