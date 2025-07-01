using Hangfire;
using Hangfire.Redis.StackExchange;
using Serilog;
using StackExchange.Redis;

namespace Configuration.Configs
{
    public static class HangfireConfig
    {
        public static IServiceCollection AddHangfireConfig(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {
            try
            {
                // Options for production Redis
                var redisOptions = new ConfigurationOptions
                {
                    EndPoints = { "redis-19846.crce181.sa-east-1-2.ec2.redns.redis-cloud.com:19846" },
                    User = "default",
                    Password = config["Redis:Password"],
                    Ssl = true,
                    AbortOnConnectFail = false
                };

                ConnectionMultiplexer multiplexer;

                if (env.IsDevelopment())
                    multiplexer = ConnectionMultiplexer.Connect(config.GetConnectionString("RedisConnection")!);
                else
                    multiplexer = ConnectionMultiplexer.Connect(redisOptions);


                if (multiplexer.IsConnected)
                {
                    services.AddHangfire(cfg =>

                        cfg.UseRedisStorage(multiplexer, new RedisStorageOptions
                        {
                            Prefix = "hangfire:",
                            Db = 1,
                        }));

                    services.AddHangfireServer(options =>
                    {
                        options.WorkerCount = 4;
                    });
                }
                else
                {
                    Log.Warning("Hangfire no se configuró porque Redis no está conectado.");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error al conectar Hangfire a Redis. Hangfire no será inicializado.");
            }

            return services;
        }
    }
}
