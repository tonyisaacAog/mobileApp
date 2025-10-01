using CompanyApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApi.DTOs.DocumentDtos
{
    public class DocumentLinesDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal VAT { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
