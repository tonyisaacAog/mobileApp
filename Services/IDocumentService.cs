using CompanyApi.DTOs;

namespace CompanyApi.Services
{
    public interface IDocumentService
    {
        Task CreateDocumentAsync(DocumentDto document);
        Task<DocumentDto?> GetDocumentByIdAsync(int id);
        Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync();
        Task UpdateDocumentAsync(int id, DocumentDto document);
        Task DeleteDocumentAsync(int id);
    }
}
