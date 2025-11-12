using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CompanyApi.Models
{
    public class TemporaryDocumentLine: BaseEntity
    {
        public int ReceiptId { get; set; }

        public int ProductId { get; set; }
        [Column(TypeName = "decimal(18,5)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal NetTotal { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal TotalPrice { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal VAT { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("ReceiptId")]
        public virtual TemporaryDocument Receipt { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
