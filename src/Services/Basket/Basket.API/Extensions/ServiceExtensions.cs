using Basket.API.Repositories.Interfaces;
using Basket.API.Repositories;
using Infrastructure.Common;
using Contracts.Common.Interfaces;
using Shared.Configurations;
using MassTransit;
using Infrastructure.Extensions;
using EventBus.Messages.IntegrationEvents.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;


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

        //private static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var connectionString = configuration.GetSection("CacheSettings:ConnectionString").Value;
        //    if (string.IsNullOrEmpty(connectionString))
        //    {
        //        throw new ArgumentException("Redis Connection string is not configured!");
        //    }

        //    services.AddStackExchangeRedisCache(options =>
        //    {
        //        options.Configuration = connectionString;
        //    });
        //}

        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
            if (string.IsNullOrEmpty(settings.ConnectionString))
                throw new ArgumentNullException("Redis Connection string is not configured.");

            //Redis Configuration
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = settings.ConnectionString;
            });
        }

        public static void ConfigureMassTransit(this IServiceCollection services)
        {
            var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));
            if (settings == null || string.IsNullOrEmpty(settings.HostAddress) ||
                string.IsNullOrEmpty(settings.HostAddress)) throw new ArgumentNullException("EventBusSettings is not configured!");

            var mqConnection = new Uri(settings.HostAddress);

            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(mqConnection);
                });
                // Publish submit order message, instead of sending it to a specific queue directly.
                config.AddRequestClient<IBasketCheckoutEvent>();
            });
        }
    }
}