using CompanyApi.DTOs;
using CompanyApi.Repositories;

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
            await _unitOfWork.Receipts.AddAsync(new Models.Document
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
                CompanyId = document.CompanyId,
                CreatedAt = DateTime.UtcNow
            });
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync()
        {
            var documents = await _unitOfWork.Receipts.GetAllAsync();
            return documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                ReceiptNumber = d.ReceiptNumber,
                ReceiptDate = d.ReceiptDate,
                Subtotal = d.Subtotal,
                TaxAmount = d.TaxAmount,
                TotalDiscount = d.TotalDiscount,
                TotalAmount = d.TotalAmount,
                ExtraDiscount = d.ExtraDiscount,
                TotalVAT = d.TotalVAT,
                PaymentMethod = d.PaymentMethod,
                DocumentType = d.DocumentType,
                Notes = d.Notes,
                CustomerName = d.CustomerName,
                CustomerCode = d.CustomerCode,
                CustomerTaxId = d.CustomerTaxId,
                CustomerPhone = d.CustomerPhone,
                CustomerCountryCode = d.CustomerCountryCode,
                CustomerGovernate = d.CustomerGovernate,
                CustomerCity = d.CustomerCity,
                CustomerStreet = d.CustomerStreet,
                CustomerBuilding = d.CustomerBuilding,
                CustomerType = d.CustomerType,
                ReferenceNumber = d.ReferenceNumber,
                UserId = d.UserId,
                BranchId = d.BranchId,
                CompanyId = d.CompanyId
            });
        }

        public async Task<DocumentDto?> GetDocumentByIdAsync(int id)
        {
            var document = await _unitOfWork.Receipts.GetByIdAsync(id);
            if (document == null) return null;
            return new DocumentDto
            {
                Id = document.Id,
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
                CompanyId = document.CompanyId
            };
        }

        public async Task UpdateDocumentAsync(int id,DocumentDto document)
        {
            var existingDocument = await _unitOfWork.Receipts.GetByIdAsync(id);
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
            existingDocument.CompanyId = document.CompanyId;
            existingDocument.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Receipts.Update(existingDocument);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(int id)
        {
            var document = await _unitOfWork.Receipts.GetByIdAsync(id);
            if( document == null )
            {
                throw new KeyNotFoundException("Document not found.");
            }
            _unitOfWork.Receipts.Remove(document);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<bool> IsDocumentUniqueAsync(string receiptNumber)
        {
            var existingDocument = await _unitOfWork.Receipts.FirstOrDefaultAsync(d => d.ReceiptNumber == receiptNumber);
            return existingDocument == null;
        }
    }
}
