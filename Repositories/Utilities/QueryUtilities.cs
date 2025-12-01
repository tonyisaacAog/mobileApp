using System.Linq.Expressions;
using System.Reflection;

namespace CompanyApi.Repositories.Utilities
{
    public static class MappingUtilities
    {
        /// <summary>
        /// Creates an expression to map from TEntity to TDto.
        /// By default includes all matching properties, unless excluded.
        /// </summary>
        public static Expression<Func<TEntity, TDto>> CreateMapExpression<TEntity, TDto>(
            params string[] excludeProperties)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var bindings = new List<MemberBinding>();

            var dtoProperties = typeof(TDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var dtoProp in dtoProperties)
            {
                if (excludeProperties != null &&
                    excludeProperties.Any(p => string.Equals(p, dtoProp.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var entityProp = typeof(TEntity).GetProperty(dtoProp.Name,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (entityProp != null)
                {
                    var propertyAccess = Expression.Property(parameter, entityProp);
                    var binding = Expression.Bind(dtoProp, propertyAccess);
                    bindings.Add(binding);
                }
            }

            if (!bindings.Any())
                throw new ArgumentException(
                    $"No valid properties found to map from {typeof(TEntity).Name} to {typeof(TDto).Name}");

            var body = Expression.MemberInit(Expression.New(typeof(TDto)), bindings);
            return Expression.Lambda<Func<TEntity, TDto>>(body, parameter);
        }

        /// <summary>
        /// Apply sorting dynamically by property name.
        /// </summary>
        public static IQueryable<TEntity> ApplySorting<TEntity>(
            this IQueryable<TEntity> query,
            string sortBy,
            bool ascending = true)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = typeof(TEntity).GetProperty(sortBy,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                throw new ArgumentException($"Property '{sortBy}' not found on type '{typeof(TEntity).Name}'");

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            string methodName = ascending ? "OrderBy" : "OrderByDescending";

            var resultExp = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(TEntity), property.PropertyType },
                query.Expression,
                Expression.Quote(orderByExp));

            return query.Provider.CreateQuery<TEntity>(resultExp);
        }

        /// <summary>
        /// Apply filtering dynamically by property + value.
        /// </summary>
        public static IQueryable<TEntity> ApplyFiltering<TEntity>(
            this IQueryable<TEntity> query,
            Dictionary<string, object> filters)
        {
            if (filters == null || filters.Count == 0)
                return query;

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression? combined = null;

            foreach (var filter in filters)
            {
                var property = typeof(TEntity).GetProperty(filter.Key,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property == null) continue;

                var left = Expression.Property(parameter, property);
                var right = Expression.Constant(Convert.ChangeType(filter.Value, property.PropertyType));

                var equal = Expression.Equal(left, right);
                combined = combined == null ? equal : Expression.AndAlso(combined, equal);
            }

            if (combined == null) return query;

            var lambda = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
            return query.Where(lambda);
        }
    }
}
