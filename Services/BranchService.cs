using CompanyApi.DTOs;
using CompanyApi.Repositories;

namespace CompanyApi.Services
{
    public class BranchService : IBranchService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BranchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateBranchAsync(BranchDto dto)
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
                InitialInvoiceTaxSerial = dto.InitialInvoiceTaxSerial,
                InitialCreditTaxSerial = dto.InitialCreditTaxSerial,
                InitialDebitTaxSerial = dto.InitialDebitTaxSerial
            };
            await _unitOfWork.Branches.AddAsync(branch);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<BranchDto>> GetAllBranchesAsync()
        {
            var branches = await _unitOfWork.Branches.GetAllAsync();
            return branches.Select(b => new BranchDto
            {
                Name = b.Name,
                Code = b.Code,
                Country = b.Country,
                Governate = b.Governate,
                RegionCity = b.RegionCity,
                Street = b.Street,
                BuildingNumber = b.BuildingNumber,
                InitialInvoiceTaxSerial = b.InitialInvoiceTaxSerial,
                InitialCreditTaxSerial = b.InitialCreditTaxSerial,
                InitialDebitTaxSerial = b.InitialDebitTaxSerial
            });
        }

        public async Task<BranchDto?> GetBranchByIdAsync(int id)
        {
            var branch = await _unitOfWork.Branches.GetByIdAsync(id);
            if (branch == null) return null;
            return new BranchDto
            {
                Name = branch.Name,
                Code = branch.Code,
                Country = branch.Country,
                Governate = branch.Governate,
                RegionCity = branch.RegionCity,
                Street = branch.Street,
                BuildingNumber = branch.BuildingNumber,
                InitialInvoiceTaxSerial = branch.InitialInvoiceTaxSerial,
                InitialCreditTaxSerial = branch.InitialCreditTaxSerial,
                InitialDebitTaxSerial = branch.InitialDebitTaxSerial
            };
        }

        public async Task UpdateBranchAsync(int id, BranchDto dto)
        {
            var branch = await _unitOfWork.Branches.GetByIdAsync(id);
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
            branch.InitialInvoiceTaxSerial = dto.InitialInvoiceTaxSerial;
            branch.InitialCreditTaxSerial = dto.InitialCreditTaxSerial;
            branch.InitialDebitTaxSerial = dto.InitialDebitTaxSerial;
            _unitOfWork.Branches.Update(branch);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteBranchAsync(int id)
        {
            var branch = await _unitOfWork.Branches.GetByIdAsync(id);
            if (branch == null) throw new KeyNotFoundException("Branch not found.");
            _unitOfWork.Branches.Remove(branch);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsBranchNameUniqueAsync(string branchName)
        {
            var existingBranch = await _unitOfWork.Branches
                .FirstOrDefaultAsync(b => b.Name == branchName);
            return existingBranch == null;
        }
    }
}
