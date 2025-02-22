using Homespirations.Infrastructure.Identity;
using Homespirations.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NUlid;

namespace Homespirations.Infrastructure.Repositories
{

  public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
  {
    public new DbSet<AppUser> Users { get; set; }
    public DbSet<HomeSpace> HomeSpaces { get; set; }
    public DbSet<Media> Media { get; set; }

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

        // Store Status as a string (mapped from Enum)
        entity.Property(e => e.Status)
            .HasConversion<string>();

        // Add indexes for better search performance
        entity.HasIndex(e => e.Status);
      });

      modelBuilder.Entity<Media>(e =>
      {
        e.Property(e => e.Id)
          .HasConversion(
            id => id.ToString(),
            str => Ulid.Parse(str));

        e.HasIndex(e => e.Id);

        e.HasOne(m => m.HomeSpace)
       .WithMany(h => h.MediaItems)
       .HasForeignKey(m => m.HomeSpaceId)
       .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<AppUser>(e =>
      {
        e.Property(u => u.FirstName).IsRequired();
        e.Property(u => u.LastName).IsRequired();
        e.Property(u => u.EmailConfirmed).IsRequired();
      });


    }
  }
}


