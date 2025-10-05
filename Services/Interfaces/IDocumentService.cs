using CompanyApi.DTOs.DeviceDtos;
using CompanyApi.DTOs.DocumentDtos;
using CompanyApi.DTOs.OrderReportDtos;
using CompanyApi.DTOs.QueryParameters;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.UserDtos;

namespace CompanyApi.Services.Interfaces
{
    public interface IDocumentService
    {
        Task CreateDocumentAsync(CreateDocumentDto document);
        Task<Result<DocumentsTotalsDto>> GetDocumentsStatsAsync(string deviceCode);
        Task<Result<DocumentDetailsDto>?> GetDocumentByIdAsync(int id);
        Task<PagedResult<DocumentDto>> GetAllDocumentsAsync(DocumentQueryParamters paginationParams);
        Task UpdateDocumentAsync(int id, DocumentDto document);
        Task DeleteDocumentAsync(int id);

        // Order Report methods
        Task<Result<IEnumerable<OrderReportDto>>> GetOrdersWithFiltersAsync(OrderReportFilterDto filter);
        Task<Result<OrderDetailsDto>?> GetOrderDetailsAsync(int id);
        // Dashboard methods
        Task<Result<int>> GetDocumentCountAsync();
    }
}
