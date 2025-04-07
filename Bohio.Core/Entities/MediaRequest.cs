using System.Text.Json.Serialization;
using Bohio.Core.Helpers;
using NUlid;

namespace Bohio.Core.Entities
{
  public class MediaRequest
  {
    [JsonConverter(typeof(UlidJsonConverter))]
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public string Url { get; set; } = string.Empty;
  }
}
