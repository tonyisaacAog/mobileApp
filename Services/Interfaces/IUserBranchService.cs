using CompanyApi.DTOs;

namespace CompanyApi.Services.Interfaces
{
    public interface IUserBranchService
    {
        Task<Result<UserBranchDto>> AddBranchToUserAsync(int userId, int branchId);
        Task<Result<bool>> RemoveBranchFromUserAsync(int userId, int branchId);
        Task<Result<bool>> SetBranchActiveStatusAsync(int userId, int branchId, bool isActive);
        Task<Result<IEnumerable<BranchDto>>> GetUserBranchesAsync(int userId, bool onlyActive = true);

    }
}
