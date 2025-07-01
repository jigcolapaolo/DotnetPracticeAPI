using Application.Interfaces.Services;
using Application.Services.Background;
using Serilog;
using StackExchange.Redis;

namespace Configuration.Configs
{
    public static class RedisConfig
    {
        public static ConfigurationOptions GetRedisProductionOptions(IConfiguration config)
        {
            return new ConfigurationOptions
            {
                EndPoints = { "steady-wren-42669.upstash.io:6379" },
                Password = config["Redis:Password"],
                Ssl = true,
                AbortOnConnectFail = false
            };
        }

        public static IServiceCollection AddRedisConfig(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {
            try
            {
                ConnectionMultiplexer multiplexer;

                if (env.IsDevelopment())
                    multiplexer = ConnectionMultiplexer.Connect(config.GetConnectionString("RedisConnection")!);
                else
                    multiplexer = ConnectionMultiplexer.Connect(GetRedisProductionOptions(config));

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
