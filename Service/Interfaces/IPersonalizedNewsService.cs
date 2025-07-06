using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entity;

public interface IPersonalizedNewsService
{
    Task<(IEnumerable<News> Items, int TotalCount)> GetPersonalizedNewsAsync(int userId, int pageNo, int pageSize);
}
