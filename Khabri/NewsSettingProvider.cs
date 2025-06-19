using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NewsApi;
using Service;
using Service.Interfaces;

namespace Khabri
{
    public static class NewsSettingProvider
    {
        public static void NewsSettingsProvider(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NewsApiSettings>(configuration.GetSection("NewsApi"));

            services.AddHttpClient<INewsApiService, NewsApiService>();

            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<NewsApiSettings>>().Value);
        }

    }
}
