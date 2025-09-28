using CompanyApi.Data;
using CompanyApi.Models;

namespace CompanyApi.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IRepository<User>? _users;
        private IRepository<Branch>? _branches;
        private IRepository<Company>? _companies;
        private IRepository<Product>? _products;
        private IRepository<Document>? _receipts;
        private IRepository<DocumentLines>? _receiptItems;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<User> Users => _users ??= new Repository<User>(_context);
        public IRepository<Branch> Branches => _branches ??= new Repository<Branch>(_context);
        public IRepository<Company> Companies => _companies ??= new Repository<Company>(_context);
        public IRepository<Product> Products => _products ??= new Repository<Product>(_context);
        public IRepository<Document> Receipts => _receipts ??= new Repository<Document>(_context);
        public IRepository<DocumentLines> ReceiptItems => _receiptItems ??= new Repository<DocumentLines>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
