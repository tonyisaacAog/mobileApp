using CompanyApi.DTOs.DocumentDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Repositories.Utilities;
using CompanyApi.Services.Interfaces;

namespace CompanyApi.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DocumentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateDocumentAsync(DocumentDto document)
        {
            var isUnique = await IsDocumentUniqueAsync(document.ReceiptNumber);
            if (!isUnique)
            {
                throw new InvalidOperationException("Receipt number must be unique.");
            }

            var newDocument = new Models.Document
            {
                ReceiptNumber = document.ReceiptNumber,
                ReceiptDate = document.ReceiptDate,
                Subtotal = document.Subtotal,
                TaxAmount = document.TaxAmount,
                TotalDiscount = document.TotalDiscount,
                TotalAmount = document.TotalAmount,
                ExtraDiscount = document.ExtraDiscount,
                TotalVAT = document.TotalVAT,
                PaymentMethod = document.PaymentMethod,
                DocumentType = document.DocumentType,
                Notes = document.Notes,
                CustomerName = document.CustomerName,
                CustomerCode = document.CustomerCode,
                CustomerTaxId = document.CustomerTaxId,
                CustomerPhone = document.CustomerPhone,
                CustomerCountryCode = document.CustomerCountryCode,
                CustomerGovernate = document.CustomerGovernate,
                CustomerCity = document.CustomerCity,
                CustomerStreet = document.CustomerStreet,
                CustomerBuilding = document.CustomerBuilding,
                CustomerType = document.CustomerType,
                ReferenceNumber = document.ReferenceNumber,
                UserId = document.UserId,
                BranchId = document.BranchId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Models.Document>().AddAsync(newDocument);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<DocumentDto>> GetAllDocumentsAsync(PaginationParameters paginationParams)
        {
            var selectors = MappingUtilities.CreateMapExpression<Models.Document, DocumentDto>();
            var documents = await _unitOfWork.Repository<Models.Document>()
                .GetProjectedPaginatedAsync(selectors, paginationParams);

            return await PagedResult<DocumentDto>.SuccessAsync(
                documents.Items,
                documents.TotalCount,
                paginationParams.PageNumber,
                paginationParams.PageSize
            );
        }

        public async Task<Result<DocumentDto>?> GetDocumentByIdAsync(int id)
        {
            var selectors = MappingUtilities.CreateMapExpression<Models.Document, DocumentDto>();
            var document = await _unitOfWork.Repository<Models.Document>()
                .GetByIdAsync(obj => obj.Id == id, selectors);

            return document == null
                ? null
                : Result<DocumentDto>.Success(document);
        }

        public async Task UpdateDocumentAsync(int id, DocumentDto document)
        {
            var repo = _unitOfWork.Repository<Models.Document>();
            var existingDocument = await repo.GetByIdAsync(id);

            if (existingDocument == null)
            {
                throw new KeyNotFoundException("Document not found.");
            }

            if (existingDocument.ReceiptNumber != document.ReceiptNumber)
            {
                var isUnique = await IsDocumentUniqueAsync(document.ReceiptNumber);
                if (!isUnique)
                {
                    throw new InvalidOperationException("Receipt number must be unique.");
                }
            }

            existingDocument.ReceiptNumber = document.ReceiptNumber;
            existingDocument.ReceiptDate = document.ReceiptDate;
            existingDocument.Subtotal = document.Subtotal;
            existingDocument.TaxAmount = document.TaxAmount;
            existingDocument.TotalDiscount = document.TotalDiscount;
            existingDocument.TotalAmount = document.TotalAmount;
            existingDocument.ExtraDiscount = document.ExtraDiscount;
            existingDocument.TotalVAT = document.TotalVAT;
            existingDocument.PaymentMethod = document.PaymentMethod;
            existingDocument.DocumentType = document.DocumentType;
            existingDocument.Notes = document.Notes;
            existingDocument.CustomerName = document.CustomerName;
            existingDocument.CustomerCode = document.CustomerCode;
            existingDocument.CustomerTaxId = document.CustomerTaxId;
            existingDocument.CustomerPhone = document.CustomerPhone;
            existingDocument.CustomerCountryCode = document.CustomerCountryCode;
            existingDocument.CustomerGovernate = document.CustomerGovernate;
            existingDocument.CustomerCity = document.CustomerCity;
            existingDocument.CustomerStreet = document.CustomerStreet;
            existingDocument.CustomerBuilding = document.CustomerBuilding;
            existingDocument.CustomerType = document.CustomerType;
            existingDocument.ReferenceNumber = document.ReferenceNumber;
            existingDocument.UserId = document.UserId;
            existingDocument.BranchId = document.BranchId;
            existingDocument.UpdatedAt = DateTime.UtcNow;

            repo.Update(existingDocument);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<bool> IsDocumentUniqueAsync(string receiptNumber)
        {
            var repo = _unitOfWork.Repository<Models.Document>();
            var existing = await repo.FirstOrDefaultAsync(d => d.ReceiptNumber == receiptNumber);
            return existing == null;
        }

        public async Task DeleteDocumentAsync(int id)
        {
            var repo = _unitOfWork.Repository<Models.Document>();
            var document = await repo.GetByIdAsync(id);

            if (document == null)
            {
                throw new KeyNotFoundException("Document not found.");
            }

            repo.Remove(document);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
