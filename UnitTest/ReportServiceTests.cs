using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Exceptions;
using Data.Entity;
using Data.Repository.Interfaces;
using Moq;
using Service;
using Xunit;

namespace UnitTest
{
    public class ReportServiceTests
    {
        private readonly Mock<IBaseRepository<Report>> _reportRepoMock;
        private readonly Mock<IBaseRepository<News>> _newsRepoMock;
        private readonly ReportService _service;

        public ReportServiceTests()
        {
            _reportRepoMock = new Mock<IBaseRepository<Report>>();
            _newsRepoMock = new Mock<IBaseRepository<News>>();
            _service = new ReportService(_reportRepoMock.Object, _newsRepoMock.Object);
        }

        [Fact]
        public async Task ReportNewsAsync_Throws_WhenAlreadyReported()
        {
            _reportRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Report, bool>>>()))
                .ReturnsAsync(new Report());

            await Assert.ThrowsAsync<AlreadyExistsException>(() =>
                _service.ReportNewsAsync(1, 2, "reason"));
        }

        [Fact]
        public async Task ReportNewsAsync_AddsReport_WhenNotAlreadyReported()
        {
            _reportRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Report, bool>>>()))
                .ReturnsAsync((Report)null);
            _reportRepoMock.Setup(r => r.AddAsync(It.IsAny<Report>())).ReturnsAsync(new Report());
            _reportRepoMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Report, bool>>>()))
                .ReturnsAsync(1);

            var result = await _service.ReportNewsAsync(1, 2, "reason");

            Assert.True(result);
            _reportRepoMock.Verify(r => r.AddAsync(It.Is<Report>(rep => rep.ReporterID == 1 && rep.NewsID == 2 && rep.Reason == "reason")), Times.Once);
        }

        [Fact]
        public async Task ReportNewsAsync_DisablesNews_WhenReportCountExceeds5()
        {
            _reportRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Report, bool>>>()))
                .ReturnsAsync((Report)null);
            _reportRepoMock.Setup(r => r.AddAsync(It.IsAny<Report>())).ReturnsAsync(new Report());
            _reportRepoMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Report, bool>>>()))
                .ReturnsAsync(6);

            var news = new News { NewsID = 2, IsDisabled = false };
            _newsRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(news);
            _newsRepoMock.Setup(r => r.UpdateAsync(news)).ReturnsAsync(true);

            var result = await _service.ReportNewsAsync(1, 2, "reason");

            Assert.True(news.IsDisabled);
            _newsRepoMock.Verify(r => r.UpdateAsync(news), Times.Once);
        }

        [Fact]
        public async Task ReportNewsAsync_DoesNotDisableNews_IfAlreadyDisabled()
        {
            _reportRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Report, bool>>>()))
                .ReturnsAsync((Report)null);
            _reportRepoMock.Setup(r => r.AddAsync(It.IsAny<Report>())).ReturnsAsync(new Report());
            _reportRepoMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Report, bool>>>()))
                .ReturnsAsync(6);

            var news = new News { NewsID = 2, IsDisabled = true };
            _newsRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(news);

            var result = await _service.ReportNewsAsync(1, 2, "reason");

            _newsRepoMock.Verify(r => r.UpdateAsync(It.IsAny<News>()), Times.Never);
        }

        [Fact]
        public async Task GetAllReportsAsync_ReturnsReports()
        {
            var reports = new List<Report> { new Report { ReportID = 1 } };
            _reportRepoMock.Setup(r => r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Report, object>>>(),
                                                     It.IsAny<System.Linq.Expressions.Expression<Func<Report, object>>>()))
                .ReturnsAsync(reports);

            var result = await _service.GetAllReportsAsync();

            Assert.Single(result);
            Assert.Equal(1, result.First().ReportID);
        }
    }
}
