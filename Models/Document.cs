using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApi.Models
{
    public class Document : BaseEntity
    {

        [Required]
        [StringLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal TotalDiscount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal ExtraDiscount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal TotalVAT { get; set; }

        [StringLength(20)]
        public PaymentType PaymentMethod { get; set; }
        public DocumentType DocumentType { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
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
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public int? CompanyId { get; set; }
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }

        public virtual ICollection<DocumentLines> ReceiptItems { get; set; } = new List<DocumentLines>();
    }
}
