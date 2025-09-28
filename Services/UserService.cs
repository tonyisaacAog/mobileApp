using CompanyApi.Models;
using CompanyApi.Repositories;

namespace CompanyApi.Services
{

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public UserService(IUnitOfWork unitOfWork, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _unitOfWork.Users.GetAllAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Check if username or email already exists
            if (await _authService.IsUsernameTaken(user.Username))
                throw new ArgumentException("Username already exists");

            if (await _authService.IsEmailTaken(user.Email))
                throw new ArgumentException("Email already exists");

            // Hash password
            user.PasswordHash =  _authService.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            var existingUser = await _unitOfWork.Users.GetByIdAsync(id);
            if (existingUser == null)
                return null;

            // Check if username or email already exists (excluding current user)
            if (await _unitOfWork.Users.AnyAsync(u => u.Username == user.Username && u.Id != id))
                throw new ArgumentException("Username already exists");

            if (await _unitOfWork.Users.AnyAsync(u => u.Email == user.Email && u.Id != id))
                throw new ArgumentException("Email already exists");

            // Update fields
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.IsAdmin = user.IsAdmin;

            // Only update password if provided
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                existingUser.PasswordHash =  _authService.HashPassword(user.PasswordHash);
            }

            _unitOfWork.Users.Update(existingUser);
            await _unitOfWork.SaveChangesAsync();

            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return false;

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _unitOfWork.Users.FindAsync(u => u.IsActive);
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return false;

            user.IsActive = true;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return false;

            user.IsActive = false;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
