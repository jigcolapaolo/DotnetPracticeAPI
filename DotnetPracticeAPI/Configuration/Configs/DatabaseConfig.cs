using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Configuration.Configs
{
    public static class DatabaseConfig
    {
        public static IServiceCollection AddDatabaseConfig (this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {
            if (env.IsDevelopment())
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            else
                services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(config.GetConnectionString("DefaultConnection")));



            return services;
        }
    }
}
