using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Data.Entity;
using Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using static System.Formats.Asn1.AsnWriter;

namespace Service
{
    public class NewsSourceBackgroundService : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;
    
        public NewsSourceBackgroundService(  HttpClient httpClient, IServiceProvider serviceProvider)
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
                    Console.WriteLine($"NewsFetching Background Service Failed: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
            }
        }

        private async Task ProcessAllNewsSourcesAsync( CancellationToken stoppingToken ,IServiceProvider serviceprovider)
        {

            var newsSources = await serviceprovider.GetRequiredService<IBaseRepository<NewsSource>>().GetAllAsync(
                source => source.NewsSourceToken,
                source => source.NewsSourceMappingField);

            foreach (var source in newsSources)
            {
                await ProcessNewsSourceAsync(source,serviceprovider);
            }
        }

        private async Task ProcessNewsSourceAsync(NewsSource source, IServiceProvider serviceprovider)
        {
            try
            {
                var apiUrl = BuildApiUrl(source);
                var jsonString = await FetchApiResponseAsync(apiUrl);
                using var doc = JsonDocument.Parse(jsonString);
                var newsRepo = serviceprovider.GetRequiredService<IBaseRepository<News>>();

                var articles = doc.RootElement.GetProperty(source.NewsSourceMappingField.NewsListKeyString);


                var newsList = new List<News>();

                foreach (var article in articles.EnumerateArray())
                {
                    var news = await MapArticleToNews(article, source,serviceprovider);
                    if (await newsRepo.FindAsync(n => n.Url == news.Url) == null)
                    {

                    newsList.Add(news);
                    }
                }

                if (!newsList.IsNullOrEmpty())
                {
                    await newsRepo.AddAllAsync(newsList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {source.Name}: {ex.Message}");
            }
        }

       

        private string BuildApiUrl(NewsSource source)
        {
            return $"{source.BaseURL}{source.NewsSourceToken.TokenKeyString}={source.NewsSourceToken.Token}";
        }

        private async Task<string> FetchApiResponseAsync(string apiUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            request.Headers.Add("User-Agent", "KhabriApp/1.0");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Cache-Control", "no-cache");
            

            Console.WriteLine($"Calling API: {apiUrl}");
            foreach (var header in request.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error {response.StatusCode}: {errorBody}");
                throw new HttpRequestException($"API returned {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<News> MapArticleToNews(JsonElement article, NewsSource source,IServiceProvider serviceProvider)
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

            var categoryKey = source.NewsSourceMappingField.Category;
            if (!string.IsNullOrWhiteSpace(categoryKey) && article.TryGetProperty(categoryKey, out var categoriesElement))
            {
                var categories = new List<Category>();
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

                foreach (var catName in categoryNames)
                {
                    var existing = await serviceProvider.GetRequiredService<IBaseRepository<Category>>().FindAsync(c => c.CategoryName == catName);
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
                if (categories.Count > 0)
                    news.Categories = categories;
            }

            var keywordsKey = source.NewsSourceMappingField.Keywords;
            if (!string.IsNullOrWhiteSpace(keywordsKey) && article.TryGetProperty(keywordsKey, out var keywordsElement))
            {
                var keywords = new List<Keyword>();
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

                foreach (var kwName in keywordNames)
                {
                    var existing = await serviceProvider.GetRequiredService<IBaseRepository<Keyword>>().FindAsync(k => k.KeywordText == kwName);
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
                if (keywords.Count > 0)
                    news.Keywords = keywords;
            }

            return news;
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

    
    }
}
