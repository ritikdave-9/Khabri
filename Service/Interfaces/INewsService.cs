using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity;

namespace Service.Interfaces
{
    public interface INewsService
    {
        Task<(IEnumerable<News> Items, int TotalCount)> GetPagedNewsAsync(
                    int pageNo = 1,
                    int pageSize = 10,
                    string categoryId = "",
                    DateTime? startDate = null,
                    DateTime? endDate = null);
    }
}
