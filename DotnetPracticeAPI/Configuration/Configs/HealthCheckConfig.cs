using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Configuration.Configs
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddHealthCheckConfig (this IServiceCollection services, IConfiguration config, IHostEnvironment host)
        {

            if (host.IsDevelopment())
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
                var redisOptions = new ConfigurationOptions
                {
                    EndPoints = { "redis-19846.crce181.sa-east-1-2.ec2.redns.redis-cloud.com:19846" },
                    User = "default",
                    Password = config["Redis:Password"],
                    Ssl = true,
                    AbortOnConnectFail = false
                };

                var redis = ConnectionMultiplexer.Connect(redisOptions);

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
