using AutoMapper;
using CompanyApi.DTOs;
using CompanyApi.Models;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Repositories.Utilities;
using CompanyApi.Services.Interfaces;

namespace CompanyApi.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateCompanyAsync(CompanyDto dto)
        {
            // Ensure only one company exists
            var existingCompany = await _unitOfWork.Repository<Company>().FirstOrDefaultAsync(c => true);
            if (existingCompany != null)
            {
                throw new InvalidOperationException("A company already exists. You can only update it.");
            }

            var company = new Company
            {
                Name = dto.Name,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                TaxNumber = dto.TaxNumber,
                Description = dto.Description,
                TradeName = dto.TradeName,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Company>().AddAsync(company);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Result<CompanyDto>> GetCompanyAsync()
        {
            var selectors = MappingUtilities.CreateMapExpression<Company, CompanyDto>();
            var company = await _unitOfWork.Repository<Company>().FirstOrDefaultAsync(c => true);

            if (company == null)
            {
                return await Result<CompanyDto>.FailureAsync("No company found.");
            }

            return await Result<CompanyDto>.SuccessAsync(_mapper.Map<CompanyDto>(company));
        }

        public async Task UpdateCompanyAsync(CompanyDto dto)
        {
            var company = await _unitOfWork.Repository<Company>().FirstOrDefaultAsync(c => true);

            if (company == null)
            {
                throw new KeyNotFoundException("No company found. Please create one first.");
            }

            company.Name = dto.Name;
            company.Address = dto.Address;
            company.PhoneNumber = dto.PhoneNumber;
            company.Email = dto.Email;
            company.TaxNumber = dto.TaxNumber;
            company.Description = dto.Description;
            company.TradeName = dto.TradeName;
            company.IsActive = dto.IsActive;

            _unitOfWork.Repository<Company>().Update(company);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
