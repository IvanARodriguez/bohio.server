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
        // Ensure Id (Ulid) is stored as a string
        entity.Property(e => e.Id)
            .HasConversion(
                id => id.ToString(),
                str => Ulid.Parse(str));
        entity.Property(e => e.OwnerId)
            .HasConversion(
                ownerId => ownerId.ToString(),
                str => Ulid.Parse(str));

        // Store Status as a string (mapped from Enum)
        entity.Property(e => e.Status)
            .HasConversion<string>();

        // Add indexes for better search performance
        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.OwnerId);
      });

      modelBuilder.Entity<Media>(e =>
      {
        e.Property(e => e.Id)
          .HasConversion(
            id => id.ToString(),
            str => Ulid.Parse(str));
        e.HasOne(m => m.HomeSpace)
       .WithMany(h => h.MediaItems)
       .HasForeignKey(m => m.HomeSpaceId)
       .OnDelete(DeleteBehavior.Cascade);
      });


    }

  }
}


