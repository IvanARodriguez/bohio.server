using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Homespirations.Core.Helpers;
using NUlid;

namespace Homespirations.Core.Entities;

public class Media
{
  [NotMapped]
  [JsonConverter(typeof(UlidJsonConverter))]
  public Ulid Id { get; set; } = Ulid.NewUlid();
  public string Url { get; set; } = string.Empty;
  public MediaType MediaType { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  // Foreign Key
  public Ulid HomeSpaceId { get; set; }
  // Navigation Property
  public HomeSpace? HomeSpace { get; set; } = null;
}

public enum MediaType
{
  Image,
  Video
}