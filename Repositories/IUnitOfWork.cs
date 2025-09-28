using CompanyApi.Models;

namespace CompanyApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Branch> Branches { get; }
        IRepository<Company> Companies { get; }
        IRepository<Product> Products { get; }
        IRepository<Document> Receipts { get; }
        IRepository<DocumentLines> ReceiptItems { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
