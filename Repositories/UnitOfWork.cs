using CompanyApi.Data;
using CompanyApi.Repositories.Interfaces;
using System.Collections.Concurrent;

namespace CompanyApi.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;

        // cache for repositories
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            // Lazy initialization + caching
            return (IRepository<T>)_repositories.GetOrAdd(
                typeof(T),
                _ => new Lazy<IRepository<T>>(() => new Repository<T>(_context)).Value
            );
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync() => await _context.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync() => await _context.Database.CommitTransactionAsync();

        public async Task RollbackTransactionAsync() => await _context.Database.RollbackTransactionAsync();

        public void Dispose() => _context.Dispose();
    }
}
