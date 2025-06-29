namespace Configuration.Configs
{
    public static class CorsConfig
    {
        public static IServiceCollection AddCorsConfig (this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                    //.AllowCredentials();
                });
            });

            return services;
        }
    }
}
