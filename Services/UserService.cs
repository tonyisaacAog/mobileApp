using AutoMapper;
using CompanyApi.DTOs;
using CompanyApi.Models;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Services.Interfaces;

namespace CompanyApi.Services
{

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IAuthService authService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync<UserDto>(obj => obj.Id == id, obj => new UserDto
            {
                Email = obj.Email,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                IsActive = obj.IsActive,
                IsAdmin = obj.IsAdmin,
                PhoneNumber = obj.PhoneNumber,
                Username = obj.Username,
            });
            return await Result<UserDto>.SuccessAsync(user);
        }



        public async Task<PagedResult<UserDto>> GetAllUsersAsync(PaginationParameters paginationParams)
        {
            var users = await _unitOfWork.Repository<User>().GetProjectedPaginatedAsync(obj => new UserDto
            {
                Email = obj.Email,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                IsActive = obj.IsActive,
                IsAdmin = obj.IsAdmin,
                PhoneNumber = obj.PhoneNumber,
                Username = obj.Username,
            }, paginationParams);
            return await PagedResult<UserDto>.SuccessAsync(users.Items, users.TotalCount, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<Result<UserDto>> CreateUserAsync(User user)
        {
            // Check if username or email already exists
            if (await _authService.IsUsernameTaken(user.Username))
                throw new ArgumentException("Username already exists");

            if (await _authService.IsEmailTaken(user.Email))
                throw new ArgumentException("Email already exists");

            // Hash password
            user.PasswordHash = _authService.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return await Result<UserDto>.SuccessAsync(_mapper.Map<UserDto>(user));
        }

        public async Task<Result<UserDto>?> UpdateUserAsync(int id, User user)
        {
            var existingUser = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            if (existingUser == null)
                return null;

            // Check if username or email already exists (excluding current user)
            if (await _unitOfWork.Repository<User>().AnyAsync(u => u.Username == user.Username && u.Id != id))
                throw new ArgumentException("Username already exists");

            if (await _unitOfWork.Repository<User>().AnyAsync(u => u.Email == user.Email && u.Id != id))
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
                existingUser.PasswordHash = _authService.HashPassword(user.PasswordHash);
            }

            _unitOfWork.Repository<User>().Update(existingUser);
            await _unitOfWork.SaveChangesAsync();

            return await Result<UserDto>.SuccessAsync(_mapper.Map<UserDto>(existingUser));
        }

        public async Task<Result<bool>> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            if (user == null)
                return await Result<bool>.FailureAsync(false, "user not found");

            _unitOfWork.Repository<User>().Remove(user);
            await _unitOfWork.SaveChangesAsync();

            return await Result<bool>.SuccessAsync(true, "user deleted");
        }

        public async Task<Result<UserDto>?> GetUserByUsernameAsync(string username)
        {
            var user = await _unitOfWork.Repository<User>().FirstOrDefaultAsync(u => u.Username == username);
            return user == null ? null : await Result<UserDto>.SuccessAsync(_mapper.Map<UserDto>(user));
        }

        public async Task<PagedResult<UserDto>> GetActiveUsersAsync(PaginationParameters paginationParams)
        {
            var users = await _unitOfWork.Repository<User>().GetProjectedPaginatedAsync<UserDto>
                (u => u.IsActive, obj => new UserDto
                {
                    Email = obj.Email,
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    IsActive = obj.IsActive,
                    IsAdmin = obj.IsAdmin,
                    PhoneNumber = obj.PhoneNumber,
                    Username = obj.Username,
                }, paginationParams);
            return await PagedResult<UserDto>.SuccessAsync(users.Items, users.TotalCount, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<Result<bool>> ActivateUserAsync(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            if (user == null)
                return await Result<bool>.FailureAsync(false, "user not found");

            user.IsActive = true;
            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync();

            return await Result<bool>.FailureAsync(true, "active user");
        }

        public async Task<Result<bool>> DeactivateUserAsync(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            if (user == null)
                return await Result<bool>.FailureAsync(false, "user not found");

            user.IsActive = false;
            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync();

            return await Result<bool>.FailureAsync(true, "deactive user");
        }


    }
}
