using CompanyApi.DTOs.DocumentDtos;
using CompanyApi.DTOs.ResponseDtos;

namespace CompanyApi.Services.Interfaces
{
    public interface IDocumentService
    {
        Task CreateDocumentAsync(CreateDocumentDto document);
        Task<Result<DocumentDetailsDto>?> GetDocumentByIdAsync(int id);
        Task<PagedResult<DocumentDto>> GetAllDocumentsAsync(PaginationParameters paginationParams);
        Task UpdateDocumentAsync(int id, DocumentDto document);
        Task DeleteDocumentAsync(int id);
    }
}
