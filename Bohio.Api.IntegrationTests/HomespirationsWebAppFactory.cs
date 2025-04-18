
using Amazon.SimpleEmail;
using Bohio.Core.Interfaces;
using Bohio.Infrastructure.Repositories;
using Bohio.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace Bohio.Api.IntegrationTests;

internal class BohioWebAppFactory : WebApplicationFactory<Program>
{
  private readonly PostgreSqlContainer _dbContainer;

  public BohioWebAppFactory()
  {
    _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpassword")
        .Build();
    _dbContainer.StartAsync().Wait();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureTestServices(services =>
    {
      services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
      services.AddDbContext<AppDbContext>(options =>
      {
        options.EnableSensitiveDataLogging(false);
        options.UseNpgsql(_dbContainer.GetConnectionString());
      });

      services.RemoveAll(typeof(IEmailService));
      services.AddScoped<IEmailService, MockEmailService>();
      services.RemoveAll(typeof(IAmazonSimpleEmailService));

      using var scope = services.BuildServiceProvider().CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
      dbContext.Database.EnsureCreated();
    });

    builder.ConfigureAppConfiguration((context, configBuilder) =>
    {
      var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

      configBuilder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{env}.json", optional: true)
                   .AddEnvironmentVariables();

      Environment.SetEnvironmentVariable("JWT_SECRET", "ThisIsAValidTestKeyThatIsDefinitelyLongEnough123!");

      if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_SECRET")))
      {
        throw new ArgumentException("JWT_SECRET environment variable is missing.");
      }
    });

  }

  /// <summary>
  /// Asynchronously disposes the PostgreSQL container and any resources used by the factory.
  /// </summary>
  /// <returns>A task that represents the asynchronous dispose operation.</returns>

  public override async ValueTask DisposeAsync()
  {
    await _dbContainer.DisposeAsync();
    await base.DisposeAsync();
  }
}
