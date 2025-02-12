
using Homespirations.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Homespirations.Api.IntegrationTests;

internal class HomespirationsWebAppFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureTestServices(services =>
    {
      services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
      var connString = GetConnectionString();
      services.AddDbContext<AppDbContext>(options =>
      {
        options.EnableSensitiveDataLogging();
        options.UseNpgsql(connString);
      });

      var dbContext = CreateDbContext(services);
      try
      {
        dbContext.Database.EnsureDeleted();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error during database initialization: {ex.Message}");
      }
    });
  }


  private static string? GetConnectionString()
  {
    var configuration = new ConfigurationBuilder()
      .AddUserSecrets<HomespirationsWebAppFactory>()
      .Build();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return connectionString;
  }

  private static DbContext CreateDbContext(IServiceCollection services)
  {
    var servicesProvider = services.BuildServiceProvider();
    var scope = servicesProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
    return dbContext;
  }
}
