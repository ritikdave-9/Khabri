using Microsoft.Extensions.DependencyInjection;
using Service;
using Service.Interfaces;



namespace Services
{
    public static class ServiceProvider
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INewsService, NewsService>();


        }
    }
}


