using CompanyApi.Models;

namespace CompanyApi.DTOs.DocumentDtos
{
    public class CreateDocumentDto
    {
        public DateTime ReceiptDate { get; set; }
        public decimal ExtraDiscount { get; set; }
        public PaymentType PaymentMethod { get; set; }
        public DocumentType DocumentType { get; set; }
        public string CustomerName { get; set; }
        public string DeviceSerial { get; set; }
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
        public string? DeviceCode { get; set; }
        public bool IsCoupon { get; set; } = false;
        public List<DocumentLinesDto> Items { get; set; } = new List<DocumentLinesDto>();

    }
}
