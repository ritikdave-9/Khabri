using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsApi;

namespace NewsApi
{
    public class NewsApiBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NewsApiBackgroundService> _logger;

        private readonly TimeSpan _interval = TimeSpan.FromHours(4);

        public NewsApiBackgroundService(IServiceProvider serviceProvider, ILogger<NewsApiBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("News background service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var newsApiService = scope.ServiceProvider.GetRequiredService<INewsApiService>();

                        _logger.LogInformation("Fetching news at {Time}", DateTime.UtcNow);

                        await newsApiService.FetchAndSaveNewsAsync();

                        _logger.LogInformation("News fetched and saved successfully.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while fetching news.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("News background service stopping.");
        }
    }
}
