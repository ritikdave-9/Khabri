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
using Data.Repository.Interfaces;
using Microsoft.Extensions.Options;


namespace TheNewsApi
{
    public class TheNewsApiService : ITheNewsApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly TheNewsApiSettings _settings;
        private IBaseRepository<News> _newsRepo;
        private IBaseRepository<Keyword> _keywordRepo;
        private IBaseRepository<NewsKeyword> _newsKeywordRepo;
        private IBaseRepository<Category> _categoryRepo;
        private IBaseRepository<NewsCategory> _newsCategoryRepo;




        public TheNewsApiService(HttpClient httpClient,
    IMapper mapper,
    TheNewsApiSettings settings,
    IBaseRepository<News> newsRepo,
    IBaseRepository<Keyword> keywordRepo,
    IBaseRepository<NewsKeyword> newsKeywordRepo,
    IBaseRepository<Category> categoryRepo,
    IBaseRepository<NewsCategory> newsCategoryRepo
            )
        {
            _httpClient = httpClient;
            _newsRepo = newsRepo;
            _mapper = mapper;
            _settings = settings;
            _keywordRepo = keywordRepo;
            _newsKeywordRepo = newsKeywordRepo;
            _categoryRepo = categoryRepo;
            _newsCategoryRepo = newsCategoryRepo;
        }

        public async Task FetchAndSaveNewsAsync()
        {
            string url = $"{_settings.Endpoint}?api_token={_settings.ApiKey}&language=en&limit=3";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "KhabriApp/1.0");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"News API request failed with status code: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var newsApiResponse = JsonSerializer.Deserialize<TheNewsApiResponse>(json, options);



            if (newsApiResponse?.data == null || !newsApiResponse.data.Any())
                return;

            List<Keyword> keywordEntityList = new List<Keyword>();
            var newsEntities = new List<News>();
            List<NewsKeyword> NewskeywordEntityList = new List<NewsKeyword>();
            List<Category> CategoryEntityList = new List<Category>();
            List<NewsCategory> NewsCategoryEntityList = new List<NewsCategory>();



            foreach (var news in newsApiResponse.data)
            {
                var newsEntity = _mapper.Map<News>(news);
                newsEntity.NewsID = Guid.NewGuid();


                if (!string.IsNullOrWhiteSpace(news.Keywords))
                {
                    var keywordTexts = news.Keywords
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Distinct(StringComparer.OrdinalIgnoreCase);

                    foreach (var keyword in keywordTexts)
                    {
                        var trimmedKeyword = keyword.Trim();

                        var existingKeywordInDb = await _keywordRepo.FindAsync(k => k.KeywordText == trimmedKeyword);

                        var existingKeywordInList = keywordEntityList.FirstOrDefault(k => k.KeywordText == trimmedKeyword);

                        Keyword finalKeyword;

                        if (existingKeywordInDb == null && existingKeywordInList == null)
                        {
                            finalKeyword = new Keyword
                            {
                                KeywordID = Guid.NewGuid(),
                                KeywordText = trimmedKeyword,
                                CreatedAt = DateTime.UtcNow
                            };

                            keywordEntityList.Add(finalKeyword); 
                        }
                        else
                        {
                            finalKeyword = existingKeywordInList ?? existingKeywordInDb;
                        }

                        NewskeywordEntityList.Add(new NewsKeyword
                        {
                            NewsID = newsEntity.NewsID,
                            KeywordID = finalKeyword.KeywordID,
                            CreatedAt = DateTime.UtcNow,
                            News = newsEntity,
                            Keyword = finalKeyword
                        });
                    }


                }

                if (news.Categories != null && news.Categories.Count > 0) 
                {
                    foreach (var categoryName in news.Categories)
                    {
                        var trimmedCategory = categoryName.Trim();

                        var existingCategoryInDb = await _categoryRepo.FindAsync(c => c.CategoryName == trimmedCategory);

                        var existingCategoryInList = CategoryEntityList.FirstOrDefault(c => c.CategoryName == trimmedCategory);

                        Category finalCategory;

                        if (existingCategoryInDb == null && existingCategoryInList == null)
                        {
                            finalCategory = new Category
                            {
                                CategoryID = Guid.NewGuid(),
                                CategoryName = trimmedCategory,
                                CreatedAt = DateTime.UtcNow,
                                Slug = trimmedCategory.ToLower().Replace(" ", "-") 
                            };

                            CategoryEntityList.Add(finalCategory); 
                        }
                        else
                        {
                            finalCategory = existingCategoryInList ?? existingCategoryInDb;
                        }

                        NewsCategoryEntityList.Add(new NewsCategory
                        {
                            NewsID = newsEntity.NewsID,
                            CategoryID = finalCategory.CategoryID,
                            CreatedAt = DateTime.UtcNow,
                            News = newsEntity,
                            Category = finalCategory
                        });
                    }
                }


            }

            //await _newsRepo.AddAllAsync(newsEntities);
            //Thread.Sleep(5000);
            //await _keywordRepo.AddAllAsync(keywordEntityList);
            //Thread.Sleep(5000);

            await _newsKeywordRepo.AddAllAsync(NewskeywordEntityList);
            //Thread.Sleep(5000);

            await _newsCategoryRepo.AddAllAsync(NewsCategoryEntityList);
        }
        }
}
