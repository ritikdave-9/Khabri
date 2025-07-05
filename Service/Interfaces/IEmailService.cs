using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entity;

namespace Service.Interfaces
{
    public interface IEmailService
    {
        Task SendNewsNotificationAsync(User user, News news);
        Task SendNewsDigestAsync(User user, List<News> newsList);
    }
}
