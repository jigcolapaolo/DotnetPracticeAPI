using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Configuration.Configs
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddHealthCheckConfig (this IServiceCollection services, IConfiguration config, IHostEnvironment host)
        {
            services.AddHealthChecks()
                .AddRedis(
                    redisConnectionString: config.GetConnectionString("RedisConnection")!,
                    name: "Redis",
                    tags: ["cache", "redis"],
                    failureStatus: HealthStatus.Degraded
                );

            if (host.IsDevelopment())
                services.AddHealthChecks()
                    .AddSqlServer(
                        connectionString: config.GetConnectionString("DefaultConnection")!,
                        name: "SQL Server",
                        tags: ["db", "sql"]
                    );
            else
                services.AddHealthChecks()
                    .AddNpgSql(
                        connectionString: config.GetConnectionString("DefaultConnection")!,
                        name: "PostgreSQL",
                        tags: ["db", "sql"]
                    );

            return services;
        }
    }
}
