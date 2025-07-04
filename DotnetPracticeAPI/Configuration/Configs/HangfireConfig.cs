﻿using Hangfire;
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
                ConnectionMultiplexer multiplexer;

                if (env.IsDevelopment())
                    multiplexer = ConnectionMultiplexer.Connect(config.GetConnectionString("RedisConnection")!);
                else
                    multiplexer = ConnectionMultiplexer.Connect(RedisConfig.GetRedisProductionOptions(config));


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
