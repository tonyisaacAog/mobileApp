using CompanyApi.DTOs;

namespace CompanyApi.Services.Interfaces
{
    public interface IBranchService
    {
        Task CreateBranchAsync(BranchDto branch);
        Task<Result<BranchDto?>> GetBranchByIdAsync(int id);
        Task<Result<IEnumerable<BranchDto>>> GetAllBranchesAsync();
        Task UpdateBranchAsync(int id, BranchDto branch);
        Task DeleteBranchAsync(int id);
    }
}
