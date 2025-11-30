namespace CompanyApi.Models
{
    public class Group:BaseEntity
    {
        public ICollection<TemporaryDocument> TemporaryDocuments { get; set; }
    }
}
