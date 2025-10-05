using CompanyApi.DTOs.BranchDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.UserDtos;

namespace CompanyApi.Services.Interfaces
{
    public interface IUserBranchService
    {
        Task<Result<List<UserBranchDto>>> AddBranchToUserAsync(int userId,int[] branchIds);
        Task<Result<bool>> RemoveBranchFromUserAsync(int userId);//, int branchId
        Task<Result<bool>> SetBranchActiveStatusAsync(int userId, int branchId, bool isActive);
        Task<Result<IEnumerable<BranchDto>>> GetUserBranchesAsync(int userId, bool onlyActive = true);

    }
}
