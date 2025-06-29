//using Application.Config;
//using Application.Interfaces;
//using Application.Interfaces.Services;
//using Application.Mapping;
//using Application.Services;
//using Application.Services.Background;
//using AspNetCoreRateLimit;
//using Domain.Entities;
//using DotnetPracticeAPI.Application.Services.Background;
//using FluentValidation;
//using Hangfire;
//using Hangfire.Redis.StackExchange;
//using Hubs;
//using Infrastructure.Persistence;
//using Infrastructure.Persistence.UnitOfWork;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Diagnostics.HealthChecks;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using Middleware;
//using Prometheus;
//using Serilog;
//using StackExchange.Redis;
//using System.Text;

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

//var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog();

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                },
//            },
//            Array.Empty<string>()
//        }
//    });
//});
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
//);

//// Healthcheck SQL + Redis
//builder.Services.AddHealthChecks()
//    .AddSqlServer(
//        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
//        name: "SQL Server",
//        tags: new[] { "db", "sql" }
//    )
//    .AddRedis(
//        redisConnectionString: "localhost:6379",
//        name: "Redis",
//        tags: new[] { "cache", "redis" }
//    );

//builder.Services.AddMemoryCache();
//builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
//builder.Services.AddInMemoryRateLimiting();
//builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//builder.Services.AddAutoMapper(typeof(MappingProfile));
//builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

//builder.Services.AddSignalR();

////Redis
//builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));
//builder.Services.AddScoped<ICacheService, RedisCacheService>();

//// Hangfire + Redis
//builder.Services.AddHangfire(config =>
//    config.UseRedisStorage(
//        ConnectionMultiplexer.Connect("localhost:6379"),
//        new RedisStorageOptions
//        {
//            Prefix = "hangfire:",
//            Db = 1,
//        }
//    )
//);

//builder.Services.AddHangfireServer(options =>
//{
//    options.WorkerCount = 4;
//});

//builder.Services.AddScoped<IEmailSender, SmptEmailSender>();

////Identity + JWT
//builder.Services.AddIdentity<User, IdentityRole<Guid>>()
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();
//builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
//    };
//});
//builder.Services.AddAuthorization();

//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IProductService, ProductService>();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//              //.AllowCredentials();
//    });
//});

//var app = builder.Build();

//app.UseHangfireDashboard("/jobs");

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseMiddleware<ErrorHandlerMiddleware>();

//app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

//app.UseCors("AllowAll");

//app.UseIpRateLimiting();


//app.UseHsts();                                              // Strict Transport Security Header
//app.UseXContentTypeOptions();                               // X Content Type Options Header
//app.UseReferrerPolicy(opt => opt.NoReferrer());             // Referrer Policy Header
//app.UseXXssProtection(opt => opt.EnabledWithBlockMode());   // X Xss Protection Header
//app.UseXfo(opt => opt.Deny());                              // X Frame Options Header
//app.UseCsp(opt => opt.BlockAllMixedContent());              // Content Security Policy

//app.UseWebSockets();
//app.MapHub<ChatHub>("/chatHub");

//app.UseHttpMetrics();
//app.MapMetrics();

//app.MapHealthChecks("/health", new HealthCheckOptions
//{
//    ResponseWriter = async (context, report) =>
//    {
//        var result = new
//        {
//            Status = report.Status.ToString(),
//            Checks = report.Entries.Select(e => new
//            {
//                Name = e.Key,
//                Status = e.Value.Status.ToString(),
//            })
//        };
//        await context.Response.WriteAsJsonAsync(result);
//    }
//});

//app.MapControllers();

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    //db.Database.EnsureDeleted();
//    db.Database.Migrate();
//    DbSeeder.Seed(db);
//}

//app.Run();

////For testing
//public partial class Program { }