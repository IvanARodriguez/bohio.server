using Amazon.S3;
using Amazon.SimpleEmail;
using Amazon;
using Homespirations.Core.Interfaces;
using Homespirations.Infrastructure.Repositories;
using Homespirations.Infrastructure.Services;
using Homespirations.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using DotNetEnv;
using Homespirations.Infrastructure.Identity;
using Homespirations.Infrastructure.Mappings;
using Amazon.Runtime;

namespace Homespirations.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Env.Load();

        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddLogging();

        services.AddSingleton(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger("Infrastructure");
        });

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger>();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                 ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogCritical("Database connection string is missing.");
                throw new Exception("Database connection string is missing.");
            }

            logger.LogInformation("Configuring database with connection string: {ConnectionString}", connectionString);

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
