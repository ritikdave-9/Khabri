using Microsoft.Extensions.DependencyInjection;
using Data.Repository.Interfaces;
using Data.Repository;


namespace Rido.Data
{
    public static class RepositoryProvider
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<INewsRepository, NewsRepository>();
        }
    }
}
