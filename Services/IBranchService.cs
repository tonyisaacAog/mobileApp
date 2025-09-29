using CompanyApi.DTOs;

namespace CompanyApi.Services
{
    public interface IBranchService
    {
        Task CreateBranchAsync(BranchDto branch);
        Task<BranchDto?> GetBranchByIdAsync(int id);
        Task<IEnumerable<BranchDto>> GetAllBranchesAsync();
        Task UpdateBranchAsync(int id, BranchDto branch);
        Task DeleteBranchAsync(int id);
    }
}
