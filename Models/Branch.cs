namespace CompanyApi.Models
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Governate { get; set; } = string.Empty;
        public string RegionCity { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string BuildingNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        //public virtual ICollection<User> Users { get; set; } = [];
        public virtual ICollection<UserBranch> UserBranches { get; set; } = [];
        public virtual ICollection<Document> Receipts { get; set; } = [];
        public virtual ICollection<Product> Products { get; set; } = [];
    }
}
