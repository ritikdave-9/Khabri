using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TheNewsApi
{
    public class TheNewsApiBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TheNewsApiBackgroundService> _logger;

        private readonly TimeSpan _interval = TimeSpan.FromHours(4);

        public TheNewsApiBackgroundService(IServiceProvider serviceProvider, ILogger<TheNewsApiBackgroundService> logger)
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
                        var newsApiService = scope.ServiceProvider.GetRequiredService<ITheNewsApiService>();

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
