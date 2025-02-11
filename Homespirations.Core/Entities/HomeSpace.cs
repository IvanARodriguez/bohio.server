using NUlid;

namespace Homespirations.Core.Entities;

public class HomeSpace
{
  public Ulid Id { get; set; } = Ulid.NewUlid();
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public List<string> ImageUrls { get; set; } = [];
  public List<string> Features { get; set; } = [];

  // Category of the home space (e.g., Living Room, Bedroom, Patio)
  public string Category { get; set; } = string.Empty;

  // Nullable location fields for optional user-provided details
  public string? City { get; set; }
  public string? State { get; set; }
  public string? Country { get; set; }

  // Ownership tracking
  public Ulid OwnerId { get; set; }

  // Strongly-typed Status field
  public HomeSpaceStatus Status { get; set; } = HomeSpaceStatus.Draft;

  // Audit fields
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }

  public List<Media> MediaItems { get; set; } = [];
}

// Enum for HomeSpace status
public enum HomeSpaceStatus
{
  Draft,
  Published,
  Archived
}