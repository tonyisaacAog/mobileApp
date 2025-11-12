namespace CompanyApi.Models
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? TradeName { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Document> Receipts { get; set; } = new List<Document>();
        public virtual ICollection<TemporaryDocument> TemporaryDocuments { get; set; } = new List<TemporaryDocument>();
    }
}
