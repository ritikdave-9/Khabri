using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Utils;
using Data.Entity;
using Data.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Interfaces;

namespace Service
{
    public class MailBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public MailBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notificationRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<Notification>>();
                        var userRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();
                        var newsRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<News>>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                        var notifications = (await notificationRepo.FindAllAsync(n => !n.IsMailed)).ToList();

                        var notificationsByUser = notifications
                            .Where(n => n.NewsID.HasValue)
                            .GroupBy(n => n.UserID);

                        foreach (var userGroup in notificationsByUser)
                        {
                            var userId = userGroup.Key;
                            var user = await userRepo.GetByIdAsync(userId);
                            if (user == null) continue;

                            var newsList = new List<News>();
                            foreach (var notification in userGroup)
                            {
                                var news = await newsRepo.GetByIdAsync(notification.NewsID.Value);
                                if (news != null)
                                    newsList.Add(news);
                            }

                            if (newsList.Any())
                            {
                                try
                                {
                                    await emailService.SendNewsDigestAsync(user, newsList);

                                    foreach (var notification in userGroup)
                                    {
                                        notification.IsMailed = true;
                                        await notificationRepo.UpdateAsync(notification);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError($"Failed to send digest mail to {user.Email}: {ex.Message}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"MailBackgroundService error: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
            }
        }
    }
}
