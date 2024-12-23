using Basket.API.Repositories.Interfaces;
using Basket.API.Repositories;
using Infrastructure.Common;
using Contracts.Common.Interfaces;

namespace Basket.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddConfigurationSettings(configuration);
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.ConfigureRedis(configuration);
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.ConfigureServices();
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
            services.AddScoped<IBasketRepository, BasketRepository>()
               .AddTransient<ISerializeService, SerializeService>();
        //private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var cacheSettings = configuration.GetSection("CacheSettings:ConnectionString").Value;
        //    services.AddSingleton(cacheSettings);
        //}

        private static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("CacheSettings:ConnectionString").Value;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Redis Connection string is not configured!");
            }

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
            });
        }
    }
}