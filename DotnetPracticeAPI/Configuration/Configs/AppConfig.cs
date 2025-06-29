using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Services;
using Application.Services.Background;
using FluentValidation;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Configuration.Configs
{
    public static class AppConfig
    {
        public static IServiceCollection AddAppConfig (this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEmailSender, SmptEmailSender>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
