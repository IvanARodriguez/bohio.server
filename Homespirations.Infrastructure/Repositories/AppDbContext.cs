using Homespirations.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Homespirations.Core.Repositories;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<HomeSpace> HomeSpaces { get; set; }
}