using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Bohio.Core.Helpers;
using NUlid;

namespace Bohio.Core.DTOs;

public class ConfirmEmailRequest
{
  [NotMapped]
  [JsonConverter(typeof(UlidJsonConverter))]
  public Ulid UserId { get; set; }
  public string Token { get; set; } = string.Empty;
}
