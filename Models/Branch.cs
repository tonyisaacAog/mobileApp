using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApi.Models
{
    public class Branch :BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Governate { get; set; } = string.Empty;
        public string RegionCity { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string BuildingNumber { get; set; } = string.Empty;
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only numbers are allowed.")]
        public int InitialInvoiceTaxSerial { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only numbers are allowed.")]
        public int InitialCreditTaxSerial { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only numbers are allowed.")]
        public int InitialDebitTaxSerial { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Document> Receipts { get; set; } = new List<Document>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
