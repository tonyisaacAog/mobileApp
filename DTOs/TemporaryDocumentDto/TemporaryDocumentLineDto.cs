namespace CompanyApi.DTOs.TemporaryDocumentDto
{
    public class TemporaryDocumentLineDto
    {
        public int Id { get; set; }

        public int ReceiptId { get; set; }
        public int ProductId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetTotal { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal VAT { get; set; }

        public string Notes { get; set; }

        // Optional: product basic info if needed
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
    }
}
