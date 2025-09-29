using CompanyApi.Data;
using CompanyApi.DTOs;
using CompanyApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace CompanyApi.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private IQueryable<T> _query;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _query = _dbSet.AsQueryable();
        }

        public IRepository<T> AddIncludes(params string[] includes)
        {
            foreach (var include in includes)
            {
                _query = _query.Include(include);
            }
            return this;
        }

        private IQueryable<T> CurrentQuery => _query;

        private void ResetQuery()
        {
            _query = _dbSet.AsQueryable();
        }

        #region Basic CRUD
        public async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await CurrentQuery.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
            }
            finally { ResetQuery(); }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await CurrentQuery.ToListAsync();
            }
            finally { ResetQuery(); }
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await CurrentQuery.Where(predicate).ToListAsync();
            }
            finally { ResetQuery(); }
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await CurrentQuery.FirstOrDefaultAsync(predicate);
            }
            finally { ResetQuery(); }
        }

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
        #endregion

        #region Counting / Existence
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await CurrentQuery.CountAsync(predicate);
            }
            finally { ResetQuery(); }
        }

        public async Task<int> CountAsync()
        {
            try
            {
                return await CurrentQuery.CountAsync();
            }
            finally { ResetQuery(); }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await CurrentQuery.AnyAsync(predicate);
            }
            finally { ResetQuery(); }
        }
        #endregion

        #region Pagination
        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(PaginationParameters parameters)
        {
            try
            {
                var query = CurrentQuery;
                return await ApplyPaginationAsync(query, parameters);
            }
            finally { ResetQuery(); }
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(
            Expression<Func<T, bool>> predicate,
            PaginationParameters parameters)
        {
            try
            {
                var query = CurrentQuery.Where(predicate);
                return await ApplyPaginationAsync(query, parameters);
            }
            finally { ResetQuery(); }
        }
        #endregion

        #region Projection (DTO Support)
        public async Task<IEnumerable<TProjection>> GetProjectedAsync<TProjection>(
            Expression<Func<T, TProjection>> selector)
        {
            try
            {
                return await CurrentQuery.Select(selector).ToListAsync();
            }
            finally { ResetQuery(); }
        }

        public async Task<IEnumerable<TProjection>> GetProjectedAsync<TProjection>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TProjection>> selector)
        {
            try
            {
                var query = CurrentQuery.Where(predicate);
                return await query.Select(selector).ToListAsync();
            }
            finally { ResetQuery(); }
        }

        public async Task<TProjection?> GetByIdAsync<TProjection>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TProjection>> selector)
        {
            try
            {
                return await CurrentQuery.Where(predicate).Select(selector).FirstOrDefaultAsync();
            }
            finally { ResetQuery(); }
        }

        public async Task<(IEnumerable<TProjection> Items, int TotalCount)> GetProjectedPaginatedAsync<TProjection>(
            Expression<Func<T, TProjection>> selector,
            PaginationParameters parameters)
        {
            try
            {
                var query = CurrentQuery.Select(selector);
                return await ApplyPaginationAsync(query, parameters);
            }
            finally { ResetQuery(); }
        }

        public async Task<(IEnumerable<TProjection> Items, int TotalCount)> GetProjectedPaginatedAsync<TProjection>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TProjection>> selector,
            PaginationParameters parameters)
        {
            try
            {
                var query = CurrentQuery.Where(predicate).Select(selector);
                return await ApplyPaginationAsync(query, parameters);
            }
            finally { ResetQuery(); }
        }
        #endregion

        #region Helpers
        private async Task<(IEnumerable<T> Items, int TotalCount)> ApplyPaginationAsync(
            IQueryable<T> query,
            PaginationParameters parameters)
        {
            query = ApplySorting(query, parameters);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(parameters.Skip()).Take(parameters.PageSize).ToListAsync();
            return (items, totalCount);
        }

        private async Task<(IEnumerable<TProjection> Items, int TotalCount)> ApplyPaginationAsync<TProjection>(
            IQueryable<TProjection> query,
            PaginationParameters parameters)
        {
            var totalCount = await query.CountAsync();
            var items = await query.Skip(parameters.Skip()).Take(parameters.PageSize).ToListAsync();
            return (items, totalCount);
        }

        private static IQueryable<T> ApplySorting(IQueryable<T> query, PaginationParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
                return query;

            var propertyInfo = typeof(T).GetProperty(parameters.SortBy,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo == null) return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

            return parameters.SortDescending ? query.OrderByDescending(lambda) : query.OrderBy(lambda);
        }
        #endregion
    }

}