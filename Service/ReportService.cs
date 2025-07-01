using Data.Entity;
using Data.Repository.Interfaces;
using Service.Interfaces;

namespace Service
{
    public class ReportService : IReportService
    {
        private readonly IBaseRepository<Report> _reportRepo;
        private readonly IBaseRepository<News> _newsRepo;

        public ReportService(IBaseRepository<Report> reportRepo, IBaseRepository<News> newsRepo)
        {
            _reportRepo = reportRepo;
            _newsRepo = newsRepo;
        }

        public async Task<bool> ReportNewsAsync(int reporterId, int newsId, string reason)
        {
            var existing = await _reportRepo.FindAsync(r => r.ReporterID == reporterId && r.NewsID == newsId);
            if (existing != null)
                throw new InvalidOperationException("You have already reported this news.");

            var report = new Report
            {
                ReporterID = reporterId,
                NewsID = newsId,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };
            await _reportRepo.AddAsync(report);

            var reportCount = await _reportRepo.CountAsync(r => r.NewsID == newsId);

            if (reportCount > 5)
            {
                var news = await _newsRepo.GetByIdAsync(newsId);
                if (news != null && !news.IsDisabled)
                {
                    news.IsDisabled = true;
                    await _newsRepo.UpdateAsync(news);
                }
            }

            return true;
        }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await _reportRepo.GetAllAsync(r => r.Reporter, r => r.News);
        }
    }
}
