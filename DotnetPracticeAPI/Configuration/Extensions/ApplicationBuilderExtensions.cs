using Domain.Entities;
using Hangfire;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Middleware;
using Serilog;
using StackExchange.Redis;

namespace Configuration.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAppMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseHttpsRedirection();
            return app;
        }

        public static IApplicationBuilder UseSwaggerUIConfig(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.UseHsts();                                              // Strict Transport Security Header
            app.UseXContentTypeOptions();                               // X Content Type Options Header
            app.UseReferrerPolicy(opt => opt.NoReferrer());             // Referrer Policy Header
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());   // X Xss Protection Header
            app.UseXfo(opt => opt.Deny());                              // X Frame Options Header
            app.UseCsp(opt => opt.BlockAllMixedContent());              // Content Security Policy
            return app;
        }

        public static IEndpointRouteBuilder MapHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (ctx, report) =>
                {
                    var result = new
                    {
                        Status = report.Status.ToString(),
                        Checks = report.Entries.Select(e => new { Name = e.Key, Status = e.Value.Status.ToString() })
                    };
                    await ctx.Response.WriteAsJsonAsync(result);
                }
            });
            return endpoints;
        }

        public static IApplicationBuilder UseHangfireDashboardSafe(this IApplicationBuilder app)
        {
            try
            {

                var multiplexer = app.ApplicationServices.GetService<IConnectionMultiplexer>();
                if (multiplexer?.IsConnected == true)
                {
                    app.UseHangfireDashboard("/jobs", new DashboardOptions
                    {
                        Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
                    });
                }
                else
                {
                    Log.Warning("HangfireDashboard no se activó porque Redis no está disponible.");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error al iniciar el dashboard de Hangfire. Se omitió.");
            }

            return app;
        }


        public static async Task<IApplicationBuilder> ApplyMigrationsAndSeed(this IApplicationBuilder app, IHostEnvironment env)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            if (!db.Database.CanConnect() && env.IsDevelopment()) return app;

            if (env.IsProduction())
            {
                await db.Database.MigrateAsync();
            }
            else
            {
                if (!db.Database.CanConnect())
                    db.Database.EnsureCreated();
            }


            if (!await db.Users.AnyAsync())
            {
                await DbSeeder.Seed(db, userManager, roleManager);
            }

            return app;
        }
    }

}
