using CompanyApi.Models;

namespace CompanyApi.DTOs.TemporaryDocumentDto
{
    public class TemporaryDocumentDto
    {
        public int Id { get; set; }

        public string ReceiptNumber { get; set; }
        public string DeviceSerial { get; set; }

        public DateTime ReceiptDate { get; set; }

        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ExtraDiscount { get; set; }
        public decimal TotalVAT { get; set; }

        public PaymentType PaymentMethod { get; set; }
        public DocumentType DocumentType { get; set; }

        public string Notes { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerTaxId { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerCountryCode { get; set; }
        public string? CustomerGovernate { get; set; }
        public string? CustomerCity { get; set; }
        public string? CustomerStreet { get; set; }
        public string? CustomerBuilding { get; set; }

        public string? ReferenceNumber { get; set; }
        public CustomerType CustomerType { get; set; }

        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public int? CompanyId { get; set; }
        public int? DeviceId { get; set; }

        public bool IsCoupon { get; set; }

        public int GroupId { get; set; }

        public List<TemporaryDocumentLineDto> DocumentLines { get; set; } = new();
    }
}
