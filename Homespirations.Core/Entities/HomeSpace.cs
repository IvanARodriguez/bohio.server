using NUlid;

namespace Homespirations.Core.Entities;

public class HomeSpace
{
  public Ulid Id { get; set; } = new Ulid();
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public List<string> ImageUrls { get; set; } = [];
  public List<string> Features { get; set; } = [];
  public DateTime CreatedAt { get; set; } = new DateTime().ToUniversalTime();
}