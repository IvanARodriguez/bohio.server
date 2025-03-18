using Amazon.S3;
using Amazon.SimpleEmail;
using Amazon;
using Bohio.Core.Interfaces;
using Bohio.Infrastructure.Repositories;
using Bohio.Infrastructure.Services;
using Bohio.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DotNetEnv;
using Bohio.Infrastructure.Identity;
using Bohio.Infrastructure.Mappings;
using Amazon.Runtime;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;



namespace Bohio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Env.Load();

        var jwtSettings = configuration.GetSection("JwtSettings");

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
        if (string.IsNullOrEmpty(secret))
        {
            throw new ArgumentException("missing jwt secret");
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = key,
            };
            options.Events = new()
            {
                OnMessageReceived = ctx =>
                {
                    ctx.Request.Cookies.TryGetValue("AccessToken", out var accessToken);
                    if (!string.IsNullOrEmpty(accessToken))
                        ctx.Token = accessToken;
                    return Task.CompletedTask;

                }
            };
        });

        services.AddLogging();

        services.AddSingleton(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger("Infrastructure");
        });

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger>();

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"); ;

            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogCritical("Database connection string is missing.");
                throw new Exception("Database connection string is missing.");
            }
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();

            if (environment.IsDevelopment())
                options.EnableSensitiveDataLogging();

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IImageOptimizer, ImageOptimizer>();
        services.AddScoped<ICloudStorage, CloudStorage>();
        services.AddSingleton<IAmazonS3, AmazonS3Client>();
        services.AddScoped<IUserService, UserService>();
        services.AddAutoMapper(typeof(Profiles));

        services.AddSingleton<IAmazonSimpleEmailService>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var awsAccessKey = Environment.GetEnvironmentVariable("AWS_SES_ACCESS_KEY");
            var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SES_SECRET_KEY");
            var awsRegion = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-2";

            if (string.IsNullOrEmpty(awsAccessKey) || string.IsNullOrEmpty(awsSecretKey))
            {
                throw new InvalidOperationException("AWS SES credentials are missing.");
            }

            var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
            var config = new AmazonSimpleEmailServiceConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsRegion),
            };

            return new AmazonSimpleEmailServiceClient(credentials, config);
        });

        services.AddScoped<IEmailService, SesEmailService>();
        return services;
    }
}
