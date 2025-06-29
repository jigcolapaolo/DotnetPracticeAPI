namespace Configuration.Configs
{
    public static class CacheConfig
    {
        public static IServiceCollection AddCacheConfig (this IServiceCollection services, IConfiguration config)
        {
            services.AddMemoryCache();
            return services;
        }
    }
}
