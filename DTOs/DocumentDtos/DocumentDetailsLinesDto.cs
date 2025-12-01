using System.Globalization;

namespace CompanyApi.DTOs.DocumentDtos
{
    public class DocumentDetailsLinesDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal VAT { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
