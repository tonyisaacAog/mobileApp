using CompanyApi.DTOs;

namespace CompanyApi.Services.Interfaces
{
    public interface ICompanyService
    {
        Task CreateCompanyAsync(CompanyDto dto);
        Task<Result<CompanyDto>> GetCompanyAsync();
        Task UpdateCompanyAsync(CompanyDto dto);
    }
}
