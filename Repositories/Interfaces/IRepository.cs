using CompanyApi.DTOs.ResponseDtos;
using System.Linq.Expressions;

namespace CompanyApi.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IRepository<T> AddIncludes(params string[] includes);
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<TProjection>> GetAllByConditionAsync<TProjection>(Expression<Func<T, bool>> predicate, Expression<Func<T, TProjection>> selector);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T,bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T,bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> AnyAsync(Expression<Func<T,bool>> predicate);
        Task<int> CountAsync(Expression<Func<T,bool>> predicate);
        Task<int> CountAsync();
        Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(PaginationParameters parameters);
        Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(Expression<Func<T,bool>> predicate,PaginationParameters parameters);
        Task<IEnumerable<TProjection>> GetProjectedAsync<TProjection>(Expression<Func<T,TProjection>> selector);
        Task<IEnumerable<TProjection>> GetProjectedAsync<TProjection>(Expression<Func<T,bool>> predicate,Expression<Func<T,TProjection>> selector);
        Task<TProjection?> GetByIdAsync<TProjection>(Expression<Func<T,bool>> predicate,Expression<Func<T,TProjection>> selector);
        Task<(IEnumerable<TProjection> Items, int TotalCount)> GetProjectedPaginatedAsync<TProjection>(Expression<Func<T,TProjection>> selector,PaginationParameters parameters);
        Task<(IEnumerable<TProjection> Items, int TotalCount)> GetProjectedPaginatedAsync<TProjection>(Expression<Func<T,bool>> predicate,Expression<Func<T,TProjection>> selector,PaginationParameters parameters);
        Task<decimal> SumAsync(Expression<Func<T,bool>> predicate,Expression<Func<T,decimal>> selector,CancellationToken cancellationToken = default);
    }
}
