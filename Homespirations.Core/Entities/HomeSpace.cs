using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Homespirations.Core.Helpers;
using NUlid;

namespace Homespirations.Core.Entities;

public class HomeSpace
{
  [NotMapped]
  [JsonConverter(typeof(UlidJsonConverter))]
  public Ulid Id { get; set; } = Ulid.NewUlid();
  [Required(ErrorMessage = "Name is required.")]
  [MaxLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
  public string Name { get; set; } = string.Empty;
  [MaxLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
  public string? Description { get; set; }
  [MinLength(1, ErrorMessage = "At least one feature is required.")]
  public List<string> Features { get; set; } = [];
  public List<string> Tags { get; set; } = [];


  // Category of the home space (e.g., Living Room, Bedroom, Patio)
  [Required(ErrorMessage = "Category is required.")]
  public string Category { get; set; } = string.Empty;

  // Nullable location fields for optional user-provided details
  public string? City { get; set; }
  public string? State { get; set; }
  public string? Country { get; set; }

  // Strongly-typed Status field
  [JsonConverter(typeof(JsonStringEnumConverter))]
  [Required]
  public HomeSpaceStatus Status { get; set; } = HomeSpaceStatus.Draft;

  // Audit fields
  [JsonPropertyName("createdAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }

  [Required]
  public List<Media>? MediaItems { get; set; } = [];

}

// Enum for HomeSpace status
public enum HomeSpaceStatus
{
  Draft,
  Published,
  Archived
}