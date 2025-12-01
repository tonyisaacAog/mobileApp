namespace CompanyApi.DTOs.DocumentDtos
{
    public class DocumentLinesDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal VAT { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
