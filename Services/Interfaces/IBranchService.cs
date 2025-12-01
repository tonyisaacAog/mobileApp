using CompanyApi.DTOs.BranchDtos;
using CompanyApi.DTOs.ResponseDtos;

namespace CompanyApi.Services.Interfaces
{
    public interface IBranchService
    {
        Task CreateBranchAsync(CreateBranchDto branch);
        Task<Result<BranchDto?>> GetBranchByIdAsync(int id);
        Task<Result<IEnumerable<BranchDto>>> GetAllBranchesAsync();
        Task UpdateBranchAsync(int id, BranchDto branch);
        Task DeleteBranchAsync(int id);
        Task<Result<int>> GetCountBranches();

    }
}
