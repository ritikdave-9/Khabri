using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;
using Service;
using NewsApi;



namespace Services
    {
        public static class ServiceProvider
        {
            public static void ConfigureServices(this IServiceCollection services)
            {
                services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<INewsApiService, NewsApiService>();

        }
    }
    }


