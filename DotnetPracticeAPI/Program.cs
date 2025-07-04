using AspNetCoreRateLimit;
using Configuration.Configs;
using Configuration.Extensions;
using Hubs;
using Prometheus;
using Serilog;

Log.Logger = SerilogConfig.CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Host.UseSerilog();

builder.Services
    .AddAppConfig(builder.Configuration)
    .AddDatabaseConfig(builder.Configuration, builder.Environment)
    .AddAuthConfig(builder.Configuration, builder.Environment)
    .AddCacheConfig(builder.Configuration)
    .AddCorsConfig()
    .AddSwaggerConfig()
    .AddRateLimitConfig(builder.Configuration)
    .AddHealthCheckConfig(builder.Configuration, builder.Environment)
    .AddHangfireConfig(builder.Configuration, builder.Environment)
    .AddRedisConfig(builder.Configuration, builder.Environment)
    .AddSignalRConfig();


var app = builder.Build();

app.UseAppMiddleware();
app.UseSwaggerUIConfig();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();
app.UseSecurityHeaders();

app.UseHangfireDashboardSafe();

app.MapHub<ChatHub>("/chatHub");
app.UseWebSockets();

app.UseHttpMetrics();
app.MapMetrics();

app.MapHealthCheckEndpoints();

app.MapControllers();

await app.ApplyMigrationsAndSeed(builder.Environment);

app.Run();

//For testing
public partial class Program { }