using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Bohio.Core.Helpers;
using NUlid;

namespace Bohio.Core.Entities;

public class HomeSpace
{
  [Key]
  [NotMapped]
  [JsonConverter(typeof(UlidJsonConverter))]
  public Ulid Id { get; set; } = Ulid.NewUlid();

  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public List<string> Features { get; set; } = [];
  public List<string> Tags { get; set; } = [];

  public string Category { get; set; } = string.Empty;

  public string? City { get; set; }
  public string? State { get; set; }
  public string? Country { get; set; }

  [JsonConverter(typeof(JsonStringEnumConverter))]
  public HomeSpaceStatus Status { get; set; } = HomeSpaceStatus.Draft;

  [JsonPropertyName("createdAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }

  public List<Media>? MediaItems { get; set; } = [];
}

// Enum for HomeSpace status
public enum HomeSpaceStatus
{
  Draft,
  Published,
  Archived
}
