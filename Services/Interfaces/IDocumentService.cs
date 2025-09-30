using CompanyApi.DTOs.DocumentDtos;
using CompanyApi.DTOs.ResponseDtos;

namespace CompanyApi.Services.Interfaces
{
    public interface IDocumentService
    {
        Task CreateDocumentAsync(DocumentDto document);
        Task<Result<DocumentDto>?> GetDocumentByIdAsync(int id);
        Task<PagedResult<DocumentDto>> GetAllDocumentsAsync(PaginationParameters paginationParams);
        Task UpdateDocumentAsync(int id, DocumentDto document);
        Task DeleteDocumentAsync(int id);
    }
}
