using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Bohio.Core.Helpers;
using NUlid;

namespace Bohio.Core.DTOs;

public class GetUserResponse
{
  [NotMapped]
  [JsonConverter(typeof(UlidJsonConverter))]
  public Ulid Id { get; set; }

  public string Username { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string FullName { get; set; } = string.Empty; // Add Full Name if applicable
  public string? ProfilePictureUrl { get; set; } // Optional Profile Picture URL
  public List<string> Roles { get; set; } = new List<string>(); // Optional Roles
  public DateTime? LastLogin { get; set; } // Optional Last Login time
  public string? SubscriptionStatus { get; set; } // Optional subscription or membership status
}
