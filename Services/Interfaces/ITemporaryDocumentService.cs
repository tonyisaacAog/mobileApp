using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.TemporaryDocumentDto;
using CompanyApi.Models;

namespace CompanyApi.Services.Interfaces
{
    public interface ITemporaryDocumentService
    {
        Task<Result<List<TemporaryDocument>>> GenerateReceiptsAsync(ReceiptGenerationDto generationDto);
        Task<Result<bool>> ApproveReceiptsAsync(ApproveTempDocumentDto request);
    }
}
