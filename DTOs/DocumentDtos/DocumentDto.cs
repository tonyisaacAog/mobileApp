using CompanyApi.Models;

namespace CompanyApi.DTOs.DocumentDtos
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; }
        public string DeviceSerial { get; set; }
        public DateTime ReceiptDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal ExtraDiscount { get; set; }
        public decimal TotalVAT { get; set; }
        public PaymentType PaymentMethod { get; set; }
        public DocumentType DocumentType { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerCode { get; set; } = string.Empty;
        public string? CustomerTaxId { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; } = string.Empty;
        public string? CustomerCountryCode { get; set; }
        public string? CustomerGovernate { get; set; }
        public string? CustomerCity { get; set; }
        public string? CustomerStreet { get; set; }
        public string? CustomerBuilding { get; set; }
        public string? ReferenceNumber { get; set; }
        public CustomerType CustomerType { get; set; }
        public string Notes { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
    }
}
