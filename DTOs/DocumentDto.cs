namespace CompanyApi.DTOs
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public int? CompanyId { get; set; }
    }
}
