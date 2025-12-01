using CompanyApi.DTOs.CompanyDtos;
using CompanyApi.DTOs.ResponseDtos;

namespace CompanyApi.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Result<List<CompanyDto>>> GetAllCompaniesAsync();
        Task<Result<CompanyDto>?> GetCompanyByIdAsync(int id);
        Task<Result<CompanyDto>> CreateCompanyAsync(CompanyDto dto);
        Task<Result<CompanyDto>?> UpdateCompanyAsync(int id, CompanyDto dto);
        Task<Result<bool>> DeleteCompanyAsync(int id);
        Task<Result<int>> GetCountCompanies();
    }
}
