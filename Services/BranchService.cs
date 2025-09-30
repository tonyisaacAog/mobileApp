using CompanyApi.DTOs.BranchDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.Models;
using CompanyApi.Repositories;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Services.Interfaces;

namespace CompanyApi.Services
{
    public class BranchService : IBranchService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BranchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateBranchAsync(CreateBranchDto dto)
        {
            var isUnique = await IsBranchNameUniqueAsync(dto.Name);
            if( !isUnique )
            {
                throw new InvalidOperationException("Branch name must be unique within the company.");
            }
            var branch = new Models.Branch
            {
                Name = dto.Name,
                Code = dto.Code,
                Country = dto.Country,
                Governate = dto.Governate,
                RegionCity = dto.RegionCity,
                Street = dto.Street,
                BuildingNumber = dto.BuildingNumber,
            };
            await _unitOfWork.Repository<Branch>().AddAsync(branch);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Result<IEnumerable<BranchDto>>> GetAllBranchesAsync()
        {
            var branches = await _unitOfWork.Repository<Branch>().GetProjectedAsync<BranchDto>(b => new BranchDto
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                Country = b.Country,
                Governate = b.Governate,
                RegionCity = b.RegionCity,
                Street = b.Street,
                BuildingNumber = b.BuildingNumber
            });
            return Result<IEnumerable<BranchDto>>.Success(branches);
        }

        public async Task<Result<BranchDto>?> GetBranchByIdAsync(int id)
        {
            var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(obj=>obj.Id == id,branch=> new BranchDto
            {
                Name = branch.Name,
                Code = branch.Code,
                Country = branch.Country,
                Governate = branch.Governate,
                RegionCity = branch.RegionCity,
                Street = branch.Street,
                BuildingNumber = branch.BuildingNumber
            });
        return branch == null ? null : Result<BranchDto>.Success(branch);
        }

        public async Task UpdateBranchAsync(int id, BranchDto dto)
        {
            var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(id);
            if (branch == null) throw new KeyNotFoundException("Branch not found.");
            var isUnique = await IsBranchNameUniqueAsync(dto.Name);
            if( !isUnique && branch.Name != dto.Name )
            {
                throw new InvalidOperationException("Branch name must be unique within the company.");
            }
            branch.Name = dto.Name;
            branch.Code = dto.Code;
            branch.Country = dto.Country;
            branch.Governate = dto.Governate;
            branch.RegionCity = dto.RegionCity;
            branch.Street = dto.Street;
            branch.BuildingNumber = dto.BuildingNumber;
            _unitOfWork.Repository<Branch>().Update(branch);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteBranchAsync(int id)
        {
            var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(id);
            if (branch == null) throw new KeyNotFoundException("Branch not found.");
            _unitOfWork.Repository<Branch>().Remove(branch);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsBranchNameUniqueAsync(string branchName)
        {
            var existingBranch = await _unitOfWork.Repository<Branch>()
                .FirstOrDefaultAsync(b => b.Name == branchName);
            return existingBranch == null;
        }

        public async Task<Result<int>> GetCountBranches()
        {
            var count = await _unitOfWork.Repository<Branch>().CountAsync();
            return await Result<int>.SuccessAsync(count);
        }
    }
}
