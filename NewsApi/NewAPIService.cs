using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Data.Context;
using Data.Entity;
using Microsoft.Extensions.Options;


namespace NewsApi
{
    public class NewsApiService : INewsApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly NewsApiSettings _settings;

        public NewsApiService(HttpClient httpClient, AppDbContext dbContext, IMapper mapper, NewsApiSettings settings)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _mapper = mapper;
            _settings = settings;
        }

        public async Task FetchAndSaveNewsAsync()
        {
            string url = $"{_settings.Endpoint}?q=the&apiKey={_settings.ApiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "KhabriApp/1.0");
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"News API request failed with status code: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var newsApiResponse = JsonSerializer.Deserialize<NewsApiResponse>(json, options);

            if (newsApiResponse?.Articles == null || !newsApiResponse.Articles.Any())
                return;

            var newsEntities = _mapper.Map<List<News>>(newsApiResponse.Articles);

            await _dbContext.News.AddRangeAsync(newsEntities);
            await _dbContext.SaveChangesAsync();
        }
    }
}
