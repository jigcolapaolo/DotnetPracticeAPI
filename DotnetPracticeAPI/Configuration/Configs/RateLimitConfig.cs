using AspNetCoreRateLimit;

namespace Configuration.Configs
{
    public static class RateLimitConfig
    {
        public static IServiceCollection AddRateLimitConfig (this IServiceCollection services, IConfiguration config)
        {
            services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"));
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            return services;
        }
    }
}
