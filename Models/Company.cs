using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApi.Models
{
    public class Company : BaseEntity
    {
        public string Name { get; set; }
        public string? TradeName { get; set; }
        public string TaxCode { get; set; }
        public string ActivityCode { get; set; }
        public string? ERPClientID { get; set; } = string.Empty;
        public string? ERPClientSecret { get; set; } = string.Empty;

        public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public virtual ICollection<Document> Receipts { get; set; } = new List<Document>();
    }
}
