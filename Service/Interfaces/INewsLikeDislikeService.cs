using System.Threading.Tasks;
using Data.Entity;

namespace Service.Interfaces
{
    public interface INewsLikeDislikeService
    {
        Task LikeAsync(int userId, int newsId);
        Task DislikeAsync(int userId, int newsId);
        Task RemoveReactionAsync(int userId, int newsId);
        Task<int> GetLikesCountAsync(int newsId);
        Task<int> GetDislikesCountAsync(int newsId);
        Task<NewsLikeDislike> GetUserReactionAsync(int userId, int newsId);
    }
}
