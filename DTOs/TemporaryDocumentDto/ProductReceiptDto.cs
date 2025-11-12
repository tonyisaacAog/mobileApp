namespace CompanyApi.DTOs.TemporaryDocumentDto
{
    public class ProductReceiptDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal minRange { get; set; }
        public decimal maxRange { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal VAT { get; set; }
        public string Notes { get; set; } = string.Empty;

    }
}
