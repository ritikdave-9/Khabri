using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Utils;
using Data.Entity;
using Data.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Service
{
    public class NewsSourceBackgroundService : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;
        private int pageCount = 0;

        public NewsSourceBackgroundService(HttpClient httpClient, IServiceProvider serviceProvider)
        {
            _httpClient = httpClient;
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
                        var scopedProvider = scope.ServiceProvider;
                        await ProcessAllNewsSourcesAsync(stoppingToken, scopedProvider);
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LogError($"NewsFetching Background Service Failed: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
            }
        }

        private string BuildApiUrl(NewsSource source)
        {
            return $"{source.BaseURL}{source.NewsSourceToken.TokenKeyString}={source.NewsSourceToken.Token}&page={pageCount}";
        }

        private async Task CreateNotificationIfNotExistsAsync(int userId, int newsId, IBaseRepository<Notification> notificationRepo)
        {
            var alreadyNotified = await notificationRepo.FindAsync(
                n => n.UserID == userId && n.NewsID == newsId
            );
            if (alreadyNotified == null)
            {
                var notification = new Notification
                {
                    UserID = userId,
                    NewsID = newsId,
                    CreatedAt = DateTime.UtcNow,
                    IsMailed = false,
                    IsSeen = false
                };
                await notificationRepo.AddAsync(notification);
            }
        }

        private async Task<string> FetchApiResponseAsync(string apiUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            request.Headers.Add("User-Agent", "KhabriApp/1.0");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Cache-Control", "no-cache");

            CustomLogger.LogInformation($"Calling API: {apiUrl}");
            foreach (var header in request.Headers)
            {
                CustomLogger.LogInformation($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                CustomLogger.LogInformation($"API Error {response.StatusCode}: {errorBody}");
                throw new HttpRequestException($"API returned {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        private string GetMappedValue(JsonElement element, string mappingKey)
        {
            if (string.IsNullOrWhiteSpace(mappingKey))
                return string.Empty;

            var keys = mappingKey.Split('.');
            JsonElement current = element;

            foreach (var key in keys)
            {
                if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(key, out var next))
                {
                    current = next;
                }
                else
                {
                    return string.Empty;
                }
            }

            return current.ValueKind switch
            {
                JsonValueKind.String => current.GetString(),
                JsonValueKind.Number => current.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => string.Empty
            };
        }

        private async Task<List<News>> GetNewNewsListAsync(JsonElement articles, NewsSource source, IServiceProvider serviceProvider, IBaseRepository<News> newsRepo)
        {
            var newsList = new List<News>();
            foreach (var article in articles.EnumerateArray())
            {
                var news = await MapArticleToNews(article, source, serviceProvider);
                if (await newsRepo.FindAsync(n => n.Url == news.Url) == null)
                {
                    newsList.Add(news);
                }
            }
            return newsList;
        }

        private async Task<List<int>> GetSubscribedUserIdsAsync(News news, IBaseRepository<UserSubscription> userSubscriptionRepo)
        {
            var categoryIds = news.Categories?.Select(c => c.CategoryID).ToList() ?? new List<int>();
            var keywordIds = news.Keywords?.Select(k => k.KeywordID).ToList() ?? new List<int>();

            var userSubscriptions = await userSubscriptionRepo.FindAllAsync(
                s => (s.CategoryID != null && categoryIds.Contains(s.CategoryID.Value)) ||
                     (s.KeywordID != null && keywordIds.Contains(s.KeywordID.Value))
            );

            return userSubscriptions.Select(s => s.UserID).Distinct().ToList();
        }

        private async Task<News> MapArticleToNews(JsonElement article, NewsSource source, IServiceProvider serviceProvider)
        {
            var news = new News
            {
                Title = GetMappedValue(article, source.NewsSourceMappingField.Title),
                Description = GetMappedValue(article, source.NewsSourceMappingField.Description),
                Url = GetMappedValue(article, source.NewsSourceMappingField.Url),
                ImageUrl = GetMappedValue(article, source.NewsSourceMappingField.ImageUrl),
                Content = GetMappedValue(article, source.NewsSourceMappingField.Content),
                Source = GetMappedValue(article, source.NewsSourceMappingField.Source),
                Author = GetMappedValue(article, source.NewsSourceMappingField.Author),
                PublishedAt = DateTime.Parse(GetMappedValue(article, source.NewsSourceMappingField.PublishedAt)),
                CreatedAt = DateTime.UtcNow,
                Language = GetMappedValue(article, source.NewsSourceMappingField.Language)
            };

            news.Categories = await MapCategories(article, source, serviceProvider);
            news.Keywords = await MapKeywords(article, source, serviceProvider);

            return news;
        }

        private async Task<List<Category>> MapCategories(JsonElement article, NewsSource source, IServiceProvider serviceProvider)
        {
            var categoryKey = source.NewsSourceMappingField.Category;
            var categories = new List<Category>();
            if (!string.IsNullOrWhiteSpace(categoryKey) && article.TryGetProperty(categoryKey, out var categoriesElement))
            {
                var categoryNames = new List<string>();

                if (categoriesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var cat in categoriesElement.EnumerateArray())
                        if (cat.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(cat.GetString()))
                            categoryNames.Add(cat.GetString());
                }
                else if (categoriesElement.ValueKind == JsonValueKind.String)
                {
                    var catString = categoriesElement.GetString();
                    if (!string.IsNullOrWhiteSpace(catString))
                        categoryNames.AddRange(catString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                }

                var categoryRepo = serviceProvider.GetRequiredService<IBaseRepository<Category>>();
                foreach (var catName in categoryNames)
                {
                    var existing = await categoryRepo.FindAsync(c => c.CategoryName == catName);
                    if (existing != null)
                    {
                        categories.Add(existing);
                    }
                    else
                    {
                        categories.Add(new Category
                        {
                            CategoryName = catName,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }
            return categories;
        }

        private async Task<List<Keyword>> MapKeywords(JsonElement article, NewsSource source, IServiceProvider serviceProvider)
        {
            var keywordsKey = source.NewsSourceMappingField.Keywords;
            var keywords = new List<Keyword>();
            if (!string.IsNullOrWhiteSpace(keywordsKey) && article.TryGetProperty(keywordsKey, out var keywordsElement))
            {
                var keywordNames = new List<string>();

                if (keywordsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var kw in keywordsElement.EnumerateArray())
                        if (kw.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(kw.GetString()))
                            keywordNames.Add(kw.GetString());
                }
                else if (keywordsElement.ValueKind == JsonValueKind.String)
                {
                    var kwString = keywordsElement.GetString();
                    if (!string.IsNullOrWhiteSpace(kwString))
                        keywordNames.AddRange(kwString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                }

                var keywordRepo = serviceProvider.GetRequiredService<IBaseRepository<Keyword>>();
                foreach (var kwName in keywordNames)
                {
                    var existing = await keywordRepo.FindAsync(k => k.KeywordText == kwName);
                    if (existing != null)
                    {
                        keywords.Add(existing);
                    }
                    else
                    {
                        keywords.Add(new Keyword
                        {
                            KeywordText = kwName,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }
            return keywords;
        }

        private async Task NotifySubscribedUsersAsync(
            List<News> newsList,
            IBaseRepository<UserSubscription> userSubscriptionRepo,
            IBaseRepository<Notification> notificationRepo)
        {
            foreach (var news in newsList)
            {
                var userIds = await GetSubscribedUserIdsAsync(news, userSubscriptionRepo);
                foreach (var userId in userIds)
                {
                    await CreateNotificationIfNotExistsAsync(userId, news.NewsID, notificationRepo);
                }
            }
        }

        private async Task ProcessAllNewsSourcesAsync(CancellationToken stoppingToken, IServiceProvider serviceProvider)
        {
            var newsSourceRepo = serviceProvider.GetRequiredService<IBaseRepository<NewsSource>>();
            var newsSources = await newsSourceRepo.FindAllAsync(ns => ns.Status == Common.Enums.NewsSourceStatus.Active,
                source => source.NewsSourceToken,
                source => source.NewsSourceMappingField);

            foreach (var source in newsSources)
            {
                for (int page = 0; page < 20; page++)
                {
                    try
                    {
                        await ProcessNewsSourceAsync(source, serviceProvider);
                    }
                    catch (Exception ex) {
                        CustomLogger.LogError($"Error processing {source.Name}: {ex.Message}");
                        break;
                    }

                }
            }
        }

        private async Task ProcessNewsSourceAsync(NewsSource source, IServiceProvider serviceProvider)
        {
            try
            {
                var apiUrl = BuildApiUrl(source);
                var jsonString = await FetchApiResponseAsync(apiUrl);
                using var doc = JsonDocument.Parse(jsonString);

                var newsRepo = serviceProvider.GetRequiredService<IBaseRepository<News>>();
                var userSubscriptionRepo = serviceProvider.GetRequiredService<IBaseRepository<UserSubscription>>();
                var notificationRepo = serviceProvider.GetRequiredService<IBaseRepository<Notification>>();

                var articles = doc.RootElement.GetProperty(source.NewsSourceMappingField.NewsListKeyString);

                var newsList = await GetNewNewsListAsync(articles, source, serviceProvider, newsRepo);

                if (newsList.Any())
                {
                    await SaveNewsListAsync(newsList, newsRepo);
                    await NotifySubscribedUsersAsync(newsList, userSubscriptionRepo, notificationRepo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task SaveNewsListAsync(List<News> newsList, IBaseRepository<News> newsRepo)
        {
            await newsRepo.AddAllAsync(newsList);
        }
    }
}
