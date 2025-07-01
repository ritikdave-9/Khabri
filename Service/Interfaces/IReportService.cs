using Data.Entity;

namespace Service.Interfaces
{
    public interface IReportService
    {
        Task<bool> ReportNewsAsync(int reporterId, int newsId, string reason);
        Task<IEnumerable<Report>> GetAllReportsAsync();
    }
}
