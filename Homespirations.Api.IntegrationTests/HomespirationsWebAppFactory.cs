
using Homespirations.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace Homespirations.Api.IntegrationTests;

internal class HomespirationsWebAppFactory : WebApplicationFactory<Program>
{
  private readonly PostgreSqlContainer _dbContainer;

  public HomespirationsWebAppFactory()
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
      services.RemoveAll(typeof(Serilog.ILogger));
      services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
      services.AddDbContext<AppDbContext>(options =>
      {
        options.EnableSensitiveDataLogging();
        options.UseNpgsql(_dbContainer.GetConnectionString());
      });

      using var scope = services.BuildServiceProvider().CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
      dbContext.Database.EnsureCreated();
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