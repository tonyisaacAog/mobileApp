using AutoMapper;
using CompanyApi.DTOs.DocumentDtos;
using CompanyApi.DTOs.OrderReportDtos;
using CompanyApi.DTOs.ProductDtos;
using CompanyApi.DTOs.QueryParameters;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.UserDtos;
using CompanyApi.Models;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Repositories.Utilities;
using CompanyApi.Services.Interfaces;
using LinqKit;
using System.Linq.Expressions;

namespace CompanyApi.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DocumentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DocumentDetailsDto>> CreateDocumentAsync(CreateDocumentDto document)
        {
            const decimal VAT_RATE = 0.14m;

            if( document.BranchId == null )
                return await Result<DocumentDetailsDto>.FailureAsync("Branch is required.");

            if( document.UserId == null )
                return await Result<DocumentDetailsDto>.FailureAsync("User is required.");

            if( string.IsNullOrEmpty(document.DeviceCode) )
                return await Result<DocumentDetailsDto>.FailureAsync("Device code is required.");

            var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(document.BranchId.Value);
            if( branch == null )
                return await Result<DocumentDetailsDto>.FailureAsync($"Branch with ID {document.BranchId} does not exist.");

            var device = await _unitOfWork.Repository<Device>()
                .FirstOrDefaultAsync(d => d.Code == document.DeviceCode && d.BranchId == document.BranchId);

            if( device == null )
                return await Result<DocumentDetailsDto>.FailureAsync($"Device with code {document.DeviceCode} does not exist in branch {branch.Name}.");

            if( document.DocumentType == DocumentType.RR )
            {
                if( string.IsNullOrEmpty(document.ReferenceNumber) )
                    return await Result<DocumentDetailsDto>.FailureAsync("Reference number is required for return receipts.");

                var saleDocRepo = _unitOfWork.Repository<Document>();

                var saleDoc = await saleDocRepo.FirstOrDefaultAsync(d => d.Id == Convert.ToInt32(document.ReferenceNumber) && d.DocumentType == DocumentType.SR);
                if( saleDoc == null )
                    return await Result<DocumentDetailsDto>.FailureAsync($"Sale receipt with number {document.ReferenceNumber} not found.");

                var returnDocs = await saleDocRepo.FindAsync(d =>
                    d.ReferenceNumber == document.ReferenceNumber && d.DocumentType == DocumentType.RR);

                var linesRepo = _unitOfWork.Repository<DocumentLines>();
                var saleLines = await linesRepo.FindAsync(l => l.ReceiptId == saleDoc.Id);

                var returnedLines = await linesRepo.FindAsync(l =>
                    returnDocs.Select(r => r.Id).Contains(l.ReceiptId));

                var returnedQuantities = returnedLines
                    .GroupBy(l => l.ProductId)
                    .ToDictionary(g => g.Key,g => g.Sum(x => x.Quantity));

                foreach( var item in document.Items )
                {
                    var saleLine = saleLines.FirstOrDefault(l => l.ProductId == item.ProductId);
                    if( saleLine == null )
                        return await Result<DocumentDetailsDto>.FailureAsync($"Product {item.ProductId} not found in sale receipt {document.ReferenceNumber}.");

                    var alreadyReturned = returnedQuantities.ContainsKey(item.ProductId)
                        ? returnedQuantities[item.ProductId]
                        : 0;

                    var newTotalReturned = alreadyReturned + item.Quantity;

                    if( newTotalReturned > saleLine.Quantity )
                        return await Result<DocumentDetailsDto>.FailureAsync(
                            $"Cannot return more than sold quantity for product {item.ProductId}. " +
                            $"Sold: {saleLine.Quantity}, Already returned: {alreadyReturned}, Trying to return: {item.Quantity}."
                        );
                }
            }
        

            var newDocument = new Document
            {
                ReceiptNumber = Guid.NewGuid().ToString(),
                DeviceSerial = document.DeviceSerial,
                ReceiptDate = document.ReceiptDate,
                PaymentMethod = document.PaymentMethod,
                DocumentType = document.DocumentType,
                Notes = document.Notes,
                CustomerName = document.CustomerName,
                CustomerCode = document.CustomerCode,
                CustomerTaxId = document.CustomerTaxId,
                CustomerPhone = document.CustomerPhone,
                ReferenceNumber = document.ReferenceNumber,
                BranchId = document.BranchId,
                UserId = document.UserId,
                DeviceId = device.Id,
                CreatedAt = DateTime.UtcNow
            };

            decimal subtotal = 0, totalDiscount = 0, totalVAT = 0;

            foreach( var item in document.Items )
            {
                var lineTotal = item.Quantity * item.UnitPrice;
                var lineNet = lineTotal - item.DiscountAmount;
                var lineVAT = item.VAT > 0 ? lineNet * VAT_RATE : 0;

                subtotal += lineTotal;
                totalDiscount += item.DiscountAmount;
                totalVAT += lineVAT;

                newDocument.ReceiptItems.Add(new DocumentLines
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountAmount = item.DiscountAmount,
                    TotalPrice = lineTotal,
                    NetTotal = lineNet,
                    VAT = lineVAT,
                    Notes = item.Notes
                });
            }

            totalDiscount += document.ExtraDiscount;
            newDocument.Subtotal = subtotal;
            newDocument.TotalDiscount = totalDiscount;
            newDocument.TotalVAT = totalVAT;
            newDocument.ExtraDiscount = document.ExtraDiscount;
            newDocument.TotalAmount = subtotal - totalDiscount + totalVAT;

            await _unitOfWork.Repository<Document>().AddAsync(newDocument);
            await _unitOfWork.SaveChangesAsync();

            var documentDetails = _mapper.Map<DocumentDetailsDto>(newDocument);
            return await Result<DocumentDetailsDto>.SuccessAsync(documentDetails,"Document created successfully");
        }


        public async Task<PagedResult<DocumentDto>> GetAllDocumentsAsync(DocumentQueryParamters paginationParams)
        {
            var selectors = MappingUtilities.CreateMapExpression<Models.Document, DocumentDto>();

            // نبدأ بشرط دائم صحيح (يعني لا يمنع أي نتائج)
            Expression<Func<Models.Document, bool>> predicate = x => x.Device != null && x.Device.Code == paginationParams.DeviceCode;

            // نضيف الشروط لو اتوفر قيمها
            if (paginationParams.DateFrom.HasValue)
                predicate = predicate.And(x => x.ReceiptDate.Date >= paginationParams.DateFrom);

            if (paginationParams.DateTo.HasValue)
                predicate = predicate.And(x => x.ReceiptDate.Date <= paginationParams.DateTo.Value);

            if (paginationParams.UserId.HasValue)
                predicate = predicate.And(x => x.UserId == paginationParams.UserId.Value);

            var documents = await _unitOfWork.Repository<Models.Document>()
                .GetProjectedPaginatedAsync(predicate, selectors, paginationParams);

            return await PagedResult<DocumentDto>.SuccessAsync(
                documents.Items,
                paginationParams.PageNumber,
                documents.TotalCount,
                paginationParams.PageSize
            );
        }


        public async Task<Result<DocumentsTotalsDto>> GetDocumentsStatsAsync(string deviceCode)
        {
            var device = await _unitOfWork.Repository<Models.Device>()
                .FirstOrDefaultAsync(d => d.Code == deviceCode);

            if (device == null)
                return await Result<DocumentsTotalsDto>.FailureAsync("Device not found");

            var today = DateTime.Now.Date;

            var documents = await _unitOfWork.Repository<Models.Document>()
                .GetAllByConditionAsync(x =>
                    x.ReceiptDate.Date == today &&
                    x.DeviceId == device.Id,
                    x => new { x.DocumentType, x.TotalAmount, x.TotalDiscount, x.TotalVAT });

            var sumSR = documents.Where(x => x.DocumentType == DocumentType.SR);
            var sumRR = documents.Where(x => x.DocumentType == DocumentType.RR);

            var totals = new DocumentsTotalsDto
            {
                SumOfTotals = sumSR.Sum(d => d.TotalAmount) - sumRR.Sum(d => d.TotalAmount),
                SumOfDiscount = sumSR.Sum(d => d.TotalDiscount) - sumRR.Sum(d => d.TotalDiscount),
                SumOfTaxes = sumSR.Sum(d => d.TotalVAT) - sumRR.Sum(d => d.TotalVAT)
            };

            return await Result<DocumentsTotalsDto>.SuccessAsync(totals);
        }


        public async Task<Result<List<ProductTotalsDto>>> GetProductsTotalsAsync(string deviceCode)
        {
            var device = await _unitOfWork.Repository<Models.Device>()
                .FirstOrDefaultAsync(d => d.Code == deviceCode);

            if (device == null)
                return await Result<List<ProductTotalsDto>>.FailureAsync("Device not found");

            var today = DateTime.Now.Date;

            var lines = await _unitOfWork.Repository<DocumentLines>()
                .GetAllByConditionAsync(l =>
                    l.Receipt.ReceiptDate.Date == today &&
                    l.Receipt.DeviceId == device.Id,
                    l => new
                    {
                        l.Receipt.DocumentType,
                        l.ProductId,
                        l.Quantity,
                        l.TotalPrice,
                        l.DiscountAmount,
                        l.VAT,
                        ProductName = l.Product.Name
                    });

            var grouped = lines
                .GroupBy(x => new { x.ProductId, x.ProductName })
                .Select(g => new ProductTotalsDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Where(obj=>obj.DocumentType == DocumentType.SR).Sum(x => x.Quantity)- g.Where(obj => obj.DocumentType == DocumentType.RR).Sum(x => x.Quantity),
                    TotalSales = g.Where(obj => obj.DocumentType == DocumentType.SR).Sum(x => x.TotalPrice)- g.Where(obj => obj.DocumentType == DocumentType.RR).Sum(x => x.TotalPrice),
                    TotalDiscount = 0,
                    TotalVAT =0
                })
                .ToList();

            return await Result<List<ProductTotalsDto>>.SuccessAsync(grouped);
        }



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
            existingDocument.DeviceSerial = document.DeviceSerial;
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

        // Order Report methods
        public async Task<Result<IEnumerable<OrderReportDto>>> GetOrdersWithFiltersAsync(OrderReportFilterDto filter)
        {
            try
            {
                var documentRepo = _unitOfWork.Repository<Document>();
                var userRepo = _unitOfWork.Repository<User>();
                var deviceRepo = _unitOfWork.Repository<Models.Device>();
                var branchRepo = _unitOfWork.Repository<Branch>();
                var lineRepo = _unitOfWork.Repository<DocumentLines>();

                var documents = await documentRepo.FindAsync(d =>
                    ( !filter.DateFrom.HasValue || d.ReceiptDate.Date >= filter.DateFrom.Value.Date ) &&
                    ( !filter.DateTo.HasValue || d.ReceiptDate.Date <= filter.DateTo.Value.Date ) &&
                    ( !filter.UserId.HasValue || d.UserId == filter.UserId.Value )
                );

                var users = ( await userRepo.GetAllAsync() ).ToDictionary(u => u.Id);
                var devices = ( await deviceRepo.GetAllAsync() ).ToDictionary(d => d.Id);
                var branches = ( await branchRepo.GetAllAsync() ).ToDictionary(b => b.Id);
                var lines = await lineRepo.GetAllAsync();

                var orderReports = documents
                    .Select(d => new OrderReportDto
                    {
                        Id = d.Id,
                        ReceiptNumber = d.ReceiptNumber,
                        ReceiptDate = d.ReceiptDate,
                        CustomerName = d.CustomerName,
                        TotalAmount = d.TotalAmount,
                        PaymentMethod = d.PaymentMethod.ToString(),
                        DeviceName = d.DeviceId.HasValue && devices.ContainsKey(d.DeviceId.Value)
                            ? devices[d.DeviceId.Value].Name : "غير محدد",
                        DeviceCode = d.DeviceId.HasValue && devices.ContainsKey(d.DeviceId.Value)
                            ? devices[d.DeviceId.Value].Code : "غير محدد",
                        UserName = d.UserId.HasValue && users.ContainsKey(d.UserId.Value)
                            ? $"{users[d.UserId.Value].FirstName} {users[d.UserId.Value].LastName}" : "غير محدد",
                        BranchName = d.BranchId.HasValue && branches.ContainsKey(d.BranchId.Value)
                            ? branches[d.BranchId.Value].Name : "غير محدد",
                        ItemsCount = lines.Count(l => l.ReceiptId == d.Id)
                    })
                    .OrderByDescending(o => o.ReceiptDate)
                    .ToList();

                if( !string.IsNullOrEmpty(filter.DeviceCode) )
                    orderReports = orderReports.Where(o => o.DeviceCode == filter.DeviceCode).ToList();

                return await Result<IEnumerable<OrderReportDto>>.SuccessAsync(orderReports, "Orders retrieved successfully");
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<OrderReportDto>>.FailureAsync($"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<Result<OrderDetailsDto>?> GetOrderDetailsAsync(int id)
        {
            try
            {
                // Get document
                var document = await _unitOfWork.Repository<Models.Document>().GetByIdAsync(id);
                if (document == null)
                {
                    return null;
                }

                // Get user
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(document.UserId ?? 0);

                // Get device
                Models.Device? device = null;
                if (document.DeviceId.HasValue)
                {
                    device = await _unitOfWork.Repository<Models.Device>().GetByIdAsync(document.DeviceId.Value);
                }

                // Get branch
                Branch? branch = null;
                if (document.BranchId.HasValue)
                {
                    branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(document.BranchId.Value);
                }

                // Get company
                Company? company = null;
                if (document.CompanyId.HasValue)
                {
                    company = await _unitOfWork.Repository<Company>().GetByIdAsync(document.CompanyId.Value);
                }

                // Get document lines
                var documentLines = await _unitOfWork.Repository<DocumentLines>()
                    .FindAsync(l => l.ReceiptId == document.Id);

                var orderDetails = new OrderDetailsDto
                {
                    Id = document.Id,
                    ReceiptNumber = document.ReceiptNumber,
                    ReceiptDate = document.ReceiptDate,
                    CustomerName = document.CustomerName,
                    CustomerPhone = document.CustomerPhone ?? "",
                    CustomerAddress = $"{document.CustomerCountryCode} {document.CustomerGovernate} {document.CustomerCity} {document.CustomerStreet} {document.CustomerBuilding}".Trim(),
                    CustomerTaxId = document.CustomerTaxId ?? "",
                    Subtotal = document.Subtotal,
                    TaxAmount = document.TaxAmount,
                    TotalDiscount = document.TotalDiscount,
                    ExtraDiscount = document.ExtraDiscount,
                    TotalVAT = document.TotalVAT,
                    TotalAmount = document.TotalAmount,
                    PaymentMethod = document.PaymentMethod.ToString(),
                    Notes = document.Notes,
                    DeviceName = device?.Name ?? "غير محدد",
                    DeviceCode = device?.Code ?? "غير محدد",
                    UserName = user != null ? $"{user.FirstName} {user.LastName}" : "غير محدد",
                    BranchName = branch?.Name ?? "غير محدد",
                    CompanyName = company?.Name ?? "غير محدد",
                    Items = documentLines?.Select(item => new OrderItemDto
                    {
                        ProductName = item.Product?.Name ?? "غير محدد",
                        ProductCode = item.Product?.SKU ?? "",
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Discount = item.DiscountAmount,
                        TotalPrice = item.TotalPrice
                    }).ToList() ?? new List<OrderItemDto>()
                };

                return await Result<OrderDetailsDto>.SuccessAsync(orderDetails, "Order details retrieved successfully");
            }
            catch (Exception ex)
            {
                return await Result<OrderDetailsDto>.FailureAsync($"Error retrieving order details: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                var repo = _unitOfWork.Repository<User>();
                var users = await repo.GetAllAsync();

                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    IsActive = u.IsActive,
                    IsAdmin = u.IsAdmin
                });

                return await Result<IEnumerable<UserDto>>.SuccessAsync(userDtos, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<UserDto>>.FailureAsync($"Error retrieving users: {ex.Message}");
            }
        }

        public async Task<Result<int>> GetDocumentCountAsync()
        {
            try
            {
                var count = await _unitOfWork.Repository<Models.Document>().CountAsync();
                return await Result<int>.SuccessAsync(count, "Document count retrieved successfully");
            }
            catch (Exception ex)
            {
                return await Result<int>.FailureAsync($"Error retrieving document count: {ex.Message}");
            }
        }
    }
}
