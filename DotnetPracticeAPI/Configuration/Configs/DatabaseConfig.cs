using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Configuration.Configs
{
    public static class DatabaseConfig
    {
        public static IServiceCollection AddDatabaseConfig (this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
