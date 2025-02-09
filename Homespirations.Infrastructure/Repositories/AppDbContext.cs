using Homespirations.Core.Entities;
using Microsoft.EntityFrameworkCore;
using NUlid;

namespace Homespirations.Infrastructure.Repositories
{

  public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)  // Add the `: DbContext` properly
  {
    public DbSet<HomeSpace> HomeSpaces { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<HomeSpace>(entity =>
      {
        entity.Property(e => e.Id)
                    .HasConversion(
                        id => id.ToString(),
                        str => Ulid.Parse(str));
      });
    }
  }
}


