using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using StackExchange.Redis;

namespace Configuration.Configs
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddHealthCheckConfig (this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                services.AddHealthChecks()
                    .AddSqlServer(
                        connectionString: config.GetConnectionString("DefaultConnection")!,
                        name: "SQL Server",
                        tags: ["db", "sql"]
                    )
                    .AddRedis(
                        redisConnectionString: config.GetConnectionString("RedisConnection")!,
                        name: "Redis (Development)",
                        tags: ["cache", "redis"],
                        failureStatus: HealthStatus.Degraded
                    );

            }
            else
            {
                var options = RedisConfig.GetRedisProductionOptions(config);
                var redis = ConnectionMultiplexer.Connect(options);

                Log.Information($"OPTIONS PASSWORD: {options.Password}");

                services.AddHealthChecks()
                    .AddNpgSql(
                        connectionString: config.GetConnectionString("DefaultConnection")!,
                        name: "PostgreSQL",
                        tags: ["db", "sql"]
                    )
                    .AddRedis(
                        redis,
                        name: "Redis (Production)",
                        tags: ["cache", "redis"],
                        failureStatus: HealthStatus.Degraded
                    );
            }

            return services;
        }
    }
}
