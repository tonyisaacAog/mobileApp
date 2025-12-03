using CompanyApi.DTOs.QueryParameters;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.TemporaryDocumentDto;
using CompanyApi.Models;

namespace CompanyApi.Services.Interfaces
{
    public interface ITemporaryDocumentService
    {
        Task<Result<int>> GenerateReceiptsAsync(ReceiptGenerationDto generationDto);
        Task<Result<bool>> ApproveReceiptsAsync(ApproveTempDocumentDto request);

        Task<PagedResult<TemporaryDocumentDto>> GetReceiptsByGroupId(TemporaryDocumentQueryParameters parameters);
    }
}
