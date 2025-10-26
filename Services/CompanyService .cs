using AutoMapper;
using CompanyApi.DTOs.CompanyDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.Models;
using CompanyApi.Repositories.Interfaces;
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

        public async Task<Result<List<CompanyDto>>> GetAllCompaniesAsync()
        {
            try
            {
                var companies = await _unitOfWork.Repository<Company>().GetAllAsync();
                var companyDtos = _mapper.Map<List<CompanyDto>>(companies);
                return await Result<List<CompanyDto>>.SuccessAsync(companyDtos, "تم استرجاع الشركات بنجاح");
            }
            catch (Exception ex)
            {
                return await Result<List<CompanyDto>>.FailureAsync($"خطأ في استرجاع الشركات: {ex.Message}");
            }
        }

        public async Task<Result<CompanyDto>?> GetCompanyByIdAsync(int id)
        {
            try
            {
                var company = await _unitOfWork.Repository<Company>().GetByIdAsync(id);
                if (company == null)
                    return null;

                var companyDto = _mapper.Map<CompanyDto>(company);
                return await Result<CompanyDto>.SuccessAsync(companyDto, "تم استرجاع الشركة بنجاح");
            }
            catch (Exception ex)
            {
                return await Result<CompanyDto>.FailureAsync($"خطأ في استرجاع الشركة: {ex.Message}");
            }
        }

        public async Task<Result<CompanyDto>> CreateCompanyAsync(CompanyDto dto)
        {
            try
            {
                var company = _mapper.Map<Company>(dto);
                company.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<Company>().AddAsync(company);
                await _unitOfWork.SaveChangesAsync();

                var companyDto = _mapper.Map<CompanyDto>(company);
                return await Result<CompanyDto>.SuccessAsync(companyDto, "تم إنشاء الشركة بنجاح");
            }
            catch (Exception ex)
            {
                return await Result<CompanyDto>.FailureAsync($"خطأ في إنشاء الشركة: {ex.Message}");
            }
        }

        public async Task<Result<CompanyDto>?> UpdateCompanyAsync(int id, CompanyDto dto)
        {
            try
            {
                var existingCompany = await _unitOfWork.Repository<Company>().GetByIdAsync(id);
                if (existingCompany == null)
                    return null;

                _mapper.Map(dto, existingCompany);
                existingCompany.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Repository<Company>().Update(existingCompany);
                await _unitOfWork.SaveChangesAsync();

                var companyDto = _mapper.Map<CompanyDto>(existingCompany);
                return await Result<CompanyDto>.SuccessAsync(companyDto, "تم تحديث الشركة بنجاح");
            }
            catch (Exception ex)
            {
                return await Result<CompanyDto>.FailureAsync($"خطأ في تحديث الشركة: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteCompanyAsync(int id)
        {
            try
            {
                var company = await _unitOfWork.Repository<Company>().GetByIdAsync(id);
                if (company == null)
                    return await Result<bool>.FailureAsync("الشركة غير موجودة");

                _unitOfWork.Repository<Company>().Remove(company);
                await _unitOfWork.SaveChangesAsync();

                return await Result<bool>.SuccessAsync(true, "تم حذف الشركة بنجاح");
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailureAsync($"خطأ في حذف الشركة: {ex.Message}");
            }
        }

        public async Task<Result<int>> GetCountCompanies()
        {
            var count = await _unitOfWork.Repository<Company>().CountAsync();
            return await Result<int>.SuccessAsync(count);
        }
    }
}
