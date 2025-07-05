using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IBaseService<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<T> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T> FindFirstAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<(IEnumerable<T> Items, int TotalCount)> SearchPageAsync(
            string searchTerm,
            int pageNo,
            int pageSize,
            params Expression<Func<T, string>>[] properties);
        Task<(IEnumerable<T> Items, int TotalCount)> FindPageAsync(
            Expression<Func<T, bool>> predicate,
            int pageSize,
            int pageNo,
            List<Func<IQueryable<T>, IOrderedQueryable<T>>> orderBys = null,
            params Expression<Func<T, object>>[] includes);
    }

}
