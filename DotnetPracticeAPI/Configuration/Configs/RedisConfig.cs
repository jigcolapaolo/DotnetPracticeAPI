using Application.Interfaces.Services;
using Application.Services.Background;
using Serilog;
using StackExchange.Redis;

namespace Configuration.Configs
{
    public static class RedisConfig
    {
        public static IServiceCollection AddRedisConfig(this IServiceCollection services, IConfiguration config)
        {
            try
            {
                var multiplexer = ConnectionMultiplexer.Connect(config.GetConnectionString("RedisConnection")!);

                if (multiplexer.IsConnected)
                {
                    services.AddSingleton<IConnectionMultiplexer>(multiplexer);
                    services.AddScoped<ICacheService, RedisCacheService>();
                }
                else
                {
                    Log.Warning("Redis no conectado. Se usará NullCacheService.");
                    services.AddScoped<ICacheService, NullCacheService>();
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error al conectar con Redis. Se usará NullCacheService.");
                services.AddScoped<ICacheService, NullCacheService>();
            }

            return services;
        }

    }
}
