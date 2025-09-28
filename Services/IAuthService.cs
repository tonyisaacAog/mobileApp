using CompanyApi.Models;

namespace CompanyApi.Services
{
    public interface IAuthService
    {
        Task<string> GenerateJwtToken(User user);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        Task<User?> AuthenticateUser(string username, string password);
        Task<bool> IsUsernameTaken(string username);
        Task<bool> IsEmailTaken(string email);
    }
}
