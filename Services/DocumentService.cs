using AutoMapper;
using CompanyApi.DTOs.DocumentDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.Models;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Repositories.Utilities;
using CompanyApi.Services.Interfaces;

namespace CompanyApi.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DocumentService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateDocumentAsync(CreateDocumentDto document)
        {
            const decimal VAT_RATE = 0.14m;

            // 1. Validate branch
            if (document.BranchId == null)
                throw new InvalidOperationException("Branch is required.");

            var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(document.BranchId.Value);
            if (branch == null)
                throw new InvalidOperationException($"Branch with ID {document.BranchId} does not exist.");

            // 2. Validate user
            if (document.UserId == null)
                throw new InvalidOperationException("User is required.");


            if(document.DeviceCode != null)
            {
                var device = await _unitOfWork.Repository<Device>()
                    .FirstOrDefaultAsync(d => d.Code == document.DeviceCode && d.BranchId == document.BranchId);
                if (device == null)
                    throw new InvalidOperationException($"Device with code {document.DeviceCode} does not exist in branch {branch.Name}.");
                // Optionally, you can associate the device with the document here if needed
                // newDocument.DeviceId = device.Id;
            }

            //var user = await _unitOfWork.Repository<UserBranch>()
            //    .FirstOrDefaultAsync(obj=>obj.BranchId== document.BranchId && obj.UserId== document.UserId.Value);
            //if (user == null)
            //    throw new InvalidOperationException($"User with ID {document.UserId} does not exist.");

            //// 3. Check user belongs to branch
            //if (user.BranchId != document.BranchId)
            //    throw new UnauthorizedAccessException("This user does not have access to the specified branch.");

            // 4. Create Document
            var newDocument = new Models.Document
            {
                ReceiptNumber = Guid.NewGuid().ToString(),
                ReceiptDate = document.ReceiptDate,
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

            decimal subtotal = 0;
            decimal totalDiscount = 0;
            decimal totalVAT = 0;

            foreach (var item in document.Items)
            {
                var lineTotal = item.Quantity * item.UnitPrice; // before discount
                var lineNet = lineTotal - item.DiscountAmount;  // after discount
                var lineVAT = lineNet * VAT_RATE;

                subtotal += lineTotal;
                totalDiscount += item.DiscountAmount;
                totalVAT += lineVAT;

                var lineEntity = new Models.DocumentLines
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountAmount = item.DiscountAmount,
                    TotalPrice = lineTotal,
                    NetTotal = lineNet,
                    VAT = lineVAT,
                    Notes = item.Notes
                };

                newDocument.ReceiptItems.Add(lineEntity);
            }

            totalDiscount += document.ExtraDiscount;

            newDocument.Subtotal = subtotal;
            newDocument.TotalDiscount = totalDiscount;
            newDocument.TotalVAT = totalVAT;
            newDocument.ExtraDiscount = document.ExtraDiscount;
            newDocument.TotalAmount = subtotal - totalDiscount + totalVAT;

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

        //public async Task<PagedResult<DocumentDto>> GetDocumentsStatsAsync(PaginationParameters paginationParams)
        //{
        //    var selectors = MappingUtilities.CreateMapExpression<Models.Document, DocumentsTotalsDto>();
        //    var documents = await _unitOfWork.Repository<Models.Document>()
        //        .SumAsync(x => x.CreatedAt.Date == DateTime.Now.Date);

        //    return await PagedResult<DocumentDto>.SuccessAsync(
        //        documents.Items,
        //        documents.TotalCount,
        //        paginationParams.PageNumber,
        //        paginationParams.PageSize
        //    );
        //}

        public async Task<Result<DocumentDetailsDto>?> GetDocumentByIdAsync(int id)
        {
            //var selectors = MappingUtilities.CreateMapExpression<Models.Document, DocumentDetailsDto>();
            var document = await _unitOfWork.Repository<Models.Document>()
                .GetByIdAsync(obj => obj.Id == id, x => new DocumentDetailsDto
                {
                    BranchId = x.BranchId,
                    CustomerBuilding = x.CustomerBuilding,
                    CustomerCity = x.CustomerCity,
                    CustomerCode = x.CustomerCode,
                    CustomerCountryCode = x.CustomerCountryCode,
                    CustomerGovernate = x.CustomerGovernate,
                    CustomerName = x.CustomerName,
                    CustomerPhone = x.CustomerPhone,
                    CustomerTaxId = x.CustomerTaxId,
                    CustomerStreet = x.CustomerStreet,
                    CustomerType = x.CustomerType,
                    DocumentType = x.DocumentType,
                    ExtraDiscount = x.ExtraDiscount,
                    Id = x.Id,
                    Notes = x.Notes,
                    PaymentMethod = x.PaymentMethod,
                    ReceiptDate = x.ReceiptDate,
                    ReceiptNumber = x.ReceiptNumber,
                    ReferenceNumber = x.ReferenceNumber,
                    Subtotal = x.Subtotal,
                    TaxAmount = x.TaxAmount,
                    TotalAmount = x.TotalAmount,
                    TotalDiscount = x.TotalDiscount,
                    TotalVAT = x.TotalVAT,
                    UserId = x.UserId,
                    ReceiptItems = x.ReceiptItems.Select(ri => new DocumentLinesDto
                    {
                        ProductId = ri.ProductId,
                        Quantity = ri.Quantity,
                        UnitPrice = ri.UnitPrice,
                        DiscountAmount = ri.DiscountAmount,
                        ProductName = ri.Product != null ? ri.Product.Name : string.Empty,
                        VAT = ri.VAT,
                        Notes = ri.Notes
                    }).ToList()
                });

            return document == null
                ? null
                : Result<DocumentDetailsDto>.Success(document);
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
