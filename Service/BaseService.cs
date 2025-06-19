using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;

namespace Service
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMapper _mapper;

        public BaseService(IServiceProvider serviceProvider)
        {
            _repository = serviceProvider.GetRequiredService<IBaseRepository<T>>();
            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
        }

        protected string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        protected string GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        }

        protected string GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        }
        public async Task<T> AddAsync(T entity)
        {
            return await _repository.AddAsync(entity);
        }

        public async Task<T> GetByIdAsync(string id, params Expression<Func<T, object>>[] includes)
        {
            return await _repository.FindAsync(e => EF.Property<string>(e, "Id") == id, includes);
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            return await _repository.GetAllAsync(includes);
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return await _repository.FindAsync(predicate, includes);
        }

        public async Task<T> FindFirstAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return await _repository.FindFirstAsync(predicate, includes);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return await _repository.FindAllAsync(predicate, includes);
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> FindPageAsync(
            Expression<Func<T, bool>> predicate,
            int pageSize,
            int pageNo,
            List<Func<IQueryable<T>, IOrderedQueryable<T>>> orderBys = null,
            params Expression<Func<T, object>>[] includes)
        {
            return await _repository.FindPageAsync(predicate, pageSize, pageNo, orderBys, includes);
        }
    }
}
