using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Bohio.Core.Helpers;
using NUlid;

public class User
{
  [Key]
  [JsonConverter(typeof(UlidJsonConverter))]
  public string Id { get; set; } = Ulid.NewUlid().ToString(); // Stored as string

  public string Email { get; set; } = string.Empty;
  public string Hash { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public bool EmailConfirmed { get; set; }
  public string RefreshToken { get; set; } = string.Empty;
  public DateTime RefreshTokenExpiryTime { get; set; }
}
