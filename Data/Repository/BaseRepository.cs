using Common.Exceptions;
using Common.Utils;
using Data.Context;
using Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to add entity of type {typeof(T).Name}.", ex);
            }
        }
        public async Task<bool> AddAllAsync(List<T> entities)
        {
            try
            {
                await _context.Set<T>().AddRangeAsync(entities);
                var changes = await _context.SaveChangesAsync();
                return changes > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to add entities of type {typeof(T).Name}.", ex);
            }
        }


        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<T>().FindAsync(id);
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to get entity by ID for type {typeof(T).Name}.", ex);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            try
            {
                var query = ApplyIncludes(_context.Set<T>(), includes);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to get all entities for type {typeof(T).Name}.", ex);
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to update entity of type {typeof(T).Name}.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null)
                    return false;

                _context.Set<T>().Remove(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to delete entity of type {typeof(T).Name} with ID {id}.", ex);
            }
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                var query = ApplyIncludes(_context.Set<T>().Where(predicate), includes);
                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to find entity of type {typeof(T).Name}.", ex);
            }
        }

        public async Task<T> FindFirstAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                var query = ApplyIncludes(_context.Set<T>().Where(predicate), includes);
                return await query.FirstAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to find first entity of type {typeof(T).Name}.", ex);
            }
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                var query = ApplyIncludes(_context.Set<T>().Where(predicate), includes);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to find all entities of type {typeof(T).Name}.", ex);
            }
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> FindPageAsync(
            Expression<Func<T, bool>> predicate,
            int pageSize,
            int pageNo,
            List<Func<IQueryable<T>, IOrderedQueryable<T>>> orderBys = null,
            params Expression<Func<T, object>>[] includes)
        {
            try
            {
                var query = ApplyIncludes(_context.Set<T>().Where(predicate), includes);

                if (orderBys != null && orderBys.Count > 0)
                {
                    foreach (var orderBy in orderBys)
                        query = orderBy(query);
                }

                int totalCount = await query.CountAsync();

                var items = await query.Skip((pageNo - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();

                return (items, totalCount);
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to find paginated entities of type {typeof(T).Name}.", ex);
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _context.Set<T>().AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to check existence for type {typeof(T).Name}.", ex);
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                return predicate == null
                    ? await _context.Set<T>().CountAsync()
                    : await _context.Set<T>().CountAsync(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to count entities of type {typeof(T).Name}.", ex);
            }
        }
        public async Task<(IEnumerable<T> Items, int TotalCount)> SearchPageAsync(
    string searchTerm,
    int pageNo,
    int pageSize,
    params Expression<Func<T, string>>[] properties)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm) || properties == null || properties.Length == 0)
                    return (Enumerable.Empty<T>(), 0);

                var parameter = Expression.Parameter(typeof(T), "n");
                Expression predicateBody = null;

                var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
                    nameof(DbFunctionsExtensions.Like),
                    new[] { typeof(DbFunctions), typeof(string), typeof(string) }
                );
                if (likeMethod == null)
                    throw new InvalidOperationException("EF.Functions.Like method not found.");

                foreach (var property in properties)
                {
                    var propertyAccess = Expression.Invoke(property, parameter);

                    var likeCall = Expression.Call(
                        likeMethod,
                        Expression.Constant(EF.Functions),
                        propertyAccess,
                        Expression.Constant($"%{searchTerm}%")
                    );

                    predicateBody = predicateBody == null
                        ? (Expression)likeCall
                        : Expression.OrElse(predicateBody, likeCall);
                }

                var lambda = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);

                var query = _context.Set<T>().Where(lambda);

                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (items, totalCount);
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to search entities of type {typeof(T).Name}.", ex);
            }
        }
        public async Task<bool> UpdateAllAsync(IEnumerable<T> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    _context.Entry(entity).State = EntityState.Modified;
                }
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Repository Error] {ex.Message}");
                throw new RepositoryException($"Failed to update entities of type {typeof(T).Name}.", ex);
            }
        }



        private IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        {
            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return query;
        }
    }
}
