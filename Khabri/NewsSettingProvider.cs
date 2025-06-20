using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NewsApi;
using Service;
using Service.Interfaces;
using TheNewsApi;

namespace Khabri
{
    public static class NewsSettingProvider
    {
        public static void NewsSettingsProvider(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NewsApiSettings>(configuration.GetSection("NewsApi"));
            services.Configure<TheNewsApiSettings>(configuration.GetSection("TheNewsApi"));


            services.AddHttpClient<INewsApiService, NewsApiService>();
            services.AddHttpClient<ITheNewsApiService, TheNewsApiService>();


            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<NewsApiSettings>>().Value);
            services.AddSingleton(sp =>
               sp.GetRequiredService<IOptions<TheNewsApiSettings>>().Value);

            //services.AddHostedService<NewsApiBackgroundService>();
            services.AddHostedService<TheNewsApiBackgroundService>();


        }

    }
}
