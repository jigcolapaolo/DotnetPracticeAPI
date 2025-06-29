namespace Configuration.Configs
{
    public static class SignalRConfig
    {
        public static IServiceCollection AddSignalRConfig(this IServiceCollection services)
        {
            services.AddSignalR();
            return services;
        }
    }
}
