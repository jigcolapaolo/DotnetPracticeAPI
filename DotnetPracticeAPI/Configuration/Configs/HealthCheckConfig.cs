using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Configuration.Configs
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddHealthCheckConfig (this IServiceCollection services, IConfiguration config)
        {
            services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: config.GetConnectionString("DefaultConnection")!,
                    name: "SQL Server",
                    tags: ["db", "sql"]
                )
                .AddRedis(
                    redisConnectionString: config.GetConnectionString("RedisConnection")!,
                    name: "Redis",
                    tags: ["cache", "redis"],
                    failureStatus: HealthStatus.Degraded
                );

            return services;
        }
    }
}
