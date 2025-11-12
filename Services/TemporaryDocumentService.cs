using AutoMapper;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.TemporaryDocumentDto;
using CompanyApi.Models;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Services.Interfaces;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace CompanyApi.Services
{
    public class TemporaryDocumentService : ITemporaryDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TemporaryDocumentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<bool>> ApproveReceiptsAsync(ApproveTempDocumentDto request)
        {
            try
            {
                if (request.temporaryReceiptsIds == null || request.temporaryReceiptsIds.Count == 0)
                {
                    return await Result<bool>.FailureAsync("No receipts selected for Approval");
                }

                var tempReceipts = await _unitOfWork.Repository<TemporaryDocument>()
                                            .AddIncludes("DocumentLines")
                    .GetAllByConditionAsync(x => request.temporaryReceiptsIds.Contains(x.Id), x => new TemporaryDocument
                    {
                        ReceiptNumber = x.ReceiptNumber,
                        ReceiptDate = DateTime.Now,
                        DeviceSerial = x.DeviceSerial, // Needed
                        DeviceId = x.DeviceId, // Needed
                        CustomerName = x.CustomerName, //Needed
                        DocumentType = DocumentType.SR,
                        TotalDiscount = x.TotalDiscount,
                        TotalVAT = x.TotalVAT,
                        Subtotal = x.Subtotal,
                        TotalAmount = x.TotalAmount,
                        PaymentMethod = PaymentType.Cash,
                        Notes = x.Notes,
                        DocumentLines = x.DocumentLines.Select(l => new TemporaryDocumentLine
                        {
                            ProductId = l.ProductId,
                            Quantity = l.Quantity,
                            UnitPrice = l.UnitPrice, // replace with actual price
                            TotalPrice = l.TotalPrice,
                            NetTotal = l.NetTotal,
                            DiscountAmount = l.DiscountAmount,
                            VAT = l.VAT,
                            Notes = l.Notes
                        }).ToList()
                    });

                if (!tempReceipts.Any())
                    return await Result<bool>.FailureAsync("No temporary receipts found for the provided IDs.");

                var approvedReceipts = new List<Models.Document>();

                foreach(var temp in tempReceipts)
                {
                    var approveDocItem = new Models.Document
                    {
                        ReceiptNumber = temp.ReceiptNumber,
                        ReceiptDate = DateTime.Now,
                        DeviceSerial = temp.DeviceSerial, // Needed
                        DeviceId = temp.DeviceId, // Needed
                        CustomerName = temp.CustomerName, //Needed
                        DocumentType = DocumentType.SR,
                        TotalDiscount = temp.TotalDiscount,
                        TotalVAT = temp.TotalVAT,
                        Subtotal = temp.Subtotal,
                        TotalAmount = temp.TotalAmount,
                        PaymentMethod = PaymentType.Cash,
                        Notes =temp.Notes,
                        ReceiptItems = temp.DocumentLines.Select(dl => new DocumentLines
                        {
                            ProductId = dl.ProductId,
                            Quantity = dl.Quantity,
                            UnitPrice = dl.UnitPrice, // replace with actual price
                            TotalPrice = dl.TotalPrice,
                            NetTotal = dl.NetTotal,
                            DiscountAmount = dl.DiscountAmount,
                            VAT = dl.VAT,
                            Notes = dl.Notes
                        }).ToList()
                    };
                    approvedReceipts.Add(approveDocItem);
                }

                await _unitOfWork.Repository<Models.Document>().AddRangeAsync(approvedReceipts);
                await _unitOfWork.SaveChangesAsync();

                return await Result<bool>.SuccessAsync(true);

            }
            catch (Exception ex)
            {
                return await Result<bool>.FailureAsync(ex.Message, 400);
            }
        }

        public async Task<Result<List<TemporaryDocument>>> GenerateReceiptsAsync(ReceiptGenerationDto generationDto)
        {
            const decimal VAT_RATE = 0.14m;
            try
            {
                if(generationDto.NumberOfReceipts <= 0)
                {
                    return await Result<List<TemporaryDocument>>.FailureAsync("Number of receipts must be greater than 0.");
                }

                if(generationDto.ProductReceiptDtos == null || generationDto.ProductReceiptDtos.Count == 0)
                {
                    return await Result<List<TemporaryDocument>>.FailureAsync("At least one product must be provided");
                }

                decimal totalQuantities = generationDto.ProductReceiptDtos.Sum(p => p.TotalQuantity);

                if(totalQuantities <= 0)
                {
                    return await Result<List<TemporaryDocument>>.FailureAsync("Total Quantity must be greater than 0.");
                }

                var random = new Random();
                var allReceipts = new List<TemporaryDocument>();

                var allocationsProductWithNoReceipts = new List<(ProductReceiptDto product, int count)>();
                int remainingReceipts = generationDto.NumberOfReceipts;

                foreach(var p in generationDto.ProductReceiptDtos)
                {
                    decimal weight = p.TotalQuantity / totalQuantities;
                    int NumberOfProductReceipts = (int) Math.Floor( weight * generationDto.NumberOfReceipts);
                    allocationsProductWithNoReceipts.Add((p, NumberOfProductReceipts));
                    remainingReceipts -= NumberOfProductReceipts;
                }

                while(remainingReceipts > 0)
                {
                    var biggest = allocationsProductWithNoReceipts.OrderBy(t => t.product.TotalQuantity).First();
                    allocationsProductWithNoReceipts.Remove(biggest);
                    allocationsProductWithNoReceipts.Add((biggest.product, biggest.count + 1));
                    remainingReceipts -= 1;
                }

                foreach (var alloc in allocationsProductWithNoReceipts)
                {
                    var product = alloc.product;
                    var receiptsCount = alloc.count;

                    decimal minQuantityCalculated = receiptsCount * product.minRange;
                    decimal maxQuantityCalculated = receiptsCount * product.maxRange;

                    if(product.TotalQuantity <  minQuantityCalculated || product.TotalQuantity > maxQuantityCalculated)
                    {
                        return await Result<List<TemporaryDocument>>.FailureAsync(
                            $"Cannot generate Receipts for Product {product.ProductName}" +
                            $"Total Quantity {product.TotalQuantity} cannot fit into {receiptsCount} receipts" +
                            $"with minRange = {product.minRange} and maxRange = {product.maxRange}"
                            );
                    }

                }

                foreach(var alloc in  allocationsProductWithNoReceipts)
                {
                    var productEntity = await _unitOfWork.Repository<Product>().GetByIdAsync(alloc.product.ProductId);
                    var productAlloc = alloc.product;

                    var splittedQuantities = SplitTotalQuantitiesWithinRange(productAlloc.TotalQuantity, alloc.count, productAlloc.minRange, productAlloc.maxRange, random);



                    foreach(var qty in  splittedQuantities)
                    {

                        decimal subtotal = 0, totalDiscount = 0, totalVAT = 0;

                        
                        var lineTotal = qty * productAlloc.UnitPrice;
                        var lineNet = lineTotal - productAlloc.DiscountAmount;
                        var lineVAT = productAlloc.VAT > 0 ? lineNet * VAT_RATE : 0;

                        subtotal += lineTotal;
                        totalDiscount += productAlloc.DiscountAmount;
                        totalVAT += lineVAT;
                        

                        totalDiscount += generationDto.ExtraDiscount;


                        var receipt = new TemporaryDocument
                        {
                            ReceiptNumber = $"TMP-{Guid.NewGuid().ToString("N").Substring(0, 10)}",
                            ReceiptDate = DateTime.Now,
                            DeviceSerial = generationDto.DeviceSerial, // Needed
                            DeviceId = generationDto.DeviceId, // Needed
                            CustomerName = generationDto.CustomerName, //Needed
                            DocumentType = DocumentType.SR,
                            TotalDiscount = totalDiscount,
                            TotalVAT = totalVAT,
                            Subtotal = subtotal,
                            TotalAmount = subtotal - totalDiscount + totalVAT,
                            PaymentMethod = PaymentType.Cash,
                            Notes = $"Auto-generated receipt for {productAlloc.ProductName}",
                            DocumentLines = new List<TemporaryDocumentLine> 
                            {
                                new TemporaryDocumentLine
                                {
                                    ProductId = productAlloc.ProductId,
                                    Quantity = qty,
                                    UnitPrice = productAlloc.UnitPrice, // replace with actual price
                                    TotalPrice = lineTotal,
                                    NetTotal = lineNet,
                                    DiscountAmount = productAlloc.DiscountAmount,
                                    VAT = lineVAT,
                                    Notes = productAlloc.Notes
                                }
                            }
                        };

                        allReceipts.Add(receipt);   
                    }

                }

                allReceipts = allReceipts.OrderBy(_ => random.Next()).ToList();

                await _unitOfWork.Repository<TemporaryDocument>().AddRangeAsync(allReceipts);
                await _unitOfWork.SaveChangesAsync();

                return await Result<List<TemporaryDocument>>.SuccessAsync(allReceipts);


            }
            catch (Exception ex)
            {
                return await Result<List<TemporaryDocument>>.FailureAsync(ex.Message, 400);
            }
        }

        private List<decimal> SplitTotalQuantitiesWithinRange(decimal total, int count, decimal min, decimal max, Random random)
        {
            var results = new List<decimal>();
            decimal remaining = total;
            for (int i = 0; i < count; i++)
            {
                int left = count - i - 1;
                decimal minNeeded = left * min;
                decimal maxNeeded = left * max;

                decimal minAllowed = Math.Min(min, remaining - minNeeded);
                decimal maxAllowed = Math.Max(max, remaining - maxNeeded);

                if (minAllowed > maxAllowed)
                    throw new Exception("Cannot satisfy range constraints during quantity split.");

                decimal qty = Math.Round(
                    (decimal)(random.NextDouble() * (double)(maxAllowed - minAllowed) + (double)minAllowed), 2);

                results.Add(qty);
                remaining -= qty;
            }

            decimal diff = total - results.Sum();
            if (Math.Abs(diff) > 0.01m)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (diff == 0) break;

                    decimal spaceAvailable = max - results[i];
                    decimal adjustment = Math.Min(spaceAvailable, diff);
                    results[i] += adjustment;
                    diff -= adjustment;
                }

                // If there is still a tiny diff (due to rounding), adjust the last element
                if (Math.Abs(diff) > 0.001m)
                {
                    results[^1] += diff;
                }
            }

            return results;

        }

    }
}
