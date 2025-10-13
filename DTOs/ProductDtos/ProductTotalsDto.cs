namespace CompanyApi.DTOs.ProductDtos
{
    public class ProductTotalsDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalVAT { get; set; }
    }
}
