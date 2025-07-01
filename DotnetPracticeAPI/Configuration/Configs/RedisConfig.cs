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
                EndPoints = { { "redis-19846.crce181.sa-east-1-2.ec2.redns.redis-cloud.com", 19846 } },
                User = "default",
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
                    multiplexer = ConnectionMultiplexer.Connect(config.GetConnectionString("RedisConnection")!);


                services.AddSingleton<IConnectionMultiplexer>(multiplexer);
                services.AddScoped<ICacheService, RedisCacheService>();

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
