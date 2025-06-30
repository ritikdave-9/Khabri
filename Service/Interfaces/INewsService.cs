using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Dtos;
using Data.Entity;

namespace Service.Interfaces
{
    public interface INewsService
    {
        Task<(IEnumerable<NewsResponseDto> Items, int TotalCount)> GetPagedNewsAsync(
                    int categoryId,
                    int pageNo = 1,
                    int pageSize = 10,
                    DateTime? startDate = null,
                    DateTime? endDate = null);
        Task SaveNewsForUserAsync(int userId, int newsId);
    }
}
