using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.UserDtos;
using CompanyApi.Models;

namespace CompanyApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>?> GetUserByIdAsync(int id);
        Task<PagedResult<UserDto>> GetAllUsersAsync(PaginationParameters paginationParams);
        Task<Result<UserDto>> CreateUserAsync(CreateUserDto user);
        Task<Result<UserDto>?> UpdateUserAsync(int id, UpdateUserDto user);
        Task<Result<bool>> DeleteUserAsync(int id);
        Task<Result<UserDto>?> GetUserByUsernameAsync(string username);
        Task<PagedResult<UserDto>> GetActiveUsersAsync(PaginationParameters paginationParams);
        Task<Result<bool>> ActivateUserAsync(int id);
        Task<Result<bool>> DeactivateUserAsync(int id);
        Task<Result<int>> GetCountUsers();
    }
}
