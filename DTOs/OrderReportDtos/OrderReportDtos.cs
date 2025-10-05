namespace CompanyApi.DTOs.OrderReportDtos
{
    public class OrderReportFilterDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? DeviceCode { get; set; }
        public int? UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class OrderReportDto
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceCode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
    }

    public class OrderReportViewDto
    {
        public IEnumerable<OrderReportDto> Orders { get; set; } = new List<OrderReportDto>();
        public OrderReportFilterDto Filter { get; set; } = new OrderReportFilterDto();
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Filter.PageSize);
        public bool HasPreviousPage => Filter.PageNumber > 1;
        public bool HasNextPage => Filter.PageNumber < TotalPages;
    }

    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerTaxId { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal ExtraDiscount { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceCode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

    public class OrderItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
